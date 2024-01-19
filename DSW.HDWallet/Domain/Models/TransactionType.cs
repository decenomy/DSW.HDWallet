namespace DSW.HDWallet.Domain.Models
{
    public enum TransactionType
    {
        Unknown,
        Incoming,
        Outgoing,
        Internal,
        Mining,
        Staking,
        MasternodeReward
    }
}
