using System;
using System.Security.Cryptography.X509Certificates;
using JobSpawn.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JobSpawn.ILTests
{
    [TestClass]
    public class ILTests
    {
        [TestMethod]
        public void TestOne()
        {
            ITestOne testOne = new SpawnProxy().As<ITestOne>();
            Assert.AreEqual(3, testOne.GetNumber());
        }

        public class TestOneClass : ITestOne
        {
            public int GetNumber()
            {
                return 3;
            }
        }

        public interface ITestOne
        {
            int GetNumber();
        }

        [TestMethod]
        public void TestTwo()
        {
            ITestTwo testTwo = new SpawnProxy().As<ITestTwo>();
            Assert.AreEqual(4, testTwo.GetNumber(4));
        }

        public class TestTwoClass : ITestTwo
        {
            public int GetNumber(int number)
            {
                return number;
            }
        }

        public interface ITestTwo
        {
            int GetNumber(int number);
        }

        [TestMethod]
        public void TestThree()
        {
            ITestThree testThree = new SpawnProxy().As<ITestThree>();
            Assert.AreEqual(9, testThree.GetNumber(4, 5));
        }

        public class TestThreeClass : ITestThree
        {
            public int GetNumber(int number, int numberTwo)
            {
                return number + numberTwo;
            }
        }

        public interface ITestThree
        {
            int GetNumber(int number, int numberTwo);
        }

        [TestMethod]
        public void TestFour()
        {
            ITestFour testFour = new SpawnProxy().As<ITestFour>();
            Assert.AreEqual(1, testFour.ReturnFirstInt(1, 2, 3));
        }

        public class TestFourClass : ITestFour
        {
            public int ReturnFirstInt(int numberOne, int numberTwo, int numberThree)
            {
                return 0;
            }
        }

        public interface ITestFour
        {
            int ReturnFirstInt(int numberOne, int numberTwo, int numberThree);
        }

        [TestMethod]
        public void TestFive()
        {
            ITestFive testFive = new SpawnProxy().As<ITestFive>();
            Assert.AreEqual("[1,2,3]", testFive.SerialiseParameters(1, 2, 3));
        }

        public class TestFiveClass : ITestFive
        {
            public string SerialiseParameters(int numberOne, int numberTwo, int numberThree)
            {
                object[] array = { numberOne, numberTwo, numberThree };
                return JsonConvert.SerializeObject(array);
            }
        }

        public interface ITestFive
        {
            string SerialiseParameters(int numberOne, int numberTwo, int numberThree);
        }
    }
}
