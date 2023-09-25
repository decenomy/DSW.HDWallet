using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Infrastructure;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure.WebSocket;
using NBitcoin;

namespace DSW.HDWallet.Application
{
    public class WalletService : IWalletService
    {        
        private readonly IWalletRepository _walletRepository;
        private readonly IMnemonicRepository _mnemonicRepository;
        private readonly IApiDecenomyExplorerRepository _apiDecenomyExplorer;
        private readonly IWebSocketDecenomyExplorerRepository _webSocket;

        public WalletService(IWalletRepository walletRepository, 
            IMnemonicRepository mnemonicRepository, 
            IApiDecenomyExplorerRepository apiDecenomyExplorer,
            IWebSocketDecenomyExplorerRepository webSocket)
        {
            _walletRepository = walletRepository;
            _mnemonicRepository = mnemonicRepository;
            _apiDecenomyExplorer = apiDecenomyExplorer;
            _webSocket = webSocket;
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

        public async Task<AddressObject> GetAddressAsync(string coin, string address)
        {           
            return await _apiDecenomyExplorer.GetAddressAsync(coin, address);
        }

        public async Task<TransactionObject> GetTransactionAsync(string coin, string txid)
        {
            return await _apiDecenomyExplorer.GetTransactionAsync(coin, txid);
        }

        public async Task<TransactionSpecificObject> GetTransactionSpecificAsync(string coin, string txid)
        {          
            return await _apiDecenomyExplorer.GetTransactionSpecificAsync(coin, txid);
        }

        public Task GetWSTransactionAsync(string coin, string txId)
        {
            _webSocket.GetWSTransactionAsync(coin, txId).Wait();
            
            return Task.CompletedTask;
        }
    }
}
