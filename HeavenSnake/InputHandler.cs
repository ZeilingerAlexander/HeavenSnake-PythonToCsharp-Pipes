using System.IO.Pipes;

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
                    var str = new string(br.ReadChars(1));

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
