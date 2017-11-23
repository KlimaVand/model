using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Housing.Housing;
using Housing.Ventilation;
using Housing.Utility;
using Housing.Animal;

namespace Housing
{
    class Program
    {
        static void Main(string[] args)
        {           
            IHousing aHouse1 = new AnimalHousing(new ForcedVentilation(6.0, 22.0, 1.5, 5.0, 0.8, 0.04, 0.8, 293.0, 2.0, 4.0, 10000.0), new Utility.Utility(), new DummyAnimal(1, 50, 650.0, 25.0, 0, 40.0));
            aHouse1.Ventilation(1);
        }
    }
}
