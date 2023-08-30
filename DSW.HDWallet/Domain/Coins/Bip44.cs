namespace DSW.HDWallet.Domain.Coins
{
    public class Bip44
    {
        private struct CoinInfo
        {
            public int Code;
            public string HexCode;
            public string Symbol;
            public string Name;

            public CoinInfo(int code, string hexCode, string symbol, string name)
            {
                Code = code;
                HexCode = hexCode;
                Symbol = symbol;
                Name = name;
            }
        }

        private static List<CoinInfo> coinList = new List<CoinInfo>
        {
            new CoinInfo(835, "0x80000343", "AZR", "Azzure"),
            new CoinInfo(841, "0x80000349", "BECN", "Beacon"),
            new CoinInfo(848, "0x80000350", "BIR", "Birake"),
            new CoinInfo(836, "0x80000344", "CFL", "CryptoFlow"),
            new CoinInfo(843, "0x8000034b", "SAGA", "CryptoSaga"),
            new CoinInfo(837, "0x80000345", "DASHD", "Dash Diamond"),
            new CoinInfo(845, "0x8000034d", "ESK", "EskaCoin"),
            new CoinInfo(850, "0x80000352", "FLS", "Flits"),
            new CoinInfo(833, "0x80000341", "777", "Jackpot"),
            new CoinInfo(834, "0x80000342", "KYAN", "Kyanite"),
            new CoinInfo(849, "0x80000351", "MOBIC", "MobilityCoin"),
            new CoinInfo(842, "0x8000034a", "MONK", "Monk"),
            new CoinInfo(846, "0x8000034e", "OWO", "OneWorld Coin"),
            new CoinInfo(840, "0x80000348", "PNY", "Peony"),
            new CoinInfo(832, "0x80000340", "SAPP", "Sapphire"),
            new CoinInfo(844, "0x8000034c", "SUV", "Suvereno"),
            new CoinInfo(839, "0x80000347", "UCR", "Ultra Clear")
        };

        public static string GetCoinCodeBySymbol(string symbol)
        {
            CoinInfo coin = coinList.Find(c => c.Symbol == symbol);
            if (coin.Code != 0)
                return coin.Code.ToString();
            else
                return "Coin not found.";
        }
    }
}