using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvantShop.FileSystemTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace AdvantShop.FileSystemTests.Systems.AdvantShop.Infrastructure
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class TestFilesEncoding
    {
        [Test]
        [Parallelizable(ParallelScope.All)]
        [TestCase(".cs")]
        [TestCase(".csproj")]
        [TestCase(".config")]
        public void Files_ShouldBeInUTF8(string fileExtension)
        {
            // Arrange
            var infrastructureDirectory = new FileInfo(Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\AdvantShop.Web.Infrastructure\")).Directory!;
            var encodings = new List<Encoding>
            {
                Encoding.UTF8
            };
            var (success, numberOfCheckedFiles, errors) = EncodingHelper.CheckFilesEncodingInDirectory(infrastructureDirectory, encodings, fileExtension, new List<string>());
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
