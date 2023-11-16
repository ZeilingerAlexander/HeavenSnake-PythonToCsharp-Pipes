namespace HeavenSnake
{
    internal class Program
    {
        internal enum Rotation
        {
            Up = 1,
            Down = 3,
            Left = 4,
            Right = 2,
        }
        internal struct Vector2INT
        {
            public int x; public int y;
        }
        static void Main(string[] args)
        {
            GameEngine gameEngine = new GameEngine(new Vector2INT() { x=20,y=20});
        }
    }
}