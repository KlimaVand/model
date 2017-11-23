using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Housing.Animal
{
    public interface IAnimalStrategy
    {
        double HeatProduction();
        double GetHeatProduction();
        double GetmaximumTemperature();
    }
}
