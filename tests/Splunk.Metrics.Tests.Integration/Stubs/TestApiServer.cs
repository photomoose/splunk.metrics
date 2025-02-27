using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Splunk.Metrics.Abstractions;
using Splunk.Metrics.Statsd;

namespace Splunk.Metrics.Tests.Integration.Stubs
{
    public class TestApiServer : IDisposable
    {
        private readonly int _port;
        private TestServer server;

        public void Dispose()
        {
            server.Dispose();
        }

        public TestApiServer(int port)
        {
            _port = port;
        }

        public HttpClient Start()
        {
            server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(s =>
                {
                    s.AddTransient(sp => Options.Create(new StatsConfiguration
                    {
                        Prefix = "Integration.Tests",
                        Port = _port
                    }));
                    s.AddTransient<IStatsPublisher, StatsPublisher>();
                }));

            return server.CreateClient();
        }
    }
}