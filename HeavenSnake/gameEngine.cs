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
        Program.Rotation HeadRotation { get; set; }
        /// <summary>
        /// Field size is the direct width and height of the field + 1 meaning that positions start from 0 and end at x so the snake can always move from 0 to x
        /// </summary>
        Program.Vector2INT FieldSize { get; set; }
        Program.Vector2INT FruitPos { get; set; }
        
        void RespawnFruit()
        {

        }
        void PrintGame()
        {

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
        void MoveUp()
        {

        }
        void MoveDown()
        {

        }
        void MoveLeft()
        {

        }
        void MoveRight()
        {

        }
        void GameLoop()
        {
            PrintGame();
            ConsoleKey key = RequestInput();
            HandleInput(key);
        }

        public GameEngine(Program.Vector2INT FieldSize)
        {
            // Initialize
            Head = new Part(null);
            Parts= new List<Part>();
            this.FieldSize = FieldSize;
            ConsoleKeysWithActions = new Dictionary<ConsoleKey, Delegate>();

            // Set the delegates for the console keys
            ConsoleKeysWithActions.Add(ConsoleKey.W, MoveUp);
            ConsoleKeysWithActions.Add(ConsoleKey.S, MoveDown);
            ConsoleKeysWithActions.Add(ConsoleKey.A, MoveLeft);
            ConsoleKeysWithActions.Add(ConsoleKey.D, MoveRight);

            // Set the default Position of the Head and Default Rotation
            Head.Position = new Program.Vector2INT() { x = FieldSize.x/2, y = FieldSize.y/2 };
            HeadRotation = Program.Rotation.Up;

            // Create a single default existing part
            Part FirstPart = new Part(Head);
            FirstPart.Position = new Program.Vector2INT() { y = Head.Position.y-1, x= Head.Position.x };
            Parts.Add(FirstPart);

            // Spawn The Fruit
            RespawnFruit();

            // Start the Game Loop
        }

    }
}
