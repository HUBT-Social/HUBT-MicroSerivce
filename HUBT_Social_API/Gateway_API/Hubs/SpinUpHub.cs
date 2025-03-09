using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;

namespace Gateway_API.Hubs
{
    public class SpinUpHub : Hub
    {
        public ChannelReader<string> SpinUpServers(string url)
        {
            var channel = Channel.CreateUnbounded<string>();

            _ = RunRequestAsync(channel.Writer, url);

            return channel.Reader;
        }

        private static async Task RunRequestAsync(ChannelWriter<string> writer, string url)
        {
            string serviceUrl = $"https://{url}/ping";
            var httpClient = new HttpClient();

            try
            {
                await writer.WriteAsync($"Connecting to Sever... ⏳");

                var response = await httpClient.GetAsync(serviceUrl);
                if (response.IsSuccessStatusCode)
                {
                    await writer.WriteAsync($"is up and running. ✅");
                }
                else
                {
                    await writer.WriteAsync($"is not responding. ❌");
                }
            }
            catch (Exception)
            {
                await writer.WriteAsync($"failed to Spin up ❌");
            }
            finally
            {
                writer.Complete();
            }
        }
    }
}
