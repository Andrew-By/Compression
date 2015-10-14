using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Compression;
using System.Text;

namespace CompressionTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCompression()
        {

            string testString = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";

            Archiver Arc = new Archiver();
            Arc.LoadText(testString);
            Arc.Compress();
            Assert.AreEqual(Arc.Decompress(), testString);

        }
    }
}
