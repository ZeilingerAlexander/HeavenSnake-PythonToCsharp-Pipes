using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace HeavenSnake
{
    internal class GameEngine
    {
        Random rnd = new Random();

        /// <summary>
        /// The Time between each movement action
        /// </summary>
        TimeSpan TimeBetweenMovement = TimeSpan.FromSeconds(0.5);
        /// <summary>
        /// Contains the key and the method that handles that keys input
        /// </summary>
        Dictionary<ConsoleKey, Delegate> ConsoleKeysWithActions { get; set; }
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

        /// <summary>
        /// Gets all the positions availiabel for the fruit to spawn into
        /// </summary>
        /// <returns></returns>
        Program.Vector2INT[] GetAvailiableFruitPositions()
        {
            // Get all availiable Positions
            int TotalPositions = (FieldSize.x+1) * (FieldSize.y+1); // Total positions
            int FreePositions = TotalPositions - (Parts.Count + 1); // Positions not occupied by parts
            Program.Vector2INT[] AvailiablePositons = new Program.Vector2INT[FreePositions]; // Will contain all availiable positions
            int currentIndexPos = 0; // Temporary variable to store the current index of the array where to write a free position to
            for (int xAxis = 0; xAxis <= FieldSize.x; xAxis++)
            {
                for (int yAxis = 0; yAxis <= FieldSize.y; yAxis++)
                {
                    if (!Parts.Any(p => p.Position.x == xAxis && p.Position.y == yAxis) &&!(Head.Position.x == xAxis && Head.Position.y == yAxis))
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
            // Get a Random Position out of the availiable positions
            int rndIndex = rnd.Next(0, AvailiablePositons.Length);
            FruitPos = AvailiablePositons[rndIndex];
        }
        void PrintGame()
        {

        }
        /// <summary>
        /// Checks if the Snake head is on the Fruit
        /// </summary>
        bool CheckIfEligibleForScore()
        {
            if (Head.Position.x == FruitPos.x && Head.Position.y == FruitPos.y) { return true; } return false;
        }
        /// <summary>
        /// Moves the Snake to the current direction
        /// </summary>
        void MoveSnake()
        {
            // Move the Entire Snake First so it is on the "old positon"+1 meaning that head is now under piece 1, after this head gets moved
            foreach (Part SnakePart in Parts)
            {
                SnakePart.moveToParent();
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
        /// Gets the Input from a user, or Consolekey.Noname if user does not enter a key in a given timespan
        /// </summary>
        /// <returns></returns>
        ConsoleKey RequestInput()
        {
            ConsoleKey UserInput = ConsoleKey.NoName;
            var userInputTask = Task.Run(() =>
            {
                UserInput = Console.ReadKey().Key;
            });
            userInputTask.Wait(TimeBetweenMovement);
            // UserInput is now either a key they pressed or no name if they didnt press anything in the timespan
            return UserInput;
        }
        /// <summary>
        /// Handles The Given Consolekey and performs actions depending on it
        /// </summary>
        /// <param name="key"></param>
        void HandleInput(ConsoleKey key)
        {
            if (ConsoleKeysWithActions.ContainsKey(key))
            {
                ConsoleKeysWithActions[key].DynamicInvoke();
            }
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
        void GameLoop()
        {
            // Print the game then wait for input until time is over, handle that input if there even was any then move the snake and check if elegible for score
            PrintGame();
            ConsoleKey key = RequestInput();
            HandleInput(key);
            MoveSnake();
            if (CheckIfEligibleForScore())
            {
                Score++;
                RespawnFruit();
            }
        }

        public GameEngine(Program.Vector2INT FieldSize)
        {
            // Initialize
            Head = new Part(null);
            Parts = new List<Part>();
            this.FieldSize = FieldSize;
            ConsoleKeysWithActions = new Dictionary<ConsoleKey, Delegate>
            {
                // Set the delegates for the console keys
                { ConsoleKey.W, () => HandleDirectionChange(Program.Rotation.Up) },
                { ConsoleKey.S, () => HandleDirectionChange(Program.Rotation.Down) },
                { ConsoleKey.A, () => HandleDirectionChange(Program.Rotation.Left) },
                { ConsoleKey.D, () => HandleDirectionChange(Program.Rotation.Right) },
                { ConsoleKey.UpArrow, () => HandleDirectionChange(Program.Rotation.Up) },
                { ConsoleKey.DownArrow, () => HandleDirectionChange(Program.Rotation.Down) },
                { ConsoleKey.LeftArrow, () => HandleDirectionChange(Program.Rotation.Left) },
                { ConsoleKey.RightArrow, () => HandleDirectionChange(Program.Rotation.Right) }
            };

            // Set the default Position of the Head and Default Rotation
            Head.Position = new Program.Vector2INT() { x = FieldSize.x / 2, y = FieldSize.y / 2 };
            HeadRotation = Program.Rotation.Up;

            // Create a single default existing part
            Part FirstPart = new Part(Head);
            FirstPart.Position = new Program.Vector2INT() { y = Head.Position.y - 1, x = Head.Position.x };
            Parts.Add(FirstPart);

            // Spawn The Fruit
            RespawnFruit();

            // Start the Game Loop
            while (true)
            {
                GameLoop();
            }
        }

    }
}
