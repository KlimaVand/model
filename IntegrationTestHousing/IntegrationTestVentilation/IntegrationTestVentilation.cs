using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Housing.Housing;
using Housing.Ventilation;
using Housing.Utility;
using Housing.Animal;

namespace IntegrationTestHousing.IntegrationTestVentilation
{
    [TestClass]
    public class IntegrationTestVentilation
    {
        private IHousing ho;

        [TestMethod]
        public void TestForcedVentilationIntegration()
        {
            // Configure Animal Housing to be the Forced Ventilated Animal Housing
            ho = new AnimalHousing(new ForcedVentilation(6.0, 22.0, 1.5, 5.0, 0.8, 0.04, 0.8, 293.0, 2.0, 4.0, 10000.0), new Utility(), new DummyAnimal(1, 50, 650.0, 25.0, 0, 40.0));
            ho.Ventilation(1);
            Assert.AreEqual(0.0041322314049586778, ho.getVelocity());
        }

        //[TestMethod]
        //public void TestFreeVentilationIntegration()
        //{
        //    // Configure Animal Housing to be the Free Ventilated Animal Housing
        //}
    }
}
