using DSW.HDWallet.Domain.Wallets;

namespace DSW.HDWallet.Infrastructure
{
    public class WalletRepository : IWalletRepository
    {
        public Wallet Create(Wallet wallet)
        {
            // Implementação para salvar a carteira no banco de dados ou outra ação necessária
            return wallet;
        }
    }
}
