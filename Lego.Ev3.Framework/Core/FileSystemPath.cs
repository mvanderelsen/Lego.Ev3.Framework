using System;

namespace Lego.Ev3.Framework.Core
{
    /* 
 
    see bytecodes.h
 FOLDERS

#define   vmMEMORY_FOLDER               "/mnt/ramdisk"                //!< Folder for non volatile user programs/data
#define   vmPROGRAM_FOLDER              "../prjs/BrkProg_SAVE"        //!< Folder for On Brick Programming programs
#define   vmDATALOG_FOLDER              "../prjs/BrkDL_SAVE"          //!< Folder for On Brick Data log files
#define   vmSDCARD_FOLDER               "../prjs/SD_Card"             //!< Folder for SD card mount
#define   vmUSBSTICK_FOLDER             "../prjs/USB_Stick"           //!< Folder for USB stick mount

#define   vmPRJS_DIR                    "../prjs"                     //!< Project folder
#define   vmAPPS_DIR                    "../apps"                     //!< Apps folder
#define   vmTOOLS_DIR                   "../tools"                    //!< Tools folder
#define   vmTMP_DIR                     "../tmp"                      //!< Temporary folder

#define   vmSETTINGS_DIR                "../sys/settings"             //!< Folder for non volatile settings

#define   vmDIR_DEEPT                   127                           //!< Max directory items allocated including "." and ".." 
 * 
 */

    /// <summary>
    /// Folders on the brick
    /// </summary>
    public enum FileSystemPath
    {
        /// <summary>
        /// Projects
        /// </summary>
        Projects,
        /// <summary>
        /// Secure Digital Card
        /// </summary>
        SDCard,
        /// <summary>
        /// Applications
        /// </summary>
        Applications,
        /// <summary>
        /// Tools
        /// </summary>
        Tools,
        /// <summary>
        /// USB Stick
        /// </summary>
        USBStick
    }

    internal static class FileSystemPathExtension
    {
        internal static string GetRelativePath(this FileSystemPath folder)
        {
            switch (folder)
            {
                case FileSystemPath.Applications:
                    {
                        return "../apps/";
                    }
                case FileSystemPath.Projects:
                    {
                        return "../prjs/";
                    }
                case FileSystemPath.SDCard:
                    {
                        return "../prjs/SD_Card/";
                    }
                case FileSystemPath.USBStick:
                    {
                        return "../prjs/USB_Stick/";
                    }
                case FileSystemPath.Tools:
                    {
                        return "../tools/";
                    }
                default:
                    {
                        throw new NotImplementedException("Folder " + folder + " must be implemented!");
                    }
            }
        }
    }
}
