using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Sender
{
    static void Main(string[] args)
    {
        const int senderPort = 11000;
        const int receiverPort = 11001;
        const string server = "127.0.0.1";

        UdpClient sender = new UdpClient(senderPort);
        IPEndPoint receiverEP = new IPEndPoint(IPAddress.Parse(server), receiverPort);

        try
        {
            Console.WriteLine("Enter a message to send to the receiver. Type 'exit' to quit.");
            string message;
            do
            {
                message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message) && message.ToLower() != "exit")
                {
                    byte[] sendBytes = Encoding.UTF8.GetBytes(message);
                    sender.Send(sendBytes, sendBytes.Length, receiverEP);
                    Console.WriteLine("Message sent. Waiting for confirmation...");

                    // Wait for acknowledgment
                    var asyncResult = sender.BeginReceive(null, null);
                    asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)); // 5-second wait for simplicity

                    if (asyncResult.IsCompleted)
                    {
                        IPEndPoint remoteEP = null;
                        byte[] receivedBytes = sender.EndReceive(asyncResult, ref remoteEP);
                        string receivedData = Encoding.UTF8.GetString(receivedBytes);
                        Console.WriteLine($"Confirmation received: {receivedData}");
                    }
                    else
                    {
                        Console.WriteLine("No confirmation received.");
                    }
                }
            }
            while (message != null && message.ToLower() != "exit");
        }
        finally
        {
            sender.Close();
        }
    }
}