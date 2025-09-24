using System.Collections.Generic;
using System.IO;
using AdvantShop.FileSystemTests.Helpers;
using NUnit.Framework;

namespace AdvantShop.FileSystemTests.Systems.AdvantShop.Web
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class TestFileNames
    {
        [Test]
        public void FileNames_ShouldNotContainsRussianSymbols()
        {
            // Arrange
            var fileWhiteList = new List<string>
            {
                "сartMiniFooter.js"
            };
            var webDirectory = new FileInfo(Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\AdvantShop.Web\")).Directory!;
            // Assert
            RussianSymbolsHelper.CheckFilesInDirectoryForRussianSymbolsInName(webDirectory, fileWhiteList);
        }
    }
}
