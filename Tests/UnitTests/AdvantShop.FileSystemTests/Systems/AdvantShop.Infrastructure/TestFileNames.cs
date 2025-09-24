using System.Collections.Generic;
using System.IO;
using AdvantShop.FileSystemTests.Helpers;
using NUnit.Framework;

namespace AdvantShop.FileSystemTests.Systems.AdvantShop.Infrastructure
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class TestFileNames
    {
        [Test]
        public void FileNames_ShouldNotContainsRussianSymbols()
        {
            // Arrange
            var infrastructureDirectory = new FileInfo(Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\AdvantShop.Web.Infrastructure\")).Directory!;
            // Assert
            RussianSymbolsHelper.CheckFilesInDirectoryForRussianSymbolsInName(infrastructureDirectory, new List<string>());
        }
    }
}
