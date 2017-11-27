using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svertka
{
    interface ILayersList
    {
        List<ITables> TablesList { get; }
        int TablesCount { get; }
        int NeuronsCount { get; }
        int CoreHeight { get; }
        int CoreWidth { get; }
        int Type { get; }
        void Result(ILayersList input);
        void SetWeigths(List<List<List<double>>> input);
    }
}
