using Microsoft.Extensions.DependencyInjection;
using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Application;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure;
using HDWalletConsoleApp.Infrastructure.DataStore;
using Microsoft.Extensions.Logging;
using DSW.HDWallet.Infrastructure.Interfaces;
using DSW.HDWallet.Infrastructure.Services;
using Microsoft.Extensions.Hosting;

class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();

        // Start the background service
        var hostedService = serviceProvider.GetService<IHostedService>() as RatesUpdateService;
        hostedService?.StartAsync(new CancellationTokenSource().Token);

        var app = serviceProvider.GetService<Application>();
        app?.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(configure => configure.AddConsole());
        services.AddSingleton<DataStore>();
        services.AddSingleton<IStorage>(sp => sp.GetService<DataStore>()!);
        services.AddSingleton<ISecureStorage>(sp => sp.GetService<DataStore>()!);

        // Register other services
        services.AddSingleton<ITransactionManager, TransactionManager>();
        services.AddSingleton<ICoinRepository, CoinRepository>();
        services.AddSingleton<ICoinManager, CoinManager>();
        services.AddSingleton<IWalletService, WalletService>();
        services.AddSingleton<IWalletManager, WalletManager>();
        services.AddSingleton<IAddressManager, AddressManager>();
        services.AddSingleton<IBlockbookHttpClient, BlockbookHttpClient>();

        services.AddSingleton<ICoinGeckoService, CoingeckoService>();
        services.AddHttpClient("coingeckoapi", client =>
        {
            client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/simple/");
        });
        // Register your RatesUpdateService as a hosted service
        services.AddHostedService<RatesUpdateService>();

        services.AddSingleton<Application>();
    }
}
