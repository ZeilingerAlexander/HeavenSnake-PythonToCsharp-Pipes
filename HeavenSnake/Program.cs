namespace HeavenSnake
{
    internal class Program
    {
        internal enum Rotation
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
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