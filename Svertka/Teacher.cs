using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svertka
{
    class DataCollection
    {
        public List<DataParticle> List = new List<DataParticle>();
        int batchLenght = 0;

        public void MakeMnistTeacherList(string pathToImages, string pathToLabels)
        {
            List<MnistImage> images = MnistImage.ReadMnistBase(pathToImages, pathToLabels);

            double average = 0, sum = 0, max = double.MinValue;
            int n = 0;
            for(int i =0; i < images.Count; i++)
            {
                for (int l = 0; l < images[i].height; l++)
                {                 
                    for (int m = 0; m < images[i].width; m++)
                    {
                        sum += images[i].pixels[m][l];
                        n++;
                        if (max < images[i].pixels[m][l])
                            max = images[i].pixels[m][l];
                    }
                }
            }

            average = sum / n;

            List<List<double>> im = new List<List<double>>();

            for (int i = 0; i < images.Count; i++)
            {
                DataParticle tp = new DataParticle();
                for (int l = 0; l < images[i].height; l++)
                {
                    List<double> temp = new List<double>();
                    for (int m = 0; m < images[i].width; m++)
                    {
                        temp.Add((images[i].pixels[m][l] - average) / max);
                    }
                    tp.image.Add(temp);
                }

                string s = "";
                for(int l  = 0; l < 9; l++)
                {
                    if (images[i].label != l)
                        s += "0 ";
                    else
                        s += "1 ";
                }
                if (images[i].label != 9)
                    s += "0";
                else
                    s += "1";
                tp.label = s;
                List.Add(tp);

                tp.answ = images[i].label.ToString();
            }

            batchLenght = images.Count();
        }

        public int BatchLength
        {
            get { return batchLenght; }
            set { batchLenght = value; }
        }
    }

    class DataParticle
    {
        public List<List<double>> image = new List<List<double>>();
        public string label;
        public string answ;
    }
}
