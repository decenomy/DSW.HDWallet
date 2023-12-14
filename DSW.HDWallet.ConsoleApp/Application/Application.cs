using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;

namespace DSW.HDWallet.ConsoleApp.Application
{
    public class Application
    {
        private readonly IWalletService walletService;
        private readonly IWalletManagerService walletManagerService;

        public Application(IWalletService walletService, IWalletManagerService walletManager)
        {
            this.walletService = walletService;
            this.walletManagerService = walletManager;
        }

        public void Run()
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1: Create Wallet");
            Console.WriteLine("2: Recover Wallet");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Enter a password (or leave blank):");
                    var createPassword = Console.ReadLine();
                    var wallet = walletManagerService.CreateWallet(createPassword);
                    Console.WriteLine($"Wallet created. Mnemonic: {wallet}");
                    break;
                case "2":
                    Console.WriteLine("Enter Mnemonic:");
                    var mnemonic = Console.ReadLine();
                    Console.WriteLine("Enter a password (or leave blank):");
                    var recoverPassword = Console.ReadLine();
                    var recoveredWallet = walletManagerService.RecoverWallet(mnemonic ?? "", recoverPassword);
                    Console.WriteLine($"Wallet recovered: {recoveredWallet}");
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }
}
