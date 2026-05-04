using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab5_Blazor.Models
{
    public class Vehicle
    {
        public string Name { get; private set; }
        public string Color { get; private set; }
        public double Position { get; set; } // Координата X
        public double Speed { get; private set; }
        public double Acceleration { get; private set; }
        public TimeSpan RaceTime { get; set; }
        public bool IsBroken { get; private set; }
        public double WinCoefficient { get; set; }

        private Random _rng = new Random();

        public Vehicle(string name, string color)
        {
            Name = name;
            Color = color;
            Speed = _rng.Next(5, 11); // швидкість від 5 до 10 одиниць
            WinCoefficient = Math.Round(1.5 + _rng.NextDouble() * 2, 2);
        }

        // Асинхронний метод зміни прискорення
        public async Task ChangeAcceleration()
        {
            Acceleration = 0.7 + (_rng.NextDouble() * 0.3);
            await Task.CompletedTask;
        }

        public async Task MoveAsync(double weatherMultiplier, CancellationToken ct)
        {
            if (IsBroken) return;

            // Додаємо вплив погоди на ймовірність поломки
            // Якщо дощ, шанс поломки зростає
            double breakdownChance = 0.005;
            if (weatherMultiplier < 1.0) breakdownChance = 0.02; // В погану погоду ламається частіше

            if (_rng.NextDouble() < breakdownChance)
            {
                await Breakdown();
                return;
            }

            await ChangeAcceleration();
            Position += Speed * Acceleration * weatherMultiplier;
        }

        private async Task Breakdown()
        {
            IsBroken = true;
            await Task.Delay(2000); // Зупинка на 2 секунди
            IsBroken = false;
        }

        public void ApplyNitro()
        {
            Position += 50; // Миттєвий ривок / Turbo Boost
        }
    }
}