using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Transaction;
using NBitcoin;

namespace DSW.HDWallet.Domain.Wallets
{
    public interface IWalletRepository
    {
        Wallet Create(Mnemonic mnemonic);
        Wallet CreateWithPassword(Mnemonic mnemonic, string? password = null);
        string Recover(Mnemonic mnemo, string? password = null);
        DeriveKeyDetails CreateDeriveKey(ICoinExtension coinType, Mnemonic mnemo, int index, string? password = null, bool isNetworkTest = false);
        DeriveKeyDetailsApp GenerateDerivePubKey(string pubKey, CoinType coinType, int Index, bool isNetworkTest = false);
        PubKeyDetails GeneratePubkey(ICoinExtension coinType, string seedHex, string? password = null, bool isNetworkTest = false);
        TransactionDetails GenerateTransaction(ICoinExtension coinType, List<UtxoObject> utxos, long amountToSend, string seedHex, string toAddress, long fee = 0);
        bool ValidateAddress(string ticker, string address);
        string GetCoinName(string ticker);
        List<CoinExtensionInfo> GetAllCoin();
    }
}
