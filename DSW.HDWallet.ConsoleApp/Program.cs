using Microsoft.Extensions.DependencyInjection;
using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.ConsoleApp.Infrastructure;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure;
using HDWalletConsoleApp.Infrastructure.DataStore;

class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        services.AddHttpClient();
        var serviceProvider = services.BuildServiceProvider();
        var app = serviceProvider.GetService<Application>();
        if (app != null)
        {
            app.Run();
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IDataStore, DataStore>();
        services.AddSingleton<IWalletService, WalletService>();
        services.AddSingleton<IWalletManagerService, WalletManagerService>();
        services.AddSingleton<ICoinAddressManager, CoinAddressManager>();
        services.AddSingleton<ICoinRepository, CoinRepository>();
        services.AddSingleton<IBlockbookHttpClient, BlockbookHttpClient>();
        services.AddSingleton<Application>();
    }
}
