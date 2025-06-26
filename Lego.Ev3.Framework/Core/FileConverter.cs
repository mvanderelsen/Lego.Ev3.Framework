using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework.Core
{

    /// <summary>
    /// Convert from and to Robot File formats
    /// </summary>
    public static class FileConverter
    {
        /// <summary>
        /// Robot Sound File rate
        /// </summary>
        public const int RSF_RATE = 8000;

        /// <summary>
        /// Robot Sound File bits
        /// </summary>
        public const int RSF_BITS = 8;

        /// <summary>
        /// Robot Sound File channels
        /// </summary>
        public const int RSF_CHANNELS = 1;

        /// <summary>
        /// Robot Graphic File width
        /// </summary>
        public const int RGF_WIDTH = 178;

        /// <summary>
        /// Robot Graphic File height
        /// </summary>
        public const int RGF_HEIGHT = 128;


        /// <summary>
        /// Robot Text File line break
        /// </summary>
        public const string RTF_LINE_BREAK = "\r";

        #region RSF


        /// <summary>
        /// Converts a rsf byte[] to a wav byte[]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<byte[]> RSFtoWAV(byte[] data)
        {
            byte[] wav = null;
            using (MemoryStream stream = new())
            {
                WriteWavHeader(stream, false, RSF_CHANNELS, RSF_BITS, RSF_RATE, data.Length);
                await stream.WriteAsync(data, 0, data.Length);
                stream.Position = 0;
                wav = stream.ToArray();
            }
            return wav;
        }


        // totalSampleCount needs to be the combined count of samples of all channels. So if the left and right channels contain 1000 samples each, then totalSampleCount should be 2000.
        // isFloatingPoint should only be true if the audio data is in 32-bit floating-point format.
        private static void WriteWavHeader(MemoryStream stream, bool isFloatingPoint, ushort channelCount, ushort bitDepth, int sampleRate, int totalSampleCount)
        {
            stream.Position = 0;

            // RIFF header.
            // Chunk ID.
            stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);

            // Chunk size.
            stream.Write(BitConverter.GetBytes(((bitDepth / 8) * totalSampleCount) + 36), 0, 4);

            // Format.
            stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);

            // Sub-chunk 1.
            // Sub-chunk 1 ID.
            stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);

            // Sub-chunk 1 size.
            stream.Write(BitConverter.GetBytes(16), 0, 4);

            // Audio format (floating point (3) or PCM (1)). Any other format indicates compression.
            stream.Write(BitConverter.GetBytes((ushort)(isFloatingPoint ? 3 : 1)), 0, 2);

            // Channels.
            stream.Write(BitConverter.GetBytes(channelCount), 0, 2);

            // Sample rate.
            stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);

            // Bytes rate.
            stream.Write(BitConverter.GetBytes(sampleRate * channelCount * (bitDepth / 8)), 0, 4);

            // Block align.
            stream.Write(BitConverter.GetBytes((ushort)channelCount * (bitDepth / 8)), 0, 2);

            // Bits per sample.
            stream.Write(BitConverter.GetBytes(bitDepth), 0, 2);



            // Sub-chunk 2.
            // Sub-chunk 2 ID.
            stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);

            // Sub-chunk 2 size.
            stream.Write(BitConverter.GetBytes((bitDepth / 8) * totalSampleCount), 0, 4);
        }
        #endregion

        #region RTF

        /// <summary>
        /// Converts a rtf byte[] to string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string RTFtoText(byte[] data)
        {
            if (data == null) return string.Empty;
            string value = Encoding.ASCII.GetString(data);
            value = value.Replace(RTF_LINE_BREAK, Environment.NewLine);
            return value;
        }


        /// <summary>
        /// Converts a string to a rtf byte[]
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] TexttoRTF(string text)
        {
            text = text.Replace(Environment.NewLine, RTF_LINE_BREAK);
            text = text.Replace("\n", RTF_LINE_BREAK);
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            return bytes;
        }
        #endregion
    }
}
