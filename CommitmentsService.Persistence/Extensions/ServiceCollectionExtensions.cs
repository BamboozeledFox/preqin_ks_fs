using Microsoft.Extensions.DependencyInjection;

namespace CommitmentsService.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistanceDependencies(this IServiceCollection services)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        return services.AddSingleton(_ => new DbConnectionProvider("Data Source=Commitments.db;Cache=Shared"));
    }
}
