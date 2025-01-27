using Microsoft.Extensions.DependencyInjection;

namespace CommitmentsService.ETL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddETLDependencies(this IServiceCollection services)
    {
        return services.AddSingleton<IFileReader, FileReader>()
            .AddHostedService<CommitmentLoaderService>();
    }
}
