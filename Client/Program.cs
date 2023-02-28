using System.Net.Sockets;
using System.Net;
using System.Text;
class Client
{
    public static int port = 8000;

    public static void ExecuteClient()
    {
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

        Socket sender = new Socket(ipAddr.AddressFamily,
                   SocketType.Stream, ProtocolType.Tcp);

        
        try
        {
            sender.Connect(localEndPoint);
            Console.WriteLine("Socket connected to -> {0} ",sender.RemoteEndPoint.ToString());

            while(true)
            {
                new Thread(() => 
                {   
                    while(true)
                    {
                        byte[] messageReceived = new byte[1024];
                        int byteRecv = sender.Receive(messageReceived);
                        Console.WriteLine("{0}\n",
                        Encoding.ASCII.GetString(messageReceived,
                                                    0, byteRecv));
                    }
                    
                }).Start();

                string message = Console.ReadLine();

                byte[] messageSent = Encoding.ASCII.GetBytes($"{message}<EOF>");
                int byteSent = sender.Send(messageSent);


                if(message == "quit")
                {
                    System.Environment.Exit(0);
                    break;
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static void Main(string[] args)
    {
        ExecuteClient();
    }
}
