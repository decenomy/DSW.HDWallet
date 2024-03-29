﻿namespace DSW.HDWallet.Domain.Models
{
    public class Wallet
    {
        public string? Ticker { get; set; }
        public string? PublicKey { get; set; }
        public string? Path { get; set; }
        public int CoinIndex { get; set; }
        public long? Balance { get; set; }
        public long? UnconfirmedBalance { get; set; }
    }
}
