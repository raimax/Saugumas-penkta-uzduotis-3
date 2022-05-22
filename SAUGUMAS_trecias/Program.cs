namespace SAUGUMAS_trecias
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 2222);
            server.Start();
        }
    }
}