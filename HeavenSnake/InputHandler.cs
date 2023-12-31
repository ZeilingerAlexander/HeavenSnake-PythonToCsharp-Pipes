﻿using System.IO.Pipes;

namespace HeavenSnake
{
    /// <summary>
    /// contains code that connects to pipe to then get input
    /// </summary>
    internal class InputHandler
    {
        public Program.Rotation LastDirection { get; set; }
        public bool SpaceDown { get; set; }
        public NamedPipeServerStream server { get; set; }
        public bool Intercept = false;
        public void startPipeListener()
        {
            // Pipe Name = InputHandlerSnake

            // Create Pipe Server stream
            server = new NamedPipeServerStream("InputHandlerSnakes");
            server.WaitForConnection();

            BinaryReader br = new BinaryReader(server);
            while (true)
            {
                try
                {
                    if (Intercept)
                    {
                        throw new EndOfStreamException();
                    }
                    var str = new string(br.ReadChars(1));
                    if (int.Parse(str) == 5)
                    {
                        // User pressed Space
                        SpaceDown = !SpaceDown;
                        server.Flush();
                    }
                    else
                    {
                        LastDirection = (Program.Rotation)int.Parse(str);
                    }
                }
                catch (EndOfStreamException)
                {
                    server.Close();
                    server.Dispose();
                    while (true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("CLIENT DISCONNECTED FATAL");
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
