using System;
using static System.Console;
using System.IO;

namespace DataProcessor
{
    internal class FileProcessor
    {
        public string InputFilePath { get; }

        private static readonly string BackupDirectoryName = "backup";
        private static readonly string InProgressDirectoryName = "processing";
        private static readonly string CompletedDirectoryName = "complete";

        public FileProcessor(string filePath)
        {
            InputFilePath = filePath;
        }

        public void Process()
        {
            WriteLine($"Begin process of {InputFilePath}");

            // Check if file exists
            if (!File.Exists(InputFilePath))
            {
                WriteLine($"ERROR: File {InputFilePath} does not exist.");
                return;
            }

            var rootDirectoryPath = new DirectoryInfo(InputFilePath).Parent.Parent.FullName;

            WriteLine($"Root data path is {rootDirectoryPath}");

            // check if backup dir exists
            var inputFileDirectoryPath = Path.GetDirectoryName(InputFilePath);
            var backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);
            
            Directory.CreateDirectory(backupDirectoryPath);

            // copy file to backup
            var inputFileName = Path.GetFileName(InputFilePath);

            var backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);

            WriteLine($"Copying {inputFileName} to {backupFilePath}");

            File.Copy(InputFilePath, backupFilePath, true);

            // move to progress dir
            Directory.CreateDirectory(Path.Combine(rootDirectoryPath, InProgressDirectoryName));

            var inProgressFilePath = Path.Combine(rootDirectoryPath, InProgressDirectoryName, inputFileName);

            if (File.Exists(inProgressFilePath))
            {
                WriteLine($"ERROR: A file with name{inProgressFilePath} is already being processed.");
                return;
            }

            WriteLine($"Moving {InputFilePath} to {inProgressFilePath}");
            File.Move(InputFilePath, inProgressFilePath);

            // Determine type of file
            var extension = Path.GetExtension(InputFilePath);

            var completedDirectoryPath = Path.Combine(rootDirectoryPath, CompletedDirectoryName);
            Directory.CreateDirectory(completedDirectoryPath);

            var completedFileName = $"{Path.GetFileNameWithoutExtension(InputFilePath)}--{Guid.NewGuid()}{extension}";

            var completedFilePath = Path.Combine(completedDirectoryPath, completedFileName);

            switch (extension)
            {
                case ".txt":
                    var textProcessor = new TextFileProcessor(inProgressFilePath, completedFilePath);
                    textProcessor.Process();
                    break;
                case ".data":
                    var binaryProcessor = new BinaryFileProcessor(inProgressFilePath, completedFilePath);
                    binaryProcessor.Process();
                    break;
                case ".csv":
                    var csvProcessor = new CsvFileProcessor(inProgressFilePath, completedFilePath);
                    csvProcessor.Process();
                    break;
                default:
                    WriteLine($"{extension} is an unsupported file type.");
                    break;
            }



            WriteLine($"Deleting {inProgressFilePath}");
            File.Delete(inProgressFilePath);
        }
    }
}