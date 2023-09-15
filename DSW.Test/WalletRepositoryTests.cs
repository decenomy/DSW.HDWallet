using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Infrastructure;
using NBitcoin;

namespace DSW.Test
{
    public class WalletRepositoryTests
    {
        [Fact]
        public void Create_WalletWithoutPassword_ReturnsWallet()
        {
            // Arrange
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var repository = new WalletRepository();

            // Act
            var wallet = repository.Create(mnemonic);

            // Assert
            Assert.NotNull(wallet);
            Assert.Equal(mnemonic.ToString(), wallet.Mnemonic);
        }

        [Fact]
        public void CreateWithPassword_WalletWithPassword_ReturnsWallet()
        {
            // Arrange
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var password = "test_password_123456";
            var repository = new WalletRepository();

            // Act
            var wallet = repository.CreateWithPassword(mnemonic, password);

            // Assert
            Assert.NotNull(wallet);
            Assert.Equal(mnemonic.ToString(), wallet.Mnemonic);
        }

        [Fact]
        public void Recover_WalletWithoutPassword_ReturnsSeedHex()
        {
            // Arrange
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var repository = new WalletRepository();

            // Act
            var seedHex = repository.Recover(mnemonic);

            // Assert
            Assert.NotNull(seedHex);
        }

        [Fact]
        public void Recover_WalletWithPassword_ReturnsSeedHex()
        {
            // Arrange
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var password = "test_password_123456";
            var repository = new WalletRepository();

            // Act
            var seedHex = repository.Recover(mnemonic, password);

            // Assert
            Assert.NotNull(seedHex);
        }

        [Fact]
        public void GeneratePubkey_ValidParameters_ReturnsPubKeyDetails()
        {
            // Arrange
            var coinType = CoinType.SAPP;
            var seedHex = "e921e0e1ce42a426cd403e98502c723e4f731f33e7e182db36428cd96952f3e75d6f8cc91856662f9f5425313aa321edee629d613abb18f7cf86f22726e9d95c";
            var repository = new WalletRepository();

            // Act
            var pubKeyDetails = repository.GeneratePubkey(coinType, seedHex);

            // Assert
            Assert.NotNull(pubKeyDetails);
            Assert.Equal(coinType, pubKeyDetails.CoinType);
        }

        [Fact]
        public void GenerateDerivePubKey_ValidParameters_ReturnsDerivedPublicKey()
        {
            // Arrange
            var coinType = CoinType.SAPP;
            var pubKey = "ToEGySqfw8Gkddh6h4TzjvfQyZLLFnbEVgQ7adzz3wKtzucZr734aL1f5E7rcarfLubc8vbLb4ZfQncrDXpAeWGuzuqzyPaQP4TKVcT1FbXaVnK";
            var index = 0;
            var repository = new WalletRepository();

            // Act
            var derivedPubKey = repository.GenerateDerivePubKey(pubKey, coinType, index);

            // Assert
            Assert.NotNull(derivedPubKey);
        }

        [Fact]
        public void CreateDeriveKey_ValidParameters_ReturnsDeriveKeyDetails()
        {
            // Arrange
            var coinType = CoinType.SAPP;
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var index = 0;
            var repository = new WalletRepository();

            // Act
            var deriveKeyDetails = repository.CreateDeriveKey(coinType, mnemonic, index);

            // Assert
            Assert.NotNull(deriveKeyDetails);
        }

    }
}
