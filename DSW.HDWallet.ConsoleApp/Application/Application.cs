using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Domain.Utils;
using DSW.HDWallet.Infrastructure.Interfaces;
using HDWalletConsoleApp.Infrastructure.DataStore;
using Microsoft.Extensions.Logging;

namespace DSW.HDWallet.ConsoleApp.Application
{
    public class Application
    {
        private readonly IWalletManager walletManager;
        private readonly ICoinManager coinManager;
        private readonly ITransactionManager transactionManager;
        private readonly IAddressManager addressManager;
        private readonly IStorage storage;
        private readonly ICoinBalanceRetriever coinBalanceRetriever;
        private readonly ILogger<Application> logger;

        private bool exitApp = false;

        public Application(IWalletManager walletManager, 
            ICoinManager coinManager, 
            IAddressManager addressManager,
            ITransactionManager transactionManager,
            IStorage storage,
            ICoinBalanceRetriever coinBalanceRetriever,
            ILogger<Application> logger)
        {
            this.walletManager = walletManager;
            this.coinManager = coinManager;
            this.addressManager = addressManager;
            this.transactionManager = transactionManager;
            this.storage = storage;
            this.coinBalanceRetriever = coinBalanceRetriever;
            this.logger = logger;
        }

        public void Run()
        {
            logger.LogInformation("Application starting");
            while (!exitApp)
            {
                if (walletManager.HasSeed().Result)
                {
                    DisplayHomeScreenWithWallet();
                }
                else
                {
                    DisplayHomeScreenWithoutWallet();
                }
            }
        }

        private async void DisplayHomeScreenWithWallet()
        {
            try
            {
                // Always use USD as the default currency
                var defaultCurrency = "usd";
                var totalBalanceInDefaultCurrency = await coinBalanceRetriever.GetCurrencyBalance(defaultCurrency);
                Console.WriteLine($"Balance in {defaultCurrency.ToUpper()}: {totalBalanceInDefaultCurrency}");

                // Options for the next actions
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1: Delete Wallet");
                Console.WriteLine("2: Add Coin");
                Console.WriteLine("3: Select Coin");
                Console.WriteLine("4: See All Transactions");
                Console.WriteLine("5: Exit App");
                HandleHomeScreenWithWalletChoices(Console.ReadLine() ?? "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch balance: {ex.Message}");
            }
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
                    DisplayTransactions(null);
                    break;
                case "5":
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

        private async void DisplayAddCoinScreen()
        {
            var coins = (await coinManager.GetAvailableCoins()).ToList();
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
                var success = await coinManager.AddCoin(coins[choice - 1].Ticker, password);
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

        private async void DisplayWalletCoinsScreen()
        {
            var walletCoins = await storage.GetAllWallets();
            if (!walletCoins.Any())
            {
                Console.WriteLine("No coins in wallet.");
                return;
            }

            for (int i = 0; i < walletCoins.Count(); i++)
            {
                var coin = walletCoins.ElementAt(i);
                var coinBalance = await coinBalanceRetriever.GetCoinBalance(coin.Ticker!);

                Console.WriteLine($"{i + 1}: {coin.Ticker} - Balance: {coinBalance.TotalBalance} (Confirmed: {coinBalance.Balance}, Unconfirmed: {coinBalance.UnconfirmedBalance}, Locked: {coinBalance.LockedBalance})");
            }

            Console.WriteLine("Select a coin number for more options or type '0' to return to the home screen:");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= walletCoins.Count())
            {
                var selectedCoin = walletCoins.ElementAt(choice - 1);
                DisplayCoinOptionsScreen(selectedCoin);
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

        private async void DisplayCoinOptionsScreen(Wallet selectedCoin)
        {
            var coinBalance = await coinBalanceRetriever.GetCoinBalance(selectedCoin.Ticker!);

            Console.WriteLine($"Selected Coin: {selectedCoin.Ticker}");
            Console.WriteLine($"Balance = {coinBalance.TotalBalance} (Confirmed: {coinBalance.Balance}, Unconfirmed: {coinBalance.UnconfirmedBalance}, Locked: {coinBalance.LockedBalance})");
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1: Send");
            Console.WriteLine("2: Receive");
            Console.WriteLine("3: See transactions");
            Console.WriteLine("4: Back");

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
                    DisplayTransactions(selectedCoin.Ticker);
                    break;
                case "4":
                    return; // Go back to coin list
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private async void DisplayTransactions(string? ticker = null)
        {
            try
            {
                var transactions = await storage.GetTransactions(ticker);
                // Order transactions by Timestamp from most recent to least recent
                var orderedTransactions = transactions.OrderByDescending(tx => tx.Timestamp);

                if (orderedTransactions.Any())
                {
                    Console.WriteLine($"Transactions for {(ticker ?? "all coins")}:");
                    foreach (var tx in orderedTransactions)
                    {
                        Console.WriteLine($"- TxId: {tx.TxId}, Amount: {SatoshiConverter.FromSatoshi(tx.Amount)}, Date: {tx.Timestamp}, Type: {tx.Type}");
                    }
                }
                else
                {
                    Console.WriteLine($"No transactions found for {(ticker ?? "all coins")}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving transactions: {ex.Message}");
            }
        }

        private async void CreateWallet()
        {
            Console.WriteLine("Choose the number of words for your mnemonic:");
            Console.WriteLine("1: 12 words");
            Console.WriteLine("2: 24 words");
            var wordCountChoice = Console.ReadLine();

            int wordCount = wordCountChoice == "2" ? 24 : 12;

            Console.WriteLine("Enter a password (or leave blank):");
            var createPassword = Console.ReadLine();

            var wallet = await walletManager.CreateWallet(wordCount, createPassword);
            Console.WriteLine($"Wallet created. Mnemonic: {wallet}");
        }

        private void RecoverWallet()
        {
            Console.WriteLine("Enter Mnemonic:");
            var mnemonic = Console.ReadLine();
            Console.WriteLine("Enter a password (or leave blank):");
            var recoverPassword = Console.ReadLine();
            var recoveredWallet = walletManager.RecoverWallet(mnemonic ?? "", recoverPassword);
            Console.WriteLine($"Wallet recovered: {recoveredWallet}");
        }

        private void DeleteWallet()
        {
            Console.WriteLine("Are you sure you want to delete the wallet? This action cannot be undone. (Yes/No)");
            string? confirmation = Console.ReadLine()?.Trim().ToLower();

            if (confirmation == "yes")
            {
                walletManager.DeleteWallet();
                Console.WriteLine("Wallet deleted successfully.");
            }
            else
            {
                Console.WriteLine("Wallet deletion cancelled.");
            }
        }

        private async void SendCoins(Wallet coin)
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

                var result = await transactionManager.SendCoins(coin.Ticker ?? "", numberOfCoins, address, password);
                Console.WriteLine(result.Message);
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
