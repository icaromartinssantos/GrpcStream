using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcStream.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static GrpcStream.Services.FileEventDispatcher;

namespace GrpcStream
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<FileEventDispatcher>();
            services.AddGrpc(opts =>

            {

                opts.EnableDetailedErrors = true;

                opts.MaxReceiveMessageSize = 4096;

                opts.MaxSendMessageSize = 4096;

            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, FileEventDispatcher events)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/video.mp4", async context =>
                {
                    try
                    {
                        long size = -1;
                        context.Response.Headers["Content-Type"] = "video/mp4";
                        contentSent disponibilizaVideo = async (s) =>
                        {
                            await context.Response.BodyWriter.WriteAsync(s);
                            size = s.Length;
                        };

                        events.ContentSent += disponibilizaVideo;
                        while (!context.RequestAborted.IsCancellationRequested)
                        {
                            await Task.Delay(10000);
                        }
                        events.ContentSent -= disponibilizaVideo;
                    }
                    catch (Exception e )
                    {
                        Console.WriteLine(e.Message);
                    }
                });

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <title></title>
                        </head>
                        <body>
                            <div id=""videoContainer"">

                            </div>
                            <button id=""btnLive"">Live</button>

                            <script>

                                var btnLive = document.getElementById(""btnLive"");
                                var videoContainer = document.getElementById(""videoContainer"");
                                btnLive.addEventListener('click', function () {
                                    setTimeout(function () { addVideo('/video.mp4'); }, 500);
                                });

                                var addVideo = function (url) {
                                    var video = document.createElement('video');
                                    video.src = url;
                                    video.autoplay = true;
                                    video.setAttribute(""width"", ""320"");
                                    video.setAttribute(""height"", ""240"");
                                    videoContainer.appendChild(video);
                                };

                            </script>
                        </body>
                        </html>");
                });
            });

            Server server = new Server
            {
                Services = { ImageTest.BindService(new ImageTestService(events)) },
                Ports = { new ServerPort("localhost", 5001, ServerCredentials.Insecure) }
            };

            server.Start();
        }
    }
}
