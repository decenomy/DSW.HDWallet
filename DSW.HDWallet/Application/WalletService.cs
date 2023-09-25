using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure;
using NBitcoin;

namespace DSW.HDWallet.Application
{
    public class WalletService : IWalletService
    {        
        private readonly IWalletRepository _walletRepository;
        private readonly IMnemonicRepository _mnemonicRepository;

        public WalletService(IWalletRepository walletRepository, IMnemonicRepository mnemonicRepository)
        {
            _walletRepository = walletRepository;
            _mnemonicRepository = mnemonicRepository;
        }

        public Wallet CreateWallet(WordCount wordCount)
        {
            Mnemonic mnemo = _mnemonicRepository.GenerateMnemonic(wordCount);               

            return _walletRepository.Create(mnemo);
        }

        public Wallet CreateWalletWithPassword(WordCount wordCount, string? password = null)
        {
            Mnemonic mnemo = _mnemonicRepository.GenerateMnemonic(wordCount);

            return _walletRepository.CreateWithPassword(mnemo, password);
        }

        public string RecoverWallet(string mnemonic, string? password = null)
        {
            Mnemonic mnemo = _mnemonicRepository.GetMnemonic(mnemonic);

            return _walletRepository.Recover(mnemo, password);
        }

        public DeriveKeyDetails CreateDerivedKey(CoinType coinType, string mnemonic, int index, string? password = null)
        {
            Mnemonic mnemo = _mnemonicRepository.GetMnemonic(mnemonic);
            return _walletRepository.CreateDeriveKey(coinType, mnemo, index, password);
        }

        public PubKeyDetails GeneratePubkey(CoinType coinType, string seedHex)
        {
            return _walletRepository.GeneratePubkey(coinType, seedHex);
        }

        public DeriveKeyDetailsApp GenerateDerivePubKey(string pubKey, CoinType coinType, int Index)
        {
            return _walletRepository.GenerateDerivePubKey(pubKey, coinType, Index);
        }
    }
}
