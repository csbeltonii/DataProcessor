using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DataProcessor
{
    public class BinaryFileProcessor
    {
        private readonly IFileSystem _fileSystem;

        public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }

        public BinaryFileProcessor(string inputFilePath, string outputFilePath) : this(inputFilePath, outputFilePath, new FileSystem())
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }

        public BinaryFileProcessor(string inputFilePath, string outputFilePath, IFileSystem fileSystem)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            _fileSystem = fileSystem;
        }

        //public void Process()
        //{
        //    var data = File.ReadAllBytes(InputFilePath);

        //    var largest = data.Max();

        //    var newData = new byte[data.Length + 1];

        //    Array.Copy(data, newData, data.Length);

        //    newData[newData.Length - 1] = largest;

        //    File.WriteAllBytes(OutputFilePath, newData);
        //}

        public void Process()
        {
            using var inputFileStream = _fileSystem.File.Open(InputFilePath, 
                FileMode.Open, 
                FileAccess.Read);

            using var binaryStreamReader = new BinaryReader(inputFileStream);
            using var outputFileStream = _fileSystem.File.Create(OutputFilePath);
            using var binaryWriter = new BinaryWriter(outputFileStream);

            // with input/output File.Open
            //const int endOfStream = -1;

            //var largestByte = 0;

            //var currentByte = inputFileStream.ReadByte();

            //while (currentByte != endOfStream)
            //{
            //    outputFileStream.WriteByte((byte)currentByte);

            //    if (currentByte > largestByte)
            //    {
            //        largestByte = currentByte;
            //    }

            //    currentByte = inputFileStream.ReadByte();
            //}

            //outputFileStream.WriteByte((byte)largestByte);

            var largest = 0;

            while (binaryStreamReader.BaseStream.Position < binaryStreamReader.BaseStream.Length)
            {
                var currentByte = binaryStreamReader.ReadByte();

                binaryWriter.Write(currentByte);

                if (currentByte > largest)
                {
                    largest = currentByte;
                }

                binaryWriter.Write(largest);
            }
        }
    }
}