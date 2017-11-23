using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Housing.Animal;
using Housing.Utility;

namespace Housing.Ventilation
{
    public class ForcedVentilation : IVentilationStrategi
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
        double minVentilation = 0.0; // min ventilation rate in cubic metres per sec - controlled ventilation only
        double maxVentilation = 0.0; // max ventilation rate in cubic metres per sec - controlled ventilation only
        double targetTemperature = 0; //this is the temperature that we would like to maintain - Celsius
        double maxSupplementaryHeat = 0; //maximum heating capacity in house in kW
        double airVelocity = 0.0;

        /*  Freely ventilated only
        */
        double thermalTransRoof = 0;

        /* outputs
        */
        double insideTemperature = 0.0; //inside temperature in Celsius
        double ventilationRate = 0.0; // ventilation rate (cubic metres per second)

        /* get access to utility functions
        */
        IUtility utilities = new Utility.Utility();

        /* create instance of a dummy animal, to provide input
        */

        /* constructor for controlled ventilated housing
        */
        public ForcedVentilation(double AmeanWallHeight, double AmeanWallLength, double AthermalTransRoof, double AthermalTransWall, double Aemissivity,
            double AexternSurfResis, double AabsorbCoeff, double AtargetTemperature, double AminVentilation, double AmaxVentilation, double AmaxSupplementaryHeat)
        {
            meanWallHeight = AmeanWallHeight;
            meanWallLength = AmeanWallLength;
            thermalTransRoof = AthermalTransRoof;
            thermalTransWall = AthermalTransWall;
            emissivity = Aemissivity;
            externSurfResis = AexternSurfResis;
            absorbCoeff = AabsorbCoeff;
            minVentilation = AminVentilation;
            maxVentilation = AmaxVentilation;
            targetTemperature = AtargetTemperature;
            planArea = Math.Pow(meanWallLength, 2);
            wallArea = 4 * meanWallLength * meanWallHeight;
            maxSupplementaryHeat = AmaxSupplementaryHeat;
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

            /* !calculate the amount of sensible heat produced in the animal housing
             * assume that inside temperature is the target temperature
            */
            double airVelocity = 0.0;
            double propSensibleHeat = CalcPropSensible(targetTemperature);
            double sensibleHeatOp = propSensibleHeat * heatOp;
            double surfaceArea = planArea + wallArea; // sq meters - surface area of house
            double tmeanThermalTrans = (planArea * thermalTransRoof + wallArea * thermalTransWall) / surfaceArea; // in W per metre square per K

            outsideAirTemp += 273.13; //convert from Celsius to Kelvin

            double deltaTemper = 0.0;
            supplementaryHeat = 0.0;
            ventilationRate = 0.0;
            double airDensity = utilities.GetdensityAir(utilities.GetStandardAirPressure(), outsideAirTemp, utilities.GetsaturatedWaterVapourPressure(outsideAirTemp));
            double longWave = 5.67E-8 * Math.Pow(outsideAirTemp, 4) * (utilities.GetSkyEmissivity(outsideAirTemp, watervapourPressure) - emissivity); //equation 1.4

            /* !tempSol = outside air temperature which, in the absence of solar radiation, would give the same temperature distribution and rate of energy transfer
             * !through a wall or roof as that which exists with the actual air temperature and incident radiation
            */
            double tempSol = outsideAirTemp + externSurfResis * (absorbCoeff * solarRad - emissivity * longWave); // equation 1.5
            double specificHeatCapAir = utilities.GetspecificHeatCapAir(watervapourPressure, outsideAirTemp); // used in equation 1.8 ("c")

            /* Note - it is possible that the following can return a negative ventilation rate, 
             * if the heat produced can all be disappated through the walls and roof
            */
            if (targetTemperature > outsideAirTemp)
            {

                /* it will be possible to maintain target temp
                 * equation 1.8
                */
                ventilationRate = (sensibleHeatOp - thermalTransRoof * planArea * (outsideAirTemp - tempSol) - wallArea * thermalTransWall * (targetTemperature - outsideAirTemp)) /
                                   (specificHeatCapAir * airDensity * (targetTemperature - outsideAirTemp));
                insideTemperature = targetTemperature - 273.13;
            }

            if ((ventilationRate > maxVentilation) || (targetTemperature <= outsideAirTemp))
            {
                // it will not be possible to maintain target temp, so set ventilation to maximum value
                //equation 1.10

                /* it will be possible to maintain target temp
                 * equation 1.8
                */
                ventilationRate = maxVentilation;
                insideTemperature = (sensibleHeatOp + planArea * thermalTransRoof * tempSol + outsideAirTemp * (specificHeatCapAir * airDensity * maxVentilation + wallArea * thermalTransWall)) /
                        (specificHeatCapAir * airDensity * maxVentilation + planArea * thermalTransRoof + wallArea * thermalTransWall);
            }

            if (ventilationRate < minVentilation)
            {
                /* to keep the animals healthy, there needs to be some ventilation
                */
                ventilationRate = minVentilation;

                /* calculate the supplementary heat input necessary to maintain the inside temperature at the target temperature (joules)
                 * equation 1.11
                */
                supplementaryHeat = (planArea * thermalTransRoof * (targetTemperature - tempSol) + (wallArea * thermalTransWall
                     + specificHeatCapAir * airDensity * minVentilation) * (targetTemperature - outsideAirTemp)) - sensibleHeatOp;
                if (supplementaryHeat > maxSupplementaryHeat)
                {
                    supplementaryHeat = maxSupplementaryHeat;
                    insideTemperature = (sensibleHeatOp + supplementaryHeat + planArea * thermalTransRoof * tempSol + outsideAirTemp * (specificHeatCapAir * airDensity * minVentilation + wallArea * thermalTransWall)) /
                            (specificHeatCapAir * airDensity * minVentilation + planArea * thermalTransRoof + wallArea * thermalTransWall);
                }
            }

            // Calculate Air velocity for Forced Ventilation
            return airVelocity = ventilationRate / planArea;
        }
    }
}
