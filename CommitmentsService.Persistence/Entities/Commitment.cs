namespace CommitmentsService.Persistence.Entities;

public record Commitment(string CommitmentAssetClass, double CommitmentAmount)
{
    public string CommitmentCurrency { get;  } = "GBP";
}
