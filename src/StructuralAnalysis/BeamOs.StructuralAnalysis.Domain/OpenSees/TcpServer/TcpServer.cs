using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace BeamOs.StructuralAnalysis.Domain.OpenSees.TcpServer;

public sealed class TcpServer : IDisposable
{
    private static readonly IPAddress localAddress = IPAddress.Parse("127.0.0.1");
    private readonly TcpListener server;
    private readonly ILogger logger;

    public int Port { get; }

    private TcpServer(ILogger logger, TcpListener server)
    {
        this.logger = logger;
        this.Port = ((IPEndPoint)server.LocalEndpoint).Port;
        this.server = server;
    }

    public static TcpServer CreateStarted(ILogger logger)
    {
        TcpListener server = new(IPAddress.Loopback, 0);
        server.Start();
        return new(logger, server);
    }

    public void ListenCallback(Action<double[]> processData)
    {
        this.server.BeginAcceptTcpClient(
            new AsyncCallback(TcpServerCallback.ServerCallback),
            this.server
        );
    }

    public async Task Listen(Action<double[]> processData)
    {
        logger.LogDebug(
            "Opening Tcp Server on endpoint {server}",
            this.server.LocalEndpoint.ToString()
        );

        double[]? data = null;

        Console.Write("Waiting for a connection... ");

        // Perform a blocking call to accept requests.Hi
        // You could also use server.AcceptSocket() here.
        using TcpClient client = await this.server.AcceptTcpClientAsync();
        Console.WriteLine("Connected!");

        //data = null;

        // Get a stream object for reading and writing
        NetworkStream stream = client.GetStream();

        // Get data size
        byte[] rawSize = new byte[sizeof(double)];
        //stream.Read(rawSize, 0, rawSize.Length);
        ServicePointManager.Expect100Continue = false;
        //var content = stream.BeginRead(
        //    rawSize,
        //    0,
        //    rawSize.Length,
        //    new AsyncCallback(TcpServerCallback.ServerCallback2),
        //    stream
        //);
        await stream.ReadExactlyAsync(rawSize, 0, rawSize.Length);
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

        Console.WriteLine(
            $"Received array of length {data.Length} from endpoint {this.server.LocalEndpoint}"
        );

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
