using CommitmentsService.Persistence;
using CommitmsentsService;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace CommitmentsService.Tests.Stubs;

internal class StubCommitmentsService
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public DbConnectionProvider ConnectionProvider { get; }

    public StubCommitmentsService()
    {
        _webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(x => x.AddSingleton(new DbConnectionProvider("Data Source=Test;Mode=Memory;Cache=Shared")));
            });

        ConnectionProvider = _webApplicationFactory.Services
            .GetRequiredService<DbConnectionProvider>();
    }

    public HttpClient GetClient() => _webApplicationFactory.CreateClient();


}
