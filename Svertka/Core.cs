using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svertka
{
    class Core
    {
        public int height = 2, width = 2, leftCornerPositionX = 0, leftCornerPositionY = 0;
        public List<List<double>> weigth, mistake, pastGradients;

        public Core(int w, int h, List<List<double>> wg)
        {
            this.height = h;
            this.width = w;
            this.weigth = wg;

            mistake = new List<List<double>>();
            for (int i = 0; i < height; i++)
            {
                List<double> temp = new List<double>();
                for (int j = 0; j < width; j++)
                    temp.Add(0);
                mistake.Add(temp);
            }
        }


        public void Convolution(List<List<double>> neuronsOut, INeurons neuronIn)
        {
            double sum = 0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    sum += neuronsOut[i][j] * weigth[i][j];

            neuronIn.Sum = sum;
            if (sum > 0)
                neuronIn.Result = sum;
            else
                neuronIn.Result = 0.01 * sum;
        }

        public INeurons Pulling(List<List<INeurons>> neuronsOut, INeurons neuronIn)
        {
            int iMax = 0, jMax = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (neuronsOut[iMax][jMax].Result < neuronsOut[i][j].Result)
                    {
                        iMax = i;
                        jMax = j;
                    }
                }
            }

            neuronIn.Result = neuronsOut[iMax][jMax].Result;

            return neuronsOut[iMax][jMax];
        }
    }
}
