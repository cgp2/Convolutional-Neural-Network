using System;
using System.Collections.Generic;

namespace Svertka
{
    class Input_Layers_List : ILayersList
    {
        List<ITables> layersList = new List<ITables>();
        int layersCount = 0, neuronsCount = 0, type = 0;

        public Input_Layers_List(int height, int width)
        {
            neuronsCount = height * width;
            Layer_Input li = new Layer_Input(height, width);
            layersList.Add(li);
            layersCount++;
        }

        public int TablesCount
        {
            get
            {
                return layersCount;
            }
        }

        public int NeuronsCount
        {
            get
            {
                return neuronsCount;
            }
        }

        public List<ITables> TablesList
        {
            get
            {
                return layersList;
            }
        }

        public int CoreHeight
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int CoreWidth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Type
        {
            get
            {
                return type;
            }
        }

        public List<List<List<double>>> Hessian
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Result(List<List<List<double>>> input)
        {
            foreach (Layer_Input l in layersList)
                l.Result(input[0]);
        }

        public void Result(ILayersList input)
        {
            throw new NotImplementedException();
        }

        public void SetWeigths(List<List<List<double>>> input)
        {
        }
    }

    class Layer_Input : ITables
    {
        List<List<INeurons>> neuronsList = new List<List<INeurons>>(); // Подумать над логикой
        int neuronsCount = 0;
        int type = 0; // 0 - входной слой, 1 - скрытые слои, 2 - выходной


        public Layer_Input(int height, int width)
        {
            neuronsCount = width * height;

            for (int i = 0; i < height; i++)
            {
                List<INeurons> temp = new List<INeurons>();
                for (int j = 0; j < width; j++)
                {
                    temp.Add(new Neuron_Input(i * 10 + j, 0));
                }
                NeuronsList.Add(temp);
            }
        }

        public void Result(List<List<double>> input)
        {
            for (int i = 0; i < neuronsList.Count; i++)
            {
                for (int j = 0; j < neuronsList[i].Count; j++)
                    neuronsList[i][j].Result = input[i][j];
            }
        }

        public void Result(ITables input)
        {
            throw new NotImplementedException();
        }

        public int NeuronsCount
        {
            get
            {
                return neuronsCount;
            }
        }
        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        public List<List<INeurons>> NeuronsList
        {
            get
            {
                return neuronsList;
            }

            set
            {
                neuronsList = value;
            }
        }
        public Core Core
        {
            get
            {
                return null;
            }
        }
    }

    class Neuron_Input : INeurons
    {
        int type = 0;
        double result, sum = 1, dlt = 0, mistake = 0;
        int id;

        public Neuron_Input(int id, int type)
        {
            this.id = id;
            this.type = type;
        }

        public double Derivate(double x)
        {
            return 0;
        }

        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        public double Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }
        public double Sum
        {
            get
            {
                return sum;
            }
            set
            {
                sum = value;
            }
        }
        public int Id
        {
            get
            {
                return id;
            }
        }
        public double Delta
        {
            get
            {
                return dlt;
            }

            set
            {
                dlt = value;
            }
        }
        public double Mistake
        {
            get
            {
                return mistake;
            }

            set
            {
                mistake = value;
            }
        }
    }
}
