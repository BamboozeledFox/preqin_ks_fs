using System.Collections.Generic;

namespace CommitmentsService.Models;

public record InvestorCommitmentsDto(AssestSummary[] AssetSummaries, Commitment[] Commitments);

public record Commitment(string AssetClass, string Currency, string Amount);
public record AssestSummary(string AssetClass, string Amount);