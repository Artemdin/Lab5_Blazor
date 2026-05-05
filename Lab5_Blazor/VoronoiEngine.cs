using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Lab5_Blazor
{
    public enum MetricType { Euclidean, Manhattan, Chebyshev }

    public class VoronoiPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Id { get; set; }
        public string Color { get; set; }
        public int PixelCount { get; set; } 
    }

    public class CalculationResult
    {
        public int[,] PixelMap { get; set; } // ID найближчої вершини для кожного пікселя
        public long ElapsedMilliseconds { get; set; }
        public TimeSpan ProcessorTime { get; set; }
        public long MemoryUsed { get; set; }
    }

    public class VoronoiEngine
    {
        // Розрахунок відстані за трьома формулами
        private double GetDistance(int x1, int y1, int x2, int y2, MetricType metric)
        {
            return metric switch
            {
                MetricType.Euclidean => Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)),
                MetricType.Manhattan => Math.Abs(x1 - x2) + Math.Abs(y1 - y2),
                MetricType.Chebyshev => Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2)),
                _ => throw new ArgumentException("Unknown metric")
            };
        }

        //  Головний метод обчислень (підтримує один потік або паралельність)
        public CalculationResult Generate(int width, int height, List<VoronoiPoint> vertices, MetricType metric, bool useParallel)
        {
            var result = new CalculationResult();
            int[,] map = new int[width, height];

            //  Початок вимірювань
            GC.Collect();
            long memBefore = GC.GetTotalMemory(true);
            var sw = Stopwatch.StartNew();
            var cpuBefore = Process.GetCurrentProcess().TotalProcessorTime;

            if (useParallel)
            {
               //  Поділ на рядки
                Parallel.For(0, height, y =>
                {
                    for (int x = 0; x < width; x++)
                    {
                        map[x, y] = FindNearestVertex(x, y, vertices, metric);
                    }
                });
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        map[x, y] = FindNearestVertex(x, y, vertices, metric);
                    }
                }
            }

            //  Кінець вимірювань
            sw.Stop();
            var cpuAfter = Process.GetCurrentProcess().TotalProcessorTime;
            long memAfter = GC.GetTotalMemory(false);

            result.PixelMap = map;
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            result.ProcessorTime = cpuAfter - cpuBefore;
            result.MemoryUsed = Math.Max(0, memAfter - memBefore);

            //  Підрахунок пікселів
            UpdatePixelCounts(vertices, map, width, height);

            return result;
        }

        private int FindNearestVertex(int x, int y, List<VoronoiPoint> vertices, MetricType metric)
        {
            if (vertices.Count == 0) return -1;

            int nearestId = -1;
            double minDistance = double.MaxValue;

            // Тут можна додати логіку відкидання далеких вершин
            foreach (var v in vertices)
            {
                double dist = GetDistance(x, y, v.X, v.Y, metric);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestId = v.Id;
                }
            }
            return nearestId;
        }

        // Підрахунок кількості пікселів у кожній області
        private void UpdatePixelCounts(List<VoronoiPoint> vertices, int[,] map, int width, int height)
        {
            foreach (var v in vertices) v.PixelCount = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int id = map[x, y];
                    var vertex = vertices.FirstOrDefault(v => v.Id == id);
                    if (vertex != null) vertex.PixelCount++;
                }
            }
        }

        // Алгоритм видалення X% найменших локусів
        public List<VoronoiPoint> RemoveSmallestLoci(List<VoronoiPoint> vertices, double percent)
        {
            int countToRemove = (int)(vertices.Count * (percent / 100.0));
            return vertices
                .OrderBy(v => v.PixelCount)
                .Skip(countToRemove)
                .ToList();
        }
    }
}   