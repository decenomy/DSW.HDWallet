using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.Infrastructure;

namespace DSW.HDWallet.ConsoleApp.Application
{
    public class Application
    {
        private readonly IWalletManagerService walletManagerService;
        private bool exitApp = false;

        public Application(IWalletManagerService walletManager)
        {
            this.walletManagerService = walletManager;
        }

        public void Run()
        {
            while (!exitApp)
            {
                if (walletManagerService.HasSeed())
                {
                    DisplayHomeScreenWithWallet();
                }
                else
                {
                    DisplayHomeScreenWithoutWallet();
                }
            }
        }

        private void DisplayHomeScreenWithWallet()
        {
            Console.WriteLine("Balance: $1000 (mocked)");
            Console.WriteLine("My Coins: BTC, ETH (mocked)");
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1: Delete Wallet");
            Console.WriteLine("2: Add Coin");
            Console.WriteLine("3: Select Coin");
            Console.WriteLine("4: Exit App");
            HandleHomeScreenWithWalletChoices(Console.ReadLine() ?? "");
        }

        private void DisplayHomeScreenWithoutWallet()
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1: Create Wallet");
            Console.WriteLine("2: Recover Wallet");
            Console.WriteLine("3: Exit App");
            HandleHomeScreenWithoutWalletChoices(Console.ReadLine() ?? "");
        }

        private void HandleHomeScreenWithWalletChoices(string choice)
        {
            switch (choice)
            {
                case "1":
                    DeleteWallet();
                    break;
                case "2":
                    DisplayAddCoinScreen();
                    break;
                case "3":
                    DisplaySelectCoinScreen();
                    break;
                case "4":
                    exitApp = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private void HandleHomeScreenWithoutWalletChoices(string choice)
        {
            switch (choice)
            {
                case "1":
                    CreateWallet();
                    break;
                case "2":
                    RecoverWallet();
                    break;
                case "3":
                    exitApp = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private void DisplayAddCoinScreen()
        {
            Console.WriteLine("Available Coins: BTC, ETH, XRP (mocked)");
            Console.WriteLine("Select Coin to Add:");
            Console.WriteLine("1: BTC");
            Console.WriteLine("2: ETH");
            Console.WriteLine("3: XRP");
            Console.WriteLine("4: Back");
            var choice = Console.ReadLine();
            if (choice == "4")
            {
                return; // Go back to home
            }
            AddCoin();
        }

        private void DisplaySelectCoinScreen()
        {
            Console.WriteLine("Balance: $500 (mocked)");
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1: Send");
            Console.WriteLine("2: Receive");
            Console.WriteLine("3: Back");
            var choice = Console.ReadLine();
            if (choice == "3")
            {
                return; // Go back to home
            }
            Console.WriteLine($"Option {choice} selected (mocked).");
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
            walletManagerService.DeleteWallet();
        }

        private void AddCoin()
        {
            // Mock-up for adding a coin
            Console.WriteLine("Coin added.");
        }
    }

}
