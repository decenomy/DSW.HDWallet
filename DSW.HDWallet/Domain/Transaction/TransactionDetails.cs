using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Wallets;
using NBitcoin;

namespace DSW.HDWallet.Domain.Transaction
{
    public class TransactionDetails
    {
        public string? ToAddress { get; set; }
        public AddressInfo? ChangeAddress { get; set; }
        public Money? Balance { get; set; }
        public Money? Amount { get; set; }
        public Money? Change { get; set; }
        public Money? Fee { get; set; }
        public decimal SizeKb { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
        public List<UtxoObject>? Utxos { get; set; }
        public NBitcoin.Transaction? Transaction { get; set; }
    }
}
