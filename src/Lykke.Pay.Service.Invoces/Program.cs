using System;
using System.IO;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;

namespace Lykke.Pay.Service.Invoces
{
    [UsedImplicitly]
    internal class Program
    {
        [UsedImplicitly]
        private static void Main(string[] args)
        {
            var webHostCancellationTokenSource = new CancellationTokenSource();
            IWebHost webHost;
            Task webHostTask = null;

            var end = new ManualResetEvent(false);

            try
            {
                AssemblyLoadContext.Default.Unloading += ctx =>
                {
                    Console.WriteLine("SIGTERM recieved");

                    webHostCancellationTokenSource.Cancel();

                    end.WaitOne();
                };

                webHost = new WebHostBuilder()
                    .UseKestrel()
                    .UseUrls("http://*:5000")
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>()
                    .UseApplicationInsights()
                    .Build();

                
                webHostTask = Task.Factory.StartNew(() => webHost.Run());

                // WhenAny to handle any task termination with exception, 
                // or gracefully termination of webHostTask
                Task.WhenAny(webHostTask).Wait(webHostCancellationTokenSource.Token);
            }
            finally
            {
                Console.WriteLine("Terminating...");

                webHostCancellationTokenSource.Cancel();
              
                webHostTask?.Wait();
                
                end.Set();
            }
        }
    }
}