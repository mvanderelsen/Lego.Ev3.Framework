using Lego.Ev3.Framework.Firmware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Lego.Ev3.Framework
{

    /// <summary>
    /// Port to all sound methods
    /// Methods can only be called after Brick is connected
    /// </summary>
    /// <example>
    /// Example illustrates handling soundfiles through the soundport
    /// <code>
    /// using Lego.Ev3.Framework;
    /// using System.Threading.Tasks;
    /// 
    /// namespace Examples.SoundPort
    /// {
    ///     public class Controller:Brick
    ///     {
    /// 
    ///         public async Task Start()
    ///         {
    ///             //connect to the brick first
    ///             await Connect();
    /// 
    ///             // get the project resource directory
    ///             Directory directory = await Drive.Projects.GetDirectory("My Project");
    ///             if (directory == null) directory = await Drive.Projects.CreateDirectory("My Project");
    ///             
    ///             
    ///             //gets all soundfiles in the directory
    ///             SoundFile[] sounds = await directory.GetSoundFiles();
    /// 
    ///             if (sounds.Length > 0) SoundPort.PlayLoop(sounds[1], 30, 2, 0);
    ///             else
    ///             {
    ///                 //upload a new soundfile
    ///                 SoundFile newSound = await directory.UploadFile(@"C:\Tmp\newSound.rsf");
    /// 
    ///                 //play the new soundfile
    ///                 SoundPort.Play(newSound, 60);
    ///             }
    /// 
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public sealed class SoundPort
    {

        internal SoundPort(){}

        #region Play SoundFiles and Tones

        /// <summary>
        /// Plays a tone based on frequency for given duration and at a given volume
        /// </summary>
        /// <param name="volume">Specify volume for playback, [0 - 100]</param>
        /// <param name="frequency">Specify frequency, [250 - 10000]</param>
        /// <param name="duration">Specify duration in milliseconds [1-n]</param>
        /// <exception cref="ArgumentOutOfRangeException">volume, frequency or duration out of range</exception>
        public async void Tone(int volume, int frequency, int duration)
        {
            await SoundMethods.Tone(Brick.Socket, volume, frequency, duration);
        }

        /// <summary>
        /// Plays a tone based on frequency for given duration and at a given volume and at certain interval
        /// </summary>
        /// <param name="volume">Specify volume for playback, [0 - 100]</param>
        /// <param name="frequency">Specify frequency, [250 - 10000</param>
        /// <param name="duration">Specify duration in milliseconds [1-n]</param>
        /// <param name="numberOfLoops">Specify number of loops [1-n] if 1 will play single tone.</param>
        /// <param name="timeOut">Time in milliseconds between tones [1-n]</param>
        /// <exception cref="ArgumentOutOfRangeException">volume, frequency, duration, numberOfLoops or timeOut out of range</exception>
        public async void ToneLoop(int volume, int frequency, int duration, int numberOfLoops, int timeOut)
        {
            if (timeOut < 1) throw new ArgumentOutOfRangeException("Time out must > 0 ms", "timeOut");
            if (numberOfLoops < 1) throw new ArgumentOutOfRangeException("Amount must be > 0", "numberOfLoops");

            if (numberOfLoops == 1) await SoundMethods.Tone(Brick.Socket, volume, frequency, duration);
            else
            {
                int time = timeOut + duration;
                CancellationToken token = Brick.Socket.CancellationToken;
                await Task.Factory.StartNew(
                   async () =>
                   {
                       for (int i = 0; i < numberOfLoops; i++)
                       {
                           await SoundMethods.Tone(Brick.Socket, volume, frequency, duration);
                           if (i + 1 < numberOfLoops)
                           {
                               if (token.WaitHandle.WaitOne(time)) break;
                           }
                       }
                   }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// Plays a sound file on the brick at a given volume.
        /// The sound file must be stored in BrickFolder.Projects and in FolderName
        /// </summary>
        /// <param name="soundFile">A playable soundfile 
        /// </param>
        /// <param name="volume">Specify volume for playback, [0 - 100]</param>
        /// <exception cref="ArgumentOutOfRangeException">volume out of range</exception>
        /// <see cref="Devices.FileSystem"/>
        public async void Play(SoundFile soundFile, int volume)
        {
            await SoundMethods.Play(Brick.Socket, volume, soundFile.FilePath);
        }


        /// <summary>
        /// Plays a sound file on the brick at a given volume and at certain interval
        /// The sound file must be stored in BrickFolder.Projects and in FolderName
        /// </summary>
        /// <param name="soundFile">A playable soundfile 
        /// </param>
        /// <param name="volume">Specify volume for playback, [0 - 100]</param>
        /// <param name="numberOfLoops">Specify number of loops [1-n], 1 will play soundfile once</param>
        /// <param name="timeOut">Time in milliseconds between plays [0-n]</param>
        /// <exception cref="ArgumentOutOfRangeException">volume, numberOfLoops or timeOut out of range</exception>
        /// <see cref="Devices.FileSystem"/>
        public async void PlayLoop(SoundFile soundFile, int volume, int numberOfLoops, int timeOut)
        {
            if (timeOut < 0) throw new ArgumentException("Time out must >= 0 ms", "timeOut");
            if (numberOfLoops < 1) throw new ArgumentException("Amount must be > 0", "numberOfLoops");

            if (numberOfLoops == 1) await SoundMethods.Play(Brick.Socket, volume, soundFile.FilePath);
            else
            {
                CancellationToken token = Brick.Socket.CancellationToken;
                await Task.Factory.StartNew(
                   async () =>
                   {
                       for (int i = 0; i < numberOfLoops; i++)
                       {
                           await SoundMethods.Play(Brick.Socket, volume, soundFile.FilePath);
                           while (await IsBusy()) // test if file is still playing
                           {
                               token.WaitHandle.WaitOne(500);
                           }
                           if (i + 1 < numberOfLoops) // only pause between sounds
                           {
                               if (token.WaitHandle.WaitOne(timeOut)) break;
                           }
                       }
                   }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }


        /// <summary>
        /// Repeats playing a sound file on the brick at a given volume untill Stop() is called
        /// </summary>
        /// <param name="soundFile">A playable soundfile 
        /// </param>
        /// <param name="volume">Specify volume for playback, [0 - 100]</param>
        /// <exception cref="ArgumentOutOfRangeException">volume out of range</exception>
        /// <see cref="Devices.FileSystem"/>
        public async void Repeat(SoundFile soundFile, int volume)
        {
            await SoundMethods.Repeat(Brick.Socket, volume, soundFile.FilePath);
        }

        #endregion

        /// <summary>
        /// Stops current sound playback.
        /// </summary>
        /// <returns></returns>
        public async Task Stop()
        {
             await SoundMethods.Break(Brick.Socket);
        }

        /// <summary>
        /// This function enables the program to test if sound is busy (Playing sound or tone)
        /// </summary>
        /// <returns>Output busy flag, [0 = Ready, 1 = Busy]</returns>
        public async Task<bool> IsBusy()
        {
            return await SoundMethods.Test(Brick.Socket);
        }


        //TODO
        /*
        SoundMethods
        internal static async Task Ready(Socket socket)
         
        */

        /// <summary>
        /// Gets all SoundFiles natively stored on brick within firmware.
        /// </summary>
        public SoundFile[] GetOnBrickSoundFiles()
        {
            List<SoundFile> sounds = new List<SoundFile>();
            for(int i=1; i <=12; i++)
            {
                string id = string.Format("OnBrickSound{0:00}", i);
                SoundFile sound = new SoundFile(id, id + ".rsf", "../apps/Brick Program/" + id + ".rsf");
                sound.IsOnBrickFile = true;
                sounds.Add(sound);
            }
            return sounds.ToArray();
        }
}
}
