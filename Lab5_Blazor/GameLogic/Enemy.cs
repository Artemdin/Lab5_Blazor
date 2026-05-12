using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab5_Blazor.GameLogic
{
    public class Enemy
    {
        public Guid Id { get; } = Guid.NewGuid();
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; } // Поворот машинки в градусах
        public int HP { get; set; } = 100;
        public int MaxHP { get; set; } = 100;
        public double DisplayHP { get; set; } = 100;
        public bool IsBoss { get; set; }
        public int PathPointIndex { get; set; }
        public bool IsDead => HP <= 0;
        public bool HasEscaped { get; set; }

        public async Task MoveAsync(double targetX, double targetY, CancellationToken ct)
        {
            while (!IsDead && !ct.IsCancellationRequested)
            {
                // Проста логіка: рухаємось до цілі по трохи
                if (X < targetX) X += 1.5;
                if (Y < targetY) Y += 1.5;

                await Task.Delay(20, ct); // частота оновлення
            }
        }
    }
}