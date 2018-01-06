using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiwindowClassLibrary
{
    public class NumberEventArgs : EventArgs
    {
        public int Id { get; private set; }
        public int Number { get; private set; }

        public NumberEventArgs(int id, int number)
        {
            Id = id;
            Number = number;
        }
    }
}
