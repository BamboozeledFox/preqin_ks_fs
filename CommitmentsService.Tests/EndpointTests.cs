using CommitmentsService.Models;
using CommitmentsService.Persistence;
using CommitmentsService.Tests.Stubs;
using Microsoft.Data.Sqlite;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace CommitmentsService.Tests;

public class EndpointTests
{
    private readonly StubCommitmentsService _commitmentsService;

    public EndpointTests()
    {
        _commitmentsService = new();
    }

    [Fact]
    public async Task GetCommitmentSummaries_Success()
    {
        // ARRANGE
        using var connection = _commitmentsService.ConnectionProvider.GetOpenConnectionString();
        SeedDatabase(connection);
        var client = _commitmentsService.GetClient();
        
        // ACT
        var results = await client.GetFromJsonAsync<InvestorSummaryDto[]>("/commitments/summaries");

        // ASSERT
        Assert.Equivalent(new InvestorSummaryDto[]
        {
            new("64823135-a9c9-af8f-7996-5c270ac80c56", "Alice", "asset manager", "January 12, 2020", "United Kingdom", "45M"),
            new("f64566da-2be2-f7f5-5974-dc7eed5fcd61", "Bob", "bank", "March 07, 2022", "United States", "100M")
        }, results);
    }

    [Fact]
    public async Task GetInvestorCommitments_Success()
    {
        // ARRANGE
        using var connection = _commitmentsService.ConnectionProvider.GetOpenConnectionString();
        SeedDatabase(connection);
        var client = _commitmentsService.GetClient();

        // ACT
        var results = await client.GetFromJsonAsync<InvestorCommitmentsDto>("/commitments/investors/64823135-a9c9-af8f-7996-5c270ac80c56");

        // ASSERT
        var expected = new InvestorCommitmentsDto([new("Hedge Funds", "44M"), new("Natural Resources", "1M")], [new("Hedge Funds", "GBP", "44M"), new("Natural Resources", "GBP", "1M")]);
        Assert.Equivalent(expected, results);
    }

    private static void SeedDatabase(SqliteConnection connection)
    {
        connection.DeleteInvestorCommitmentsTable();
        connection.CreateInvestorCommitmentsTable();
        connection.InsertToCreateInvestorCommitments(
        [
            new("Alice", "asset manager", "United Kingdom", new DateTime(2020, 1, 12), new DateTime(2023, 1, 12), "Hedge Funds", 44_000_000),
            new("Alice", "asset manager", "United Kingdom", new DateTime(2020, 1, 12), new DateTime(2023, 1, 12), "Natural Resources", 1_000_000),
            new("Bob", "bank", "United States", new DateTime(2022, 3, 7), new DateTime(2024, 12, 22), "Private Equity", 100_000_000),
        ]);
    }
}
