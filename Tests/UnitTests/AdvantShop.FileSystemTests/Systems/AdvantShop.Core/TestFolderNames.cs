using System.IO;
using AdvantShop.FileSystemTests.Helpers;
using NUnit.Framework;

namespace AdvantShop.FileSystemTests.Systems.AdvantShop.Core
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class TestFolderNames
    {
        [Test]
        public void FolderNames_ShouldNotContainsRussianSymbols()
        {
            // Arrange
            var coreDirectory = new FileInfo(Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\AdvantShop.Core\")).Directory!;
            // Assert
            RussianSymbolsHelper.CheckDirectoryForRussianSymbolsInName(coreDirectory);
        }
    }
}
