using NBitcoin;

namespace DSW.HDWallet.Domain.Utils
{
    public class AddressValidator : IAddressValidator
    {
        public bool ValidateAddress(string address)
        {
            try
            {
                BitcoinAddress bitcoinAddress = BitcoinAddress.Create(address, Network.Main);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

}
