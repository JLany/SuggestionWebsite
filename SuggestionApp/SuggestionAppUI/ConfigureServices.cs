using SuggestionAppLibrary.DataAccess.Repositories;

namespace SuggestionAppUI;

public static class ConfigureServices
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<IDbConnection, DbConnection>();

        builder.Services.AddSingleton<ICategoryRepository, MongoCategoryRepository>();
        builder.Services.AddSingleton<IStatusRepository, MongoStatusRepository>();
        builder.Services.AddSingleton<IUserRepository, MongoUserRepository>();
        builder.Services.AddSingleton<IPostRepository, MongoPostRepository>();
    }
}