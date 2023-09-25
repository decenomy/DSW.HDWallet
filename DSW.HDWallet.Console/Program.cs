using DSW.HDWallet.Application.Provider;
using DSW.HDWallet.Domain.Coins;
using NBitcoin;

Decenomy.Project.HDWallet();

namespace Decenomy
{
    class Project
    {
        public static void HDWallet()
        {
            WordCount wordCount = WordCount.TwentyFour;
            HDWalletServiceProvider.Initialize();            
            var walletAppService = HDWalletServiceProvider.GetWalletService();
            
            while (true)
            {
                #region Menu
                WriteLine($" __      __         _   _         _       ___                                               ", ConsoleColor.DarkGreen);
                WriteLine($" \\ \\    / /  __ _  | | | |  ___  | |_    |   \\   ___   __   ___   _ _    ___   _ __    _  _ ", ConsoleColor.DarkGreen);
                WriteLine($"  \\ \\/\\/ /  / _` | | | | | / -_) |  _|   | |) | / -_) / _| / -_) | ' \\  / _ \\ | '  \\  | || |", ConsoleColor.DarkGreen);
                WriteLine($"   \\_/\\_/   \\__,_| |_| |_| \\___|  \\__|   |___/  \\___| \\__| \\___| |_||_| \\___/ |_|_|_|  \\_, |", ConsoleColor.DarkGreen);
                WriteLine($"                                                                                       |__/ ", ConsoleColor.DarkGreen);
                WriteLine($"                                                                HDWallet Decenomy v.1.0 2023    ", ConsoleColor.DarkGreen);
                Console.WriteLine(" Select a option: \n");
                Console.WriteLine(" [ 1 ]  - Create Wallet");
                Console.WriteLine(" [ 2 ]  - Create Wallet With Password");
                Console.WriteLine(" [ 3 ]  - Create Derived Key");
                Console.WriteLine(" [ 4 ]  - New Wallet - App Process");
                Console.WriteLine(" ");
                Console.WriteLine(" [ 8 ]  - Recover Wallet");
                Console.WriteLine(" [ 9 ]  - Random Secrect Words");
                Console.WriteLine(" ");
                Console.WriteLine(" [ 20 ] - API Explorer Get Address");
                Console.WriteLine(" [ 21 ] - API Explorer Get Transaction");
                Console.WriteLine(" [ 22 ] - API Explorer Get transaction Specific");
                Console.WriteLine(" ");
                Console.WriteLine(" [ 0 ]  - Exit");
                Console.Write("\n Opção: ");
                #endregion                

                #region Menu Option
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        var createdWallet = walletAppService?.CreateWallet(wordCount);

                        Console.WriteLine("\n\n Wallet Created");
                        // Exibindo informações da carteira criada
                        WriteLine($"\n Seed Hex : {createdWallet?.SeedHex}", ConsoleColor.DarkGreen);
                        WriteLine($" Mnemonic : {createdWallet?.Mnemonic}", ConsoleColor.DarkGreen);

                        WriteLine($"\n Mnemonic Index :", ConsoleColor.DarkYellow);
                        for (int i = 0; i < Convert.ToInt32(wordCount); i++)
                        {
                            WriteLine($" [{i}] {createdWallet?.MnemonicArray?[i]}", ConsoleColor.DarkYellow);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case "2":
                        Console.WriteLine("\n\n Wallet Created With Password");

                        Console.Write("\n Enter your Password: ");
                        string password = Console.ReadLine();
                        // With Password
                        var createWalletWithPassword = walletAppService?.CreateWalletWithPassword(wordCount, password);

                        // Exibindo informações da carteira criada                        
                        WriteLine($"\n Seed Hex : {createWalletWithPassword?.SeedHex}", ConsoleColor.DarkGreen);
                        WriteLine($" Mnemonic : {createWalletWithPassword?.Mnemonic}", ConsoleColor.DarkGreen);

                        WriteLine($"\n Mnemonic Index :", ConsoleColor.DarkYellow);
                        for (int i = 0; i < Convert.ToInt32(wordCount); i++)
                        {
                            WriteLine($" [{i}] {createWalletWithPassword?.MnemonicArray?[i]}", ConsoleColor.DarkYellow);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case "3":
                        WriteLine($"\n\n Create Derived Key", ConsoleColor.DarkGreen);
                        Console.Write(" Mnemonic : ");
                        string mnemonic = Console.ReadLine();

                        Console.Write("\n Password : ");
                        string _password = Console.ReadLine();

                        WriteLine($"\n Select a coin:", ConsoleColor.DarkGreen);

                        foreach (CoinType coin in Enum.GetValues(typeof(CoinType)))
                        {
                            Console.WriteLine($" {(int)coin}: {coin}");
                        }

                        int choice;
                        bool validChoice = false;

                        do
                        {
                            Console.Write(" Enter the coin code: ");
                            if (int.TryParse(Console.ReadLine(), out choice))
                            {
                                if (Enum.IsDefined(typeof(CoinType), choice))
                                {
                                    validChoice = true;
                                }
                                else
                                {
                                    WriteLine($" Invalid coin. Try again.", ConsoleColor.DarkRed);
                                }
                            }
                            else
                            {
                                WriteLine($" Invalid coin. Try again.", ConsoleColor.DarkRed);
                            }
                        } while (!validChoice);

                        CoinType selectedCoin = (CoinType)choice;

                        Console.Write("\n Enter derived number of keys: ");
                        string index = Console.ReadLine();

                        for (int i = 0; i < Convert.ToInt32(index); i++)
                        {
                            var createDeriveKey = walletAppService?.CreateDerivedKey(selectedCoin, mnemonic, Convert.ToInt32(i), _password);
                            WriteLine($"\n Index [{i}] Address={createDeriveKey?.Address} KeyPath={createDeriveKey?.Path} \n {createDeriveKey?.PubKey}", ConsoleColor.DarkGreen);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;
                    
                    case "4":
                        Console.WriteLine("\n\n New Wallet - App Process");

                        Console.Write("\n Enter your Password: ");
                        string pwd = Console.ReadLine();
                        // With Password
                        var newWalletWithPassword = walletAppService?.CreateWalletWithPassword(wordCount, pwd);

                        // Exibindo informações da carteira criada                        
                        WriteLine($"\n Seed Hex : {newWalletWithPassword?.SeedHex}", ConsoleColor.DarkGreen);
                        WriteLine($" Mnemonic : {newWalletWithPassword?.Mnemonic}", ConsoleColor.DarkGreen);

                        WriteLine($"\n Mnemonic Index :", ConsoleColor.DarkYellow);
                        for (int i = 0; i < Convert.ToInt32(wordCount); i++)
                        {
                            WriteLine($" [{i}] {newWalletWithPassword?.MnemonicArray?[i]}", ConsoleColor.DarkYellow);
                        }



                        WriteLine($"\n Select a coin:", ConsoleColor.DarkGreen);

                        foreach (CoinType coin in Enum.GetValues(typeof(CoinType)))
                        {
                            Console.WriteLine($" {(int)coin}: {coin}");
                        }

                        int choiceAPP;
                        bool isValidChoice = false;

                        do
                        {
                            Console.Write(" Enter the coin code: ");
                            if (int.TryParse(Console.ReadLine(), out choiceAPP))
                            {
                                if (Enum.IsDefined(typeof(CoinType), choiceAPP))
                                {
                                    isValidChoice = true;
                                }
                                else
                                {
                                    WriteLine($" Invalid coin. Try again.", ConsoleColor.DarkRed);
                                }
                            }
                            else
                            {
                                WriteLine($" Invalid coin. Try again.", ConsoleColor.DarkRed);
                            }
                        } while (!isValidChoice);

                        CoinType selectedCoinAPP = (CoinType)choiceAPP;



                        //GeneratePubkey
                        var generatePubKey = walletAppService?.GeneratePubkey(selectedCoinAPP, newWalletWithPassword?.SeedHex);

                        WriteLine($"\n Public Key : {generatePubKey?.PubKey}", ConsoleColor.DarkGreen);
                        WriteLine($" Coin Type : {generatePubKey?.CoinType}", ConsoleColor.DarkGreen);
                        WriteLine($" Path : {generatePubKey?.Path}", ConsoleColor.DarkGreen);

                        Console.Write("\n Enter derived number of keys: ");
                        string indexKey = Console.ReadLine();

                        for (int i = 0; i < Convert.ToInt32(indexKey); i++)
                        {
                            var createDeriveKey = walletAppService?.GenerateDerivePubKey(generatePubKey?.PubKey, selectedCoinAPP, i);
                            WriteLine($" Index [{i}] Address={createDeriveKey?.Address} KeyPath={generatePubKey?.Path}/{createDeriveKey?.Path}", ConsoleColor.DarkGreen);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case "8":
                        WriteLine($"\n Recover Wallett Address", ConsoleColor.DarkRed);
                        Console.Write(" Enter Mnemonic: ");
                        string mnemonicWords = Console.ReadLine();

                        Console.Write("\n Enter your Password: ");
                        string passwordRecorver = Console.ReadLine();

                        string? address = string.Empty;

                        if (mnemonicWords != null)
                            address = walletAppService?.RecoverWallet(mnemonicWords, passwordRecorver);
         
                        WriteLine($" Seed Hex : {address}", ConsoleColor.Green);

                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case "9":
                        var secretWords = walletAppService?.CreateWallet(WordCount.TwentyFour);
                        string[]? randomWords = secretWords?.GetRandomMnemonic(3);

                        WriteLine($"\n Mnemonic Random :", ConsoleColor.White);
                        foreach (var word in randomWords)
                        {
                            WriteLine($" {word}", ConsoleColor.DarkCyan);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;


                    case "20":
                        Console.WriteLine("\n\n API Explorer - Get Address");
                        
                        //var wss = walletAppService?.GetWSTransactionAsync("SAPP", "443944e2c62f8eb4d321b64db94bc30c819219a8ee352e1ea3eaa2960eca070a");
                        
                        var resultAPI = walletAppService?.GetAddressAsync("SAPP", "SYKUyKpTKU45c8kEwWaYinWo8AoZqARuMh").Result;
                        WriteLine($"\n Address  \t: {resultAPI?.Address}", ConsoleColor.DarkGreen);
                        WriteLine($" Balance        : {resultAPI?.Balance}", ConsoleColor.DarkGreen);
                        WriteLine($" Total Received : {resultAPI?.TotalReceived}", ConsoleColor.DarkGreen);
                        WriteLine($" Total Sent     : {resultAPI?.TotalSent}", ConsoleColor.DarkGreen);
                        WriteLine($" Txs            : {resultAPI?.Txs} \n\n", ConsoleColor.DarkGreen);
                        Console.WriteLine("\n TxIds List => ");
                        for (int i = 0; i < resultAPI.Txids.Count; i++)
                        {
                            WriteLine($" TxId [{i}] {resultAPI.Txids[i]}", ConsoleColor.DarkGreen);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case "21":
                        Console.WriteLine("\n\n API Explorer - Get Transaction");
                        var resultTransaction = walletAppService?.GetTransactionAsync("SAPP", "443944e2c62f8eb4d321b64db94bc30c819219a8ee352e1ea3eaa2960eca070a").Result;

                        WriteLine($"\n Block Hash  \t: {resultTransaction?.BlockHash}", ConsoleColor.DarkGreen);
                        WriteLine($" Block Height  \t: {resultTransaction?.BlockHeight}", ConsoleColor.DarkGreen);
                        WriteLine($" Block Time    \t: {resultTransaction?.BlockTime}", ConsoleColor.DarkGreen);
                        WriteLine($" Confirmations \t: {resultTransaction?.Confirmations}", ConsoleColor.DarkGreen);
                        WriteLine($" Size        \t: {resultTransaction?.Size}", ConsoleColor.DarkGreen);
                        WriteLine($" Txid        \t: {resultTransaction?.Txid}", ConsoleColor.DarkGreen);
                        WriteLine($" Value       \t: {resultTransaction?.Value}", ConsoleColor.DarkGreen);
                        WriteLine($" Value In    \t: {resultTransaction?.ValueIn}", ConsoleColor.DarkGreen);
                        WriteLine($" Version     \t: {resultTransaction?.Version}", ConsoleColor.DarkGreen);
                        WriteLine($" Hex         \t: {resultTransaction?.Hex}", ConsoleColor.DarkGreen);
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case "22":
                        Console.WriteLine("\n\n API Explorer - Get Transaction Specific");
                        var resultTransactionSpecific = walletAppService?.GetTransactionSpecificAsync("SAPP", "e58263233185eaea6ca27996a63f82cf58623f048b5c26f01381af54b537f66e").Result;

                        WriteLine($"\n Block Hash  \t: {resultTransactionSpecific?.Blockhash}", ConsoleColor.DarkGreen);
                        WriteLine($" Block Time    \t: {resultTransactionSpecific?.Blocktime}", ConsoleColor.DarkGreen);
                        WriteLine($" Confirmations \t: {resultTransactionSpecific?.Confirmations}", ConsoleColor.DarkGreen);
                        WriteLine($" Expity Height \t: {resultTransactionSpecific?.Expiryheight}", ConsoleColor.DarkGreen);
                        WriteLine($" Time          \t: {resultTransactionSpecific?.Time}", ConsoleColor.DarkGreen);
                        WriteLine($" Txid          \t: {resultTransactionSpecific?.Txid}", ConsoleColor.DarkGreen);
                        WriteLine($" Value Balance \t: {resultTransactionSpecific?.ValueBalance}", ConsoleColor.DarkGreen);
                        WriteLine($" Version     \t: {resultTransactionSpecific?.Version}", ConsoleColor.DarkGreen);
                        WriteLine($" Hex         \t: {resultTransactionSpecific?.Hex}", ConsoleColor.DarkGreen);

                        Console.ReadLine();
                        Console.Clear();
                        break;


                    case "0":
                        Console.WriteLine(" Exit...");                        
                        Console.Clear();
                        return;

                    default:
                        Console.WriteLine("\n Invalid option. Please! Choose again.");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                }

                Console.WriteLine();
                #endregion
            }
        }

        #region Helper Methods
        static void WriteLine(string output, ConsoleColor color = ConsoleColor.DarkGray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(output);
            Console.ResetColor();
        }
        static void WriteLabel(string output)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(output);
            Console.ResetColor();
        }
        #endregion
    }
}