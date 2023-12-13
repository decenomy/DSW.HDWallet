﻿using Microsoft.Extensions.DependencyInjection;
using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.ConsoleApp.Infrastructure;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure.WS;
using DSW.HDWallet.Infrastructure;

class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        services.AddHttpClient();
        var serviceProvider = services.BuildServiceProvider();
        var app = serviceProvider.GetService<Application>();
        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Register services with DI container
        services.AddSingleton<IWalletService, WalletService>();
        services.AddSingleton<IWalletManagerService, WalletManagerService>();
        services.AddSingleton<ICoinAddressManager, CoinAddressManager>();
        services.AddSingleton<ICoinRepository, CoinRepository>();
        services.AddSingleton<IBlockbookHttpClient, BlockbookHttpClient>();
        services.AddSingleton<Application>();
    }
}
