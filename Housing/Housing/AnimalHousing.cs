using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Housing.Animal;
using Housing.Utility;
using Housing.Ventilation;

namespace Housing.Housing
{
    public class AnimalHousing : IHousing
    {
        // Composition
        private IVentilationStrategi ventilationForced;
        private IUtility utilities;
        private IAnimalStrategy dummyAnimal;

        public AnimalHousing(IVentilationStrategi forcedVentilation, IUtility utility, IAnimalStrategy animalStrategy)
        {
            this.ventilationForced = forcedVentilation;
            this.utilities = utility;
            this.dummyAnimal = animalStrategy;
        }

        // return value
        private double airVelocity;

        // plan area of house in square metres
        double planArea = 0.0; 

        public void Ventilation(int controlledVent)
        {

            /*  these values should be read as parameters
             *  these values should be read as daily inputs
            */
            double Ameantemp = 10.0;
            double Aradiation = 600.0; //W per square metre
            double ArelativeHumidity = 1.0;
            double Awindspeed = 3.0;

            double heatOp = dummyAnimal.GetHeatProduction();
            double waterVapourPressure = ArelativeHumidity * utilities.GetsaturatedWaterVapourPressure(Ameantemp);

            /*  !calculate the air velocity, using the appropriate functions for controlled or freely ventilated systems
            */
            if (controlledVent > 0)
            {
                double supplementaryHeat = 0.0;
                airVelocity = ventilationForced.Control(heatOp, Ameantemp, 0.0, Aradiation, waterVapourPressure, ref supplementaryHeat);
            }
            else
            {
                //uncontrolled(heatOp, Ameantemp, Awindspeed, Aradiation, ArelativeHumidity);
            }
        }

        public double getVelocity()
        {
            return airVelocity;
        }
    }
}
