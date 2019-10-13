using Lego.Ev3.Framework.Firmware;
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Sockets
{
    internal class UsbSocket : SocketBase, IDisposable, ISocket
    {
        private byte[] _input;
        private byte[] _output;
        private FileStream _stream;
        private CancellationTokenSource _cancellationTokenSource;

        public CancellationToken CancellationToken { get; private set; }

        public override string ConnectionInfo { get { return "Usb"; } }


        public bool IsConnected { get { return _stream != null && _stream.CanRead && _stream.CanWrite; } }


        /// <summary>
        /// Connect to the EV3 brick.
        /// </summary>
        public Task<bool> Connect()
        {
            if (IsConnected) return Task.FromResult(true);
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken = _cancellationTokenSource.Token;
            ConnectToEv3();
            OpenSocket();
            return Task.FromResult(IsConnected);
        }

        /// <summary>
        /// Disconnect from the EV3 brick.
        /// </summary>
        public Task Disconnect()
        {
            // close up the stream and handle
            if (IsConnected)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _stream.SafeFileHandle.Close();
                _stream.Close();
                _stream = null;
                _input = null;
                _output = null;
                Clear();
            }
            return Task.CompletedTask;
        }

        private void OpenSocket()
        {
            Task.Factory.StartNew(async () =>
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await _stream.ReadAsync(_input, 0, _input.Length, CancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }

                    short size = (short)(_input[1] | _input[2] << 8);
                    if (size > 0)
                    {
                        byte[] payLoad = new byte[size];
                        Array.Copy(_input, 3, payLoad, 0, size);
                        ResponseBuffer.TryAdd(Response.GetId(payLoad), payLoad);
                    }
                }
            }, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);

            Task.Factory.StartNew(async () =>
            {

                while (!CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        Command command;
                        if (NoReplyCommandBuffer.TryDequeue(out command))
                        {
                            command.PayLoad.CopyTo(_output, 1);
                            await _stream.WriteAsync(_output, 0, _output.Length);
                            _stream.Flush();
                            ResponseBuffer.TryAdd(command.Id, null);
                        }



                        if (CommandBuffer.TryDequeue(out command))
                        {
                            command.PayLoad.CopyTo(_output, 1);
                            await _stream.WriteAsync(_output, 0, _output.Length);
                            _stream.Flush();

                            int retry = 0;
                            while (!ResponseBuffer.ContainsKey(command.Id) && retry < 20)
                            {
                                await Task.Delay(50, CancellationToken);
                                retry++;
                            }
                        }

                        if (EventBuffer.TryDequeue(out command))
                        {
                            command.PayLoad.CopyTo(_output, 1);
                            await _stream.WriteAsync(_output, 0, _output.Length);
                            _stream.Flush();
                        }
                    }
                    catch (TaskCanceledException) { }
                }


            }, CancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }


        #region connect to usb
        //////////////////////////////////////////////////////////////////////////////////
        //	ConnectToEv3
        //	Managed Wiimote Library
        //	Written by Brian Peek (http://www.brianpeek.com/)
        //	for MSDN's Coding4Fun (http://msdn.microsoft.com/coding4fun/)
        //	Visit http://blogs.msdn.com/coding4fun/archive/2007/03/14/1879033.aspx
        //  and http://www.codeplex.com/WiimoteLib
        //	for more information
        //////////////////////////////////////////////////////////////////////////////////
        private void ConnectToEv3()
        {
            // Ev3 brick VID and PID
            const ushort VID = 0x0694;
            const ushort PID = 0x0005;
            int index = 0;
            bool found = false;

            // get the GUID of the HID class
            HidImports.HidD_GetHidGuid(out Guid guid);

            // get a handle to all devices that are part of the HID class
            IntPtr hDevInfo = HidImports.SetupDiGetClassDevs(ref guid, null, IntPtr.Zero, HidImports.DIGCF_DEVICEINTERFACE | HidImports.DIGCF_PRESENT);

            // create a new interface data struct and initialize its size
            HidImports.SP_DEVICE_INTERFACE_DATA diData = new HidImports.SP_DEVICE_INTERFACE_DATA();
            diData.cbSize = Marshal.SizeOf(diData);

            // get a device interface to a single device (enumerate all devices)
            while (HidImports.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guid, index, ref diData))
            {

                // get the buffer size for this device detail instance (returned in the size parameter)
                HidImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, IntPtr.Zero, 0, out uint size, IntPtr.Zero);

                // create a detail struct and set its size
                HidImports.SP_DEVICE_INTERFACE_DETAIL_DATA diDetail = new HidImports.SP_DEVICE_INTERFACE_DETAIL_DATA
                {

                    // yeah, yeah...well, see, on Win x86, cbSize must be 5 for some reason.  On x64, apparently 8 is what it wants.
                    // someday I should figure this out.  Thanks to Paul Miller on this...
                    cbSize = (uint)(IntPtr.Size == 8 ? 8 : 5)
                };

                // actually get the detail struct
                if (HidImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, ref diDetail, size, out _, IntPtr.Zero))
                {
                    //Debug.WriteLine("{0}: {1} - {2}", index, diDetail.DevicePath, Marshal.GetLastWin32Error());

                    // open a read/write handle to our device using the DevicePath returned
                    SafeFileHandle handle = HidImports.CreateFile(diDetail.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HidImports.EFileAttributes.Overlapped, IntPtr.Zero);

                    // create an attributes struct and initialize the size
                    HidImports.HIDD_ATTRIBUTES attrib = new HidImports.HIDD_ATTRIBUTES();
                    attrib.Size = Marshal.SizeOf(attrib);

                    // get the attributes of the current device
                    if (HidImports.HidD_GetAttributes(handle.DangerousGetHandle(), ref attrib))
                    {
                        // if the vendor and product IDs match up
                        if (attrib.VendorID == VID && attrib.ProductID == PID)
                        {
                            // it's a Ev3
                            found = true;

                            if (!HidImports.HidD_GetPreparsedData(handle.DangerousGetHandle(), out IntPtr preparsedData))
                                throw new Exception("Could not get preparsed data for HID device");

                            if (HidImports.HidP_GetCaps(preparsedData, out HidImports.HIDP_CAPS caps) != HidImports.HIDP_STATUS_SUCCESS)
                                throw new Exception("Could not get CAPS for HID device");

                            HidImports.HidD_FreePreparsedData(ref preparsedData);

                            _input = new byte[caps.InputReportByteLength];
                            _output = new byte[caps.OutputReportByteLength];

                            // create a nice .NET FileStream wrapping the handle above
                            _stream = new FileStream(handle, FileAccess.ReadWrite, _input.Length, true);

                            break;
                        }

                        handle.Close();
                    }
                }
                else
                {
                    // failed to get the detail struct
                    throw new Exception("SetupDiGetDeviceInterfaceDetail failed on index " + index);
                }

                // move to the next device
                index++;
            }

            // clean up our list
            HidImports.SetupDiDestroyDeviceInfoList(hDevInfo);

            // if we didn't find a EV3, throw an exception
            if (!found) throw new Exception("No LEGO EV3s found in HID device list.");
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disconnect().ConfigureAwait(false).GetAwaiter().GetResult();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UsbSocket()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
