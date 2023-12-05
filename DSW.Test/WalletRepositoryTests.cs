using DSW.HDWallet.Domain.ApiObjects;
using DSW.HDWallet.Domain.Coins;
using DSW.HDWallet.Infrastructure;
using NBitcoin;
using Moq;
using DSW.HDWallet.Application;
using DSW.HDWallet.Infrastructure.Api;
using DSW.HDWallet.Domain.Transaction;

namespace DSW.Test
{
    public class WalletRepositoryTests
    {
        [Fact]
        public void Create_WalletWithoutPassword_ReturnsWallet()
        {
            var mockCoinRepository = new Mock<CoinRepository>();
            var mockBlockbookHttpClient = new Mock<IBlockbookHttpClient>();

            // Arrange
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var service = new WalletService(mockBlockbookHttpClient.Object, mockCoinRepository.Object);

            // Act
            var wallet = service.CreateWallet(WordCount.Twelve);

            // Assert
            Assert.NotNull(wallet);
        }


        [Fact]
        public void CreateWithPassword_WalletWithPassword_ReturnsWallet()
        {
            var mockCoinRepository = new Mock<CoinRepository>();
            var mockBlockbookHttpClient = new Mock<IBlockbookHttpClient>();

            // Arrange
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var password = "test_password_123456";
            var service = new WalletService(mockBlockbookHttpClient.Object, mockCoinRepository.Object);

            // Act
            var wallet = service.CreateWallet(WordCount.Twelve, password);

            // Assert
            Assert.NotNull(wallet);
        }

        [Fact]
        public void Recover_WalletWithoutPassword_ReturnsSeedHex()
        {
            var mockCoinRepository = new Mock<CoinRepository>();
            var mockBlockbookHttpClient = new Mock<IBlockbookHttpClient>();
            // Arrange
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var service = new WalletService(mockBlockbookHttpClient.Object, mockCoinRepository.Object);

            // Act
            var seedHex = service.GetSeedHex(mnemonic);

            // Assert
            Assert.NotNull(seedHex);
        }

        [Fact]
        public void Recover_WalletWithPassword_ReturnsSeedHex()
        {
            var mockCoinRepository = new Mock<CoinRepository>();
            var mockBlockbookHttpClient = new Mock<IBlockbookHttpClient>();

            // Arrange
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var password = "test_password_123456";
            var service = new WalletService(mockBlockbookHttpClient.Object, mockCoinRepository.Object);

            // Act
            var seedHex = service.GetSeedHex(mnemonic, password);

            // Assert
            Assert.NotNull(seedHex);
        }

        [Fact]
        public void GeneratePubkey_ValidParameters_ReturnsPubKeyDetails()
        {
            var mockCoinRepository = new Mock<CoinRepository>();
            var mockBlockbookHttpClient = new Mock<IBlockbookHttpClient>();

            // Arrange
            var coinType = "SAPP";
            var seedHex = "e921e0e1ce42a426cd403e98502c723e4f731f33e7e182db36428cd96952f3e75d6f8cc91856662f9f5425313aa321edee629d613abb18f7cf86f22726e9d95c";
            var service = new WalletService(mockBlockbookHttpClient.Object, mockCoinRepository.Object);

            // Act
            var pubKeyDetails = service.GeneratePubkey(coinType, seedHex);

            // Assert
            Assert.NotNull(pubKeyDetails);
            Assert.Equal(coinType, pubKeyDetails.Ticker);
        }

        [Fact]
        public void GenerateDerivePubKey_ValidParameters_ReturnsDerivedPublicKey()
        {
            var mockCoinRepository = new Mock<CoinRepository>();
            var mockBlockbookHttpClient = new Mock<IBlockbookHttpClient>();

            // Arrange
            var coinType = "SAPP";
            var pubKey = "ToEGySqfw8Gkddh6h4TzjvfQyZLLFnbEVgQ7adzz3wKtzucZr734aL1f5E7rcarfLubc8vbLb4ZfQncrDXpAeWGuzuqzyPaQP4TKVcT1FbXaVnK";
            var index = 0;
            var service = new WalletService(mockBlockbookHttpClient.Object, mockCoinRepository.Object);

            // Act
            var derivedPubKey = service.GetAddress(pubKey, coinType, index);

            // Assert
            Assert.NotNull(derivedPubKey);
        }

        //[Fact]
        //public void CreateDeriveKey_ValidParameters_ReturnsDeriveKeyDetails()
        //{
        //    var mockCoinRepository = new Mock<CoinRepository>();
        //    // Arrange
        //    var coinType = "SAPP";
        //    var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
        //    var index = 0;
        //    var repository = new WalletRepository(mockCoinRepository.Object);

        //    // Act
        //    var deriveKeyDetails = repository.CreateDeriveKey(coinType, mnemonic, index);

        //    // Assert
        //    Assert.NotNull(deriveKeyDetails);
        //}

        [Fact]
        public async Task GenerateTransaction_WithValidUtxos_ShouldCreateTransactionDetails()
        {
            var mockCoinRepository = new Mock<CoinRepository>();
            var mockBlockbookHttpClient = new Mock<IBlockbookHttpClient>();

            // Arrange
            var service = new WalletService(mockBlockbookHttpClient.Object, mockCoinRepository.Object);
            string ticker = "TKYAN";
            long amountToSend = 200;
            string seedHex = "18c9ea841bb7c8fd9ec5c0a721925abc6262b10df638a99dac2f9153a47a196dece486e692583dc22f44662824a8cb0330f719c0ac75e9bd237842c83210f982";
            string toAddress = "kTEGTDVr6TnAyT7mxzhBZcwbC97DNui9hh";

            var mockUtxos = GetMockUtxos();

            // Setup BlockbookHttpClient mock to return mock UTXO data
            mockBlockbookHttpClient.Setup(m => m.GetUtxo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                   .ReturnsAsync(mockUtxos.ToArray()); // Convert the list to an array

            // Act
            var transactionDetails = await service.GenerateTransactionAsync(ticker, seedHex, amountToSend, toAddress);

            // Assert
            Assert.NotNull(transactionDetails);
        }


        [Fact]
        public void GenerateTransaction_WithInsufficientFunds_ShouldReturnNull()
        {
            var mockCoinRepository = new Mock<CoinRepository>();
            var mockBlockbookHttpClient = new Mock<IBlockbookHttpClient>();

            // Arrange
            var service = new WalletService(mockBlockbookHttpClient.Object, mockCoinRepository.Object);
            string ticker= "TKYAN";
            List<UtxoObject> utxos = GetMockUtxosLowValues(); 
            long amountToSend = 995200000000; 
            string seedHex = "03da1ed344a3094a4869339844849b98499fc8d56309d6951fabefec35d7f5f3302a8870cb8e64e8e6015295300690feea202ec93af818dc92546ba36143a7fd";
            string toAddress = "Kjs13q3bxt9Hcpsy9EKJ9fvPBBgnoKLiB9"; 
            long fee = 900; 

            // Act
            var transactionDetails = service.GenerateTransactionAsync(ticker, seedHex, amountToSend, toAddress);

            // Assert
            Assert.NotNull(transactionDetails);
            Assert.Equal("The UTXOs or balance are insufficient for this transaction.", transactionDetails?.Result.Message);
        }

        [Fact]
        public async void GenerateTransaction_WithInvalidSeed_ShouldReturnErrorMessage()
        {
            var mockCoinRepository = new Mock<CoinRepository>();
            var mockBlockbookHttpClient = new Mock<IBlockbookHttpClient>();

            // Arrange
            var service = new WalletService(mockBlockbookHttpClient.Object, mockCoinRepository.Object);
            string ticker = "TKYAN";
            List<UtxoObject> utxos = GetMockUtxos();
            long amountToSend = 900000000;
            string seedHex = "7aL9D9skvSD1ykfKBoWcDSqSPm76abuFGbQ5tcZwfd6CG1EzyPHdnyTxwr1LMnFB5KSB8qV8fwaykFib1YmKXZACDYE4tGBrPu7HgBmmgfCC3Cc";
            string toAddress = "Kjs13q3bxt9Hcpsy9EKJ9fvPBBgnoKLiB9";

            try
            {
                var transactionDetails = await service.GenerateTransactionAsync(ticker, seedHex, amountToSend, toAddress);
            }
            catch(Exception ex) {

                Assert.Equal("Invalid Hex String", ex.Message);
            }
        }

        [Theory]
        [InlineData("SAPP", "SeZQ1jpMHms63CejL4nTTbVYoLNQXLsbJ6")]
        [InlineData("TKYAN", "Kisn6NZVjHt6qAd7b1VakPw2DyZkU9GQQy")]
        public void ValidateAddress_ValidAddresses_ReturnsTrue(string ticker, string address)
        {
            var mockCoinRepository = new Mock<CoinRepository>();
            var mockBlockbookHttpClient = new Mock<IBlockbookHttpClient>();

            // Arrange
            var service = new WalletService(mockBlockbookHttpClient.Object, mockCoinRepository.Object);

            // Act
            bool isValid = service.ValidateAddress(ticker, address);

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("SAPP", "SuKUyKpTKU45c8kEwWaYinWo8AoZqARuMi")]
        [InlineData("TKYAN", "kisn6NZVjHt6qAd7b1VakPw2DyZkU9GHQy")]
        public void ValidateAddress_InvalidAddresses_ReturnsFalse(string ticker, string address)
        {
            var mockCoinRepository = new Mock<CoinRepository>();
            var mockBlockbookHttpClient = new Mock<IBlockbookHttpClient>();

            // Arrange
            var service = new WalletService(mockBlockbookHttpClient.Object, mockCoinRepository.Object);

            // Act
            bool isValid = service.ValidateAddress(ticker, address);

            // Assert
            Assert.False(isValid);
        }

        private List<UtxoObject> GetMockUtxos()
        {
            return new List<UtxoObject>
            {
                new UtxoObject { Txid = "0135604ec9e37d02adc3970ad95ad6035534fa4178eea21cbf0b05589996dada", Vout = 1, Value = "183614583333", Height = 2302489, Confirmations = 32 },
                new UtxoObject { Txid = "c0ce511e66b5aa9ddacca9592fa5c1627cc79f54b9a2083d72925b8d904a6160", Vout = 1, Value = "187575931363", Height = 2302457, Confirmations = 64 },
                new UtxoObject { Txid = "e5c28ac17c9b9383547484a93b2ea17066bf7239259a189baa7cfb2fe761250e", Vout = 1, Value = "187743055555", Height = 2302446, Confirmations = 75 },
                new UtxoObject { Txid = "dd5418d5ad3c6f3b47a10eba3d9ebc8a0b028a9335430d11825b3204bcb020ac", Vout = 1, Value = "104819444444", Height = 2302401, Confirmations = 120 },
                new UtxoObject { Txid = "46558af6a79a86a3c07dd115dc7fc330d0d7a35a7d4c6bc40247abf345cf214d", Vout = 1, Value = "183614583333", Height = 2302384, Confirmations = 137 },
                new UtxoObject { Txid = "72c562fdae380c5a439de21f7654ce2cc468515019269f0ef99c1d737b37123c", Vout = 1, Value = "143418336589", Height = 2302357, Confirmations = 164 },
                new UtxoObject { Txid = "ddd64409c6f47ec68d47846621f5d517350191276f8e6a7c7381bdd88c04dd31", Vout = 1, Value = "208871760417", Height = 2302342, Confirmations = 179 },
                new UtxoObject { Txid = "71d887f969df6be4e8c808f84940a1ae0f1a2663083fa4cf9e706acc7a138ed4", Vout = 1, Value = "163557291666", Height = 2302340, Confirmations = 181 },
                new UtxoObject { Txid = "c17c32c16c6caec84df131b068c40cd9c1c421fd098cbe86065c11834e3a51ec", Vout = 1, Value = "171229166666", Height = 2302335, Confirmations = 186 },
                new UtxoObject { Txid = "561a20af52fffd37c374d05ebf35313df62bdbacd0626f4bc48b4def0f6aefaf", Vout = 1, Value = "203034994792", Height = 2302209, Confirmations = 312 },
                new UtxoObject { Txid = "545b46cde8c645db6c2b61d706ff5737367955579875317b6eb9da8f324693a2", Vout = 1, Value = "132505253257", Height = 2302205, Confirmations = 316 },
                new UtxoObject { Txid = "f263bc685796e513ac33a14c7f7fa8909f2cb215800b6ff21ffade875829796c", Vout = 1, Value = "168807291667", Height = 2302195, Confirmations = 326 },
                new UtxoObject { Txid = "e26176b28e5215c5c4a94b4d6d68366f8926b5fdc1a6bc2dc4133f6cc595a56d", Vout = 1, Value = "167558687500", Height = 2302120, Confirmations = 401 },
                new UtxoObject { Txid = "d3f668eb46319a4fb7b53763ce5d9437ee173a0a36fdc29eaa267d9e98fdd9d9", Vout = 1, Value = "156153645833", Height = 2302115, Confirmations = 406 },
                new UtxoObject { Txid = "90923f381e88d4f03effa161f8e347858bad8194761d87ebca407bbb2c238bad", Vout = 1, Value = "154807291666", Height = 2302069, Confirmations = 452 },
                new UtxoObject { Txid = "afe85645c7a02a0ea01043b409179e6e35410afd2e6d7e3adf252a601d6182b2", Vout = 1, Value = "141614583333", Height = 2302059, Confirmations = 462 },
                new UtxoObject { Txid = "90440459e5561b237816251498213a3590c3c13db8165d909590fd4ab7a6fd38", Vout = 1, Value = "132779343750", Height = 2302037, Confirmations = 484 },
                new UtxoObject { Txid = "8529a50b2575c7c6b73c1fd3beffbd49a54b0934434a2b0ef350979196074bfb", Vout = 1, Value = "225614583333", Height = 2302036, Confirmations = 485 },
                new UtxoObject { Txid = "709a7d29fff09de2537c267d0fe75ed2f1950d217b3bf2b176754bd759110e66", Vout = 1, Value = "147786398845", Height = 2302022, Confirmations = 499 },
                new UtxoObject { Txid = "f384d33a6a04221976c5d54a02357b89b1155382ec39502a4e01b6a784efbd71", Vout = 1, Value = "118836673177", Height = 2302010, Confirmations = 511 },
                new UtxoObject { Txid = "7f5b39c50accb1eb25fac371b481b2db199e2057bf1f1d6b363e2b44d6a6567d", Vout = 1, Value = "185418336588", Height = 2301953, Confirmations = 568 },
                new UtxoObject { Txid = "5ddd28b19cbede05ab26285c0ee57ab0ebd23e370648d973f6fe7999f7aa9ef0", Vout = 1, Value = "162709168295", Height = 2301912, Confirmations = 609 },
                new UtxoObject { Txid = "f7c613268ca742f4835fd7c4d4fcd54c9ffff30c90fc6fc7ae02958d9d05e67b", Vout = 1, Value = "115229166666", Height = 2301861, Confirmations = 660 },
                new UtxoObject { Txid = "2492657e8218df2cf3ebf0d73074e8fcaedf5bce930b1e6f9f18e60bc7cafe19", Vout = 1, Value = "118836673177", Height = 2301860, Confirmations = 661 },
                new UtxoObject { Txid = "e4bd341e0d24f8e41118591f66937a4b11fed548633b7e0ac131d95921342ef8", Vout = 1, Value = "188505253257", Height = 2301859, Confirmations = 662 },
                new UtxoObject { Txid = "fccc5114ab26936210d10ffb7d1284140364da52ddbe2148b85739182d2420d8", Vout = 1, Value = "227229166667", Height = 2301857, Confirmations = 664 },
                new UtxoObject { Txid = "8874c34e8a5cc370bc5554c51e5960f4e910711412f8af7428dfb4ac23537088", Vout = 1, Value = "133575931363", Height = 2301842, Confirmations = 679 },
                new UtxoObject { Txid = "d8bc909be0ed896f1d9c912fec84dae3a2beb36dfc9306cc6b9e2b9478d5395c", Vout = 1, Value = "141709168294", Height = 2301819, Confirmations = 702 },
                new UtxoObject { Txid = "42220616b1be49611f96b2c963859144ab2627a48b4dc5ea5d07c87d669cbd54", Vout = 1, Value = "146836673177", Height = 2301814, Confirmations = 707 },
                new UtxoObject { Txid = "78218de50576e85006231fafc32797ee478ebec3e180c8ee2e480ce80e143859", Vout = 1, Value = "132819444444", Height = 2301801, Confirmations = 720 },
                new UtxoObject { Txid = "898a94a832ade7c32d5aa3ba52c84312dd8f201db86174fcb142a182cce5ba9f", Vout = 1, Value = "168807291668", Height = 2301762, Confirmations = 759 },
                new UtxoObject { Txid = "90037c8f6f3e83b89098fc42b7dc94e78bb80a8d7d61bf79fc343b7d4435a410", Vout = 1, Value = "178409722222", Height = 2301754, Confirmations = 767 },
                new UtxoObject { Txid = "6179dd91c5b0a494a6757227fbcb1fd82654d1e05b46745c2da9968987acc8e3", Vout = 1, Value = "141614583335", Height = 2301736, Confirmations = 785 },
                new UtxoObject { Txid = "4cc7ecc7de3c25bfcbd48fd912f487f79ddf3c76d78ce1ad9e777a021d730cad", Vout = 1, Value = "196807291667", Height = 2301733, Confirmations = 788 },
                new UtxoObject { Txid = "5b07dab9bd0c17f5a897310060616a29f9242605497ec39582f61eab590b96c0", Vout = 1, Value = "78614583333", Height = 2301730, Confirmations = 791 },
                new UtxoObject { Txid = "f8a16e750e43e76bc353f9f31a0652fe0e1690013416eda10cc0181dab388208", Vout = 1, Value = "130307291667", Height = 2301723, Confirmations = 798 },
                new UtxoObject { Txid = "9da2b0f51992372ff6c06bd690ef1d96001da067e446ccbcde55677ab2f29417", Vout = 1, Value = "196807291667", Height = 2301702, Confirmations = 819 },
                new UtxoObject { Txid = "ae269ea864ce44b23887f2e966741a0cc20819e6edf857390c656b9ef5499455", Vout = 1, Value = "227229166667", Height = 2301679, Confirmations = 842 },
                new UtxoObject { Txid = "5301f131f3fe851331c0c07fb14b447ab59290bb08cade043b6e64fcac30d5d1", Vout = 1, Value = "216505253257", Height = 2301656, Confirmations = 865 },
                new UtxoObject { Txid = "a906c45a0f237f79ee81f65579cab9c02ee0e4f60bf715b103d080e5da9e8e77", Vout = 1, Value = "155614583333", Height = 2301622, Confirmations = 899 },
                new UtxoObject { Txid = "a060eec80496ddb3a99ccd7831fb2fbba6352d0da740f1bc0234cecab398e45e", Vout = 1, Value = "225614583333", Height = 2301610, Confirmations = 911 },
                new UtxoObject { Txid = "0ba30e5a2e4f593dc6b44b9deb969fde1cb46da02a154be9dca5ee5067565305", Vout = 1, Value = "253709168294", Height = 2301606, Confirmations = 915 },
                new UtxoObject { Txid = "1e118dad4a24b105a0fa445dbe3be119386028ed53db0ccc9bba2cd4f3764d33", Vout = 1, Value = "199229166669", Height = 2301592, Confirmations = 929 },
                new UtxoObject { Txid = "0e800082f97a133911b8c0209dbd1a540a6a1292235385b37a4b34de54198c20", Vout = 1, Value = "141614583333", Height = 2301581, Confirmations = 940 },
                new UtxoObject { Txid = "a15a82d3d825a2b98eaa509bda45324fe3e66df7b3ce17792a00dd29cf31ad9a", Vout = 1, Value = "174836673177", Height = 2301562, Confirmations = 959 },
                new UtxoObject { Txid = "425aaa46795d018affe4b711caac88b22a97c3fe92345c40e9539841d77c9098", Vout = 1, Value = "170278891059", Height = 2301543, Confirmations = 978 },
                new UtxoObject { Txid = "bd3d602f508126428f3d6c95fb160c296d4133f0b95fa9f363dae17d8dae8fc9", Vout = 1, Value = "202836673177", Height = 2301526, Confirmations = 995 },
                new UtxoObject { Txid = "0b2ba7005109d87661c54bf3c98a4f0656d3e3cd344660414b15022f23ffb625", Vout = 1, Value = "258836673177", Height = 2301523, Confirmations = 998 },
                new UtxoObject { Txid = "b50ef531e786746c0308715b8ffb51859167de569b50ecdd2fc1b15cea4b2041", Vout = 1, Value = "160754638547", Height = 2301522, Confirmations = 999 },
                new UtxoObject { Txid = "33f30d63471948a4fb9bb65dddf7dd6d5fcb84b4ce1a47c4f13f02f2c261a43d", Vout = 1, Value = "145218836676", Height = 2301502, Confirmations = 1019 },
                new UtxoObject { Txid = "46f6df12c5a7ea3dfbcf8b79c6118e655d2eaf119e97700f19e2b542d9d1b57f", Vout = 1, Value = "134836673178", Height = 2301482, Confirmations = 1039 },
                new UtxoObject { Txid = "ea0b9e41f05a7d9bda0130ab662a18e56a3f4af1f2b2124b9c74deaa4d18e61", Vout = 1, Value = "198613281250", Height = 2301474, Confirmations = 1047 },
                new UtxoObject { Txid = "ec2e4ab1211dcb5e01b2f2c366101f74b7b63f8f51a7213e17e25e09f930f204", Vout = 1, Value = "200836673177", Height = 2301452, Confirmations = 1069 },
                new UtxoObject { Txid = "f206c6837f488b4b7ef71a0f812fcf8c4b87180e4ff45e4ec0c1eb8a0e32f111", Vout = 1, Value = "186836673177", Height = 2301440, Confirmations = 1081 },
                new UtxoObject { Txid = "0f7c1ec95e2e7aa9c47728731f53c6a9e34f7cf98e8bfeb05d540e7bde15cb3d", Vout = 1, Value = "187029276765", Height = 2301425, Confirmations = 1096 },
                new UtxoObject { Txid = "eb8d25e34099d59e70a26743e8e35e99c29e6b4d0fde4d7a8662e8781693a1ca", Vout = 1, Value = "160636673178", Height = 2301421, Confirmations = 1100 },
                new UtxoObject { Txid = "b5b62ec3f15d01cf08396d0e6a8be6dd9c740726e3e0f99a4b19c8b1a82c96a5", Vout = 1, Value = "192878891060", Height = 2301404, Confirmations = 1117 },
                new UtxoObject { Txid = "a174586364a8f56e86db671dd6b17b48a19c41d675ba2064bf115ed7de1f3cd0", Vout = 1, Value = "184836673178", Height = 2301385, Confirmations = 1136 },
                new UtxoObject { Txid = "a38a6b6d63f5b50c4b57bfb3ed6b3d9797ae8a5e27ac1026f1d4aaff8749fb71", Vout = 1, Value = "222555362896", Height = 2301368, Confirmations = 1153 },
                new UtxoObject { Txid = "75b3c980b9e4e438a007bf307e5b4a7a5ad2622b0ef5bf8302bf60706219012f", Vout = 1, Value = "170407291668", Height = 2301350, Confirmations = 1171 },
                new UtxoObject { Txid = "1021280c13003a1d6b3c6f9f8db7e94ab99036519005e0c8d18e5f2ee2e93e26", Vout = 1, Value = "173361058562", Height = 2301331, Confirmations = 1190 },
                new UtxoObject { Txid = "a50fba5b43c96c3d5f5f486bf73e8dd3f40c1ea9e693b37ad369dc8cd01ebe74", Vout = 1, Value = "179878891062", Height = 2301310, Confirmations = 1211 },
                new UtxoObject { Txid = "303cd77e24e1a00478e99e5017d52a5b433c4f188ad3806d67eb4e5a1c72ecef", Vout = 1, Value = "158878891062", Height = 2301295, Confirmations = 1226 },
                new UtxoObject { Txid = "e6ce6b4dbb5f1dfc51c34b59d7ac5ef7e1e007d50b8d9c9f775036d9f79b5c34", Vout = 1, Value = "134836673179", Height = 2301280, Confirmations = 1241 },
                new UtxoObject { Txid = "f1e9b5905cb2ea1d7db4c64a3f2d2bbaf43128ac590d6b0d71d1b9d0c04127bb", Vout = 1, Value = "177878891063", Height = 2301261, Confirmations = 1260 },
                new UtxoObject { Txid = "5a674aa8775b2ad47b2c3be5c78c7e501dca4ac13a6fcff1f5cc3e099de68f8a", Vout = 1, Value = "193836673180", Height = 2301240, Confirmations = 1281 },
                new UtxoObject { Txid = "ea8a95a2c4e32e0c6a14ff9c8fbc1809b59d9ecaf1710f09be9ab44671f72b0d", Vout = 1, Value = "228878891064", Height = 2301220, Confirmations = 1301 },
            };
        }

        private List<UtxoObject> GetMockUtxosLowValues()
        {
            return new List<UtxoObject>
            {
                new UtxoObject { Txid = "0135604ec9e37d02adc3970ad95ad6035534fa4178eea21cbf0b05589996dada", Vout = 1, Value = "458718361", Height = 2302489, Confirmations = 32 },
                new UtxoObject { Txid = "c0ce511e66b5aa9ddacca9592fa5c1627cc79f54b9a2083d72925b8d904a6160", Vout = 1, Value = "185677753", Height = 2302457, Confirmations = 64 },
                new UtxoObject { Txid = "e5c28ac17c9b9383547484a93b2ea17066bf7239259a189baa7cfb2fe761250e", Vout = 1, Value = "185678774", Height = 2302446, Confirmations = 75 },
                new UtxoObject { Txid = "dd5418d5ad3c6f3b47a10eba3d9ebc8a0b028a9335430d11825b3204bcb020ac", Vout = 1, Value = "108567481", Height = 2302401, Confirmations = 120 },
                new UtxoObject { Txid = "46558af6a79a86a3c07dd115dc7fc330d0d7a35a7d4c6bc40247abf345cf214d", Vout = 1, Value = "183566143", Height = 2302384, Confirmations = 137 },
                new UtxoObject { Txid = "72c562fdae380c5a439de21f7654ce2cc468515019269f0ef99c1d737b37123c", Vout = 1, Value = "143419783", Height = 2302357, Confirmations = 164 },
                new UtxoObject { Txid = "ddd64409c6f47ec68d47846621f5d517350191276f8e6a7c7381bdd88c04dd31", Vout = 1, Value = "208788717", Height = 2302342, Confirmations = 179 },
                new UtxoObject { Txid = "71d887f969df6be4e8c808f84940a1ae0f1a2663083fa4cf9e706acc7a138ed4", Vout = 1, Value = "165635572", Height = 2302340, Confirmations = 181 },
                new UtxoObject { Txid = "c17c32c16c6caec84df131b068c40cd9c1c421fd098cbe86065c11834e3a51ec", Vout = 1, Value = "416666681", Height = 2302335, Confirmations = 186 },
                new UtxoObject { Txid = "561a20af52fffd37c374d05ebf35313df62bdbacd0626f4bc48b4def0f6aefaf", Vout = 1, Value = "203479213", Height = 2302209, Confirmations = 312 },
            };
        }


    }
}
