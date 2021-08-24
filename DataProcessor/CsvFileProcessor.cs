using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using CsvHelper;
using CsvHelper.Configuration;

namespace DataProcessor
{
    public class CsvFileProcessor
    {
        private readonly IFileSystem _fileSystem;

        public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }

        public CsvFileProcessor(string inputFilePath, string outputFilePath, IFileSystem fileSystem)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            _fileSystem = fileSystem;
        }

        public CsvFileProcessor(string inputFilePath, string outputFilePath)
            :this(inputFilePath, outputFilePath, new FileSystem())
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }

        public void Process()
        {
            using var input = _fileSystem.File.OpenText(InputFilePath);
            using var output = _fileSystem.File.CreateText(OutputFilePath);

            var configuration = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                TrimOptions = TrimOptions.Trim,
                Comment = '@',
                AllowComments = true,
                //Delimiter = ";"
            };


            using var csvReader = new CsvReader(input, 
                configuration);
            using var csvWriter = new CsvWriter(output, CultureInfo.InvariantCulture);

            csvReader.Context.RegisterClassMap<ProcessedOrderMap>();

            var records = csvReader.GetRecords<ProcessedOrder>();

            csvWriter.WriteRecords(records);
        }
    }
}
