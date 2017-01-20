using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PTE_APP;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1 
    {
        [TestMethod]
        public void TestMethod1()
        {
            //Arrange
            MainActivity att = new MainActivity();
            //Act

            Assert.AreEqual(1, 0 + 1);
        }
    }
}
