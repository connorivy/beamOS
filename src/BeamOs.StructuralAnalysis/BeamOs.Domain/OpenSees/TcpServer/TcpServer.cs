using System.Net;
using System.Net.Sockets;

namespace BeamOs.Domain.OpenSees.TcpServer;

public sealed class TcpServer : IDisposable
{
    private readonly TcpListener server;

    public TcpServer(int port)
    {
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        this.server = new(localAddr, port);
        this.server.Start();
    }

    public async Task Listen(Action<double[]> processData)
    {
        double[]? data = null;
        try
        {
            Console.Write("Waiting for a connection... ");

            // Perform a blocking call to accept requests.
            // You could also use server.AcceptSocket() here.
            using TcpClient client = await this.server.AcceptTcpClientAsync();
            Console.WriteLine("Connected!");

            //data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            // Get data size
            byte[] rawSize = new byte[sizeof(double)];
            stream.Read(rawSize, 0, rawSize.Length);
            double dataSize = BitConverter.ToDouble(rawSize, 0);
            Console.WriteLine($"Data Size == {dataSize}");

            // Loop over each step
            int sizeReceived = 0;
            data = new double[((int)dataSize - 1)];

            for (int i = 0; i < dataSize + 1; i++)
            {
                byte[] rawValue = new byte[sizeof(double)];
                stream.Read(rawValue, 0, rawValue.Length);

                if (i <= 1)
                {
                    // first value is a marker with the size
                    continue;
                }

                double value = BitConverter.ToDouble(rawValue, 0);
                data[i - 2] = value;
            }

            Console.WriteLine("Received data:");
            foreach (double val in data)
                Console.WriteLine(val);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }

        if (data is not null)
        {
            processData(data);
        }
    }

    public void Dispose()
    {
        this.server.Stop();
        this.server.Dispose();
    }
}
