using DSW.HDWallet.Domain.WSObject;
using System.Text.Json;
using WebSocketSharp;

namespace DSW.HDWallet.Infrastructure.WS
{
    public class WebSocketDecenomyExplorerRepository : IWebSocketDecenomyExplorerRepository
    {
        private readonly JsonSerializerOptions serializerOptions = new() { PropertyNameCaseInsensitive = true };
        //public async Task<WSTransactionObject> GetWSTransactionAsync(string coin, string txId)
        //{
        //    try
        //    {
        //        string? response = null;

        //        using WebSocket ws = new(getWebSocketUri(coin));
        //        TaskCompletionSource<string> t = new();

        //        ws.OnMessage += (sender, e) =>
        //        {
        //            if (e.IsText)
        //            {
        //                response = e.Data;
        //                t.SetResult(response);
        //            }
        //        };

        //        ws.Connect();                
        //        ws.Send(GetTransactionRequest(txId));
        //        await t.Task;
        //        ws.Close();

        //        return JsonSerializer.Deserialize<WSTransactionObject>(response!, serializerOptions)!;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error in WebSocket Request: {ex.Message}");
        //    }            
        //}

        //public async Task<WSSubscribeObject> SubscribeNewTransaction(string coin)
        //{
        //    try
        //    {
        //        string? response = null;
        //        using WebSocket ws = new(getWebSocketUri(coin));
        //        TaskCompletionSource<string> t = new();

        //        ws.OnOpen += (sender, e) =>
        //        {
        //            ws.Send(GetSubscribeNewTransactionRequest());                    
        //        };

        //        ws.OnMessage += (sender, e) =>
        //        {
        //            if (e.IsText)
        //            {
        //                response = e.Data;
        //                t.SetResult(response);
        //            }
        //        };

        //        ws.Connect();
        //        await t.Task;
        //        ws.Close();

        //        return JsonSerializer.Deserialize<WSSubscribeObject>(response!, serializerOptions)!;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error in WebSocket Request: {ex.Message}");
        //    }

        //}

        private string GetSubscribeNewTransactionRequest()
        {
            WSRequest wsRequest = new()
            {
                method = "subscribeNewTransaction",
                @params = new Params { },
                flag = "-enablesubnewtx"
            };

            return JsonSerializer.Serialize(wsRequest, serializerOptions);
        }

        private string GetTransactionRequest(string txId)
        {
            WSRequest wsRequest = new()
            {
                method = "getTransaction",
                @params = new Params() { txid = txId }
            };

            return JsonSerializer.Serialize(wsRequest, serializerOptions);
        }

        private string getWebSocketUri(string coin)
        {
            return $"wss://{coin.ToLower()}.decenomy.net/websocket";
        }
    }
}