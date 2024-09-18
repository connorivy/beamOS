using System.Net.Sockets;

namespace BeamOs.Domain.OpenSees.TcpServer;

public class TcpServerCallback
{
    public static void ServerCallback(IAsyncResult asyncResult)
    {
        using TcpClient client = ((TcpListener)asyncResult.AsyncState).AcceptTcpClient();
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
        double[] data = new double[(int)dataSize];

        for (int i = 0; i < dataSize; i++)
        {
            byte[] rawValue = new byte[sizeof(double)];
            stream.Read(rawValue, 0, rawValue.Length);
            double value = BitConverter.ToDouble(rawValue, 0);

            // Do not record the first value
            if (i > 0)
                data[i - 1] = value;
        }

        Console.WriteLine("Received data:");
        foreach (double val in data)
            Console.WriteLine(val);
    }
}
