using System.IO;
using AdvantShop.FileSystemTests.Helpers;
using NUnit.Framework;

namespace AdvantShop.FileSystemTests.Systems.AdvantShop.Infrastructure
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class TestFolderNames
    {
        [Test]
        public void FolderNames_ShouldNotContainsRussianSymbols()
        {
            // Arrange
            var infrastructureDirectory = new FileInfo(Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\AdvantShop.Web.Infrastructure\")).Directory!;
            // Assert
            RussianSymbolsHelper.CheckDirectoryForRussianSymbolsInName(infrastructureDirectory);
        }
    }
}
