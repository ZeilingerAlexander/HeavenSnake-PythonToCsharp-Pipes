namespace HeavenSnake
{
    /// <summary>
    /// Contains method for visualizing the game
    /// </summary>
    internal partial class GameEngine
    {
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
    }
}
