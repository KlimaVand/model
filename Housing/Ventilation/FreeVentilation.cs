using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Housing.Animal;
using Housing.Utility;

namespace Housing.Ventilation
{
    public class FreeVentilation : IVentilationStrategi
    {
        double meanWallHeight = 0; //mean height of walls in metres
        double meanWallLength = 0; //mean length of wall in metres
        double wallArea = 0.0; // area of one wall, assume housing is cubic
        double thermalTransWall = 0;
        double emissivity = 0;
        double externSurfResis = 0;
        double absorbCoeff = 0;

        /*  Controlled ventilation only
        */
        double planArea = 0.0; // plan area of house in square metres

        /*  Freely ventilated only
        */
        double minTemperature = 0.0; //the temperature below which we would not like the temperature to fall - Celsius
        double maxTemperature = 0.0; //the temperature above which we would not like the temperature to rise - Celsius
        double apertureWidth = 0.0; //
        double maxapertureHeight = 0.0;
        double minPropApertureHeight = 0.0;
        double thermalTransRoof = 0;
        double optimumAirVelocity = 0;

        /* outputs
        */
        double insideTemperature = 0.0; //inside temperature in Celsius
        double ventilationRate = 0.0; // ventilation rate (cubic metres per second)

        /* get access to utility functions
        */
        IUtility utilities = new Utility.Utility();

        /* create instance of a dummy animal, to provide input
        */
        IAnimalStrategy dummyAnimal = new DummyAnimal(1, 50, 650.0, 25.0, 0, 40.0);

        /* constructor for freely-ventilated housing
        */
        public FreeVentilation(int dumInt, double AmeanWallHeight, double AmeanWallLength, double AthermalTransRoof, double AthermalTransWall, double Aemissivity,
                double AexternSurfResis, double AabsorbCoeff, double AoptimumAirVelocity, double AapertureWidth, double AmaxapertureHeight, double AminPropApertureHeight)
        {
            meanWallHeight = AmeanWallHeight;
            meanWallLength = AmeanWallLength;
            thermalTransRoof = AthermalTransRoof;
            thermalTransWall = AthermalTransWall;
            emissivity = Aemissivity;
            externSurfResis = AexternSurfResis;
            absorbCoeff = AabsorbCoeff;
            optimumAirVelocity = AoptimumAirVelocity;
            apertureWidth = AapertureWidth;
            maxapertureHeight = AmaxapertureHeight;
            minPropApertureHeight = AminPropApertureHeight;
            wallArea = 4 * meanWallLength * meanWallHeight; //equation 1.2
            planArea = Math.Pow(meanWallLength, 2); //equation 1.3
        }

        public void ErrorHandling(int errorNo)
        {
            string ErrorString = "";
            string ErrorString1 = "Error - ";
            string ErrorString2 = " Press any key to exit";
            switch (errorNo)
            {
                case 1:
                    ErrorString += ErrorString1 + "Aperture width exceeds mean wall length " + ErrorString2;
                    Console.WriteLine(ErrorString);
                    Console.ReadKey();
                    break;
                case 2:
                    ErrorString += ErrorString1 + "Minimum temperature exceeds maximum temperature" + ErrorString2;
                    Console.WriteLine(ErrorString);
                    Console.ReadKey();
                    break;
                default: Console.Write("Unknown error");
                    Console.ReadKey();
                    break;
            }
            System.Environment.Exit(1);
        }

        /*  From 4th Report of Working Group on Climatization of Animal Houses Heat and moisture production at animal and house levels Editors: Pedersen, S. & Sällvik, K. 2002
         *  equation 29
         *  returns proportion of livestock heat production that is as sensible heat 
         *  param temperature double Temperature inside animal house, either Celsius or Kelvin
        */
        public double CalcPropSensible(double temperature)
        {
            if (temperature > 173)
                temperature -= 273.15;

            /* equation 1.1
            */
            double ret_val = 0.8 - 0.38 * Math.Pow(temperature, 2) / 1000.0; //divide 0.38 * Math.Pow(temperature, 2) by 1000, as this is scaled to 1000W animal
            ret_val *= 0.85; // assumes wet surfaces, so 0.85
            return ret_val;
        }

        public double Control(double heatOp, double outsideAirTemp, double windspeed, double solarRad, double watervapourPressure, ref double supplementaryHeat)
        {
            if (apertureWidth > meanWallLength)
                ErrorHandling(1);
            if (minTemperature > maxTemperature)
                ErrorHandling(2);

            /* !calculate the amount of sensible heat produced in the animal housing
             * assume that inside temperature is close to outside temperature
            */
            double propSensibleHeat = CalcPropSensible(outsideAirTemp);
            double sensibleHeatOp = propSensibleHeat * heatOp;

            maxTemperature = dummyAnimal.GetmaximumTemperature();
            maxTemperature += 273.15;
            minTemperature += 273.15;
            outsideAirTemp += 273.15;
            double longWave = 5.67E-8 * Math.Pow(outsideAirTemp, 4) * (utilities.GetSkyEmissivity(outsideAirTemp, watervapourPressure) - emissivity); //equation 1.4
            double airDensity = utilities.GetdensityAir(utilities.GetStandardAirPressure(), outsideAirTemp, utilities.GetsaturatedWaterVapourPressure(outsideAirTemp));
            double specificHeatCapAir = utilities.GetspecificHeatCapAir(watervapourPressure, outsideAirTemp);

            /* !tempSol = outside air temperature which, in the absence of solar radiation, would give the same temperature distribution and rate of energy transfer
             * !through a wall or roof as that which exists with the actual air temperature and incident radiation
            */
            double tempSol = outsideAirTemp + externSurfResis * (absorbCoeff * solarRad - emissivity * longWave); // equation 1.5

            /* !Temperature difference between outside surfaces of roof and air temperature
             * used in equation 10 of Cooper et al
            */
            double deltaSol = tempSol - outsideAirTemp;

            /*   // !Heat input or output to housing through the roof material
               double q = thermalTransRoof * planArea * deltaSol;    // Watts
                                                                     // !First set ventilation rate to that which is necessary to have the optimum air velocity
               double targetventilationRate = optimumAirVelocity * planArea;

               // !Calculate the temperature difference that will be achieved by the ventilation with outside air
               double deltaTemper = (sensibleHeatOp + q)/ (specificHeatCapAir * airDensity * targetventilationRate + meanThermalTrans * surfaceArea);

               // !if the temperature achieved obtaining the optimal ventilation rate exceeds the maximum permitted, recalculate the ventilation rate to prevent this
               if ((outsideAirTemp + deltaTemper > maxTemperature) && (outsideAirTemp < maxTemperature))
               {
                   double maxDeltaT = maxTemperature - outsideAirTemp;
                   double heatTransportThroughWalls = maxDeltaT * meanThermalTrans * surfaceArea;

                   targetventilationRate = ((sensibleHeatOp + q) - heatTransportThroughWalls)
                                           / (maxDeltaT * specificHeatCapAir * airDensity);
                   deltaTemper = (sensibleHeatOp + q)
                                 / (specificHeatCapAir * airDensity * targetventilationRate + meanThermalTrans * surfaceArea);
               }
               // !if the temperature achieved obtaining the optimal ventilation rate is below the minimum permitted, recalculate the ventilation rate to prevent this
               if (outsideAirTemp + deltaTemper < minTemperature)
               {
                   double tempDeltaT = minTemperature - outsideAirTemp;
                   double heatTransportThroughWalls = tempDeltaT * meanThermalTrans * surfaceArea;

                   targetventilationRate = ((sensibleHeatOp + q) - heatTransportThroughWalls)
                                           / (tempDeltaT * specificHeatCapAir * airDensity);
                   deltaTemper = (sensibleHeatOp + q)
                                 / (specificHeatCapAir * airDensity * targetventilationRate + meanThermalTrans * surfaceArea);
               }

               // !now we know the target ventilation rate, we need to see if this can be achieved by adjusting the aperture size
               // !assume initially that this is possible
               ventilationRate = targetventilationRate;
               double minVent = 0;
               double tempDeltaTemp = 0;

               // !calculate ventilation with the minimum ventilation
               double apertureHeight = minPropApertureHeight * maxapertureHeight;

               CalcFreeVentilation(airDensity, meanThermalTrans, surfaceArea, apertureHeight, windspeed, outsideAirTemp, watervapourPressure, heatOp,
                                   q, ref minVent, ref tempDeltaTemp);
               // !if the target ventilation is less than the minimum, use the minimum
               if (targetventilationRate < minVent)
               {
                   ventilationRate = minVent;
                   deltaTemper = tempDeltaTemp;
               }

               double maxVent=0;

               // !calculate ventilation with the maximum ventilation
               apertureHeight = maxapertureHeight;
               CalcFreeVentilation(airDensity, meanThermalTrans, surfaceArea, apertureHeight, windspeed, outsideAirTemp, watervapourPressure, heatOp,
                                   q, ref maxVent, ref tempDeltaTemp);

               // !If ventilation exceeds the maximum, use the maximum
               if (targetventilationRate > maxVent)
               {
                   ventilationRate = maxVent;
                   deltaTemper = tempDeltaTemp;
               }

               insideTemperature = outsideAirTemp + deltaTemper - 273.0;*/

            // dummy return
            return 0.0;
        }
    }
}
