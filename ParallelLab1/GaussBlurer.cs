using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelLab1
{
    public class GaussBlurer
    {
        List<int[,]> sourceChannels;
        int width;
        int height;
        int radius;

        public GaussBlurer() { }

        public GaussBlurer(List<int[,]> scl, int width, int height, int radius)
        {
            this.sourceChannels = scl;
            this.width = width;
            this.height = height;
            this.radius = radius;
        }

        public List<int[,]> Process(bool isAsync)
        {
            if (isAsync)
            {
                var safeChannels = new ConcurrentDictionary<int, int[,]>();
                var taskBag = new Task<(int, int[,])>[3];
                var i = 0;
                var startTime = DateTime.Now;
                foreach (var ch in sourceChannels)
                {
                    Console.WriteLine("Запущен отдельный поток обработки канала");
                    //taskBag[i] = Task.Run(() => safeChannels.TryAdd(i, GaussBlur(ch)));
                    taskBag[i] = Task.Run(() => (i, GaussBlur(ch)));
                    i++;
                }
                Task.WaitAll(taskBag);
                Console.WriteLine($"Обработка выполнена за {DateTime.Now - startTime}");
                return taskBag.Select(x => x.Result).OrderBy(x => x.Item1).Select(x => x.Item2).ToList();
                //return safeChannels.OrderBy(x => x.Key).Select(x => x.Value).ToList();
            }
            else
            {
                var channels = new List<int[,]>(); 
                var startTime = DateTime.Now;
                foreach (var ch in sourceChannels)
                {
                    Console.WriteLine("Запущена сихнронная обработка канала");
                    channels.Add(GaussBlur(ch));
                }
                Console.WriteLine($"Обработка выполнена за {DateTime.Now - startTime}");
                return channels;
            }
        }

        private async Task<int[,]> GaussBlurAsync(int[,] sourceCh)
        {
            var tcl = new int[height, width];
            var rs = Math.Ceiling(radius * 2.57);     // significant radius
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var val = 0d;
                    var wsum = 0d;
                    for (var iy = i - rs; iy < i + rs + 1; iy++)
                        for (var ix = j - rs; ix < j + rs + 1; ix++)
                        {
                            var x = Convert.ToInt32(Math.Round(Math.Min(width - 1, Math.Max(0, ix)), 0, MidpointRounding.AwayFromZero));
                            var y = Convert.ToInt32(Math.Round(Math.Min(height - 1, Math.Max(0, iy)), 0, MidpointRounding.AwayFromZero));
                            var dsq = (ix - j) * (ix - j) + (iy - i) * (iy - i);
                            var wght = Math.Exp(-dsq / (2 * radius * radius)) / (Math.PI * 2 * radius * radius);
                            val += sourceCh[y, x] * wght;
                            wsum += wght;
                        }
                    tcl[i, j] = (int)Math.Round(val / wsum, 2);
                }
            }
            return tcl;
        }

        private int[,] GaussBlur(int[,] sourceCh)
        {
            var tcl = new int[height, width];
            var rs = Math.Ceiling(radius * 2.57);     // significant radius
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var val = 0d;
                    var wsum = 0d;
                    for (var iy = i - rs; iy < i + rs + 1; iy++)
                        for (var ix = j - rs; ix < j + rs + 1; ix++)
                        {
                            var x = Convert.ToInt32(Math.Round(Math.Min(width - 1, Math.Max(0, ix)), 0, MidpointRounding.AwayFromZero));
                            var y = Convert.ToInt32(Math.Round(Math.Min(height - 1, Math.Max(0, iy)), 0, MidpointRounding.AwayFromZero));
                            var dsq = (ix - j) * (ix - j) + (iy - i) * (iy - i);
                            var wght = Math.Exp(-dsq / (2 * radius * radius)) / (Math.PI * 2 * radius * radius);
                            val += sourceCh[y, x] * wght;
                            wsum += wght;
                        }
                    tcl[i, j] = (int)Math.Round(val / wsum, 2);
                }
            }
            return tcl;
        }
    }
}
