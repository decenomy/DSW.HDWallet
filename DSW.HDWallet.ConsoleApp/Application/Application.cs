using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;

namespace DSW.HDWallet.ConsoleApp.Application
{
    public class Application
    {
        private readonly IWalletService walletService;
        private readonly IWalletManagerService walletManagerService;
        private readonly IDataStore dataStore;

        public Application(IWalletService walletService, IWalletManagerService walletManager, IDataStore dataStore)
        {
            this.walletService = walletService;
            this.walletManagerService = walletManager;
            this.dataStore = dataStore;
        }

        public void Run()
        {
            if (dataStore.Wallets.Any())
            {
                DisplayOptionsForExistingWallet();
            }
            else
            {
                DisplayOptionsForNewWallet();
            }
        }

        private void DisplayOptionsForExistingWallet()
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("3: Delete Wallet");
            Console.WriteLine("4: Add Coin");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "3":
                    DeleteWallet();
                    break;
                case "4":
                    AddCoin();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private void DisplayOptionsForNewWallet()
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1: Create Wallet");
            Console.WriteLine("2: Recover Wallet");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateWallet();
                    break;
                case "2":
                    RecoverWallet();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private void CreateWallet()
        {
            Console.WriteLine("Enter a password (or leave blank):");
            var createPassword = Console.ReadLine();
            var wallet = walletManagerService.CreateWallet(createPassword);
            Console.WriteLine($"Wallet created. Mnemonic: {wallet}");
        }

        private void RecoverWallet()
        {
            Console.WriteLine("Enter Mnemonic:");
            var mnemonic = Console.ReadLine();
            Console.WriteLine("Enter a password (or leave blank):");
            var recoverPassword = Console.ReadLine();
            var recoveredWallet = walletManagerService.RecoverWallet(mnemonic ?? "", recoverPassword);
            Console.WriteLine($"Wallet recovered: {recoveredWallet}");
        }

        private void DeleteWallet()
        {
            // Mock-up for deleting wallet
            Console.WriteLine("Wallet deleted.");
        }

        private void AddCoin()
        {
            // Mock-up for adding a coin
            Console.WriteLine("Coin added.");
        }
    }

}
