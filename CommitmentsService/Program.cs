using CommitmentsService.ETL.Extensions;
using CommitmentsService.Extensions;
using CommitmentsService.Models;
using CommitmentsService.Persistence;
using CommitmentsService.Persistence.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CommitmsentsService;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services
            .AddCors(opt => opt.AddPolicy("anyOrigin", policy => policy.AllowAnyOrigin()))
            .AddETLDependencies()
            .AddPersistanceDependencies();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseCors("anyOrigin");
        }

        app.UseHttpsRedirection();

        app.MapGet("/commitments/summaries", (DbConnectionProvider dbConnectionProvider) =>
        {
            using var connection = dbConnectionProvider.GetOpenConnectionString();
            var summaries = connection.InvestorCommitementSummaries();
            return summaries.Select(x => new InvestorSummaryDto
            (
                x.InvestorId,
                x.InvestorName,
                x.InvestorType,
                DateTime.Parse(x.InvestorDateAdded).ToString("MMMM dd, yyyy"),
                x.InvestorCountry,
                x.TotalCommitment.ToScaledString()
            ));
        });

        app.MapGet("/commitments/investors/{investorId}", (DbConnectionProvider dbConnectionProvider, string investorId) =>
        {
            using var connection = dbConnectionProvider.GetOpenConnectionString();

            var commitmentsAssetSummary = connection.InvestorCommitmentsAssetSummary(investorId)
                .Select(x => new AssestSummary(x.CommitmentAssetClass, x.Total.ToScaledString()))
                .ToArray();

            var commitments = connection.InvestorCommitments(investorId)
                .Select(x => new Commitment(x.CommitmentAssetClass, x.CommitmentCurrency, x.CommitmentAmount.ToScaledString()))
                .ToArray();

            return new InvestorCommitmentsDto(commitmentsAssetSummary, commitments);
        });

        await app.RunAsync();
    }
}