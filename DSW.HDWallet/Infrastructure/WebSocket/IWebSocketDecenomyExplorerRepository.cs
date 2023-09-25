namespace DSW.HDWallet.Infrastructure.WebSocket
{
    public interface IWebSocketDecenomyExplorerRepository
    {
        Task GetWSTransactionAsync(string coin, string txId);
    }
}
