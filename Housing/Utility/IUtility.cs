using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Housing.Utility
{
    public interface IUtility
    {
        double GetgravitationalAcceleration();

        double GetdynamicViscosityAir();

        double GetGasConstant();

        double GetCconcInOrgmatter();

        double GetStandardAirPressure();

        double GetFrictionVelocity(double height, double displacementHt, double zeroPlane, double windVelocity);

        void SolveCubic(double a, double b, double c, double d, ref int solutions, ref double[] x);

        double GetspecificHeatCapAir(double waterVapourPressure, double absoluteTemperture);

        double GetdensityDryAir(double airPressure, double airTemperature);

        double GetdensityAir(double airPressure, double temperature, double vapourPressure);

        double GetdensityWaterVapour(double waterVapourPressure, double absoluteTemperture);

        double GetsaturatedWaterVapourPressure(double airTemperature);

        double GetSkyTemperature(double airTemperature);

        double GetSkyEmissivity(double theairTemperature, double thewaterVapourPressure);

        double GetlongWaveDown(double theairTemperature, double thewaterVapourPressure);

        double GetlongWaveUp(double emissivity, double aTemperature);

        bool SolveQuadratic(bool posRoot, double a, double b, double c, ref double x);

        double GetLatentHeatVaporisationWater(double temperature);

        double GetWindAtHeight(double origHt, double origWind, double newHt, double roughnessLength);

        double GetGrossEnergyinDryMatter();

        double GetNetLongWave(double airTemperature, double waterVapourPressure, double surfaceTemp, double emissivity);
    }
}
