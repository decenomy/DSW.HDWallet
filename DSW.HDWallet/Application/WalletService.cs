using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Transaction;
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
        private readonly IBlockbookHttpClient _apiDecenomyExplorer;
        private readonly IWebSocketDecenomyExplorerRepository _webSocket;

        public WalletService(IWalletRepository walletRepository,
            IMnemonicRepository mnemonicRepository,
            IBlockbookHttpClient apiDecenomyExplorer,
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

        public DeriveKeyDetails CreateDerivedKey(string ticker, string mnemonic, int index, string? password = null, bool isNetworkTest = false)
        {
            Mnemonic mnemo = _mnemonicRepository.GetMnemonic(mnemonic);
            return _walletRepository.CreateDeriveKey(ticker, mnemo, index, password);
        }

        public PubKeyDetails GeneratePubkey(string ticker, string seedHex, bool isNetworkTest = false)
        {
            return _walletRepository.GeneratePubkey(ticker, seedHex, null, isNetworkTest);
        }

        public DeriveKeyDetailsApp GenerateDerivePubKey(string pubKey, string ticker, int Index, bool isNetworkTest = false)
        {
            return _walletRepository.GenerateDerivePubKey(pubKey, ticker, Index, isNetworkTest);
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

        public async Task<XpubObject> GetXpub(string coin, string xpub, int page = 1, int pageSize = 1000)
        {
            return await _apiDecenomyExplorer.GetXpub(coin, xpub, page, pageSize);
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

        public async Task<TransactionDetails> GenerateTransactionAsync(string ticker, long amountToSend, string seedHex, string fromAddress, string toAddress)
        {
            var utxos = await GetUtxo(ticker, fromAddress);

            return _walletRepository.GenerateTransaction(ticker, utxos.ToList(), amountToSend, seedHex, toAddress);
        }

        public bool ValidateAddress(string ticker, string address)
        {
            return _walletRepository.ValidateAddress(ticker, address);
        }

        public string GetCoinName(string ticker)
        {
            return _walletRepository.GetCoinName(ticker);
        }

        public List<ICoinExtension> GetAllCoin()
        {
            return _walletRepository.GetAllCoin();
        }
    }
}