using NBitcoin;
using NBitcoin.Altcoins.HashX11;
using NBitcoin.Crypto;
using NBitcoin.DataEncoders;
using NBitcoin.Protocol;

namespace DSW.HDWallet.AltCoins
{

    public class CryptoSaga : NetworkSetBase
    {
        public static CryptoSaga Instance { get; } = new CryptoSaga();

        public override string CryptoCode => "SAGA";

        private CryptoSaga()
        {
        }

        public class CryptoSagaConsensusFactory : ConsensusFactory
        {
            private CryptoSagaConsensusFactory()
            {
            }

            public static CryptoSagaConsensusFactory Instance { get; } = new CryptoSagaConsensusFactory();

            public override BlockHeader CreateBlockHeader()
            {
                return new CryptoSagaBlockHeader();
            }

            public override Block CreateBlock()
            {
                return new CryptoSagaBlock(new CryptoSagaBlockHeader());
            }
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public class CryptoSagaBlockHeader : BlockHeader
        {
            private static byte[] CalculateHash(byte[] data, int offset, int count)
            {
                var h = new Quark().ComputeBytes(data.Skip(offset).Take(count).ToArray());

                return h;
            }

            protected override HashStreamBase CreateHashStream()
            {
                return BufferedHashStream.CreateFrom(CalculateHash);
            }
        }

        public class CryptoSagaBlock : Block
        {
            public CryptoSagaBlock(CryptoSagaBlockHeader h) : base(h)
            {
            }

            public override ConsensusFactory GetConsensusFactory()
            {
                return Instance.Mainnet.Consensus.ConsensusFactory;
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete

        protected override void PostInit()
        {
            RegisterDefaultCookiePath("SAGA");
        }

        protected override NetworkBuilder CreateMainnet()
        {
            var builder = new NetworkBuilder();
            builder.SetConsensus(new Consensus
            {
                SubsidyHalvingInterval = 210000,
                MajorityEnforceBlockUpgrade = 8100,
                MajorityRejectBlockOutdated = 10260,
                MajorityWindow = 10800,
                BIP34Hash = new uint256("00000eef0583695d6da23a78bab1c39939bbb54cf9bd5f0d4881c8eef364cd26"),
                PowLimit = new Target(0 >> 1),
                MinimumChainWork = new uint256("000000000000000000000000000000000000000000000000c2ba8ca4fb1f06cb"),
                PowTargetTimespan = TimeSpan.FromSeconds(24 * 60 * 60),
                PowTargetSpacing = TimeSpan.FromSeconds(1 * 60),
                PowAllowMinDifficultyBlocks = true,
                CoinbaseMaturity = 100,
                PowNoRetargeting = false,
                RuleChangeActivationThreshold = 1916,
                MinerConfirmationWindow = 2016,
                ConsensusFactory = CryptoSagaConsensusFactory.Instance,
                SupportSegwit = false,
                CoinType = 843
            })
                .SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 125 })
                .SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 44 })
                .SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 142 })
                .SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E })
                .SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 })
                .SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("CryptoSaga"))
                .SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("CryptoSaga"))
                .SetMagic(0x191643a0)
                .SetPort(37552)
                .SetRPCPort(37553)
                .SetMaxP2PVersion(70920)
                .SetName("CryptoSaga-main")
                .AddAlias("CryptoSaga-mainnet")
                .AddDNSSeeds(new[]
                {
                    new DNSSeedData("seed1", "seed1.cryptosaga.net"),
                    new DNSSeedData("seed2", "seed2.cryptosaga.net"),
                    new DNSSeedData("seed3", "seed3.cryptosaga.net")
                })
                .AddSeeds(new NetworkAddress[0])
                .SetGenesis("01000000000000000000000000000000000000000000000000000000000000000000000014e427b75837280517873799a954e87b8b0484f3f1df927888a0ff4fd3a0c9f7bb2eac56f0ff0f1edfa624000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff8604ffff001d01044c7d323031372d30392d32312032323a30313a3034203a20426974636f696e20426c6f636b204861736820666f722048656967687420343836333832203a2030303030303030303030303030303030303039326431356535623365366538323639333938613834613630616535613264626434653766343331313939643033ffffffff0100ba1dd205000000434104c10e83b2703ccf322f7dbd62dd5855ac7c10bd055814ce121ba32607d573b8810c02c0582aed05b4deb9c4b77b26d92428c61256cd42774babea0a073b2ed0c9ac00000000");

            return builder;
        }

        protected override NetworkBuilder CreateTestnet()
        {
            var builder = new NetworkBuilder();
            builder.SetConsensus(new Consensus
            {
                SubsidyHalvingInterval = 210000,
                MajorityEnforceBlockUpgrade = 51,
                MajorityRejectBlockOutdated = 75,
                MajorityWindow = 100,
                BIP34Hash = new uint256("0000000000000000000000000000000000000000000000000000000000000000"),
                PowLimit = new Target(0 >> 1),
                MinimumChainWork = new uint256("0000000000000000000000000000000000000000000000000000000000100010"),
                PowTargetTimespan = TimeSpan.FromSeconds(24 * 60 * 60),
                PowTargetSpacing = TimeSpan.FromSeconds(1 * 60),
                PowAllowMinDifficultyBlocks = true,
                CoinbaseMaturity = 30,
                PowNoRetargeting = false,
                RuleChangeActivationThreshold = 1512,
                MinerConfirmationWindow = 2016,
                ConsensusFactory = CryptoSagaConsensusFactory.Instance,
                SupportSegwit = false,
                CoinType = 1
            })
                .SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 125 })
                .SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 44 })
                .SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 142 })
                .SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E })
                .SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 })
                .SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("CryptoSaga"))
                .SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("CryptoSaga"))
                .SetMagic(0x191643a0)
                .SetPort(37552)
                .SetRPCPort(37553)
                .SetMaxP2PVersion(70920)
                .SetName("CryptoSaga-test")
                .AddAlias("CryptoSaga-testnet")
                .AddSeeds(new NetworkAddress[0])
                //testnet down for now
                .SetGenesis("0100000000000000000000000000000000000000000000000000000000000000000000008c5b00d67050180b3a90addb9cd1aabbb3dd79ce20fc071d428ce374581b3f7cde30df5cf0ff0f1e1a1754000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff3b04ffff001d010433446f676543617368205265706f7765726564204c61756e6368202d20616b736861796e65787573202d204c6971756964333639ffffffff0100000000000000004341047a7df379bd5e6b93b164968c10fcbb141ecb3c6dc1a5e181c2a62328405cf82311dd5b40bf45430320a4f30add05c8e3e16dd56c52d65f7abe475189564bf2b1ac00000000");

            return builder;
        }

        protected override NetworkBuilder CreateRegtest()
        {
            var builder = new NetworkBuilder();
            var res = builder.SetConsensus(new Consensus
            {
                SubsidyHalvingInterval = 150,
                MajorityEnforceBlockUpgrade = 750,
                MajorityRejectBlockOutdated = 950,
                MajorityWindow = 1000,
                BIP34Hash = new uint256(),
                PowLimit = new Target(0 >> 1),
                MinimumChainWork = new uint256("0x0000529df5fae941569b6466128042f5f036a8d1d380dd484a06e8a12fb275a3"),
                PowTargetTimespan = TimeSpan.FromSeconds(1 * 60 * 40),
                PowTargetSpacing = TimeSpan.FromSeconds(1 * 60),
                PowAllowMinDifficultyBlocks = false,
                CoinbaseMaturity = 100,
                PowNoRetargeting = true,
                RuleChangeActivationThreshold = 1916,
                MinerConfirmationWindow = 2016,
                ConsensusFactory = CryptoSagaConsensusFactory.Instance,
                SupportSegwit = false
            })
                .SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 125 })
                .SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 44 })
                .SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 142 })
                .SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E })
                .SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 })
                .SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("CryptoSaga"))
                .SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("CryptoSaga"))
                .SetMagic(0x191643a0)
                .SetPort(37552)
                .SetRPCPort(37553)
                .SetMaxP2PVersion(70920)
                .SetName("CryptoSaga-reg")
                .AddAlias("CryptoSaga-regtest")
                .AddDNSSeeds(new DNSSeedData[0])
                .AddSeeds(new NetworkAddress[0])
                //No regtest at the moment
                .SetGenesis("01000000000000000000000000000000000000000000000000000000000000000000000014e427b75837280517873799a954e87b8b0484f3f1df927888a0ff4fd3a0c9f7bb2eac56ffff7f20393000000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff8604ffff001d01044c7d323031372d30392d32312032323a30313a3034203a20426974636f696e20426c6f636b204861736820666f722048656967687420343836333832203a2030303030303030303030303030303030303039326431356535623365366538323639333938613834613630616535613264626434653766343331313939643033ffffffff0100ba1dd205000000434104c10e83b2703ccf322f7dbd62dd5855ac7c10bd055814ce121ba32607d573b8810c02c0582aed05b4deb9c4b77b26d92428c61256cd42774babea0a073b2ed0c9ac00000000");

            return builder;
        }
    }
}