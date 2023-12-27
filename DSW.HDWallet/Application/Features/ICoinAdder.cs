namespace DSW.HDWallet.Application.Features
{
    public interface ICoinAdder
    {
        Task AddCoin(string coinTicker, string? password = null);
    }
}