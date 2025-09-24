using System.IO;

namespace AdvantShop.FileSystemTests.Helpers
{
    public static class DirectoryHelper
    {
        public static DirectoryInfo? GetRootDirectory => Directory.GetParent(
                Directory.GetCurrentDirectory()// `.netVesion`
            )// Release
            ?.Parent// bin
            ?.Parent// AdvantShop.FileSystemTests
            ?.Parent// UnitTests
            ?.Parent// Tests
            ?.Parent;// root
    }
}
