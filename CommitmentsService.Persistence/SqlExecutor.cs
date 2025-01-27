using CommitmentsService.Persistence.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace CommitmentsService.Persistence;

public static class SqlExecutor
{
    public static void DeleteInvestorCommitmentsTable(this SqliteConnection connection)
    {
        connection.Execute("DROP TABLE IF EXISTS investor_commitments;");
    }

    public static void CreateInvestorCommitmentsTable(this SqliteConnection connection)
    {
        connection.Execute("""
            CREATE TABLE IF NOT EXISTS investor_commitments (
                investor_id TEXT,
                investor_name TEXT,
                investor_type TEXT,
                investor_country TEXT,
                investor_date_added DATE,
                investor_last_updated DATE,
                commitment_asset_class TEXT,
                commitment_amount REAL,
                commitment_currency TEXT
            );
            """);
    }

    // Unfortunately there is no support for sqlite bulk insert and dapper will insert rows one at a time so to preserve transactional integrity wrap with outer transaction
    public static int InsertToCreateInvestorCommitments(this SqliteConnection connection, List<InvestorCommitment> investorCommitments)
    {
        using var transaction = connection.BeginTransaction();

        var rowsInserted = connection.Execute("""
            INSERT INTO investor_commitments(investor_id, investor_name, investor_type, investor_country, investor_date_added, investor_last_updated, commitment_asset_class, commitment_amount, commitment_currency)
            VALUES (@InvestorId, @InvestorName, @InvestorType, @InvestorCountry, @InvestorDateAdded, @InvestorLastUpdated, @CommitmentAssetClass, @CommitmentAmount, @CommitmentCurrency)
            """, investorCommitments);

        transaction.Commit();
        return rowsInserted;
    }

    public static IEnumerable<InvestorCommitmentSummary> InvestorCommitementSummaries(this SqliteConnection connection)
    {
        return connection.Query<InvestorCommitmentSummary>("""
            SELECT
                investor_id,
                investor_name,
                investor_type,
                investor_date_added,
                investor_country,
                SUM(commitment_amount) 'total_commitment'
            FROM investor_commitments
            GROUP BY
                investor_id,
                investor_name,
                investor_type,
                investor_date_added,
                investor_country;
            """);
    }

    public static IEnumerable<Commitment> InvestorCommitments(this SqliteConnection connection, string investorId)
    {
        return connection.Query<Commitment>("""
            SELECT
                commitment_asset_class,
                commitment_amount
            FROM investor_commitments
            WHERE investor_id = @InvestorId;
            """, new { InvestorId = investorId});
    }

    public static IEnumerable<CommitmentAssetSummary> InvestorCommitmentsAssetSummary(this SqliteConnection connection, string investorId)
    {
        return connection.Query<CommitmentAssetSummary>("""
            SELECT
                'All' 'commitment_asset_class',
                SUM(commitment_amount) 'total'
            FROM investor_commitments
            WHERE investor_id = @InvestorId
            UNION ALL
            SELECT
                commitment_asset_class,
                SUM(commitment_amount) 'total'
            FROM investor_commitments
            WHERE investor_id = @InvestorId
            GROUP BY commitment_asset_class;
            """, new { InvestorId = investorId });
    }
}
