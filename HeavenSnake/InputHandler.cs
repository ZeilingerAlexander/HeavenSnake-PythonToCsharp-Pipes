using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeavenSnake
{
    /// <summary>
    /// contains code that connects to pipe to then get input
    /// </summary>
    internal class InputHandler
    {
        public Program.Rotation LastDirection { get; set; }
        public void startPipeListener()
        {
            // Pipe Name = InputHandlerSnake

            // Create Pipe Server stream
            NamedPipeServerStream server = new NamedPipeServerStream("InputHandlerSnakes");
            server.WaitForConnection();

            BinaryReader br = new BinaryReader(server);
            while (true)
            {
                try
                {
                    var len = (int)br.ReadUInt32();            // Read string length, this needs to be one since before reading we need to see how far we are reading
                    var str = new string(br.ReadChars(len));   // Read string with lenghsdf

                    LastDirection = (Program.Rotation)int.Parse(str);
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
