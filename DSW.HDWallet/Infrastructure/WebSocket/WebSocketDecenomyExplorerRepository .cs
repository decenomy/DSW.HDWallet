using System.Net.WebSockets;

namespace DSW.HDWallet.Infrastructure.WebSocket
{
    public class WebSocketDecenomyExplorerRepository : IWebSocketDecenomyExplorerRepository
    {
        public async Task GetWSTransactionAsync(string coin, string txId)
        {
            var uri = new Uri(getWebSocketUri(coin));

            using (var socket = new ClientWebSocket())
            {
                try
                {
                    await socket.ConnectAsync(uri, CancellationToken.None);

                    var bytes = new byte[4096];
                    var buffer = new ArraySegment<byte>(bytes);
                    var response = await socket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);

                }
                catch (WebSocketException ex)
                {
                    throw;
                }
            }
        }

        private string getWebSocketUri(string coin)
        {
            return $"wss://{coin.ToLower()}.decenomy.net";
        }
    }
}