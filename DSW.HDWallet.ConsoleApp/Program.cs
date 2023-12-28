using Microsoft.Extensions.DependencyInjection;
using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.ConsoleApp.Infrastructure;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure;
using HDWalletConsoleApp.Infrastructure.DataStore;
using Microsoft.Extensions.Logging;
using DSW.HDWallet.Infrastructure.Interfaces;

class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        services.AddHttpClient();
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();
        var app = serviceProvider.GetService<Application>();
        if (app != null)
        {
            app.Run();
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(configure => configure.AddConsole());
        services.AddSingleton<DataStore>();
        services.AddSingleton<IStorage>(sp => sp.GetService<DataStore>()!);
        services.AddSingleton<ISecureStorage>(sp => sp.GetService<DataStore>()!);

        services.AddSingleton<ITransactionManager, TransactionManager>();
        services.AddSingleton<ICoinRepository, CoinRepository>();
        services.AddSingleton<ICoinManagerService, CoinManagerService>();
        services.AddSingleton<IWalletService, WalletService>();
        services.AddSingleton<IWalletManagerService, WalletManagerService>();
        services.AddSingleton<IAddressManager, AddressManager>();
        services.AddSingleton<ICoinRepository, CoinRepository>();
        services.AddSingleton<IBlockbookHttpClient, BlockbookHttpClient>();
        services.AddSingleton<Application>();
    }
}
