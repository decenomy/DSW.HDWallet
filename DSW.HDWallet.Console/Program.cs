using DSW.HDWallet.Application.Extension;
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
            WordCount wordCount = WordCount.Twelve;
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
                Console.WriteLine(" [ 5 ]  - Create a Transaction");
                Console.WriteLine(" ");
                Console.WriteLine(" [ 8 ]  - Recover Wallet");
                Console.WriteLine(" [ 9 ]  - Random Secrect Words");
                Console.WriteLine(" ");
                Console.WriteLine(" [ 20 ] - API Explorer Get Address");
                Console.WriteLine(" [ 21 ] - API Explorer Get Transaction");
                Console.WriteLine(" [ 22 ] - API Explorer Get Transaction Specific");
                Console.WriteLine(" [ 23 ] - API Explorer Get Block Hash");
                Console.WriteLine(" [ 24 ] - API Explorer Get XPub");
                Console.WriteLine(" [ 25 ] - API Explorer Get UTxo");
                Console.WriteLine(" ");
                Console.WriteLine(" [ 30 ] - WebSocket GET Transaction");
                Console.WriteLine(" [ 31 ] - WebSocket Subscribe New Transaction");
                Console.WriteLine(" ");
                Console.WriteLine(" [ 0 ]  - Exit");
                Console.Write("\n Opção: ");
                #endregion                

                #region Menu Option
                string? option = Console.ReadLine();

                switch (option)
                {
                    #region Create Wallet
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
                    #endregion

                    #region Create Wallet With Password
                    case "2":
                        Console.WriteLine("\n\n Wallet Created With Password");

                        Console.Write("\n Enter your Password: ");
                        string? password = Console.ReadLine();

                        var createWalletWithPassword = walletAppService?.CreateWalletWithPassword(wordCount, password);
                     
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
                    #endregion

                    #region Create Derived Key
                    case "3":
                        WriteLine($"\n\n Create Derived Key", ConsoleColor.DarkGreen);
                        Console.Write(" Mnemonic : ");
                        string? mnemonic = Console.ReadLine();

                        Console.Write("\n Password : ");
                        string? _password = Console.ReadLine();

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
                        string? index = Console.ReadLine();

                        for (int i = 0; i < Convert.ToInt32(index); i++)
                        {
                            var createDeriveKey = walletAppService?.CreateDerivedKey(selectedCoin, mnemonic!, Convert.ToInt32(i), _password);
                            WriteLine($"\n Index [{i}] Address={createDeriveKey?.Address} KeyPath={createDeriveKey?.Path} \n {createDeriveKey?.PubKey}", ConsoleColor.DarkGreen);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;
                    #endregion

                    #region New Wallet - App Process
                    case "4":
                        Console.WriteLine("\n\n New Wallet - App Process");

                        Console.Write("\n Enter your Password: ");
                        string? pwd = Console.ReadLine();

                        var newWalletWithPassword = walletAppService?.CreateWalletWithPassword(wordCount, pwd);
                       
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

                        var generatePubKey = walletAppService?.GeneratePubkey(selectedCoinAPP, newWalletWithPassword?.SeedHex!, true);

                        WriteLine($"\n Public Key : {generatePubKey?.PubKey}", ConsoleColor.DarkGreen);
                        WriteLine($" Coin Type : {generatePubKey?.CoinType}", ConsoleColor.DarkGreen);
                        WriteLine($" Path : {generatePubKey?.Path}", ConsoleColor.DarkGreen);

                        Console.Write("\n Enter derived number of keys: ");
                        string? indexKey = Console.ReadLine();

                        for (int i = 0; i < Convert.ToInt32(indexKey); i++)
                        {
                            var createDeriveKey = walletAppService?.GenerateDerivePubKey(generatePubKey?.PubKey!, selectedCoinAPP, i, true);
                            WriteLine($" Index [{i}] Address={createDeriveKey?.Address} KeyPath={generatePubKey?.Path}/{createDeriveKey?.Path}", ConsoleColor.DarkGreen);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;
                    #endregion


                    #region Create a Transaction
                    case "5":
                        WriteLine($"\n Create a Transaction", ConsoleColor.DarkRed);

                        Console.Write("\n Enter SeedHex: ");
                        string walletSeed = Console.ReadLine()!;

                        WriteLine($"\n Select a coin:", ConsoleColor.DarkGreen);

                        foreach (CoinType coin in Enum.GetValues(typeof(CoinType)))
                        {
                            Console.WriteLine($" {(int)coin}: {coin}");
                        }

                        int choiceTransaction;
                        bool validChoiceTransaction = false;

                        do
                        {
                            Console.Write(" Enter the coin code: ");
                            if (int.TryParse(Console.ReadLine(), out choiceTransaction))
                            {
                                if (Enum.IsDefined(typeof(CoinType), choiceTransaction))
                                {
                                    validChoiceTransaction = true;
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
                        } while (!validChoiceTransaction);

                        CoinType transactionCoin = (CoinType)choiceTransaction;


                        Console.Write(" From Address: ");
                        string fromAddress = Console.ReadLine()!;

                        Console.Write(" To Address: ");
                        string toAddress = Console.ReadLine()!;

                        Console.Write("\n Transaction Value: ");
                        long transactionValue = Console.ReadLine()!.ToLong();


                        var _tr = walletAppService?.GenerateTransactionAsync(transactionCoin, transactionValue, walletSeed, fromAddress, toAddress).Result;

                        WriteLine($"\n Transaction Details", ConsoleColor.DarkRed);
                        ulong trTotal = 0;

                        if(_tr!.Status!.Equals("Error"))
                        {
                            WriteLine($"\n Status:\t {_tr.Status} ", ConsoleColor.DarkRed);
                            WriteLine($" Message:\t {_tr.Message} ", ConsoleColor.DarkRed);
                        }
                        else
                        {
                            foreach (var tr in _tr!.Utxos!)
                            {
                                WriteLine($" Vout index: {tr.Vout} TxId: {tr.Txid} \t Value: {tr.Value!.ToFormattedString()} {transactionCoin}", ConsoleColor.Cyan);
                                trTotal += tr.Value!.ToULong();
                            }

                            WriteLine($"\n To Address:\t {_tr.ToAddress} ", ConsoleColor.Green);
                            WriteLine($"\n Balance:\t {_tr.Balance} ", ConsoleColor.Green);
                            WriteLine($" Amount to Send: {_tr.Amount} ", ConsoleColor.Green);
                            WriteLine($" Change Amount:\t {_tr.Change} ", ConsoleColor.Green);
                            WriteLine($" Fee:\t\t {_tr.Fee} ", ConsoleColor.Green);
                            WriteLine($" Size KB:\t {_tr.SizeKb} ", ConsoleColor.Green);
                            WriteLine($" Message:\t {_tr.Message} ", ConsoleColor.Green);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;
                    #endregion

                    #region Recover Wallet
                    case "8":
                        WriteLine($"\n Recover Wallett Address", ConsoleColor.DarkRed);
                        Console.Write(" Enter Mnemonic: ");
                        string? mnemonicWords = Console.ReadLine();

                        Console.Write("\n Enter your Password: ");
                        string? passwordRecorver = Console.ReadLine();

                        string? address = string.Empty;

                        if (mnemonicWords != null)
                            address = walletAppService?.RecoverWallet(mnemonicWords, passwordRecorver);
         
                        WriteLine($" Seed Hex : {address}", ConsoleColor.Green);

                        Console.ReadLine();
                        Console.Clear();
                        break;
                    #endregion

                    #region Random Secrect Words
                    case "9":
                        var secretWords = walletAppService?.CreateWallet(WordCount.TwentyFour);
                        string[]? randomWords = secretWords?.GetRandomMnemonic(3);

                        WriteLine($"\n Mnemonic Random :", ConsoleColor.White);
                        foreach (var word in randomWords!)
                        {
                            WriteLine($" {word}", ConsoleColor.DarkCyan);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;
                    #endregion

                    #region API Explorer Get Address
                    case "20":
                        Console.WriteLine("\n\n API Explorer - Get Address");
                        Console.Write("\n Enter Coin Name: ");
                        string? addressCoinName = Console.ReadLine();

                        Console.Write(" Enter Address: ");
                        string? addressCoin = Console.ReadLine();

                        var resultAPI = walletAppService?.GetAddressAsync(addressCoinName!, addressCoin!).Result;

                        WriteLine($"\n Address  \t: {resultAPI?.Address}", ConsoleColor.DarkGreen);
                        WriteLine($" Balance        : {resultAPI?.Balance}", ConsoleColor.DarkGreen);
                        WriteLine($" Total Received : {resultAPI?.TotalReceived}", ConsoleColor.DarkGreen);
                        WriteLine($" Total Sent     : {resultAPI?.TotalSent}", ConsoleColor.DarkGreen);
                        WriteLine($" Txs            : {resultAPI?.Txs} \n\n", ConsoleColor.DarkGreen);
                        Console.WriteLine("\n TxIds List => ");
                        for (int i = 0; i < resultAPI!.Txids!.Count; i++)
                        {
                            WriteLine($" TxId [{i}] {resultAPI.Txids[i]}", ConsoleColor.DarkGreen);
                        }

                        Console.ReadLine();
                        Console.Clear();
                        break;
                    #endregion

                    #region API Explorer Get Transaction
                    case "21":
                        Console.WriteLine("\n\n API Explorer - Get Transaction");
                        Console.Write("\n Enter Coin Name: ");
                        string? tCoinName = Console.ReadLine();

                        Console.Write(" Enter TxId: ");
                        string? tTxId = Console.ReadLine();

                        var resultTransaction = walletAppService?.GetTransactionAsync(tCoinName!, tTxId!).Result;

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
                    #endregion

                    #region API Explorer Get Transaction Specific
                    case "22":
                        Console.WriteLine("\n\n API Explorer - Get Transaction Specific");
                        Console.Write("\n Enter Coin Name: ");
                        string? tsCoinName = Console.ReadLine();

                        Console.Write(" Enter TxId: ");
                        string? tsTxId = Console.ReadLine();

                        var resultTransactionSpecific = walletAppService?.GetTransactionSpecificAsync(tsCoinName!, tsTxId!).Result;

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
                    #endregion

                    #region API Explorer Get Block Hash
                    case "23":
                        Console.WriteLine("\n\n API Explorer - Get Block Hash");

                        Console.Write("\n Enter Coin Name: ");
                        string? coinName = Console.ReadLine();

                        Console.Write(" Enter Block Height: ");
                        string? blockHeight = Console.ReadLine();

                        var resulGetBlockHash = walletAppService?.GetBlockHash(coinName!, blockHeight!).Result;
                        
                        WriteLine($"\n Block Hash  \t: {resulGetBlockHash?.BlockHash}", ConsoleColor.DarkGreen);                        

                        Console.ReadLine();
                        Console.Clear();
                        break;
                    #endregion

                    #region API Explorer Get XPub
                    case "24":
                        Console.WriteLine("\n\n API Explorer - Get xPub");
                        
                        Console.Write("\n Enter Coin Name: ");
                        string? xpubCoinName = Console.ReadLine();

                        Console.Write(" Enter xPub: ");
                        string? xPub = Console.ReadLine();

                        var xPubResult = walletAppService?.GetXpub(xpubCoinName!, xPub!).Result;

                        /*
                        WriteLine($"\n Block Hash  \t: {resultTransactionSpecific?.Blockhash}", ConsoleColor.DarkGreen);
                        WriteLine($" Block Time    \t: {resultTransactionSpecific?.Blocktime}", ConsoleColor.DarkGreen);
                        WriteLine($" Confirmations \t: {resultTransactionSpecific?.Confirmations}", ConsoleColor.DarkGreen);
                        WriteLine($" Expity Height \t: {resultTransactionSpecific?.Expiryheight}", ConsoleColor.DarkGreen);
                        */
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    #endregion

                    #region API Explorer Get UTxo
                    case "25":
                        Console.WriteLine("\n\n API Explorer - Get UTXO");

                        Console.Write("\n Enter Coin Name: ");
                        string? utxoCoinName = Console.ReadLine();

                        Console.Write(" Enter Address: ");
                        string? utxoAddress = Console.ReadLine();

                        Console.Write(" Confirmed: ");
                        string? utxoConfirmed = Console.ReadLine();

                        if (utxoConfirmed!.Equals(""))
                            utxoConfirmed = "false";

                        var utxoResult = walletAppService?.GetUtxo(utxoCoinName!, utxoAddress!, Convert.ToBoolean(utxoConfirmed)).Result;

                        foreach (var item in utxoResult!)
                        {                            
                            WriteLine($"\n Height: \t {item.Height}", ConsoleColor.DarkGreen);
                            WriteLine($" Txid: \t\t {item.Txid}", ConsoleColor.DarkGreen);
                            WriteLine($" Value: \t {item.Value}", ConsoleColor.DarkGreen);
                            WriteLine($" Vout: \t\t {item.Vout}", ConsoleColor.DarkGreen);
                            WriteLine($" Confirmations:  {item.Confirmations}", ConsoleColor.DarkGreen);
                        }
                        
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    #endregion

                    #region WebSocket GET Transaction
                    case "30":
                        Console.WriteLine("\n\n Websocket GET Transaction");
                        Console.Write("\n Enter Coin Name: ");
                        string? wsCoinName = Console.ReadLine();

                        Console.Write(" Enter TxId: ");
                        string? wsaddressCoin = Console.ReadLine();
                        
                        var wss = walletAppService?.GetWSTransactionAsync(wsCoinName!, wsaddressCoin!).Result;

                        WriteLine($"\n Block Hash: \t {wss?.data?.blockHash}", ConsoleColor.DarkGreen);
                        WriteLine($" Block time: \t {wss?.data?.blockTime}", ConsoleColor.DarkGreen);
                        WriteLine($" Block Height: \t {wss?.data?.blockHeight}", ConsoleColor.DarkGreen);
                        WriteLine($" Txid: \t\t {wss?.data?.txid}", ConsoleColor.DarkGreen);
                        WriteLine($" Value In: \t {wss?.data?.valueIn}", ConsoleColor.DarkGreen);
                        WriteLine($" Value: \t {wss?.data?.value}", ConsoleColor.DarkGreen);
                        WriteLine($" Size: \t\t {wss?.data?.size}", ConsoleColor.DarkGreen);
                        WriteLine($" Confirmations:  {wss?.data?.confirmations}", ConsoleColor.DarkGreen);
                        WriteLine($" Hex: \t\t {wss?.data?.hex}", ConsoleColor.DarkGreen);

                        Console.ReadLine();
                        Console.Clear();
                        break;
                    #endregion

                    #region WebSocket Subscribe New Transaction
                    case "31":
                        Console.WriteLine("\n\n Websocket GET Transaction");
                        Console.Write("\n Enter Coin Name: ");
                        string? subCoinName = Console.ReadLine();

                        var subS = walletAppService?.SubscribeNewTransaction(subCoinName!).Result;

                        Console.ReadLine();
                        Console.Clear();
                        break;
                    #endregion

                    #region Exit
                    case "0":
                        Console.WriteLine(" Exit...");                        
                        Console.Clear();
                        return;
                    #endregion

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