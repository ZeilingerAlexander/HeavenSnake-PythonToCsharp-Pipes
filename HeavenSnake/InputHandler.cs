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
            NamedPipeServerStream pipestream = new NamedPipeServerStream("InputHandlerSnakes");
            pipestream.WaitForConnection();

            BinaryReader br = new BinaryReader(pipestream);
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
                    while (true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("CLIENT DISCONNECTED FATAL");
                    }
                }
                catch (Exception)
                {
                    // failed since input wasnt 1 2 3 or 4 so just ignore lol
                }
            }
        }
    }
}
