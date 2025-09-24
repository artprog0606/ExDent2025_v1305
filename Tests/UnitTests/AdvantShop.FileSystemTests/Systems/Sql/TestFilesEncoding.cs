using System;
using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.FileSystemTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace AdvantShop.FileSystemTests.Systems.Sql
{
    [NonParallelizable]
    [TestFixture]
    public class TestFilesEncoding
    {
        [Test]
        public void SqlFiles_ShouldBeInUtf8WithBom()
        {
            // Arrange
            var testFailed = false;
            // Act
            var patchDirectory = Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\DataBase\patches");
            var dbDirectory = Path.Combine(DirectoryHelper.GetRootDirectory?.FullName + @"\DataBase\");

            foreach (var sqlPatchPath in Directory.GetFiles(dbDirectory).Concat(Directory.GetFiles(patchDirectory)).Where(x => Path.GetExtension(x) == ".sql"))
            {
                var fileEncoding = EncodingHelper.GetEncoding(sqlPatchPath);
                if (Equals(fileEncoding, Encoding.UTF8))
                    if (EncodingHelper.CheckBom(sqlPatchPath))
                        continue;

                if (fileEncoding != null)
                    Console.Error.WriteLine($"{sqlPatchPath} - wrong encoding - {fileEncoding.EncodingName}{(Equals(fileEncoding, Encoding.UTF8) ? " with no BOM" : string.Empty)}");

                testFailed = true;
            }

            // Assert
            testFailed.Should().BeFalse();
        }
    }
}
