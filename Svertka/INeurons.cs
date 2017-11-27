using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svertka
{
    interface INeurons
    {
        int Type { get; set; } // 0 - входной нейрон, 1 - сигмоид, 2 - Relu, 3 - Softmax, 4 - TH
        double Result { get; set; }
        double Sum { get; set; }
        double Mistake { get; set; }
        int Id { get; }
        double Delta { get; set; }
        double Derivate(double x);
    }
}
