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

        public Wallet CreateWallet()
        {
            Mnemonic mnemo = _mnemonicRepository.GenerateMnemonic();

            return _walletRepository.Create(mnemo);
        }

        public Wallet CreateWalletWithPassword(string? password = null)
        {
            Mnemonic mnemo = _mnemonicRepository.GenerateMnemonic();

            return _walletRepository.CreateWithPassword(mnemo, password);
        }

        public string RecoverWallet(string secretWords, string? password = null)
        {
            Mnemonic mnemo = _mnemonicRepository.GetMnemonic(secretWords);

            return _walletRepository.Recover(mnemo, password).ToString();
        }

        public BitcoinExtKey CreateDerivedKey(KeyPath keyPath)
        {
            // KeyPath precisar ser criado de acordo com as configurações da rede
            /*
                string accounting = "1'";
                int customerId = 5;
                int paymentId = 50;
                KeyPath path = new KeyPath(accounting + "/" + customerId + "/" + paymentId);
                //Path : "1'/5/50"
            */

            // TO DO Mnemonic precisa ser recuperado para criar as derivadas e para recuperar o Mneminic preciso das secrets words
            Mnemonic mnemo = _mnemonicRepository.GenerateMnemonic();
            ExtKey masterKey = mnemo.DeriveExtKey();

            return _walletRepository.CreateDeriveKey(masterKey, keyPath);
        }
    }
}
