namespace Lab5_Blazor.GameLogic
{
    public class Enemy
    {
        public Guid Id { get; } = Guid.NewGuid();
        public double X { get; set; }
        public double Y { get; set; }
        public int HP { get; set; } = 100;
        public bool IsDead => HP <= 0;

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
