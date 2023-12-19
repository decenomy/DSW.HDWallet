using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.ConsoleApp.Infrastructure;
using DSW.HDWallet.Infrastructure;

namespace DSW.HDWallet.ConsoleApp.Application
{
    public class Application
    {
        private readonly IWalletManagerService walletManagerService;
        private readonly ICoinManagerService coinManagerService;
        private bool exitApp = false;

        public Application(IWalletManagerService walletManager, ICoinManagerService coinManagerService)
        {
            this.walletManagerService = walletManager;
            this.coinManagerService = coinManagerService;
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
            var coins = coinManagerService.GetAvailableCoins().ToList();
            Console.WriteLine("Available Coins:");
            for (int index = 1; index <= coins.Count; index++)
            {
                Console.WriteLine($"{index}: {coins[index - 1].Ticker} - {coins[index - 1].Name}");
            }

            Console.WriteLine("Select Coin to Add (enter number):");
            Console.WriteLine("Type '0' to return to the previous menu");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= coins.Count)
            {
                Console.WriteLine("Enter your password:");
                var password = Console.ReadLine();
                var success = coinManagerService.AddCoin(coins[choice - 1].Ticker, password);
                if (success)
                {
                    Console.WriteLine("Coin successfully added.");
                }
                else
                {
                    Console.WriteLine("Failed to add coin.");
                }
            }
            else if (choice == 0)
            {
                return;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please try again.");
            }
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
            Console.WriteLine("Choose the number of words for your mnemonic:");
            Console.WriteLine("1: 12 words");
            Console.WriteLine("2: 24 words");
            var wordCountChoice = Console.ReadLine();

            int wordCount = wordCountChoice == "2" ? 24 : 12;

            Console.WriteLine("Enter a password (or leave blank):");
            var createPassword = Console.ReadLine();

            var wallet = walletManagerService.CreateWallet(wordCount, createPassword);
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
