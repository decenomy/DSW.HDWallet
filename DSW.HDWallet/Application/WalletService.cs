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

        public Wallet CreateWallet(CoinType coinType)
        {
            Mnemonic mnemo = _mnemonicRepository.GenerateMnemonic();               

            return _walletRepository.Create(mnemo, coinType);
        }

        public Wallet CreateWalletWithPassword(CoinType coinType, string? password = null)
        {
            Mnemonic mnemo = _mnemonicRepository.GenerateMnemonic();

            return _walletRepository.CreateWithPassword(coinType, mnemo, password);
        }

        public string RecoverWallet(CoinType coinType, string secretWords, string? password = null)
        {
            Mnemonic mnemo = _mnemonicRepository.GetMnemonic(secretWords);

            return _walletRepository.Recover(coinType, mnemo, password).ToString();
        }

        public BitcoinExtKey CreateDerivedKey(CoinType coinType, int index)
        {
            // TO DO Mnemonic precisa ser recuperado para criar as derivadas e para recuperar o Mneminic preciso das secrets words
            Mnemonic mnemo = _mnemonicRepository.GenerateMnemonic();
            ExtKey masterKey = mnemo.DeriveExtKey();

            string coin_type = Bip44.GetCoinCodeBySymbol(coinType.ToString());
            KeyPath path = new KeyPath($"m/44'/{coin_type}'/0'/0/{index}");

            return _walletRepository.CreateDeriveKey(coinType, masterKey, path);
        }
    }
}
