using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

class Server
{

    public static int port = 8000;

    public static List<Socket> clientList = new List<Socket>();

    public static void connectClient(Socket clientSocket)
    {
        Console.WriteLine($"client connected : {clientSocket.RemoteEndPoint.ToString()}");
        clientList.Add(clientSocket);

        while (true) 
        {
            byte[] bytes = new Byte[1024];
            string? data = null;

            while (true)
            {

                int numByte = clientSocket.Receive(bytes);
                
                data += Encoding.ASCII.GetString(bytes,
                                        0, numByte);
                                            
                if (data.IndexOf("<EOF>") > -1)
                    break;
            }

            string? newData = "";
            foreach (char item in data)
            {
                if(item == '<')
                    break;
                
                newData += item;
            }

            data = newData;
            Console.WriteLine("Text received -> {0} from {1}", data, clientSocket.RemoteEndPoint.ToString());

            if(data == "quit")
            {
                string disconnectMessage = $"client disconnected : {clientSocket.RemoteEndPoint.ToString()}";
                Console.WriteLine(disconnectMessage);
                byte[] messageDisconnectBytes = Encoding.ASCII.GetBytes(disconnectMessage);
                clientList.Remove(clientSocket);
                foreach (Socket client in clientList)
                {
                    client.Send(messageDisconnectBytes);
                }
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                break;
            }    

            byte[] message = Encoding.ASCII.GetBytes($"{clientSocket.RemoteEndPoint.ToString()} sends : {data}");
            // clientSocket.Send(message);
            foreach (Socket client in clientList)
            {
                client.Send(message);
            }
        }
    }

    public static void Main(string[] args)
    {
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ip = ipHost.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ip, port);

        Socket server = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try {
         
            server.Bind(localEndPoint);
            server.Listen(10);

            while(true)
            {
                Console.WriteLine("Waiting connection ... ");
                Socket clientSocket = server.Accept();
                Thread clientThread = new Thread(()=>connectClient(clientSocket));
                clientThread.Start();
            }
        }
        
        catch (Exception e) {
            Console.WriteLine(e.ToString());
        }
    }
}   
