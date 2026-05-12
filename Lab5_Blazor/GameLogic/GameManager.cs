namespace Lab5_Blazor.GameLogic
{
    public class GameManager
    {
        public int Gold { get; set; } = 100; 
        public int CurrentWave { get; set; } = 0;
        public int Score { get; set; } = 0;

        public void ProcessEnemyDeath(bool isBoss = false)
        {
            if (isBoss)
            {
                Score += 150;
                Gold += 15;
                return;
            }

            Score += 25;
            Gold += 5;
        }

        public void LevelUpTower(Tower tower)
        {
            if (Gold >= 50)
            {
                tower.Upgrade();
                Gold -= 50;
            }
        }
    }

    public class Tower
    {
        public double Range { get; set; } = 180;
        public double FireRate { get; set; } = 1.2;
        public int Level { get; set; } = 1;

        public void Upgrade()
        {
            Level++;
            Range += 25;
            FireRate += 0.3;
        }
    }
}