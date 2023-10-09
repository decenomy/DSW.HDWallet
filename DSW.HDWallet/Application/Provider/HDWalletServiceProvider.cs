using DSW.HDWallet.Application.Extension;
using Microsoft.Extensions.DependencyInjection;

namespace DSW.HDWallet.Application.Provider
{
    public static class HDWalletServiceProvider
    {
        private static IServiceProvider? _serviceProvider;

        public static void Initialize()
        {
            var services = new ServiceCollection();
            services.AddHDWalletServices();
            services.AddHttpClient();
            _serviceProvider = services.BuildServiceProvider();
        }

        public static IWalletService GetWalletService()
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("HDWalletServiceProvider must be initialized first.");
            }

            return _serviceProvider.GetService<IWalletService>()!;
        }
    }
}
