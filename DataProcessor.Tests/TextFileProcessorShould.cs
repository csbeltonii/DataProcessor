using System;
using Xunit;
using System.IO.Abstractions.TestingHelpers;

namespace DataProcessor.Tests
{
    public class TextFileProcessorShould
    {
        [Fact]
        public void MakeSecondLineUpperCase()
        {
            // create mock input file
            var mockInputFile = new MockFileData("Line 1\nLine 2\nLine 3");
            
            // setup mock file system starting state
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(@"C:\root\in\myfile.txt", mockInputFile);
            mockFileSystem.AddDirectory(@"C:\root\out");

            // create TextFileProcessor with mock file system
            var sut = new TextFileProcessor(@"C:\root\in\myfile.txt",
                @"C:\root\out\myfile.txt",
                mockFileSystem);

            // processor test file
            sut.Process();


            // check mock file system for output file
            Assert.True(mockFileSystem.FileExists(@"C:\root\out\myfile.txt"));

            // check content of output file in mock file system
            var processedFile = mockFileSystem.GetFile(@"C:\root\out\myfile.txt");

            var lines = processedFile.TextContents.SplitLines();

            Assert.Equal("Line 1", lines[0]);
            Assert.Equal("LINE 2", lines[1]);
            Assert.Equal("Line 3", lines[2]);
        }
    }
}
