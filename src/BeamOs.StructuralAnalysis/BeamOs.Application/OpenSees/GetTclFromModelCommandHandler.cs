using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Contracts.Common;
using BeamOs.Domain.OpenSees;
using BeamOs.Domain.OpenSees.TcpServer;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.Application.OpenSees;

public class GetTclFromModelCommandHandler(IModelRepository modelRepository)
    : ICommandHandler<ModelIdRequest, string>
{
    public async Task<string> ExecuteAsync(ModelIdRequest command, CancellationToken ct = default)
    {
        var model = await modelRepository.GetById(
            new(Guid.Parse(command.ModelId)),
            ct,
            nameof(Model.Element1ds),
            $"{nameof(Model.Element1ds)}.{nameof(Element1D.SectionProfile)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1D.Material)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1D.StartNode)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1D.EndNode)}",
            nameof(Model.PointLoads)
        );

        TclWriter tclWriter = new(model.Settings.UnitSettings);

        foreach (var element1d in model.Element1ds)
        {
            tclWriter.AddHydratedElement(element1d);
        }
        tclWriter.AddPointLoads(model.PointLoads);

        tclWriter.DefineAnalysis();
        string outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        tclWriter.Write(Path.Combine(outputDir, "model.tcl"));

        TcpListener server = null;
        try
        {
            // Set the TcpListener on port 13000.
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            // Perform a blocking call to accept requests.
            // You could also use server.AcceptSocket() here.
            var x = server.BeginAcceptTcpClient(
                new AsyncCallback(TcpServerCallback.ServerCallback),
                server
            );

            Thread.Sleep(2000);

            Process process =
                new()
                {
                    StartInfo = new ProcessStartInfo(Path.Combine(outputDir, "OpenSees.exe"))
                    {
                        Arguments = Path.Combine(outputDir, "model.tcl"),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    },
                    EnableRaisingEvents = true,
                };
            process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            //Console.WriteLine(process.StandardOutput.ReadToEnd());

            await process.WaitForExitAsync();

            process.Start();
            //process.BeginErrorReadLine();
            //process.BeginOutputReadLine();

            //Console.WriteLine(process.StandardOutput.ReadToEnd());

            await process.WaitForExitAsync();

            Thread.Sleep(10000);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server.Stop();
        }

        return tclWriter.ToString();
    }

    //void process_Exited(object sender, EventArgs e)
    //{
    //    Console.WriteLine(string.Format("process exited with code {0}\n", process.ExitCode.ToString()));
    //}

    void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        Console.WriteLine(e.Data + "\n");
    }

    void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        Console.WriteLine(e.Data + "\n");
    }
}
