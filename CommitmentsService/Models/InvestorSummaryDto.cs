using System;

namespace CommitmentsService.Models;

public record InvestorSummaryDto(string Id, string Name, string Type, string DateAdded, string Country, string TotalCommitment);
