using System;
using System.Collections.Generic;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Svertka
{
    class Convolutional_Layer : ILayersList
    {
        public List<ITables> tablesList = new List<ITables>();
        public int tablesCount, neuronsCount = 0, layersForEachLayer = 0, coreHeight, coreWidth, type = 2;
        public List<Matrix<double>> hessian = new List<Matrix<double>>(), s = new List<Matrix<double>>(),
               y = new List<Matrix<double>>(), p = new List<Matrix<double>>(), x = new List<Matrix<double>>();

        /// <summary>
        /// Добавить слой сверточных слоев в сеть.Связи будут инициализированны весами = 0.5
        /// </summary>
        /// <param name="layersInPreviousLayer">количество таблиц в предыдущем слое</param>
        /// <param name="numberForEachPrLayer">количество таблиц в слое</param>
        /// <param name="coreHeight">высота ядра</param>
        /// <param name="coreWidth">ширина ядра</param>
        /// <param name="inputHeight">высота таблицы предыдущего слоя</param>
        /// <param name="inputWidth">ширина таблицы предыдущего слоя</param>
        public Convolutional_Layer(int layersInPreviousLayer, int numberForEachPrLayer, int coreHeight, int coreWidth, int inputHeight, int inputWidth)
        {
            this.coreHeight = coreHeight;
            this.coreWidth = coreWidth;
            this.layersForEachLayer = numberForEachPrLayer;
            tablesCount = numberForEachPrLayer * layersInPreviousLayer;

            for (int i = 0; i < layersInPreviousLayer; i++)
            {
                for (int j = 0; j < numberForEachPrLayer; j++)
                {
                    List<List<double>> weight = new List<List<double>>();

                    for (int l = 0; l < coreHeight; l++)
                    {
                        List<double> temp = new List<double>();
                        for (int m = 0; m < coreWidth; m++)
                        {
                            temp.Add(0.5);
                        }
                        weight.Add(temp);
                    }

                    Convolutional_Table table = new Convolutional_Table(weight.Count, weight[0].Count, weight, inputHeight, inputWidth);
                    neuronsCount += table.neuronsCount;
                    tablesList.Add(table);
                }
            }

            //for (int i = 0; i < tablesCount; i++)
            //{
            //    hessian.Add(Matrix<double>.Build.Dense(TablessList[i].NeuronsList.Count, TablessList[i].NeuronsList[0].Count, 1));
            //    s.Add(Matrix<double>.Build.Dense(TablessList[i].NeuronsList.Count, TablessList[i].NeuronsList[0].Count, 1));
            //    p.Add(Matrix<double>.Build.Dense(TablessList[i].NeuronsList.Count, TablessList[i].NeuronsList[0].Count, 1));
            //    y.Add(Matrix<double>.Build.Dense(TablessList[i].NeuronsList.Count, TablessList[i].NeuronsList[0].Count, 1));
            //    y.Add(Matrix<double>.Build.Dense(TablessList[i].NeuronsList.Count, TablessList[i].NeuronsList[0].Count, 0));
            //}
        }

        public void Result(ILayersList input)
        {
            int p = 0;
            for (int i = 0; i < input.TablesCount; i++)
            {
                for (int j = 0; j < layersForEachLayer; j++)
                {
                    tablesList[p].Result(input.TablesList[i]);
                    p++;
                }
            }
        }

        public void SetWeigths(List<List<List<double>>> input)
        {
            for (int i = 0; i < TablesCount; i++)
                tablesList[i].Core.weigth = input[i];

        }

        public List<ITables> TablesList
        {
            get
            {
                return tablesList;
            }
        }


        //If something wronf take derivative of mistake
        public void CountHessian()
        {
            for (int i = 0; i < tablesCount; i++)
            {
                var grad = Matrix<double>.Build.Dense(TablesList[i].NeuronsList.Count, TablesList[i].NeuronsList[0].Count);
                for (int l = 0; l < TablesList[i].NeuronsList.Count; l++)
                {
                    for (int m = 0; m < TablesList[i].NeuronsList[0].Count; m++)
                        grad[l, m] = TablesList[i].NeuronsList[l][m].Mistake;
                }

                p[i] = (-1) * hessian[i] * grad;

                double al = 0.001, c1 = Math.Pow(10, -4), c2 = 0.9999;
                for (int l = 0; l < TablesList[i].NeuronsList.Count; l++)
                {
                    for (int m = 0; m < TablesList[i].NeuronsList[l].Count; m++)
                    {

                    }
                }


            }
        }

        public int TablesCount
        {
            get
            {
                return tablesCount;
            }
        }

        public int NeuronsCount
        {
            get
            {
                return neuronsCount;
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

        public List<Matrix<double>> Hessian
        {
            get
            {
                return hessian;
            }

            set
            {
                hessian = value;
            }
        }
    }

    class Convolutional_Table : ITables
    {
        public Core core;
        public int type = 6;
        public List<List<INeurons>> neuronsList = new List<List<INeurons>>();
        public int neuronsCount = 0, coresInWidth = 0, coresInHeight = 0, id = 0;


        public Convolutional_Table(int coreHeight, int coreWidth, List<List<double>> weight, int inputHeight, int inputWidth)
        {
            core = new Core(coreHeight, coreWidth, weight);

            coresInWidth = inputWidth - (coreWidth - 1);
            coresInHeight = inputHeight - (coreHeight - 1);
            neuronsCount = coresInHeight * coresInWidth;

            for (int i = 0; i < coresInHeight; i++)
            {
                List<INeurons> temp = new List<INeurons>();
                for (int j = 0; j < coresInWidth; j++)
                {
                    Neuron_Convolutional nr = new Neuron_Convolutional((i * 10) + j);

                    temp.Add(nr);
                }
                neuronsList.Add(temp);
            }
        }

        public void Result(ITables input)
        {
            for (int i = 0; i < coresInHeight; i++)
            {
                for (int j = 0; j < coresInWidth; j++)
                {
                    double sum = 0;

                    for (int l = 0; l < core.height; l++)
                    {
                        List<double> temp1 = new List<double>();

                        for (int m = 0; m < core.width; m++)
                            sum += core.weigth[l][m] * input.NeuronsList[l + i][m + j].Result;

                        NeuronsList[i][j].Sum = sum;
                        NeuronsList[i][j].Result = sum > 0 ? sum : 0.01 * sum;
                    }
                }
            }
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
                return core;
            }
        }
    }

    class Neuron_Convolutional : INeurons
    {
        public int type = 6;
        public double result, sum = 1, mistake = 0, dlt = 0;
        public int id;

        public Neuron_Convolutional(int id)
        {
            this.id = id;
        }

        public double Derivate(double x)
        {
            double der = 0;
            if (x > 0)
                der = 1;
            else
                der = 0.01;

            return der;
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
                result = Math.Round(value, 4);
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
                sum = Math.Round(value, 4);
            }
        }
        public int Id
        {
            get
            {
                return id;
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
                mistake = Math.Round(value, 4);
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
                dlt = Math.Round(value, 4);
            }
        }
    }
}
