using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DSW.HDWallet.Application.Extension
{
    public static class HDWalletServiceCollectionExtensions
    {
        public static void AddHDWalletServices(this IServiceCollection services)
        {
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IMnemonicRepository, MnemonicRepository>();
            services.AddScoped<IApiDecenomyExplorerRepository, ApiDecenomyExplorerRepository>();
            services.AddScoped<IWebSocketDecenomyExplorerRepository, WebSocketDecenomyExplorerRepository>();
            services.AddTransient<IApiDecenomyExplorerRepository, ApiDecenomyExplorerRepository>();
        }
    }
}