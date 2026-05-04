using Lab5_Blazor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab5_Blazor.Services
{
    public enum Weather { Sunny, Rain, Snow }

    public class RaceEngine
    {
        private Vehicle? _betOnVehicle;
        private decimal _currentBetAmount;

        public List<Vehicle> Participants { get; set; } = new();
        public Weather CurrentWeather { get; set; } = Weather.Sunny;
        public bool IsRacing { get; private set; }
        public decimal UserBalance { get; set; } = 1000;

        public event Action OnStateChanged; // Для оновлення UI в Blazor

        public async Task StartRace(double trackLength)
        {
            if (IsRacing) return;

            IsRacing = true;
            var cts = new CancellationTokenSource();
            var startTime = DateTime.Now;

            double weatherMultiplier = CurrentWeather switch
            {
                Weather.Rain => 0.5,
                Weather.Snow => 0.75,
                _ => 1
            };
            // Створюємо список асинхронних задач
            var tasks = Participants.Select(async v =>
            {
                while (v.Position < trackLength && !cts.IsCancellationRequested) // Перевірка на завершення гонки
                {
                    await v.MoveAsync(weatherMultiplier, cts.Token);
                    v.RaceTime = DateTime.Now - startTime; // Оновлюємо час гонки

                    OnStateChanged?.Invoke(); // Повідомляємо фронтенд про зміни
                    await Task.Delay(50); // Частота оновлення
                }
            }).ToList();

            await Task.WhenAll(tasks); // Очікування всіх потоків

            var winner = Participants.OrderByDescending(v => v.Position).FirstOrDefault();

            if (winner != null && _betOnVehicle != null && winner == _betOnVehicle)
            {
                UserBalance += _currentBetAmount * (decimal)winner.WinCoefficient;
            }
           
            // Очищуємо дані про ставку для наступної гонки
            _betOnVehicle = null;
            _currentBetAmount = 0;

            IsRacing = false;
            OnStateChanged?.Invoke();
        }

        public void PlaceBet(Vehicle vehicle, decimal amount) // Ставка на транспортний засіб
        {
            if (UserBalance >= amount && !IsRacing)
            {
                UserBalance -= amount;
                _betOnVehicle = vehicle;
                _currentBetAmount = amount;
            }
        }

        public void ResetRace() // Повернення всіх машин на старт
        {
            foreach (var v in Participants)
            {
                v.Position = 0;
                v.RaceTime = TimeSpan.Zero;
            }
            OnStateChanged?.Invoke();
        }
    }
}