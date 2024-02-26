namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface ISecureStorage
    {
        Task<bool> HasSeed();
        Task<string?> GetMnemonic();
    }
}
