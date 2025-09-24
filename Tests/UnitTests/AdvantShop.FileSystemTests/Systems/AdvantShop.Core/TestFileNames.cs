using System.Collections.Generic;
using System.IO;
using AdvantShop.FileSystemTests.Helpers;
using NUnit.Framework;

namespace AdvantShop.FileSystemTests.Systems.AdvantShop.Core
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class TestFileNames
    {
        [Test]
        public void FileNames_ShouldNotContainsRussianSymbols()
        {
            // Arrange
            var coreDirectory = new FileInfo(Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\AdvantShop.Core\")).Directory!;
            // Assert
            RussianSymbolsHelper.CheckFilesInDirectoryForRussianSymbolsInName(coreDirectory, new List<string>());
        }
    }
}
