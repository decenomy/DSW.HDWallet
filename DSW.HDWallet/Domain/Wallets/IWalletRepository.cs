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
        DeriveKeyDetails CreateDeriveKey(string ticker, Mnemonic mnemo, int index, string? password = null, bool isNetworkTest = false);
        DeriveKeyDetailsApp GenerateDerivePubKey(string pubKey, string ticker, int Index, bool isNetworkTest = false);
        PubKeyDetails GeneratePubkey(string ticker, string seedHex, string? password = null, bool isNetworkTest = false);
        TransactionDetails GenerateTransaction(string ticker, List<UtxoObject> utxos, long amountToSend, string seedHex, string toAddress, long fee = 0);
        bool ValidateAddress(string ticker, string address);
    }
}
