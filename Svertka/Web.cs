using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleNeuralNetwork;

namespace Svertka
{
    /// <summary>
    ///  Реализует свёрточую Нейронную сеть, состоящую из двух связанных подсетей сверточной и полносвязной.
    /// </summary>
    class Web
    {
        double al = 0.1;
        /// <summary>
        /// сверточная сеть
        /// </summary>
        public Convolutional_Network convNet;
        /// <summary>
        ///  полносвязная сеть
        /// </summary>
        public SimpleNetwork simpleNet;

        /// <summary>
        /// Инициализируем две подсети: сверточную и полносвязную.
        /// </summary>
        /// <param name="inputHeight">Высота изображений</param>
        /// <param name="inputWidth">Ширина изобржений</param>
        public Web(int inputHeight, int inputWidth)
        {
            convNet = new Convolutional_Network(inputHeight, inputWidth);
            simpleNet = new SimpleNetwork();
            simpleNet.AddInputLayer(inputHeight * inputWidth);
        }

        /// <summary>
        /// Вставить слой в полносвязную сеть
        /// </summary>
        /// <param name="type">sigmoid, th, softmax, relu</param>
        /// <param name="n">Количество нейронов</param>
        public void AddLayerToSimpleNetwork(string type, int n, int t)
        {
            switch (type)
            {
                case "sigmoid":
                    simpleNet.AddSigmoidLayer(n, t);
                    break;
                case "th":
                    simpleNet.AddTHLayer(n, t);
                    break;
                case "softmax":
                    simpleNet.AddSoftmaxLayer(n);
                    break;
                case "relu":
                    simpleNet.AddReluLayer(n);
                    break;
            }
            simpleNet.RandomizeWeights();
        }

        /// <summary>
        /// Вставить слой свертки в сверточную сеть
        /// </summary>
        /// <param name="convLayersForEachLayer">количество таблиц на каждую табилицу предыдущего слоя</param>
        /// <param name="convCoreHeight">высота ядра сверточного слоя</param>
        /// <param name="convCoreWidth">ширина ядра сверточного слоя</param>
        public void AddConvolutionalLayer(int convLayersForEachLayer, int convCoreHeight, int convCoreWidth)
        {
            convNet.AddConvolutionalLayer(convLayersForEachLayer, convCoreHeight, convCoreWidth);
            convNet.RandomWeights();

            simpleNet.LayersList.RemoveAt(0);
            simpleNet.layersCount--;
            simpleNet.AddInputLayer(convNet.LayersList.Last().NeuronsCount);
        }

        /// <summary>
        /// добавить выборочный слой в сверточную сеть
        /// </summary>
        /// <param name="pullingCoreHeight">Высота ядра подвыборки</param>
        /// <param name="pullingCoreWidth">Ширина ядра подвыборки</param>
        public void AddPullingLayer(int pullingCoreHeight, int pullingCoreWidth)
        {
            convNet.AddPullingLayer(pullingCoreHeight, pullingCoreWidth);
            convNet.RandomWeights();

            simpleNet.LayersList.RemoveAt(0);
            simpleNet.layersCount--;
            simpleNet.AddInputLayer(convNet.LayersList.Last().NeuronsCount);
        }

        /// <summary>
        ///  Расчитать результат сверточной нейронной сети
        /// </summary>
        /// <param name="input">Входное изображение</param>
        /// <returns></returns>
        public List<double> Result(List<List<double>> input)
        {
            var r1 = convNet.Result(input);
            var r2 = convNet.ExpandResult(r1);

            List<double> res = simpleNet.Result(r2);

            return res;
        }

        /// <summary>
        /// Инициализирует веса случайными значения из промежутка (-0.5, 0.5)
        /// </summary>
        public void RandomLinksWeights()
        {
            convNet.RandomWeights();
            simpleNet.RandomizeWeights();
        }

        public void TeachWithBackPropagation(DataCollection teacherList, int batchSize, int batchAmmount, double accuracy)
        {
            int epochCount = 0;
            simpleNet.eps = 0;
            double error = double.MaxValue;
            while (error > accuracy)
            {
                simpleNet.eps = 0;
                error = 0;
                DataCollection batch = new DataCollection();
                Random rd = new Random();

                batch.BatchLength = batchSize;
                for (int i = 0; i < batchSize; i++)
                    batch.List.Add(teacherList.List[rd.Next(0, teacherList.BatchLength)]);

                Random rd1 = new Random();
                for (int btch = 0; btch < batchAmmount; btch++)
                {
                    for (int m = 0; m < batch.BatchLength; m++)
                    {

                        simpleNet.SetDropout(rd1, 0);

                        List<double> result = Result(batch.List[m].image);

                        string[] standartResult = batch.List[m].label.Split();

                        List<List<double>> simpleNetMistake = simpleNet.CalculateMistakeBackProp(standartResult);

                        //Связь двух сетей . Считаем ошибку на последнем слое сверточной сети
                        int p = 0;
                        for (int l = 0; l < convNet.LayersList.Last().TablesCount; l++)
                        {
                            for (int i = 0; i < convNet.LayersList.Last().TablesList[l].NeuronsList.Count; i++)
                            {
                                for (int j = 0; j < convNet.LayersList.Last().TablesList[l].NeuronsList[i].Count; j++)
                                {
                                    double sum = 0;

                                    for (int k = 1; k < simpleNet.LayersList[1].NeuronsCount; k++)
                                    {
                                        sum += simpleNet.LayersList[1].NeuronsList[k].IncomingLinksList[p + 1].Weight * simpleNetMistake[0][k];
                                    }

                                    p++;
                                    convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Mistake = sum;
                                    convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Delta = sum
                                        * convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Derivate(convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Sum);
                                    //convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Delta = sum
                                    //    * convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Derivate(sum);
                                }
                            }
                        }

                        for (int l = convNet.layersCount - 2; l > -1; l--)
                            convNet.CountMistakeForLayer(l);

                        foreach (ILayersList ll in convNet.LayersList)
                        {
                            if (ll.Type == 2)
                            {
                                foreach (ITables l in ll.TablesList)
                                    for (int i = 0; i < l.Core.height; i++)
                                        for (int j = 0; j < l.Core.width; j++)
                                        {
                                            l.Core.weigth[i][j] += Math.Round(al * l.Core.mistake[i][j], 5);
                                            l.Core.mistake[i][j] = 0;
                                        }
                            }
                        }

                        simpleNet.RecalculateWeights(simpleNetMistake);

                        NullifyNeuronsMistake();
                    }
                }

                error = simpleNet.eps / (batchSize * batchAmmount);

                System.IO.StreamWriter sw = new System.IO.StreamWriter("C://1/svertka/eps.txt", true);
                sw.WriteLine(epochCount + "\t" + error);
                sw.Close();

                epochCount++;

                if (epochCount % 3 == 0)
                    SaveToFile("C://1/svertka");
            }

        }

        public void NullifyNeuronsMistake()
        {
            simpleNet.ClearDlt();
            convNet.NullifyNeuronsMistake();
        }


        public void TeachWithConjugateGradients(DataCollection teacherList, int batchSize, int batchAmmount, double accuracy)
        {
            int epochCount = 0;
            simpleNet.eps = 0;
            double error = double.MaxValue;
            while (error > accuracy)
            {
                simpleNet.eps = 0;
                error = 0;
                DataCollection batch = new DataCollection();
                Random rd = new Random();

                batch.BatchLength = batchSize;
                for (int i = 0; i < batchSize; i++)
                    batch.List.Add(teacherList.List[rd.Next(0, teacherList.BatchLength)]);

                Random rd1 = new Random();
                for (int btch = 0; btch < batchAmmount; btch++)
                {
                    for (int m = 0; m < batch.BatchLength; m++)
                    {
                        simpleNet.SetDropout(rd1, 0);

                        List<double> result = Result(batch.List[m].image);

                        string[] standartResult = batch.List[m].label.Split();

                        List<List<double>> simpleNetMistake = simpleNet.CalculateMistakeBackProp(standartResult);

                        int p = 0;
                        for (int l = 0; l < convNet.LayersList.Last().TablesCount; l++)
                        {
                            for (int i = 0; i < convNet.LayersList.Last().TablesList[l].NeuronsList.Count; i++)
                            {
                                for (int j = 0; j < convNet.LayersList.Last().TablesList[l].NeuronsList[i].Count; j++)
                                {
                                    double sum = 0;

                                    for (int k = 1; k < simpleNet.LayersList[1].NeuronsCount; k++)
                                    {
                                        sum += simpleNet.LayersList[1].NeuronsList[k].IncomingLinksList[p + 1].Weight * simpleNetMistake[0][k];
                                    }

                                    p++;
                                    convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Mistake = sum;
                                    convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Delta = sum
                                        * convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Derivate(convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Sum);
                                }
                            }
                        }

                        for (int l = convNet.layersCount - 2; l > -1; l--)
                            convNet.CountMistakeForLayer(l);

                    }
                }

                epochCount++;

                if (epochCount % 3 == 0)
                    SaveToFile("C://1/svertka");
            }

        }

        public void TeachWithBFGS(DataCollection teacherList, int batchLength, int batchAmmount, double accuracy)
        {
            int epochCount = 0;
            double error = double.MaxValue;

            while(error > accuracy)
            {
                error = 0;
                simpleNet.eps = 0;

                DataCollection batchCollection = new DataCollection();
                Random rd = new Random();

                batchCollection.BatchLength = batchLength;
                for (int i = 0; i < batchLength; i++)
                    batchCollection.List.Add(teacherList.List[rd.Next(0, teacherList.BatchLength)]);        

                for (int btch = 0; btch < batchAmmount; btch++)
                {
                    for(int m = 0; m < batchLength; m++)
                    {
                        simpleNet.SetDropout(rd, 0);

                        List<double> result = Result(batchCollection.List[m].image);
                       
                        string[] standartResult = batchCollection.List[m].label.Split();

                        List<List<double>> simpleNetMistake = simpleNet.CalculateMistakeBackProp(standartResult);

                        //Связь двух сетей . Считаем ошибку на последнем слое сверточной сети
                        int p = 0;
                        for (int l = 0; l < convNet.LayersList.Last().TablesCount; l++)
                        {
                            for (int i = 0; i < convNet.LayersList.Last().TablesList[l].NeuronsList.Count; i++)
                            {
                                for (int j = 0; j < convNet.LayersList.Last().TablesList[l].NeuronsList[i].Count; j++)
                                {
                                    double sum = 0;

                                    for (int k = 1; k < simpleNet.LayersList[1].NeuronsCount; k++)
                                    {
                                        sum += simpleNet.LayersList[1].NeuronsList[k].IncomingLinksList[p + 1].Weight * simpleNetMistake[0][k];
                                    }

                                    p++;
                                    convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Mistake = sum;
                                    convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Delta = sum
                                        * convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Derivate(convNet.LayersList.Last().TablesList[l].NeuronsList[i][j].Sum);
                                }
                            }
                        }

                        for (int l = convNet.layersCount - 2; l > -1; l--)
                            convNet.CountMistakeForLayer(l);

                        for(int l = simpleNet.layersCount -1; l > -1; l--)
                        {
                            var r = convNet.LayersList[1] as Convolutional_Layer;
                            r.CountHessian();
                        }
                    }
                }
             }
            


        }

        public void SaveToFile(string path)
        {
            simpleNet.SaveToFile(path + "/simpleNet.txt");
            convNet.SaveToFile(path + "/convNet.txt");
        }

        public void ReadWeightsFromFile(string path)
        {
            simpleNet.ReadWeightsFromFile(path + "/simpleNet.txt");
            convNet.ReadWeightsFromFile(path + "/convNet.txt");
        }

    }
}
