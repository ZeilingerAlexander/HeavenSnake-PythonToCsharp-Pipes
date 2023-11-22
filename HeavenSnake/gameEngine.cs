namespace HeavenSnake
{
    class SaveState
    {
        public SaveState()
        {
            PartPositions = new();
        }
        public List<Program.Vector2INT> PartPositions { get; set; }
        public Part Head { get; set; }
        public int Score { get; set; }
        public Program.Vector2INT FruitPos { get; set; }

        /// <summary>
        /// Calculates the difference between the two parts list
        /// </summary>
        /// <param name="oldParts"></param>
        /// <param name="CurrentParts"></param>
        /// <returns>A tuple with 0 containing all the ones present in list1 but not list2, 1 containing all the ones present in list2 but not in list 1</returns>
        public static (List<Program.Vector2INT>, List<Part>) CalculatePartListDifference(List<Program.Vector2INT> list1, List<Part> list2)
        {
            return (list1.Where(x => !list2.Any(y => y.Position.x == x.x && y.Position.y == x.y)).ToList(),
                list2.Where(x => !list1.Any(y => y.x == x.Position.x && y.y == x.Position.y)).ToList());
        }
    }
    internal class GameEngine
    {
        public static System.Diagnostics.Process pythonProcess { get; set; }
        Random rnd = new Random();
        public static InputHandler handler { get; set; }
        /// <summary>
        /// The Time between each movement action
        /// </summary>
        TimeSpan TimeBetweenMovement = TimeSpan.FromSeconds(0.25);
        /// <summary>
        /// Parts exclude the head
        /// </summary>
        List<Part> Parts { get; set; }
        /// <summary>
        /// When moving the Parts to the head, make sure head was moved first before initializing the movement recursion
        /// </summary>
        Part Head { get; set; }
        int Score { get; set; }
        Program.Rotation HeadRotation { get; set; }
        /// <summary>
        /// Field size is the direct width and height of the field + 1 meaning that positions start from 0 and end at x so the snake can always move from 0 to x
        /// </summary>
        Program.Vector2INT FieldSize { get; set; }
        Program.Vector2INT FruitPos { get; set; }
        SaveState LastSavedState { get; set; }

        /// <summary>
        /// Gets all the positions availiabel for the fruit to spawn into
        /// </summary>
        /// <returns></returns>
        Program.Vector2INT[] GetAvailiableFruitPositions()
        {
            // Get all availiable Positions
            int TotalPositions = (FieldSize.x + 1) * (FieldSize.y + 1); // Total positions
            int FreePositions = TotalPositions - (Parts.Count + 1); // Positions not occupied by parts
            Program.Vector2INT[] AvailiablePositons = new Program.Vector2INT[FreePositions + 1]; // Will contain all availiable positions
            int currentIndexPos = 0; // Temporary variable to store the current index of the array where to write a free position to
            for (int xAxis = 0; xAxis <= FieldSize.x; xAxis++)
            {
                for (int yAxis = 0; yAxis <= FieldSize.y; yAxis++)
                {
                    if (!Parts.Any(p => p.Position.x == xAxis && p.Position.y == yAxis) && !(Head.Position.x == xAxis && Head.Position.y == yAxis))
                    {
                        // Position is availiable
                        AvailiablePositons[currentIndexPos] = new Program.Vector2INT() { x = xAxis, y = yAxis };
                        currentIndexPos++;
                    }
                }
            }

            return AvailiablePositons;
        }
        void RespawnFruit()
        {
            Program.Vector2INT[] AvailiablePositons = GetAvailiableFruitPositions();
            if (AvailiablePositons.Length == 0)
            {
                GameOver();
                return;
            }
            // Get a Random Position out of the availiable positions
            int rndIndex = rnd.Next(0, AvailiablePositons.Length);
            FruitPos = AvailiablePositons[rndIndex];
        }
        /// <summary>
        /// Prints the Full Empty Canvas
        /// </summary>
        void PrintEmptyCanvas()
        {
            string EmptyStringToPrint = new string('O', FieldSize.x + 1);
            try
            {
                Console.SetCursorPosition(0, 0);
            }
            catch (ArgumentOutOfRangeException) { return; }
            Console.ForegroundColor = ConsoleColor.Gray;
            for (int yaxisLength = 0; yaxisLength <= FieldSize.y; yaxisLength++)
            {
                Console.WriteLine(EmptyStringToPrint);
            }
        }
        /// <summary>
        /// Prints "WINDOW TOO SMALL" until user resizes it, then re-prints everything
        /// </summary>
        void WaitUntilCorrectWindowSize()
        {
            Console.Clear();
            while (Console.BufferHeight < FieldSize.y + 3 || Console.BufferWidth < FieldSize.x + 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WINDOW TOO SMALL");
                Console.WriteLine("WINDOW TOO SMALL");
                Console.WriteLine("WINDOW TOO SMALL");
            }
            // Window Size is correct again, reprint everyhing, if user resizes during this period it will call this method again recursively. this may result in stack overflow excepts
            Console.Clear();
            PrintEmptyCanvas();

            // Print Head
            try
            {
                Console.SetCursorPosition(Head.Position.x, Head.Position.y);
            }
            catch (ArgumentOutOfRangeException)
            {
                WaitUntilCorrectWindowSize();
                return;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write('X');

            // Print Snake
            foreach (Part p in Parts)
            {
                try
                {
                    Console.SetCursorPosition(p.Position.x, p.Position.y);
                }
                catch (ArgumentOutOfRangeException)
                {
                    WaitUntilCorrectWindowSize();
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write('O');
            }

            // Print Fruit
            try
            {
                Console.SetCursorPosition(FruitPos.x, FruitPos.y);
            }
            catch (ArgumentOutOfRangeException)
            {
                WaitUntilCorrectWindowSize();
                return;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("@");
            LastSavedState.FruitPos = FruitPos;
        }
        void PrintGame()
        {
            // minimum size needed to print (size.y + 3 since we print score on size.y + 2 with is only shown on size.y + 3 AND size.x + 1 since we print to size.x + 1)
            if (Console.BufferHeight < FieldSize.y + 3 || Console.BufferWidth < FieldSize.x + 1)
            {
                WaitUntilCorrectWindowSize();
            }

            // Get difference of both lists so we can print what changed, removed and added refers to the visual effect
            (List<Program.Vector2INT>, List<Part>) differences = SaveState.CalculatePartListDifference(LastSavedState.PartPositions, Parts);
            foreach (Program.Vector2INT RemovedPart in differences.Item1)
            {
                // Print empty space where part was removed
                try
                {
                    Console.SetCursorPosition(RemovedPart.x, RemovedPart.y);
                }
                catch (ArgumentOutOfRangeException)
                {
                    WaitUntilCorrectWindowSize();
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("O");
            }
            foreach (Part AddedPart in differences.Item2)
            {
                // Print part where part was added
                try
                {
                    Console.SetCursorPosition(AddedPart.Position.x, AddedPart.Position.y);
                }
                catch (ArgumentOutOfRangeException)
                {
                    WaitUntilCorrectWindowSize();
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write('O');
            }
            // Check if FruitPos changed, this also means that score must have changed
            if (FruitPos.x != LastSavedState.FruitPos.x || FruitPos.y != LastSavedState.FruitPos.y)
            {
                // New FruitPos must be a fruit since it just respawned
                try
                {
                    // Print fruit on new fruit pos and also save the new fruit pos
                    Console.SetCursorPosition(FruitPos.x, FruitPos.y);
                }
                catch (ArgumentOutOfRangeException)
                {
                    WaitUntilCorrectWindowSize();
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("@");
                LastSavedState.FruitPos = FruitPos;

                // Then print the score at the bottom
                try
                {
                    Console.SetCursorPosition(0, FieldSize.y + 2);
                }
                catch (ArgumentOutOfRangeException)
                {
                    WaitUntilCorrectWindowSize();
                    return;
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("SCORE : ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Score);
            }

            // Head must have moved so just print that, old head pos must have been overwritten with a part anwyways
            try
            {
                Console.SetCursorPosition(Head.Position.x, Head.Position.y);
            }
            catch (ArgumentOutOfRangeException)
            {
                WaitUntilCorrectWindowSize();
                return;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write('X');

            // Always print something on the empty space between canvas and score since thats where new parts are first spawned
            try
            {
                Console.SetCursorPosition(0, FieldSize.y + 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                WaitUntilCorrectWindowSize();
                return;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write('|');

            // Save the Current State as last state 
            LastSavedState.PartPositions = Parts.Select(p => p.Position).ToList();
            LastSavedState.Head = Head;
        }
        /// <summary>
        /// Checks if the Snake head is on the Fruit
        /// </summary>
        bool CheckIfEligibleForScore()
        {
            if (Head.Position.x == FruitPos.x && Head.Position.y == FruitPos.y)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Moves the Snake to the current direction
        /// </summary>
        void MoveSnake()
        {
            // Move the Entire Snake First so it is on the "old positon"+1 meaning that head is now under piece 1, after this head gets moved
            for (int i = Parts.Count - 1; i >= 0; i--)
            {
                Parts[i].moveToParent();
            }
            // Move the Head
            switch (HeadRotation)
            {
                case Program.Rotation.Up:
                    // Check if it would go out of bounds
                    if (Head.Position.y == 0)
                    {
                        Head.Position = new Program.Vector2INT() { x = Head.Position.x, y = FieldSize.y };
                    }
                    else
                    {
                        Head.Position = new Program.Vector2INT() { x = Head.Position.x, y = Head.Position.y - 1 };
                    }
                    break;
                case Program.Rotation.Down:
                    // Check if it would go out of bounds
                    if (Head.Position.y == FieldSize.y)
                    {
                        Head.Position = new Program.Vector2INT() { x = Head.Position.x, y = 0 };
                    }
                    else
                    {
                        Head.Position = new Program.Vector2INT() { x = Head.Position.x, y = Head.Position.y + 1 };
                    }
                    break;
                case Program.Rotation.Left:
                    // Check if it would go out of bounds
                    if (Head.Position.x == 0)
                    {
                        Head.Position = new Program.Vector2INT() { x = FieldSize.x, y = Head.Position.y };
                    }
                    else
                    {
                        Head.Position = new Program.Vector2INT() { x = Head.Position.x - 1, y = Head.Position.y };
                    }
                    break;
                case Program.Rotation.Right:
                    // Check if it would go out of bounds
                    if (Head.Position.x == FieldSize.x)
                    {
                        Head.Position = new Program.Vector2INT() { x = 0, y = Head.Position.y };
                    }
                    else
                    {
                        Head.Position = new Program.Vector2INT() { x = Head.Position.x + 1, y = Head.Position.y };
                    }
                    break;
            }
        }
        /// <summary>
        /// Checks if head is inside part
        /// </summary>
        /// <returns></returns>
        bool CheckIfHeadInPart()
        {
            if (Parts.Any(p => p.Position.x == Head.Position.x && p.Position.y == Head.Position.y))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Game Over Screen
        /// </summary>
        void GameOver()
        {
            Console.Clear();

            // Show Game over for 3 Seconds
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("   _____          __  __ ______    ______      ________ _____  \r\n  / ____|   /\\   |  \\/  |  ____|  / __ \\ \\    / /  ____|  __ \\ \r\n | |  __   /  \\  | \\  / | |__    | |  | \\ \\  / /| |__  | |__) |\r\n | | |_ | / /\\ \\ | |\\/| |  __|   | |  | |\\ \\/ / |  __| |  _  / \r\n | |__| |/ ____ \\| |  | | |____  | |__| | \\  /  | |____| | \\ \\ \r\n  \\_____/_/    \\_\\_|  |_|______|  \\____/   \\/   |______|_|  \\_\\\r\n                                                               \r\n                                                               ");
            Thread.Sleep(TimeSpan.FromSeconds(3));

            // Reset Snake
            Parts = new List<Part>();
            Head.Position = new Program.Vector2INT() { x = FieldSize.x / 2, y = FieldSize.y / 2 };
            handler.LastDirection = Program.Rotation.Up;
            HeadRotation = Program.Rotation.Up;
            // Create a single default existing part
            Part FirstPart = new Part(Head);
            FirstPart.Position = new Program.Vector2INT() { y = Head.Position.y - 1, x = Head.Position.x };
            Parts.Add(FirstPart);
            Score = 0;
            TimeBetweenMovement = TimeSpan.FromSeconds(0.25);

            // Spawn The Fruit
            RespawnFruit();

            // Clear Console Again to remove the Game over
            Console.Clear();

            // Print empty canvas
            PrintEmptyCanvas();

            // After this game just continues
        }
        /// <summary>
        /// Handles Direction Change, checks if new direction is valid
        /// </summary>
        /// <param name="direction"></param>
        void HandleDirectionChange(Program.Rotation direction)
        {
            if (HeadRotation == direction)
            {
                return;
            }
            // Check if the Direction would go to the opposite
            if ((HeadRotation == Program.Rotation.Up && direction == Program.Rotation.Down) ||
                (HeadRotation == Program.Rotation.Down && direction == Program.Rotation.Up) ||
                (HeadRotation == Program.Rotation.Left && direction == Program.Rotation.Right) ||
                (HeadRotation == Program.Rotation.Right && direction == Program.Rotation.Left))
            {
                return;
            }

            // Change Direction
            HeadRotation = direction;
        }
        void SpawnNewPartOnSnake()
        {
            Part p = new Part(Parts.Last());
            // Set p position to the empty space between the score and canvas
            p.Position = new Program.Vector2INT() { x = 0, y = FieldSize.y + 1 };
            Parts.Add(p);
        }
        void StartPythonClient()
        {
            pythonProcess = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C python " + @"InputHandler\InputHandler.py";
            pythonProcess.StartInfo = startInfo;
            pythonProcess.Start();
        }
        void GameLoop()
        {
            // Print the game then wait for input until time is over, handle that input if there even was any then move the snake and check if elegible for score
            PrintGame();
            Thread.Sleep(handler.SpaceDown ? TimeBetweenMovement / 4 : TimeBetweenMovement);
            HandleDirectionChange(handler.LastDirection);
            MoveSnake();
            if (CheckIfHeadInPart())
            {
                GameOver();
            }
            if (CheckIfEligibleForScore())
            {
                Score++;
                SpawnNewPartOnSnake();
                RespawnFruit();
                // Make the speed more
                TimeBetweenMovement -= TimeSpan.FromMilliseconds(2.5);
            }
        }

        public GameEngine(Program.Vector2INT FieldSize)
        {
            // Initialize
            Head = new Part(null);
            Parts = new List<Part>();
            this.FieldSize = FieldSize;
            handler = new InputHandler();
            handler.LastDirection = Program.Rotation.Up; // default
            LastSavedState = new SaveState();

            // Start Pipeline for getting inputs
            Task.Run(handler.startPipeListener);
            // Set the default Position of the Head and Default Rotation
            Head.Position = new Program.Vector2INT() { x = FieldSize.x / 2, y = FieldSize.y / 2 };
            HeadRotation = Program.Rotation.Up;

            // Create a single default existing part
            Part FirstPart = new Part(Head);
            FirstPart.Position = new Program.Vector2INT() { y = Head.Position.y - 1, x = Head.Position.x };
            Parts.Add(FirstPart);

            // Spawn The Fruit
            RespawnFruit();

            // Start the Python Input Listener
            StartPythonClient();

            // Print empty canvas once
            PrintEmptyCanvas();

            // Start the Game Loop
            while (true)
            {
                GameLoop();
            }
        }

    }
}
