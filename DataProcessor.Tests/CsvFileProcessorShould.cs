using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DataProcessor.Tests
{
    public class CsvFileProcessorShould
    {
        [Fact]
        public void OutputProcessorOrderCsvData()
        {
            const string inputDir = @"c:\root\in";
            const string inputFileName = "myfile.csv";
            var inputFilePath = Path.Combine(inputDir, inputFileName);

            const string outputDir = @"c:\root\out";
            const string outputFileName = "myfileout.csv";
            var outputFilePath = Path.Combine(outputDir, outputFileName);

            var csvLines = new StringBuilder();

            csvLines.AppendLine("OrderNumber,CustomerNumber,Description,Quantity");
            csvLines.AppendLine("42, 100001, Shirt, II");
            csvLines.AppendLine("42, 20002, Shorts, I");
            csvLines.Append("@ This is a comment");
            csvLines.AppendLine("");
            csvLines.Append("44, 30003, Cap, V");


        }
    }
}
