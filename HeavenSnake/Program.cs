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
            new GameEngine(RequestFieldSizeInput(2));
        }

        /// <summary>
        /// Request a valid Vector2Int from the user thats bigger then minsize in both direction
        /// </summary>
        /// <returns></returns>
        static Vector2INT RequestFieldSizeInput(int minsize)
        {
            Vector2INT fieldSize = new Vector2INT();
            minsize--; // -1 so it is in the actual format (starting from 0)
            while (fieldSize.x < minsize && fieldSize.y < minsize)
            {
                Console.Clear();
                Console.WriteLine("Please Input a Field size x y with a minimum of size " + minsize + 1); // +1 because the shown size is 0 1 meaning if minsize 1 it is 2 displayed
                string? input = Console.ReadLine();
                if (input == null) { continue; }
                try
                {
                    List<string[]> availiableSplits = new List<string[]>()
                    {
                        input.Split(" "),
                        input.Split("/"),
                        input.Split("-"),
                        input.Split("_"),
                        input.Split(";"),
                        input.Split("x"),
                        input.Split("*"),
                        input.Split("-"),
                        input.Split("w"),
                        input.Split("!"),
                        input.Split("  ")
                    };
                    string[]? highestSplit = availiableSplits.Find(a => a.Length == availiableSplits.Max(x => x.Length));
                    int x = int.Parse(highestSplit[0]);
                    int y = int.Parse(highestSplit[1]);
                    fieldSize = new Vector2INT() { x = x - 1, y = y - 1 };
                }
                catch (Exception) { }
            }
            Console.Clear();
            return fieldSize;
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