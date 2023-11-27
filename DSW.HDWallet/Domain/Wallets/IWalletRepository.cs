using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Domain.Transaction;
using NBitcoin;

namespace DSW.HDWallet.Domain.Wallets
{
    public interface IWalletRepository
    {
        string GetSeedHex(Mnemonic mnemo, string? password = null);
        AddressInfo GetAddress(string pubKey, string ticker, int Index, bool IsChange = false);
        PubKeyDetails GeneratePubkey(string ticker, string seedHex);
        TransactionDetails GenerateTransaction(string ticker, List<UtxoObject> utxos, long amountToSend, string seedHex, string toAddress, long fee = 0);
        bool ValidateAddress(string ticker, string address);
    }
}
