using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Housing.Animal;

namespace UnitTestHousing
{
    [TestClass]
    public class UnitTestDummyAnimal
    {
        IAnimalStrategy a = new DummyAnimal(1, 50, 650.0, 25.0, 0, 40.0);

        //// Test HeatProduction()
        //[TestMethod]
        //public void HeatProductionshouldReturnSomething()
        //{
        //    //Assert.AreEqual(0.0, a.HeatProduction());
        //}

        //// Test GetHeatProduction()
        //[TestMethod]
        //public void GetHeatProductionShouldReturnSomething()
        //{
        //    //Assert.AreEqual(0.0, a.GetHeatProduction());
        //}
    }
}
