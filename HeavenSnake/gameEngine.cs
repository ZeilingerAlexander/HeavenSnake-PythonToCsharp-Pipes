﻿using System;
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
        InputHandler handler { get; set; }
        Task userInputTask { get; set; }
        /// <summary>
        /// The Time between each movement action
        /// </summary>
        TimeSpan TimeBetweenMovement = TimeSpan.FromSeconds(0.5);
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
            int TotalPositions = (FieldSize.x + 1) * (FieldSize.y + 1); // Total positions
            int FreePositions = TotalPositions - (Parts.Count + 1); // Positions not occupied by parts
            Program.Vector2INT[] AvailiablePositons = new Program.Vector2INT[FreePositions]; // Will contain all availiable positions
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
            // Get a Random Position out of the availiable positions
            int rndIndex = rnd.Next(0, AvailiablePositons.Length);
            FruitPos = AvailiablePositons[rndIndex];
        }
        void PrintGame()
        {
            // First print empty canvas where eveyrthing will be drawn on top of
            string EmptyStringToPrint = new string('O', FieldSize.x + 1);
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Gray;
            for (int yaxisLength = 0; yaxisLength <= FieldSize.y + 1; yaxisLength++)
            {
                Console.WriteLine(EmptyStringToPrint);
            }

            // Then print the Snake and its Head
            Console.SetCursorPosition(Head.Position.x, Head.Position.y);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write('X');
            foreach (Part SnakePart in Parts)
            {
                Console.SetCursorPosition(SnakePart.Position.x, SnakePart.Position.y);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write('X');
            }

            // lastly print fruit
        }
        /// <summary>
        /// Checks if the Snake head is on the Fruit
        /// </summary>
        bool CheckIfEligibleForScore()
        {
            if (Head.Position.x == FruitPos.x && Head.Position.y == FruitPos.y) { return true; }
            return false;
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
            HandleDirectionChange(handler.LastDirection);
            MoveSnake();
            if (CheckIfEligibleForScore())
            {
                Score++;
                RespawnFruit();
            }
            Thread.Sleep(250);
        }

        public GameEngine(Program.Vector2INT FieldSize)
        {
            // Initialize
            Head = new Part(null);
            Parts = new List<Part>();
            this.FieldSize = FieldSize;
            handler = new InputHandler();
            handler.LastDirection = Program.Rotation.Up; // default
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

            // Start the Game Loop
            while (true)
            {
                GameLoop();
            }
        }

    }
}
