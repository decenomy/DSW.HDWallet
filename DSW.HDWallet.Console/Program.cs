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
                Console.WriteLine(" [ 1 ] - Create Wallet");
                Console.WriteLine(" [ 2 ] - Create Wallet With Password");
                Console.WriteLine(" [ 3 ] - Create Derived Key");                
                Console.WriteLine(" ");
                Console.WriteLine(" [ 8 ] - Recover Wallet");
                Console.WriteLine(" [ 9 ] - Random Secrect Words");
                Console.WriteLine(" ");
                Console.WriteLine(" [ 0 ] - Exit");
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