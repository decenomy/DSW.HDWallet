using DSW.HDWallet.Application;
using DSW.HDWallet.ConsoleApp.Domain;
using DSW.HDWallet.ConsoleApp.Infrastructure;
using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Domain.Transaction;
using DSW.HDWallet.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DSW.HDWallet.ConsoleApp.Application
{
    public class Application
    {
        private readonly IWalletManagerService walletManagerService;
        private readonly ICoinManagerService coinManagerService;
        private readonly IAddressManager addressManager;
        private readonly ILogger<Application> logger;

        private bool exitApp = false;

        public Application(IWalletManagerService walletManager, 
            ICoinManagerService coinManagerService, 
            IAddressManager addressManager,
            ILogger<Application> logger)
        {
            this.walletManagerService = walletManager;
            this.coinManagerService = coinManagerService;
            this.addressManager = addressManager;
            this.logger = logger;
        }

        public void Run()
        {
            logger.LogInformation("Application starting");
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
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1: Delete Wallet");
            Console.WriteLine("2: Add Coin");
            Console.WriteLine("3: Select Coin");
            Console.WriteLine("4: List Wallet Coins");
            Console.WriteLine("5: Exit App");
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
                    DisplayWalletCoinsScreen();
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

        private void DisplayWalletCoinsScreen()
        {
            var walletCoins = coinManagerService.GetWalletCoins();
            if (!walletCoins.Any())
            {
                Console.WriteLine("No coins in wallet.");
                return;
            }

            for (int i = 0; i < walletCoins.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {walletCoins[i].Ticker} - {walletCoins[i].Balance ?? 0}");
            }

            Console.WriteLine("Select a coin number for more options or type '0' to return to the home screen:");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= walletCoins.Count)
            {
                DisplayCoinOptionsScreen(walletCoins[choice - 1]);
            }
            else if (choice == 0)
            {
                return; // Return to home screen
            }
            else
            {
                Console.WriteLine("Invalid choice. Please try again.");
            }
        }

        private void DisplayCoinOptionsScreen(Wallet selectedCoin)
        {
            Console.WriteLine($"Selected Coin: {selectedCoin.Ticker}");
            Console.WriteLine($"Balance: {selectedCoin.Balance}");
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1: Send");
            Console.WriteLine("2: Receive");
            Console.WriteLine("3: Back");

            string choice = Console.ReadLine() ?? "";
            switch (choice)
            {
                case "1":
                    SendCoins(selectedCoin);
                    break;
                case "2":
                    ReceiveCoins(selectedCoin);
                    break;
                case "3":
                    return; // Go back to coin list
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
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

        private void SendCoins(Wallet coin)
        {
            Console.WriteLine($"Send {coin.Ticker}");
            Console.WriteLine("Enter the number of coins to send (or type 'Back' to return):");
            string input = Console.ReadLine() ?? "";

            // Check for 'Back' option
            if (input?.ToLower() == "back")
            {
                return;
            }

            if (decimal.TryParse(input, out decimal numberOfCoins))
            {
                Console.WriteLine("Enter the address to send to:");
                string address = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(address))
                {
                    Console.WriteLine("Invalid address. Operation cancelled.");
                    return;
                }

                Console.WriteLine("Enter your password:");
                string password = Console.ReadLine() ?? "";

                coinManagerService.SendCoins(coin.Ticker ?? "", numberOfCoins, address, password);
                Console.WriteLine($"Mocked sending {numberOfCoins} {coin.Ticker} to {address}.");
            }
            else
            {
                Console.WriteLine("Invalid number of coins. Please try again.");
            }
        }


        private async void ReceiveCoins(Wallet coin)
        {
            try
            {
                var addressInfo = await addressManager.GetUnusedAddress(coin.Ticker!);
                if (addressInfo != null)
                {
                    Console.WriteLine($"Your address to receive {coin.Ticker}: {addressInfo.Address}");
                }
                else
                {
                    Console.WriteLine($"No address available for {coin.Ticker}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving address: {ex.Message}");
            }
        }
    }

}
