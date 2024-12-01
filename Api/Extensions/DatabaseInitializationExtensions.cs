using Infrastructure.Data.Initializer;

namespace Api.Extensions
{
    public static class DatabaseInitializationExtensions
    {
        public static void InitializeDatabase(this IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
                databaseInitializer.InitializeDatabase();
            }
        }
    }
}
