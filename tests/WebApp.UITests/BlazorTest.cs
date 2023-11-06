using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Microsoft.eShopWeb.Web;
namespace WebApp.UITests
{

    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class BlazorTest : PageTest
    {
        protected static readonly Uri RootUri = new("http://127.0.0.1");

        private readonly WebApplicationFactory<Program> _webApplicationFactory = new();

        private HttpClient _httpClient;

        [SetUp]
        public async Task BlazorSetup()
        {
            _httpClient = _webApplicationFactory.CreateClient(new()
            {
                BaseAddress = RootUri,
            });

            await Context.RouteAsync($"{RootUri.AbsoluteUri}**", async route =>
            {
                var request = route.Request;
                var content = request.PostDataBuffer is { } postDataBuffer
                    ? new ByteArrayContent(postDataBuffer)
                    : null;

                var requestMessage = new HttpRequestMessage(new(request.Method), request.Url)
                {
                    Content = content,
                };

                foreach (var header in request.Headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }

                var response = await _httpClient.SendAsync(requestMessage);
                var responseBody = await response.Content.ReadAsByteArrayAsync();
                var responseHeaders = response.Content.Headers.Select(h => KeyValuePair.Create(h.Key, string.Join(",", h.Value)));

                await route.FulfillAsync(new()
                {
                    BodyBytes = responseBody,
                    Headers = responseHeaders,
                    Status = (int)response.StatusCode,
                });
            });
        }

        [TearDown]
        public void BlazorTearDown()
        {
            _httpClient?.Dispose();
        }
    }
}
