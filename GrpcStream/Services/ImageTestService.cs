using Grpc.Core;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcStream.Services
{
    public class ImageTestService : ImageTest.ImageTestBase
    {
        private FileEventDispatcher events;

        public ImageTestService(FileEventDispatcher events)
        {
            this.events = events;
        }

        public override async Task Analyse(IAsyncStreamReader<Msg> requestStream,
                                             IServerStreamWriter<Msg> responseStream,
                                             ServerCallContext context)
        {

            while (await requestStream.MoveNext())
            {
                try
                {
                    var barr = requestStream.Current.Img.ToByteArray();
                    events.OnSentEvent(barr);
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Exit connection Stream");
                }
            }
        }
    }
}
