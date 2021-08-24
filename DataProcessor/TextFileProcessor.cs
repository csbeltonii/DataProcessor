using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

using System.IO.Abstractions;

namespace DataProcessor
{ 
    public class TextFileProcessor
    {
        private readonly IFileSystem _fileSystem;

        public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }

        public TextFileProcessor(string inputFilePath, string outputFilePath) :
            this(inputFilePath, outputFilePath, new FileSystem())
        {

        }

        public TextFileProcessor(string inputFilePath, string outputFilePath, IFileSystem fileSystem)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            _fileSystem = fileSystem;
        }

        public void Process()
        {
            // using read all text
            //var originalText = File.ReadAllText(InputFilePath);

            //var processedText = originalText.ToUpperInvariant();

            //File.WriteAllText(OutputFilePath, processedText);

            // using read all lines
            //var lines = File.ReadAllLines(InputFilePath);

            //lines[1] = lines[1].ToUpperInvariant();

            //File.WriteAllLines(OutputFilePath, lines);

            using var inputStreamReader = _fileSystem.File.OpenText(InputFilePath);
            using var outputStreamWriter = _fileSystem.File.CreateText(OutputFilePath);

            //while (!inputStreamReader.EndOfStream)
            //{
            //    var line = inputStreamReader.ReadLine();

            //    var processedLine = line!.ToUpperInvariant();

            //    outputStreamWriter.WriteLine(processedLine);

            //    bool isLastline = inputStreamReader.EndOfStream;

            //    if (isLastline)
            //    {
            //        outputStreamWriter.Write(processedLine);
            //    }
            //    else
            //    {
            //        outputStreamWriter.WriteLine(processedLine);
            //    }
            //}

            var currentLineNumber = 1;

            while (!inputStreamReader.EndOfStream)
            {
                var line = inputStreamReader.ReadLine();

                if (currentLineNumber == 2)
                {
                    Write(line.ToUpperInvariant());
                }
                else
                {
                    Write(line);
                }

                currentLineNumber++;

                void Write(string content)
                {
                    var isLastLine = inputStreamReader.EndOfStream;

                    if (isLastLine)
                    {
                        outputStreamWriter.Write(content);
                    }
                    else
                    {
                        outputStreamWriter.WriteLine(content);
                    }
                }
            }
        }
    }
}
