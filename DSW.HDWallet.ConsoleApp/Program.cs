using Microsoft.Extensions.DependencyInjection;
using DSW.HDWallet.ConsoleApp.Interfaces;
using DSW.HDWallet.ConsoleApp.Services;

var serviceProvider = new ServiceCollection()
    .AddSingleton<IWalletService, WalletService>()
    .BuildServiceProvider();

var walletService = serviceProvider.GetService<IWalletService>();

while (true)
{
    Console.WriteLine("1. Create Wallet");
    Console.WriteLine("2. Recover Wallet");
    Console.WriteLine("Select an option: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.WriteLine(walletService.CreateWallet());
            break;
        case "2":
            Console.WriteLine("Enter Mnemonic: ");
            var mnemonic = Console.ReadLine();
            Console.WriteLine(walletService.RecoverWallet(mnemonic));
            break;
        default:
            Console.WriteLine("Invalid choice");
            break;
    }
}
