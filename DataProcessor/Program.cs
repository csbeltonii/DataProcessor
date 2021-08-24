using System;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using static System.Console;
using System.IO;
using System.Runtime.Caching;
using System.Threading;

namespace DataProcessor
{
    class Program
    {

        //private static ConcurrentDictionary<string, string> FilesToProcess = new ConcurrentDictionary<string, string>();

        private static MemoryCache FilesToProcess = MemoryCache.Default;

        static void Main(string[] args)
        {
            WriteLine("Parsing command line options.");

            //var command = args[0];

            //if (command == "--file")
            //{
            //    var filePath = args[1];
            //    WriteLine($"Single file {filePath} selected");
            //    ProcessSingleFile(filePath);
            //}
            //else if (command == "--dir")
            //{
            //    var directoryPath = args[1];
            //    var fileType = args[2];
            //    WriteLine($"Directory {directoryPath} selected for {fileType} files.");
            //    ProcessDirectory(directoryPath, fileType);
            //}
            //else
            //{
            //    WriteLine("Invalid command line option.");
            //}

            //WriteLine("Please enter to quit.");
            //ReadLine();

            var directoryToWatch = args[0];

            if (!Directory.Exists(directoryToWatch))
            {
                WriteLine($"ERROR: {directoryToWatch} does not exist.");
            }
            else
            {
                WriteLine($"Watching directory {directoryToWatch} for changes");

                ProcessExistingFiles(directoryToWatch);

                using var inputFileWatch = new FileSystemWatcher(directoryToWatch);

                inputFileWatch.IncludeSubdirectories = false;
                inputFileWatch.InternalBufferSize = 32768; // 32 KB
                inputFileWatch.Filter = "*.*"; // default filter
                inputFileWatch.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

                inputFileWatch.Created += FileCreated;
                inputFileWatch.Changed += FileChanged;
                inputFileWatch.Deleted += FileDeleted;
                inputFileWatch.Renamed += FileRenamed;
                inputFileWatch.Error += WatcherError;

                inputFileWatch.EnableRaisingEvents = true;

                WriteLine("Press enter to quit.");
                ReadLine();
            }
        }

        private static void FileCreated(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File created: {e.Name} - type: {e.ChangeType}");

            //var fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            AddToCache(e.FullPath);
        }

        private static void FileChanged(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File changed: {e.Name} - type: {e.ChangeType}");

            //var fileProcessor = new FileProcessor(e.FullPath);
            //fileProcessor.Process();

            AddToCache(e.FullPath);
        }

        private static void FileDeleted(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File delete: {e.Name} - type: {e.ChangeType}");
        }

        private static void FileRenamed(object sender, RenamedEventArgs e)
        {
            WriteLine($"* File renamed: {e.OldName} to {e.Name} - type: {e.ChangeType}");
        }

        private static void WatcherError(object sender, ErrorEventArgs e)
        {
            WriteLine($"ERROR: file system watching may no longer be action: {e.GetException()}");
        }

        private static void ProcessSingleFile(string filePath)
        {
            var fileProcessor = new FileProcessor(filePath);
            fileProcessor.Process();
        }

        private static void ProcessDirectory(string directoryPath, string fileType)
        {
            //var allFiles = Directory.GetFiles(directoryPath); // files

            switch (fileType)
            {
                case "TEXT":
                    var textFile = Directory.GetFiles(directoryPath, "*.txt");

                    foreach (var textFilePath in textFile)
                    {
                        var fileProcessor = new FileProcessor(textFilePath);
                        fileProcessor.Process();
                    }
                    break;
                default:
                    WriteLine($"ERROR: {fileType} is not supported.");
                    return;
            }
        }

        //private static void ProcessFiles(Object stateInfo)
        //{
        //    foreach (var fileName in FilesToProcess.Keys)
        //    {
        //        if (FilesToProcess.TryRemove(fileName, out _))
        //        {
        //            var fileProcessor = new FileProcessor(fileName);
        //            fileProcessor.Process();
        //        }
        //    }
        //}

        private static void AddToCache(string fullPath)
        {
            var item = new CacheItem(fullPath, fullPath);

            var policy = new CacheItemPolicy
            {
                RemovedCallback = ProcessFile,
                SlidingExpiration = TimeSpan.FromSeconds(2) // must specify a value higher than one second
            };

            FilesToProcess.Add(item, policy);
        }

        private static void ProcessFile(CacheEntryRemovedArguments args)
        {
            WriteLine($"* Cache item removed: {args.CacheItem.Key} because {args.RemovedReason}.");

            if (args.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                var fileProcessor = new FileProcessor(args.CacheItem.Key);
                fileProcessor.Process();
            }
            else
            {
                WriteLine($"WARNING: {args.CacheItem.Key} was removed unexpectedly and may not be processed.");
            }
        }

        private static void ProcessExistingFiles(string inputDirectory)
        {
            WriteLine($"Check {inputDirectory} for existing files");

            foreach (var filePath in Directory.EnumerateFiles(inputDirectory))
            {
                WriteLine($"    - Found {filePath}");
                AddToCache(filePath);
            }
        }
    }
}
