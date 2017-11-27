using System;
using System.Collections.Generic;
using System.Linq;

namespace Svertka
{
    public class Convolutional_Network
    {
        private List<ILayersList> layersList = new List<ILayersList>();
        public int layersCount = 0;

        public Convolutional_Network(int inputHeight, int inputWidth)
        {
            Input_Layers_List ill = new Input_Layers_List(inputHeight, inputWidth);

            layersList.Add(ill);
            layersCount++;
        }

        public void AddConvolutionalLayer(int layersForEachLayers, int coreHeight, int coreWidth)
        {
            Convolutional_Layer cll = new Convolutional_Layer(layersList.Last().TablesCount, layersForEachLayers,
                                                                          coreHeight, coreWidth,
                                                                          layersList.Last().TablesList[0].NeuronsList.Count,
                                                                          layersList.Last().TablesList[0].NeuronsList[0].Count);

            layersList.Add(cll);
            layersCount++;
        }

        public void AddPullingLayer(int coreHeight, int coreWidth)
        {
            Pulling_Layer cll = new Pulling_Layer(layersList.Last().TablesList.Count, coreHeight, coreWidth,
                layersList.Last().TablesList[0].NeuronsList.Count, layersList.Last().TablesList[0].NeuronsList[0].Count);


            layersList.Add(cll);
            layersCount++;
        }

        public void RandomWeights()
        {
            Random rd = new Random();
            foreach (ILayersList ll in layersList)
            {
                if (ll.Type == 2)
                {
                    foreach (ITables l in ll.TablesList)
                    {
                        for (int i = 0; i < l.Core.height; i++)
                            for (int j = 0; j < l.Core.width; j++)
                                l.Core.weigth[i][j] = RandomDouble(rd, -0.5, 0.5);
                    }
                }
            }
        }

        public List<List<List<double>>> Result(List<List<double>> input)
        {
            List<List<List<double>>> temp = new List<List<List<double>>>();
            temp.Add(input);
            (layersList[0] as Input_Layers_List).Result(temp);

            for (int i = 1; i < layersList.Count; i++)
                layersList[i].Result(layersList[i - 1]);

            List<List<List<double>>> res = new List<List<List<double>>>();

            foreach (ITables table in layersList.Last().TablesList)
            {
                List<List<double>> temp1 = new List<List<double>>();
                foreach (List<INeurons> neuronsRow in table.NeuronsList)
                {
                    List<double> temp2 = new List<double>();
                    foreach (INeurons neuron in neuronsRow)
                        temp2.Add(neuron.Result);
                    
                    temp1.Add(temp2);
                }
                res.Add(temp1);
            }

            return res;
        }

        /// <summary>
        /// Расчет ошибки для слоя сверточной сети
        /// </summary>
        /// <param name="n">Номер слоя</param>
        public void CountMistakeForLayer(int n)
        {
            if (layersList[n + 1].Type == 2) //Если следующий слой сверточный
            {
                var lrl_conv = layersList[n + 1] as Convolutional_Layer;
                for (int l = 0; l < layersList[n].TablesCount; l++)
                {
                    int p = 0;
                    for (int neuronsH = 0; neuronsH < layersList[n].TablesList[l].NeuronsList.Count; neuronsH++)
                    {
                        for (int neuronsW = 0; neuronsW < layersList[n].TablesList[l].NeuronsList[neuronsH].Count; neuronsW++)
                        {
                            //layersList[n].TablesList[l].NeuronsList[neuronsH][neuronsW].Mistake = 0;

                            for (int layerForEach = 0; layerForEach < lrl_conv.layersForEachLayer; layerForEach++)
                            {
                                for (int coreH = 0; coreH < lrl_conv.coreHeight; coreH++) // -1?
                                {
                                    for (int coreW = 0; coreW < lrl_conv.coreWidth; coreW++) // -1?
                                    {
                                        if (((neuronsH - coreH) < 0) || ((neuronsW - coreW) < 0)
                                            || ((neuronsH - coreH) >= layersList[n + 1].TablesList[layerForEach + p].NeuronsList.Count)
                                            || ((neuronsW - coreW) >= layersList[n + 1].TablesList[layerForEach + p].NeuronsList[0].Count))
                                        {

                                            //layersList[n].LayersList[l].NeuronsList[neuronsH][neuronsW].Mistake += 
                                            //    layersList[n + 1].LayersList[layerForEach + p].NeuronsList[neuronsH - coreH][neuronsW - coreW].Delta
                                            //    * lrl_conv.layersList[layerForEach + p].Core.weigth[coreH][coreW];
                                        }
                                        else
                                            layersList[n].TablesList[l].NeuronsList[neuronsH][neuronsW].Mistake +=
                                               layersList[n + 1].TablesList[layerForEach + p].NeuronsList[neuronsH - coreH][neuronsW - coreW].Delta
                                               * lrl_conv.TablesList[layerForEach + p].Core.weigth[coreH][coreW];

                                    }
                                }
                            }
                            layersList[n].TablesList[l].NeuronsList[neuronsH][neuronsW].Delta =
                                layersList[n].TablesList[l].NeuronsList[neuronsH][neuronsW].Mistake
                             * layersList[n].TablesList[l].NeuronsList[neuronsH][neuronsW].Derivate(layersList[n].TablesList[l].NeuronsList[neuronsH][neuronsW].Sum);
                        }
                    }
                    p += lrl_conv.layersForEachLayer;
                }

                for (int l = 0; l < lrl_conv.TablesCount; l++)
                {
                    int p = 0;
                    for (int layerForEach = 0; layerForEach < lrl_conv.layersForEachLayer; layerForEach++)
                    {
                        for (int coreH = 0; coreH < lrl_conv.coreHeight; coreH++)
                        {
                            for (int coreW = 0; coreW < lrl_conv.CoreWidth; coreW++)
                            {
                                for (int i = 0; i < lrl_conv.tablesList[l].NeuronsList.Count; i++)
                                {
                                    for (int j = 0; j < lrl_conv.TablesList[l].NeuronsList[i].Count; j++)
                                    {
                                        int nn = Convert.ToInt32(Math.Floor(l * 1.0 / lrl_conv.layersForEachLayer));
                                        layersList[n + 1].TablesList[l].Core.mistake[coreH][coreW] +=
                                            lrl_conv.TablesList[l].NeuronsList[i][j].Delta
                                            * layersList[n].TablesList[nn].NeuronsList[i + coreH][j + coreW].Result;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (layersList[n + 1].Type == 1) //если следующий слой пуллинг
            {
                for (int l = 0; l < layersList[n].TablesCount; l++)
                {
                    for (int i = 0; i < layersList[n + 1].TablesList[l].NeuronsList.Count; i++)
                    {
                        for (int j = 0; j < layersList[n + 1].TablesList[l].NeuronsList[i].Count; j++)
                        {
                            (layersList[n + 1].TablesList[l] as Layer_Pulling).maxNeurons[i][j].Mistake = layersList[n + 1].TablesList[l].NeuronsList[i][j].Mistake;
                        }
                    }
                }
            }
        }

        public void NullifyNeuronsMistake()
        {
            foreach(ILayersList ll in layersList)
                foreach(ITables table in ll.TablesList)
                    foreach (List<INeurons> nrn_list in table.NeuronsList)
                        foreach (INeurons nrn in nrn_list)
                            nrn.Mistake = 0;
            
        }

        public List<double> ExpandResult(List<List<List<double>>> input)
        {
            List<double> res = new List<double>();

            foreach (List<List<double>> temp1 in input)
                foreach (List<double> temp2 in temp1)
                    res.AddRange(temp2);

            return res;
        }

        public void SaveToFile(string path)
        {
            System.IO.FileStream fs = new System.IO.FileStream(@path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);

            foreach (ILayersList lrl in layersList)
            {
                sw.WriteLine("{");
                sw.WriteLine(lrl.Type);
                if (lrl.Type != 0)
                {
                    foreach (ITables lr in lrl.TablesList)
                    {
                        sw.WriteLine("<");
                        for (int i = 0; i < lr.Core.weigth.Count; i++)
                        {
                            for (int j = 0; j < lr.Core.weigth[i].Count; j++)
                            {
                                sw.Write(lr.Core.weigth[i][j] + " ");
                            }
                            sw.WriteLine();
                        }
                        sw.WriteLine(">");
                    }
                }
                else
                {
                    sw.WriteLine(lrl.NeuronsCount);
                }
                sw.WriteLine("}");
            }

            sw.Close();
            fs.Close();
        }

        public void ReadWeightsFromFile(string path)
        {
            layersList.Clear();
            layersCount = 0;
            string[] input = System.IO.File.ReadAllLines(@path);

            int na = input.Count(i => i == "{");
            List<string> s = input.ToList();
            int p = 1;

            for(int i =0; i < na; i++)
            {
                if (s[p] == "0")
                {
                    int inputH = int.Parse(s[p + 1]);
                    int inputW = int.Parse(s[p + 2]);
                    Input_Layers_List inp = new Input_Layers_List(inputH, inputW);
                    LayersList.Add(inp);
                    layersCount++;
                    p = s.IndexOf("{", p + 2);
                }
                else
                {
                    List<List<List<double>>> weights = new List<List<List<double>>>();
                    int n = s.IndexOf("}", p);
                    var s1 = s.GetRange(p + 1, n - p -1);
                    int nn = s1.Count(l => l == "<");
                    string type = s1[0];

                    int pp = 0;
                    for(int j = 0; j < nn; j++)
                    {
                        List<List<double>> w = new List<List<double>>();
                        pp = s1.IndexOf("<", pp) + 1;
                        int endIndex = s1.IndexOf(">", pp);


                        for(int str = pp; str < endIndex; str++)
                        {
                            List<double> temp = new List<double>();
                            string[] splitStr = s1[str].Split();
                            for (int l = 0; l < splitStr.Length - 1; l++)
                                temp.Add(double.Parse(splitStr[l]));
                            w.Add(temp);
                        }

                        weights.Add(w);
                    }

                    if(type == "2")
                    {
                        AddConvolutionalLayer(weights.Count / LayersList.Last().TablesCount, weights[0].Count, weights[0][0].Count);
                        LayersList.Last().SetWeigths(weights);
                    }
                    else
                        AddPullingLayer(weights[0].Count, weights[0][0].Count);


                    p = n + 1;
                }
            }
        }

        public double RandomDouble(Random rd, double min, double max)
        {
            return rd.NextDouble() * (max - min) + min;
        }

        internal List<ILayersList> LayersList
        {
            get
            {
                return layersList;
            }
            set
            {
                layersList = value;
            }
        }
    }
}
