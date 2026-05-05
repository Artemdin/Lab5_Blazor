namespace Lab5_Blazor.GameLogic
{
    public static class CollisionHelper
    {
        public static bool IsHit(double x1, double y1, double x2, double y2, double radius)
        {
            // Формула відстані між точками
            double distance = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            return distance <= radius;
        }
    }
}
