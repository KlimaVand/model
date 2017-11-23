using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Housing.Utility;

namespace UnitTestHousing
{
    [TestClass]
    public class UnitTestUtility
    {
        IUtility utilities = new Utility();

        //// Test GetsaturatedWaterVapourPressure() - 50 to + 50 Celsius
        //[TestMethod]
        //public void GetsaturatedWaterVapourPressureReturnsFalseInputLeesThanLimit()
        //{
        //    //Assert.AreEqual(0.0, utilities.GetsaturatedWaterVapourPressure(0.0));
        //    //Assert.AreEqual(0.0, utilities.GetsaturatedWaterVapourPressure(-60.0));
        //}

        //// Test GetdensityAir() - 85000 to 108000, -50 to + 50 Celsius, 0.6 to 13.0
        //[TestMethod]
        //public void GetdensityAirShouldReturnSomething()
        //{
        //    //Assert.AreEqual(0.0, utilities.GetdensityAir(0.0, 0.0, 0.0));
        //}

        //// Test GetStandardAirPressure() - No inputs required
        //[TestMethod]
        //public void GetStandardAirPressureShouldReturnSomething()
        //{
        //    //Assert.AreEqual(0.0, utilities.GetStandardAirPressure());
        //}

        //// Test  GetSkyEmissivity() - -50 to + 50, 0.6 to 13.0
        //[TestMethod]
        //public void GetSkyEmissivityShouldReturnSomething()
        //{
        //    //Assert.AreEqual(0.0, utilities.GetSkyEmissivity(0.0, 0.0));
        //}

        //// Test  GetspecificHeatCapAir() - 0.6 to 13.0, 220 to 325
        //[TestMethod]
        //public void GetspecificHeatCapAirShouldReturnSomething()
        //{
        //    //Assert.AreEqual(0.0, utilities.GetspecificHeatCapAir(0.0, 0.0));
        //}

        //// Test  SolveCubic() - For any cubic equation of the type ax**3 + bx**2 + cx + d=0, a, b and c are the coefficients, solutions is the number of roots (results) and the array contains the valid roots.
        //[TestMethod]
        //public void SolveCubicShouldReturnSomething()
        //{
        //    //Assert.AreEqual(0.0, utilities.SolveCubic());
        //}
    }
}
