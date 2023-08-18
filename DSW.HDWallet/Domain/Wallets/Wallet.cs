namespace DSW.HDWallet.Domain.Wallets
{
    public class Wallet
    {
        public string? MasterKey { get; set; }
        public string? Address { get; set; }
        public string? SecretWords { get; set; }
        public string[]? SecrectWordsArray { get; set; }
        public string[] GetRandomSecretWords(int count = 1)
        {
            if (SecrectWordsArray != null && SecrectWordsArray.Length >= count)
            {
                string[] shuffledWords = (string[])SecrectWordsArray.Clone();

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
