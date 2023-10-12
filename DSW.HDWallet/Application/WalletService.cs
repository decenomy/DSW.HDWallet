using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Wallets;
using DSW.HDWallet.Domain.WSObject;
using DSW.HDWallet.Infrastructure;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Infrastructure.WS;
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

        public DeriveKeyDetails CreateDerivedKey(CoinType coinType, string mnemonic, int index, string? password = null, bool isNetworkTest = false)
        {
            Mnemonic mnemo = _mnemonicRepository.GetMnemonic(mnemonic);
            return _walletRepository.CreateDeriveKey(coinType, mnemo, index, password);
        }

        public PubKeyDetails GeneratePubkey(CoinType coinType, string seedHex, bool isNetworkTest = false)
        {
            return _walletRepository.GeneratePubkey(coinType, seedHex, null, isNetworkTest);
        }

        public DeriveKeyDetailsApp GenerateDerivePubKey(string pubKey, CoinType coinType, int Index, bool isNetworkTest = false)
        {
            return _walletRepository.GenerateDerivePubKey(pubKey, coinType, Index, isNetworkTest);
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

        public async Task<BlockHashObject> GetBlockHash(string coin, string blockHeight)
        {
            return await _apiDecenomyExplorer.GetBlockHash(coin, blockHeight);
        }

        public async Task<XpubObject> GetXpub(string coin, string xpub)
        {
            return await _apiDecenomyExplorer.GetXpub(coin, xpub);
        }

        public async Task<UtxoObject[]> GetUtxo(string coin, string address, bool confirmed = false)
        {
            return await _apiDecenomyExplorer.GetUtxo(coin, address, confirmed);
        }

        public async Task<WSTransactionObject> GetWSTransactionAsync(string coin, string txId)
        {
            return await _webSocket.GetWSTransactionAsync(coin, txId);
        }

        public async Task<WSSubscribeObject> SubscribeNewTransaction(string coin)
        {
            return await _webSocket.SubscribeNewTransaction(coin);
        }

        public async Task<List<UtxoObject>> TransactionAsync(string coin, string address, ulong value)
        {
            var _getUtxo = await GetUtxo(coin, address);

            return _walletRepository.SendTransaction(value, _getUtxo.ToList());
        }
    }
}