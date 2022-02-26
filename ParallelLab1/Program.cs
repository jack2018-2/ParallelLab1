using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace ParallelLab1
{
    class Program
    {
        static string fileName = "C:/Users/jack2/source/repos/ParallelLab1/ParallelLab1/bin/Debug/android.jpg";
        static int radius = 3;

        static void Main(string[] args)
        {
            Console.Write("Введите полный путь к картинке. Допустимые форматы - jpeg/jpg и png.\nВ остальных случаях работоспособность не гарантируется.\n>");
            var msg = Console.ReadLine();
            while (!File.Exists(msg))
            {
                Console.Write("Картинка не найдена и/или Вы ввели неверный путь.\nВведите полный путь к картинке. Допустимые форматы - jpeg/jpg и png.\nВ остальных случаях работоспособность не гарантируется.\n>");
                msg = Console.ReadLine();
            }
            fileName = msg;
            var sourceBitmap = new Bitmap(fileName);
            var imageChannels = ReadChannels(sourceBitmap);

            var producer = new GaussBlurer(imageChannels, sourceBitmap.Width, sourceBitmap.Height, radius);

            var channels1 = producer.Process(false);
            var channels2 = producer.Process(true);

            SavePic(channels1, sourceBitmap, 
                $"{Path.GetFullPath(fileName)}_resultSync.jpg");
            SavePic(channels2, sourceBitmap,
                $"{Path.GetFullPath(fileName)}_resultAsync.jpg");
        }

        static List<int[,]> ReadChannels(Bitmap source)
        {
            int[,] r1 = new int[source.Height, source.Width];
            int[,] g1 = new int[source.Height, source.Width];
            int[,] b1 = new int[source.Height, source.Width];
            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    Color color = source.GetPixel(j, i);
                    r1[i, j] = color.R;
                    g1[i, j] = color.G;
                    b1[i, j] = color.B;
                }
            }

            return new List<int[,]>() { r1, g1, b1 };
        }

        static void SavePic(List<int[,]> channels, Bitmap source, string fileName)
        {
            var resBitmap = new Bitmap(source.Width, source.Height);
            for (var i = 0; i < source.Height; i++)
            {
                for (var j = 0; j < source.Width; j++)
                {
                    resBitmap.SetPixel(j, i, Color.FromArgb(channels[0][i,j], channels[1][i, j], channels[2][i, j]));
                }
            }
            resBitmap.Save(fileName);
        }
    }
}

/*
 * интеграл
 * ядро лапласа
 * ядро собеля
 * фильтр гаусса (фильтры изображений любые)
 * аппроксимация ф-ции
 * обработка строк
 * сортировки
 */