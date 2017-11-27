using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Svertka
{
    class MnistImage
    {
        public int width, height;
        public List<List<byte>> pixels = new List<List<byte>>();
        public byte label;

        public MnistImage(int width, int height, List<List<byte>> pxl, byte label)
        {
            this.width = width;
            this.height = height;
            this.label = label;

            for (int i = 0; i < height; i++)
            {
                List<byte> cols = new List<byte>();
                pixels.Add(cols);
                pixels[i].AddRange(pxl[i]);
            }
        }

        public static List<MnistImage> ReadMnistBase(string imageFilePath, string labelFilePath)
        {
            List<MnistImage> images = new List<MnistImage>();

            System.IO.FileStream fsImage = new System.IO.FileStream(imageFilePath, System.IO.FileMode.Open);
            System.IO.FileStream fsLabel = new System.IO.FileStream(labelFilePath, System.IO.FileMode.Open);
            System.IO.BinaryReader brImage = new System.IO.BinaryReader(fsImage);
            System.IO.BinaryReader brLabel = new System.IO.BinaryReader(fsLabel);

            int magic1 = ReverseByte(brImage.ReadInt32());
            int imageCount = ReverseByte(brImage.ReadInt32());
            int rowsCount = ReverseByte(brImage.ReadInt32());
            int colsCount = ReverseByte(brImage.ReadInt32());

            int magic2 = ReverseByte(brLabel.ReadInt32());
            int labesCount = ReverseByte(brLabel.ReadInt32());

            for (int n = 0; n < imageCount; n++)
            {
                List<List<byte>> bytes = new List<List<byte>>();
                for (int i = 0; i < rowsCount; i++)
                {
                    List<byte> col = new List<byte>();
                    for (int j = 0; j < colsCount; j++)
                        col.Add(brImage.ReadByte());
                    bytes.Add(col);
                }
                byte label = brLabel.ReadByte();
                MnistImage img = new MnistImage(rowsCount, colsCount, bytes, label);
                images.Add(img);
            }

            fsImage.Close();
            fsLabel.Close();
            brImage.Close();
            brLabel.Close();

            return images;
        }

        public static Bitmap MakeBitmapFromMnist(MnistImage mnsIm, int coof)
        {
            Bitmap bmI = new Bitmap(mnsIm.width * coof, mnsIm.height * coof);
            Graphics gr = Graphics.FromImage(bmI);
            for (int i = 0; i < mnsIm.width; i++)
            {
                for (int j = 0; j < mnsIm.height; j++)
                {
                    int pixelColor = 255 - mnsIm.pixels[i][j];
                    System.Drawing.Color c = System.Drawing.Color.FromArgb(pixelColor, pixelColor, pixelColor);
                    SolidBrush sb = new SolidBrush(c);
                    gr.FillRectangle(sb, j * coof, i * coof, coof, coof);
                }
            }

            return bmI;
        }

        public static BitmapImage BMtoBMI(Bitmap bitmap)
        {
            using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public static int ReverseByte(int l)
        {
            byte[] bt = BitConverter.GetBytes(l);
            Array.Reverse(bt);
            return BitConverter.ToInt32(bt, 0);
        }
    }
}
