using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvantShop.FileSystemTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace AdvantShop.FileSystemTests.Systems.AdvantShop.Web
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class TestFilesEncoding
    {
        private readonly Dictionary<string, List<string>> _filesToIgnore = new()
        {
            {
                ".json", new List<string>
                {
                    "package-lock.json",
                    "package.json",
                    ".stylelintrc.json"
                }
            }
        };

        [Test]
        [TestCase(".cs")]
        [TestCase(".csproj")]
        [TestCase(".cshtml")]
        [TestCase(".config")]
        [TestCase(".json")]
        //[TestCase(".js")]
        //[TestCase(".html")]
        public void Files_ShouldBeInUTF8OrASCII(string fileExtension)
        {
            // Arrange
            _filesToIgnore.TryGetValue(fileExtension, out var filesToIgnore);
            var webDirectory = new FileInfo(Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\AdvantShop.Web\")).Directory!;
            var encodings = new List<Encoding>
            {
                Encoding.UTF8,
                Encoding.ASCII
            };
            var (success, numberOfCheckedFiles, errors) =
                EncodingHelper.CheckFilesEncodingInDirectory(webDirectory, encodings, fileExtension, filesToIgnore ?? new List<string>());
            // Act
            if (success is false)
                foreach (var error in errors)
                    Console.WriteLine(error);
            Console.WriteLine($"Number for checked files - {numberOfCheckedFiles}");
            // Assert
            success.Should().BeTrue();
        }
    }
}
