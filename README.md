# HDWallet Decenomy

## Overview

This repository contains a C# application for managing cryptocurrency wallets using the HDWallet library. The application provides various functionalities for creating wallets, generating derived keys, performing transactions, and exploring blockchain data through APIs and WebSockets.

## HDWallet

The `HDWallet` class serves as the entry point for the application. It provides a command-line interface for interacting with the wallet functionalities. Below, you'll find a detailed explanation of each method and how to use them.

### `HDWalletServiceProvider.Initialize()`

- Description: Initializes the HDWallet service provider.
- Usage: Call this method before using any other wallet-related functionality.

### `HDWalletServiceProvider.GetWalletService()`

- Description: Retrieves the wallet service for performing wallet-related operations.
- Usage: Call this method to get an instance of the wallet service.

## Menu Options

# HDWallet Documentation

## Overview

The `HDWallet` is a .NET library that provides functionality for creating and managing hierarchical deterministic wallets (HD wallets) for various cryptocurrencies. This library allows you to generate mnemonic phrases, create wallets with or without passwords, derive child keys, create transactions, recover wallets, and interact with cryptocurrency APIs. This documentation provides a detailed explanation of the `HDWallet` library and its methods.

## Getting Started

### Initialization

First, import the necessary namespaces:

```csharp
using DSW.HDWallet.Application.Provider;
```

Then, initialize the `HDWalletServiceProvider`:

```csharp
HDWalletServiceProvider.Initialize();
```

Now, you are ready to use the `HDWallet` library.

## HDWallet Methods

### Create Wallet

The `CreateWallet` method allows you to generate a new wallet without a password. It returns a wallet object containing the seed and mnemonic.

```csharp
WordCount wordCount = WordCount.Twelve;
var createdWallet = walletAppService.CreateWallet(wordCount);

// Access wallet properties
string seedHex = createdWallet.SeedHex;
string mnemonic = createdWallet.Mnemonic;
```

### Create Wallet With Password

The `CreateWalletWithPassword` method creates a new wallet with a password. It returns a wallet object containing the seed and mnemonic.

```csharp
WordCount wordCount = WordCount.Twelve;
string password = "your_password";
var walletWithPassword = walletAppService.CreateWalletWithPassword(wordCount, password);

// Access wallet properties
string seedHex = walletWithPassword.SeedHex;
string mnemonic = walletWithPassword.Mnemonic;
```

### Create Derived Key

You can derive child keys from a mnemonic and password using the `CreateDerivedKey` method. Provide the mnemonic, password, coin type, and the index of the derived key.

```csharp
string mnemonic = "your_mnemonic_phrase";
string password = "your_password";
CoinType coinType = CoinType.Bitcoin;
int index = 0;

var derivedKey = walletAppService.CreateDerivedKey(coinType, mnemonic, index, password);

// Access derived key properties
string address = derivedKey.Address;
string keyPath = derivedKey.Path;
PubKey pubKey = derivedKey.PubKey;
```

### New Wallet - App Process

This method combines wallet creation with deriving child keys. It creates a wallet with a password, generates a public key, and then derives child keys from it.

```csharp
WordCount wordCount = WordCount.Twelve;
string password = "your_password";
var newWalletWithPassword = walletAppService.CreateWalletWithPassword(wordCount, password);

// Generate a public key
CoinType selectedCoin = CoinType.Bitcoin;
var generatePubKey = walletAppService.GeneratePubkey(selectedCoin, newWalletWithPassword.SeedHex, true);

// Access generated public key properties
string publicKey = generatePubKey.PubKey;
string coinType = generatePubKey.CoinType;
string path = generatePubKey.Path;

// Derive child keys
int numberOfKeysToDerive = 5;
for (int i = 0; i < numberOfKeysToDerive; i++)
{
    var deriveKey = walletAppService.GenerateDerivePubKey(generatePubKey.PubKey, selectedCoin, i, true);

    // Access derived key properties
    string derivedAddress = deriveKey.Address;
    string keyPath = generatePubKey.Path + "/" + deriveKey.Path;
}
```

### Create a Transaction

You can use the `TransactionAsync` method to create a cryptocurrency transaction. Provide the coin name, recipient's address, and transaction value.

```csharp
string coinName = "Bitcoin";
string recipientAddress = "recipient_address";
ulong transactionValue = 100000; // Value in the smallest denomination of the cryptocurrency

var transactionResponse = walletAppService.TransactionAsync(coinName, recipientAddress, transactionValue).Result;

// Access transaction details
foreach (var transaction in transactionResponse)
{
    string voutIndex = transaction.Vout;
    string txId = transaction.Txid;
    string value = transaction.Value.ToFormattedString(); // Formatted value of the transaction
}
```

### Recover Wallet

The `RecoverWallet` method allows you to recover a wallet using a mnemonic phrase and password.

```csharp
string mnemonicWords = "your_mnemonic_phrase";
string passwordRecover = "your_password";

string recoveredSeedHex = walletAppService.RecoverWallet(mnemonicWords, passwordRecover);

// Access the recovered seed hex
```

### Random Secret Words

Generate random secret words using the `CreateWallet` method with a `WordCount` of your choice.

```csharp
WordCount wordCount = WordCount.TwentyFour;
var secretWordsWallet = walletAppService.CreateWallet(wordCount);

// Get random mnemonic words
string[] randomWords = secretWordsWallet.GetRandomMnemonic(3);
```

## API Explorer Methods

These methods interact with cryptocurrency APIs for various purposes.

### API Explorer Get Address (Method 20)

Retrieve information about a cryptocurrency address using the `GetAddressAsync` method.

```csharp
string coinName = "Bitcoin";
string addressCoin = "address_to_query";

var resultAPI = walletAppService.GetAddressAsync(coinName, addressCoin).Result;

// Access address information
string address = resultAPI.Address;
string balance = resultAPI.Balance;
string totalReceived = resultAPI.TotalReceived;
string totalSent = resultAPI.TotalSent;
string txs = resultAPI.Txs;

// Access a list of transaction IDs
List<string> txIds = resultAPI.Txids;
```

### API Explorer Get Transaction (Method 21)

Retrieve information about a cryptocurrency transaction using the `GetTransactionAsync` method.

```csharp
string coinName = "Bitcoin";
string txId = "transaction_id_to_query";

var resultTransaction = walletAppService.GetTransactionAsync(coinName, txId).Result;

// Access transaction details
string blockHash = resultTransaction.BlockHash;
string blockHeight = resultTransaction.BlockHeight;
string blockTime = resultTransaction.BlockTime;
string confirmations = resultTransaction.Confirmations;
string size = resultTransaction.Size;
string txid = resultTransaction.Txid;
string value = resultTransaction.Value;
string valueIn = resultTransaction.ValueIn;
string version = resultTransaction.Version;
string hex = resultTransaction.Hex;
```

### API Explorer Get Transaction Specific (Method 22)

Retrieve specific details about a cryptocurrency transaction using the `GetTransactionSpecificAsync` method.

```csharp
string coinName = "Bitcoin";
string txId = "transaction_id_to_query";

var resultTransactionSpecific = walletAppService.GetTransactionSpecificAsync(coinName, txId).Result;

// Access specific transaction details
string blockHash = resultTransactionSpecific.Blockhash;
string blockTime = resultTransactionSpecific.Blocktime;
string confirmations = resultTransactionSpecific.Confirmations;
string expiryHeight = resultTransactionSpecific.Expiryheight;
string time = resultTransactionSpecific.Time;
string txid = resultTransactionSpecific.Txid;
string valueBalance = resultTransaction

Specific.ValueBalance;
string version = resultTransactionSpecific.Version;
string hex = resultTransactionSpecific.Hex;
```

### API Explorer Get Block Hash (Method 23)

Retrieve the block hash of a specific block using the `GetBlockHash` method.

```csharp
string coinName = "Bitcoin";
string blockHeight = "block_height_to_query";

var resultBlockHash = walletAppService.GetBlockHash(coinName, blockHeight).Result;

// Access the block hash
string blockHash = resultBlockHash.BlockHash;
```

### API Explorer Get XPub (Method 24)

Retrieve extended public key (xPub) information for a specific cryptocurrency using the `GetXpub` method.

```csharp
string coinName = "Bitcoin";
string xPub = "your_xpub_to_query";

var xPubResult = walletAppService.GetXpub(coinName, xPub).Result;

// Access xPub information (not yet implemented in the code sample)
```

### API Explorer Get UTXO (Method 25)

Retrieve unspent transaction outputs (UTXOs) for a specific address using the `GetUtxo` method.

```csharp
string coinName = "Bitcoin";
string utxoAddress = "address_to_query";
bool utxoConfirmed = false; // Set to true for confirmed UTXOs, false for unconfirmed

var utxoResult = walletAppService.GetUtxo(coinName, utxoAddress, utxoConfirmed).Result;

// Access UTXO details
foreach (var item in utxoResult)
{
    string height = item.Height;
    string txid = item.Txid;
    string value = item.Value;
    string vout = item.Vout;
    string confirmations = item.Confirmations;
}
```

### WebSocket GET Transaction (Method 30)

Retrieve transaction details via WebSocket for a specific transaction using the `GetWSTransactionAsync` method.

```csharp
string coinName = "Bitcoin";
string txId = "transaction_id_to_query";

var wss = walletAppService.GetWSTransactionAsync(coinName, txId).Result;

// Access WebSocket transaction details
string blockHash = wss.data.blockHash;
string blockTime = wss.data.blockTime;
string blockHeight = wss.data.blockHeight;
string txid = wss.data.txid;
string valueIn = wss.data.valueIn;
string value = wss.data.value;
string size = wss.data.size;
string confirmations = wss.data.confirmations;
string hex = wss.data.hex;
```

### WebSocket Subscribe New Transaction (Method 31)

Subscribe to new transactions via WebSocket using the `SubscribeNewTransaction` method.

```csharp
string coinName = "Bitcoin";

var subscription = walletAppService.SubscribeNewTransaction(coinName).Result;

// This method does not return transaction details in this code sample
```


## Example Usage

To use the HDWallet application, follow these steps:

1. Initialize the HDWallet service provider by calling `HDWalletServiceProvider.Initialize()`.

2. Retrieve the wallet service by calling `HDWalletServiceProvider.GetWalletService()`.

3. Select a menu option by entering the corresponding option number.

4. Follow the prompts to perform the desired wallet operation.

5. To exit the application, select option 0.

Feel free to explore the various wallet functionalities provided by the HDWallet application and interact with cryptocurrency wallets and blockchain data.

## Conclusion

This documentation provides an overview of the `HDWallet` library and its methods for creating and managing hierarchical deterministic wallets and interacting with cryptocurrency APIs. You can use these methods to perform various cryptocurrency-related tasks in your .NET applications.
