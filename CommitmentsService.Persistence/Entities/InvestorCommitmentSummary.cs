using System;

namespace CommitmentsService.Persistence.Entities;

public record InvestorCommitmentSummary(string InvestorId, string InvestorName, string InvestorType, string InvestorDateAdded, string InvestorCountry, double TotalCommitment);
