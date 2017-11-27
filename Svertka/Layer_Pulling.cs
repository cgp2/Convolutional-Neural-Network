using System;
using System.Collections.Generic;

namespace Svertka
{
    class Pulling_Layer : ILayersList
    {
        List<ITables> layersList = new List<ITables>();
        int layersCount, neuronsCount = 0, coreHeight, coreWidth, type = 1;

        /// <summary>
        /// Создание подвыборочного слоя, состоящего из n таблиц
        /// </summary>
        /// <param name="n">Количество таблиц</param>
        /// <param name="coreHeight">Высота ядра</param>
        /// <param name="coreWidth">Ширина ядра</param>
        /// <param name="inputHeight">Высота таблицы предыдущего слоя</param>
        /// <param name="inputWidth">Ширина таблицы предыдущего слоя</param>
        public Pulling_Layer(int n, int coreHeight, int coreWidth, int inputHeight, int inputWidth)
        {
            this.coreHeight = coreHeight;
            this.coreWidth = coreWidth;
            layersCount = n;

            for (int i = 0; i < layersCount; i++)
            {
                Layer_Pulling lc = new Layer_Pulling(coreHeight, coreWidth, inputHeight, inputWidth);
                neuronsCount += lc.NeuronsCount;
                layersList.Add(lc);
            }
        }

        public void Result(ILayersList input)
        {
            for (int i = 0; i < layersList.Count; i++)
                layersList[i].Result(input.TablesList[i]);
        }

        public void SetWeigths(List<List<List<double>>> input)
        {
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
        public List<ITables> TablesList
        {
            get
            {
                return layersList;
            }
        }
        public int NeuronsCount
        {
            get
            {
                return neuronsCount;
            }
        }
        public int TablesCount
        {
            get
            {
                return layersCount;
            }
        }
        public int CoreHeight
        {
            get
            {
                return coreHeight;
            }
        }
        public int CoreWidth
        {
            get
            {
                return coreWidth;
            }
        }
        public int Type
        {
            get
            {
                return type;
            }
        }
    }

    class Layer_Pulling : ITables
    {
        Core core;
        int type = 5;
        public List<List<INeurons>> maxNeurons = new List<List<INeurons>>();
        List<List<INeurons>> neuronsList = new List<List<INeurons>>();
        int neuronsCount = 0, coresInWidth = 0, coresInHeight = 0, coreWidth = 0, coreHeight = 0;

        public Layer_Pulling(int coreHeight, int coreWidth, int inputHeight, int inputWidth)
        {
            this.coreWidth = coreWidth; 
            this.coreHeight = coreHeight;
            coresInWidth = inputWidth / coreWidth ;
            coresInHeight = inputHeight / coreHeight;

            List<List<double>> weights = new List<List<double>>();
            for (int i = 0; i < coreHeight; i++)
            {
                List<double> temp = new List<double>();
                for (int j = 0; j < coreWidth; j++)
                {
                    temp.Add(1);
                }
                weights.Add(temp);
            }

            core = new Core(coreWidth, coreHeight, weights);

            for (int i = 0; i < coresInHeight; i++)
            {
                List<INeurons> temp = new List<INeurons>();
                for (int j = 0; j < coresInWidth; j++)
                {
                    Neuron_Pulling nr = new Neuron_Pulling((i * 10) + j);
                    temp.Add(nr);
                    neuronsCount++;
                }
                neuronsList.Add(temp);
            }
        }

        public void Result(ITables input)
        {
            for (int i = 0; i < coresInHeight; i++)
            {
                List<INeurons> temp = new List<INeurons>();
                for (int j = 0; j < coresInWidth; j++)
                {
                    List<List<INeurons>> coreArgs = new List<List<INeurons>>();

                    int maxl = 0, maxm = 0;
                    for (int l = 0; l < coreHeight; l++)
                    {
                        List<INeurons> temp1 = new List<INeurons>();
                        for (int m = 0; m < coreWidth; m++)
                        {
                            if(input.NeuronsList[l + i * coreHeight][m + j * coreWidth].Result 
                                > input.NeuronsList[maxl][maxm].Result)
                            {
                                maxl = l + i * coreHeight;
                                maxm = m + j * coreWidth;
                            }
                        }
                    }

                    NeuronsList[i][j].Result = input.NeuronsList[maxl][maxm].Result;
                    temp.Add(NeuronsList[i][j]);
                }
                maxNeurons.Add(temp);
            }
        }

        public int NeuronsCount
        {
            get
            {
                return neuronsCount;
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

        public Core Core
        {
            get
            {
                return core;
            }
        }
    }

    class Neuron_Pulling : INeurons
    {
        int type = 5;
        double result, sum = 1, mistake = 0, dlt = 0;
        int id;

        public Neuron_Pulling(int id)
        {
            this.id = id;
        }

        public double Derivate(double x)
        {
            return 0;
        }

        public int Id
        {
            get
            {
                return id;
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
    }
}
