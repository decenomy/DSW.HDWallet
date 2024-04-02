using DSW.HDWallet.Domain.Models;
using NBitcoin;

namespace DSW.HDWallet.Domain.Utils
{
    public static class TransactionTypeRetriever
    {

        public static string GetTransactionType(TransactionType transactionType)
        {
            if(transactionType == TransactionType.Incoming)
            {
                return "Received";
            }
            else
            {
                return "Sent";
            }
        }

    }
}
