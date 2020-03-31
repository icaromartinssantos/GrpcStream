using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcStream.Services
{
    public static class ImageTestService
    {

        public static void StartServer()
        {
            Server server = new Server
            {
                Services = { ImageTest.BindService(new ImageTestServiceImpl()) },
                Ports = { new ServerPort("localhost", 5001, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("RouteGuide server listening on port " + 5001);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
