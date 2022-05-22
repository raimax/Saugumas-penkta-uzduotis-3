using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SAUGUMAS_trecias
{
    public class Server
    {
        private readonly TcpListener _server;
        private NetworkStream _stream;
        private SerializedData _data = null;

        public Server(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            _server = new TcpListener(localAddr, port);
        }

        public void Start()
        {
            try
            {
                // Start listening for client requests.
                _server.Start();

                // Buffer for reading data
                byte[] bytes = new byte[2048];
                _data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for files from app 2... ");

                    TcpClient client = _server.AcceptTcpClient();
                    Console.WriteLine("Connected");

                    _data = null;

                    _stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = _stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        _data = Serializer.Deserialize(bytes);

                        Console.WriteLine("Received from app 2:");
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine("Data: " + Encoding.UTF8.GetString(_data.OriginalData));
                        Console.WriteLine("Signature: " + Convert.ToBase64String(_data.SignedData));
                        Console.ResetColor();

                        bool verified = RsaSignature.VerifySignedHash(_data.OriginalData, _data.SignedData, _data.Key);

                        if (verified)
                        {
                            Console.WriteLine();
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.WriteLine("Verified: " + verified);
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine("Verified: " + verified);
                            Console.ResetColor();
                        }
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                _server.Stop();
            }
        }

        public static object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                //binForm.Binder = new PreMergeToMergedDeserializationBinder();

                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        private void Menu()
        {
            Console.WriteLine("1: edit data");
            Console.WriteLine("2: send data to app 3");

            string userInput = Console.ReadLine();

            switch (int.Parse(userInput))
            {
                case 1:
                    Console.WriteLine("Enter new data: ");
                    string userInput2 = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(userInput2))
                    {
                        Console.WriteLine("no data written");
                        Menu();
                        break;
                    }

                    _data.OriginalData = Encoding.UTF8.GetBytes(userInput2);
                    break;
                case 2:
                    SendToApp3();
                    break;
                default:
                    Console.WriteLine("Wrong input");
                    Menu();
                    break;
            }
        }

        private void SendToApp3()
        {
            Console.WriteLine("Sending data to app 3...");
        }
    }
}
