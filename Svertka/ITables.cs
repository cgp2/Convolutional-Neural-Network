using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svertka
{
    interface ITables 
    {
        int NeuronsCount { get; }
        int Type { get; set; } // 0 - входной, 1 - скрытый, 2 - выходной
        Core Core { get; }
        List<List<INeurons>> NeuronsList { get; set; }
        void Result(ITables input);
    }
}
