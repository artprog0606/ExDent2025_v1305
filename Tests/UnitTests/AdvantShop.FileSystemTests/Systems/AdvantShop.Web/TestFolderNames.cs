using System.IO;
using AdvantShop.FileSystemTests.Helpers;
using NUnit.Framework;

namespace AdvantShop.FileSystemTests.Systems.AdvantShop.Web
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class TestFolderNames
    {
        [Test]
        public void FolderNames_ShouldNotContainsRussianSymbols()
        {
            // Arrange
            var webDirectory = new FileInfo(Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\AdvantShop.Web\")).Directory!;
            // Assert
            RussianSymbolsHelper.CheckDirectoryForRussianSymbolsInName(webDirectory);
        }
    }
}
