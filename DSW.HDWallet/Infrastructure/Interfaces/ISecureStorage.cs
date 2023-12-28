namespace DSW.HDWallet.Infrastructure.Interfaces
{
    public interface ISecureStorage
    {
        bool HasSeed();
        string GetMnemonic();
    }
}
