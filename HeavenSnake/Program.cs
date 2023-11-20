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
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            new GameEngine(new Vector2INT() { x = 20, y = 20 });
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            GameEngine.pythonProcess.Kill();
            GameEngine.handler.Intercept = true;
            GameEngine.handler.server.Close();
            GameEngine.handler.server.Dispose();
            Thread.Sleep(1000);
        }
    }
}