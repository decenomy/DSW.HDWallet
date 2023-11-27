using DSW.HDWallet.Application.Extension;
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
        private readonly IBlockbookHttpClient _apiDecenomyExplorer;
        private readonly IWebSocketDecenomyExplorerRepository _webSocket;

        public WalletService(IWalletRepository walletRepository,
            IBlockbookHttpClient apiDecenomyExplorer,
            IWebSocketDecenomyExplorerRepository webSocket)
        {
            _walletRepository = walletRepository;
            _apiDecenomyExplorer = apiDecenomyExplorer;
            _webSocket = webSocket;
        }

        public Wallet CreateWallet(WordCount wordCount, string? password = null)
        {
            Mnemonic mnemo = new Mnemonic(Wordlist.English, wordCount);

            return new Wallet
            {
                SeedHex = mnemo.DeriveSeed(password).ToHexString(),
                Mnemonic = mnemo.ToString(),
                MnemonicArray = mnemo.Words
            };
        }

        public string RecoverWallet(string mnemonic, string? password = null)
        {
            Mnemonic mnemo = new Mnemonic(mnemonic, Wordlist.English);

            return _walletRepository.GetSeedHex(mnemo, password);
        }

        public PubKeyDetails GeneratePubkey(string ticker, string seedHex)
        {
            return _walletRepository.GeneratePubkey(ticker, seedHex);
        }

        public AddressInfo GetAddress(string pubKey, string ticker, int Index, bool isChange = false)
        {
            return _walletRepository.GetAddress(pubKey, ticker, Index, isChange);
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
    }
}