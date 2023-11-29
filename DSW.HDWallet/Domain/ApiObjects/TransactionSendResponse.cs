namespace DSW.HDWallet.Domain.ApiObjects
{
    public class TransactionSendResponse
    {
        public string? Result { get; set; }
        public ApiError? Error { get; set; }
    }

    public class ApiError
    {
        public string? Message { get; set; }
    }
}