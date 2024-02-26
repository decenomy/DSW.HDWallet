using NBitcoin;
using System;

public class WalletUtils
{
    public static BitcoinSecret DerivePrivateKey(string masterSeedHex, string derivationPath, Network network)
    {
        ExtKey masterKey = new ExtKey(masterSeedHex);

        KeyPath keyPath = new KeyPath(derivationPath);

        ExtKey key = masterKey.Derive(keyPath);

        return key.PrivateKey.GetBitcoinSecret(network);
    }
}

