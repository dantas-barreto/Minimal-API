using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi;
using MinimalApi.Domain.Interfaces;
using Test.Mocks;

namespace Test.Helpers
{
    public static class Setup
    {
        public const string PORT = "5001";
        public static TestContext testContext = default!;
        public static WebApplicationFactory<Startup> http = default!;
        public static HttpClient client = default!;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            testContext = context;
            http = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("http_port", PORT);
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAdministratorService, AdministratorServiceMock>();
                });
            });

            client = http.CreateClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            client?.Dispose();
            http?.Dispose();
        }
    }
}
