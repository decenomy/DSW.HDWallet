using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure;
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

            // Tras
        }
    }
}
