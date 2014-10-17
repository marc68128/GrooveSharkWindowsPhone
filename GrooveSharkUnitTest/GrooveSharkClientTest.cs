using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Helpers;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace GrooveSharkUnitTest
{
    [TestClass]
    public class GrooveSharkClientTest
    {
        [TestMethod]
        public void TestHmacMd5()
        {
            string hash = HmacMd5.Hash("secret",
                "{'method': 'addUserFavoriteSong', 'parameters': {'songID': 0}, 'header': {'wsKey': 'key', 'sessionID': 'sessionID'}}");
            Assert.AreEqual("cd3ccc949251e0ece014d620bbf306e7", hash);
        }
    }
}
