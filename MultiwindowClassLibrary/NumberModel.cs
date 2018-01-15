using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiwindowClassLibrary
{
    public class NumberModel
    {
        public int Number { get; private set; }

        public NumberModel(int number)
        {
            Number = number;
        }
    }
}
