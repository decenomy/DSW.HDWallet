using DSW.HDWallet.Domain.Models;
using DSW.HDWallet.Infrastructure.Interfaces;
using NBitcoin;

namespace DSW.HDWallet.Application.Objects
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public OperationResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static OperationResult Ok(string message = "Operation successful.")
        {
            return new OperationResult(true, message);
        }

        public static OperationResult Fail(string message)
        {
            return new OperationResult(false, message);
        }
    }
}
