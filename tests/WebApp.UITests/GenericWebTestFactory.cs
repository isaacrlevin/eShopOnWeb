using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Net.Sockets;
using System.Net;
using System.Linq;

public class GenericWebTestServerFactory<TStartup> : WebApplicationFactory<Program> where TStartup : class
{
    IWebHost _host;
    public string RootUri { get; set; }
    public string FullUri { get; set; }

    public GenericWebTestServerFactory()
    {
        FullUri = "http://localhost:" + FindFreeTCPPort();
        ClientOptions.BaseAddress = new Uri(new Uri(FullUri).GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped)); //will follow redirects by default
        var Client = this.CreateClient(); //weird side effecty thing here. This shouldn't be required but it is. How do I bury this (can it be inside server?)
    }

    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        //Real TCP port
        _host = builder.UseUrls(FullUri).Build();
        _host.Start();
        RootUri = _host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.LastOrDefault(); //Last is ssl!

        //Fake Server we won't use
        return new TestServer(new WebHostBuilder().UseStartup<TStartup>());
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _host?.Dispose();
        }
    }

    public int FindFreeTCPPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
