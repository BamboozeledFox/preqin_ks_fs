using CommitmentsService.Persistence;
using CommitmentsService.Persistence.Entities;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CommitmentsService.ETL;

public class CommitmentLoaderService : BackgroundService
{
    private readonly DbConnectionProvider _dbConnectionProvider;
    private readonly IFileReader _fileReader;

    public CommitmentLoaderService(DbConnectionProvider dbConnectionProvider, IFileReader fileReader)
    {
        _dbConnectionProvider = dbConnectionProvider;
        _fileReader = fileReader;
    }

    // Normally ETL would get data, do some processing and then bulk copy it to some staging table or data lake.
    // For this task I am instead truncating the table data is loaded into, and then loading the data in.
    // This is to simulate the ETL process, although a major issue if this was a real ETL process would be that
    // the db table could be be deleted as an instance was using it as other instances of the service start up.
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        using var connection = _dbConnectionProvider.GetOpenConnectionString();

        connection.DeleteInvestorCommitmentsTable();
        connection.CreateInvestorCommitmentsTable();

        var headerConsumed = false;
        var investorCommitments = new List<InvestorCommitment>();
        foreach (var line in _fileReader.ReadLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.csv")))
        {
            if (!headerConsumed)
            {
                headerConsumed = true;
                continue;
            }

            var values = line.Split(',');
            var investorCommitment = new InvestorCommitment(
                values[0],
                values[1],
                values[2],
                DateTime.Parse(values[3]),
                DateTime.Parse(values[4]),
                values[5],
                double.Parse(values[6]));
            investorCommitments.Add(investorCommitment);
        }

        connection.InsertToCreateInvestorCommitments(investorCommitments);
        return Task.CompletedTask;
    }
}
