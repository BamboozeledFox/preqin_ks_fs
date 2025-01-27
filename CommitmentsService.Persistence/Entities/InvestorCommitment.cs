using System;
using System.Security.Cryptography;
using System.Text;

namespace CommitmentsService.Persistence.Entities;

public record InvestorCommitment(
    string InvestorName,
    string InvestorType,
    string InvestorCountry,
    DateTime InvestorDateAdded,
    DateTime InvestorLastUpdated,
    string CommitmentAssetClass,
    double CommitmentAmount)
{
    public string CommitmentCurrency { get; } = "GBP";
    // Create a determinstic UUID to stand in as an id for the investor.
    // There is a chance for collision but it should be fairly low.
    // In a real production system I wouldn't use this hack.
    public string InvestorId { get; } = new Guid(SHA1.HashData(Encoding.UTF8.GetBytes(InvestorName))[..16]).ToString();
}
