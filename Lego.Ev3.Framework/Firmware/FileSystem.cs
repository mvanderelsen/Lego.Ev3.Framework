using System;
using System.Text.RegularExpressions;

namespace Lego.Ev3.Framework.Firmware
{
    internal static class FileSystem
    {
        public const string ROOT_PATH = "../";
        public const string PARENT_PATH = "./";
        public const string DIRECTORY_SEPERATOR = "/";
        public const string NAME = "[ -~]";
        public static readonly string BRICK_FILENAME_EXPRESSION = $@"({NAME})+(\.(\w)+)?";
        public static readonly string BRICK_ROBOT_FILENAME_EXPRESSION = $@"({NAME})+(\.(rsf|rgf|rbf|rtf|rdf|rpf|rcf|raf)+)?";
        public static readonly string BRICK_DIRECTORY_PATH_EXPRESSION = $@"^\.\./(({NAME}+)/)*$";
        public static readonly string BRICK_FILE_PATH_EXPRESSION = $@"^\.\./(({NAME}+)/)*{BRICK_FILENAME_EXPRESSION}$";
        public static readonly string BRICK_ROBOT_FILE_PATH_EXPRESSION = $@"^\.\./(({NAME}+)/)*{BRICK_ROBOT_FILENAME_EXPRESSION}$";
        
        
        public static void IsRobotFilePath(this string brickRobotFilePath)
        {
            if (string.IsNullOrEmpty(brickRobotFilePath)) throw new ArgumentNullException(nameof(brickRobotFilePath));
            if (!Regex.IsMatch(brickRobotFilePath, BRICK_ROBOT_FILE_PATH_EXPRESSION)) throw new ArgumentException("path is not a valid brick robot file path");
        }

        public static void IsBrickFilePath(this string brickFilePath) 
        {
            if (string.IsNullOrEmpty(brickFilePath)) throw new ArgumentNullException(nameof(brickFilePath));
            if (!Regex.IsMatch(brickFilePath, BRICK_FILE_PATH_EXPRESSION)) throw new ArgumentException("path is not a valid brick file path");
        }

        public static void IsBrickPath(this string brickPath)
        {
            if (string.IsNullOrEmpty(brickPath)) throw new ArgumentNullException(nameof(brickPath));
            bool isDirectory = Regex.IsMatch(brickPath, BRICK_DIRECTORY_PATH_EXPRESSION);
            bool isFile = Regex.IsMatch(brickPath, BRICK_FILE_PATH_EXPRESSION);
            if (!isDirectory && !isFile) throw new ArgumentException("path is not a valid brick path");
        }

        public static void IsBrickDirectoryPath(this string brickDirectoryPath)
        {
            if (string.IsNullOrEmpty(brickDirectoryPath)) throw new ArgumentNullException(nameof(brickDirectoryPath));
            if (!brickDirectoryPath.EndsWith("/")) throw new ArgumentException($"brick directory path should end with {DIRECTORY_SEPERATOR}");
            if (!Regex.IsMatch(brickDirectoryPath, BRICK_DIRECTORY_PATH_EXPRESSION)) throw new ArgumentException(nameof(brickDirectoryPath), "path is not a valid brick directory path");
        }

        public static string ToBrickDirectoryPath(string brickDirectoryPath) 
        {
            if (brickDirectoryPath != null && !brickDirectoryPath.EndsWith(DIRECTORY_SEPERATOR)) brickDirectoryPath = $"{brickDirectoryPath}{DIRECTORY_SEPERATOR}";
            return brickDirectoryPath;
        }
    }
}
