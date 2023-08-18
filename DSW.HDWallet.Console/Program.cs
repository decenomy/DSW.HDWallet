using DSW.HDWallet.Application;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure;
using NBitcoin;
using Microsoft.Extensions.DependencyInjection;

Decenomy.Project.HDWallet();

namespace Decenomy
{
    class Project
    {
        public static void HDWallet()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IWalletService, WalletService>()
                .AddScoped<IWalletRepository, WalletRepository>()
                .AddScoped<IMnemonicRepository, MnemonicRepository>()
                .BuildServiceProvider();

            var walletAppService = serviceProvider.GetService<IWalletService>();
            var createdWallet = walletAppService?.CreateWallet();

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
                Console.WriteLine(" 1 - Create Wallet");
                Console.WriteLine(" 2 - Recover Wallet");
                Console.WriteLine(" 3 - Random Secrect Words");
                Console.WriteLine(" 9 - Exit");
                Console.Write("\n Opção: ");
                #endregion

                #region Menu Option
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Console.WriteLine("\n\n Wallet Created");
                        // Exibindo informações da carteira criada
                        WriteLine($"\n Master key :  {createdWallet?.MasterKey}", ConsoleColor.DarkCyan);
                        WriteLine($" Wallet Address :  {createdWallet?.Address}", ConsoleColor.DarkGreen);
                        WriteLine($" Secrect Words : {createdWallet?.SecretWords}", ConsoleColor.DarkGreen);

                        WriteLine($"\n Secrect Words INDEX :", ConsoleColor.DarkYellow);
                        for (int i = 0; i < 12; i++)
                        {
                            WriteLine($" [{i}] {createdWallet?.SecrectWordsArray?[i]}", ConsoleColor.DarkYellow);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case "2":
                        WriteLine($"\n Recover Wallett Address", ConsoleColor.DarkRed);
                        Console.Write(" Input your Secrets Words: ");
                        string mnemonicWords = Console.ReadLine();

                        BitcoinAddress? address = null;

                        if (mnemonicWords != null)
                            address = walletAppService?.RecoverWallet(mnemonicWords);

                        // Imprime o endereço da carteira recuperado            
                        WriteLine($" Recovery Wallett Address : {address?.ToString()}", ConsoleColor.Green);

                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case "3":
                        // Chamando o método para gerar as palavras aleatoria
                        string[]? randomWords = createdWallet?.GetRandomSecretWords(3);

                        WriteLine($"\n Secrect Words Random :", ConsoleColor.White);
                        foreach (var word in randomWords)
                        {
                            WriteLine($" {word}", ConsoleColor.DarkCyan);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case "9":
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