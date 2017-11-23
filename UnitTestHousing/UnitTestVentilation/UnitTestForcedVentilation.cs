using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Housing.Ventilation;
using Housing.Animal;
using Housing.Utility;


namespace UnitTestHousing
{
    [TestClass]
    public class UnitTestForcedVentilation
    {
        IVentilationStrategi vent;
        IAnimalStrategy anim;
        IUtility util;

        // Test Control method
        [TestMethod]
        public void ControlShouldReturnVelocity()
        {
            double supplementaryHeat = 0.0;
            vent = new ForcedVentilation(6.0, 22.0, 1.5, 5.0, 0.8, 0.04, 0.8, 293.0, 2.0, 4.0, 10000.0);
            anim = new DummyAnimal(1, 50, 650.0, 25.0, 0, 40.0);
            util = new Utility();
            Assert.AreEqual(0.0041322314049586778, vent.Control(anim.HeatProduction(), 10.0, 0.0, 600.0, 1.0 * util.GetsaturatedWaterVapourPressure(10.0), ref supplementaryHeat));
        }
    }
}
