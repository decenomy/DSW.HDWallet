using NBitcoin;

Decenomy.Project.HDWallet();

namespace Decenomy
{
    class Project
    {
        public static void HDWallet()
        {
            #region Recovery Address
            // Criar config Network
            #endregion

            #region Create Wallet
            // Gera uma nova frase mnemônica com 12 palavras em inglês
            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);

            // Cria uma chave mestre a partir das palavras mnemônicas
            ExtKey masterKey = mnemo.DeriveExtKey();

            // Deriva a chave pública mestre
            ExtPubKey masterPubKey = masterKey.Neuter();

            // Obtém o endereço público a partir da chave pública mestre
            BitcoinAddress address = masterPubKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);


            WriteLabel("\nMaster key : " + masterKey.ToString(Network.Main) + "\n");
            WriteLine($"Wallet Address =:  {address}", ConsoleColor.DarkGreen);
            WriteLine($"Secrect Words =: {mnemo}", ConsoleColor.DarkYellow);

            WriteLine($"\nSecrect Words INDEX =:", ConsoleColor.Magenta);
            for (int i = 0; i < 12; i++)
            {
                WriteLine($"[{i}] {mnemo.Words[i]}", ConsoleColor.Magenta);
            }
            Console.ReadLine();
            #endregion

            #region Recovery Address
            Console.Write("\n\nRecover Wallett Address");
            Console.Write("\nInput your Secrets Words: ");
            string mnemonicWords = Console.ReadLine();

            // Cria uma instância de Mnemonic a partir das palavras secretas
            Mnemonic mnemo3 = new Mnemonic(mnemonicWords, Wordlist.English);
            BitcoinAddress address1 = GetAddressFromMnemonic(mnemo3);

            // Imprime o endereço da carteira recuperado            
            WriteLine($"Recovery Wallett Address =:  {address1.ToString()}", ConsoleColor.Green);
            #endregion

            Console.ReadLine();
        }

        static BitcoinAddress GetAddressFromMnemonic(Mnemonic mnemonic)
        {
            ExtKey masterKey = mnemonic.DeriveExtKey();
            ExtPubKey masterPubKey = masterKey.Neuter();
            BitcoinAddress address = masterPubKey.PubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
            return address;
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