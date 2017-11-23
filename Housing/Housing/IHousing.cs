using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Housing.Housing
{
    public interface IHousing
    {
        void Ventilation(int controlledVent);
        double getVelocity();
    }
}
