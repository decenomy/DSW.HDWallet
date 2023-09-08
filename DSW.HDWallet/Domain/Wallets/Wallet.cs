namespace DSW.HDWallet.Domain.Wallets
{
    public class Wallet
    {
        public string? SeedHex { get; set; }
        public string? Mnemonic { get; set; }
        public string[]? MnemonicArray { get; set; }
        public string[] GetRandomMnemonic(int count = 1)
        {
            if (MnemonicArray != null && MnemonicArray.Length >= count)
            {
                string[] shuffledWords = (string[])MnemonicArray.Clone();

                Random random = new Random();
                for (int i = shuffledWords.Length - 1; i > 0; i--)
                {
                    int j = random.Next(0, i + 1);
                    string temp = shuffledWords[i];
                    shuffledWords[i] = shuffledWords[j];
                    shuffledWords[j] = temp;
                }

                return new ArraySegment<string>(shuffledWords, 0, count).ToArray();
            }
            return Array.Empty<string>();
        }

    }
}
