using DSW.HDWallet.Domain.ApiObjects;

namespace DSW.HDWallet.Domain.Models
{
    public class TransactionRecord
    {
        public string? TxId { get; set; }
        public string? Ticker { get; set; }
        public string? Type { get; set; } // "Incoming" or "Outgoing"
        public decimal Amount { get; set; }
        public string? FromAddress { get; set; }
        public string? ToAddress { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsConfirmed { get; set; }
        public decimal? TransactionFee { get; set; } 
        public string? Notes { get; set; }

    }
}