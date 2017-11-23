using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Housing.Ventilation
{
    public interface IVentilationStrategi
    {
        double CalcPropSensible(double temperature);
        void ErrorHandling(int errorNo);
        double Control(double heatOp, double outsideAirTemp, double windspeed, double solarRad, double watervapourPressure, ref double supplementaryHeat);
    }
}
