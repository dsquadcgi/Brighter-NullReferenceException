using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplication;
using WebApplication.Models;
using Xunit;
using Xunit.Abstractions;

namespace WebApplicationTests
{
    public class StupidTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public StupidTests(
            WebApplicationFactory<Startup> factory,
            ITestOutputHelper xUnitLogger)
        {
            _client = factory
                .WithWebHostBuilder(builder =>
                {
                    // capture webapi logs in xunit
                    builder.ConfigureServices(svc => { svc.AddLogging(lb => lb.AddXUnit(xUnitLogger)); });
                })
                .CreateClient();
        }

        [Fact]
        public async Task StupidParallelAsyncSpam()
        {
            // arrange
            const int nbRequests = 5000;
            const int nbThreads = 5;

            var requestsToSendInParallel = new List<HttpRequestMessage>();

            for (var j = 0; j < nbRequests; j++)
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/");
                requestsToSendInParallel.Add(httpRequest);
            }

            // act
            // reproduces seem to be more frequent when API is called in parallel ...
            await ParallelHelpers.ParallelForEachAsync(requestsToSendInParallel, nbThreads, async request =>
            {
                HttpResponseMessage response = await _client.SendAsync(request);

                // assert
                ((int)response.StatusCode).Should().NotBe(Settings.StatusCodeWhenReproduced);
            });
        }
    }
}