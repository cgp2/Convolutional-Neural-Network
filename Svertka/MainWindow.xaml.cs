using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Svertka
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string path1 = "C://1/test_images.idx3-ubyte";
            string path2 = "C://1/test_labels.idx1-ubyte";

            DataCollection teacher = new DataCollection();
            teacher.MakeMnistTeacherList(path1, path2);

            Web wb = new Web(28, 28);


            wb.AddConvolutionalLayer(5, 5, 5);
            wb.AddPullingLayer(2, 2);
            wb.AddConvolutionalLayer(5, 4, 4);
            wb.AddPullingLayer(2, 2);
            wb.AddConvolutionalLayer(5, 3, 3);
            wb.AddLayerToSimpleNetwork("sigmoid", 120, 1);
            wb.AddLayerToSimpleNetwork("softmax", 10, 2);
            //wb.ReadWeightsFromFile("C://1/svertka");

            wb.TeachWithBackPropagation(teacher, 100, 20, 0.001);

            DataCollection test = new DataCollection();
            test.MakeMnistTeacherList("C://1/train-images.idx3-ubyte", "C://1/train-labels.idx1-ubyte");

            double errors_count = 0;
            for (int i = 0; i < test.BatchLength; i++)
            {
                List<double> res = wb.Result(test.List[i].image);
                int answ = res.IndexOf(res.Max());

                if (answ != int.Parse(test.List[i].answ))
                    errors_count++;
            }

            //var r= wb.Result(teacher.List[10].image);

        }

  
    }
}
