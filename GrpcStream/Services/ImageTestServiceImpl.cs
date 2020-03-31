using Grpc.Core;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcStream.Services
{
    public class ImageTestServiceImpl: ImageTest.ImageTestBase
    {
        public override async Task Analyse(IAsyncStreamReader<Msg> requestStream,
                                             IServerStreamWriter<Msg> responseStream,
                                             ServerCallContext context)
        {
            try
            {
                while (await requestStream.MoveNext())
                {
                    await responseStream.WriteAsync(requestStream.Current);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Exit connection Stream");
            }

        }
    }
}
