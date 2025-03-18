using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;

namespace Gateway_API.Hubs
{
    public class SpinUpHub(IConfiguration configuration) : Hub
    {
        private readonly string? CloudflareWorkerUrl = configuration.GetSection("CloudflareWorkerUrl").Get<string>();

        public ChannelReader<string> SpinUpServers(string url)
        {
            var channel = Channel.CreateUnbounded<string>();

            _ = RunRequestAsync(channel.Writer, url);

            return channel.Reader;
        }

        private async Task RunRequestAsync(ChannelWriter<string> writer, string url)
        {
            if (string.IsNullOrEmpty(CloudflareWorkerUrl))
            {
                await writer.WriteAsync("CloudflareWorkerUrl is not configured ❌");
                writer.Complete();
                return;
            }

            string serviceUrl = $"{CloudflareWorkerUrl}?host={url}";
            var httpClient = new HttpClient();

            try
            {
                await writer.WriteAsync($"Connecting to Server... ⏳");

                var response = await httpClient.GetAsync(serviceUrl);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await writer.WriteAsync($"Server is up and running ✅");
                }
                else
                {
                    await writer.WriteAsync($"Server is not responding ❌");
                }

                await writer.WriteAsync($"Không thể kích hoạt service sau nhiều lần thử. ❌");
            }
            catch (Exception ex)
            {
                await writer.WriteAsync($"Failed to spin up server ❌");
            }
            finally
            {
                writer.Complete();
            }
        }
    }
}
