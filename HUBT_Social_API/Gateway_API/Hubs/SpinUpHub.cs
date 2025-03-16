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

        //private static async Task RunRequestAsync(ChannelWriter<string> writer, string url)
        //{
        //    string serviceUrl = $"https://{url}/ping";
        //    var httpClient = new HttpClient();

        //    try
        //    {
        //        await writer.WriteAsync($"Connecting to Sever... ⏳");

        //        var response = await httpClient.GetAsync(serviceUrl);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            await writer.WriteAsync($"is up and running. ✅");
        //        }
        //        else
        //        {
        //            await writer.WriteAsync($"is not responding. ❌");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        await writer.WriteAsync($"failed to Spin up ❌");
        //    }
        //    finally
        //    {
        //        writer.Complete();
        //    }
        //}
        private static async Task RunRequestAsync(ChannelWriter<string> writer, string url)
        {
            string serviceUrl = $"https://{url}/ping";
            var httpClient = new HttpClient
            {
                // Tăng timeout lên đáng kể để đợi service spin up
                Timeout = TimeSpan.FromMinutes(2)
            };

            try
            {
                await writer.WriteAsync($"Đang kết nối đến server... ⏳");

                // Thử lại nhiều lần với thời gian chờ giữa các lần thử
                int maxRetries = 5;
                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        if (i > 0)
                        {
                            await writer.WriteAsync($"Đang thử lần thứ {i + 1}...");
                        }

                        var response = await httpClient.GetAsync(serviceUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            await writer.WriteAsync($"Đã hoạt động và sẵn sàng. ✅");
                            return;
                        }
                        else
                        {
                            await writer.WriteAsync($"Đang khởi động (Status: {response.StatusCode})...");
                        }
                    }
                    catch (Exception ex)
                    {
                        await writer.WriteAsync($"Lỗi: {ex.Message}. Đang đợi và thử lại...");
                    }

                    // Tăng thời gian đợi theo cấp số nhân (exponential backoff)
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
                }

                await writer.WriteAsync($"Không thể kích hoạt service sau nhiều lần thử. ❌");
            }
            catch (Exception ex)
            {
                await writer.WriteAsync($"Lỗi không xác định: {ex.Message} ❌");
            }
            finally
            {
                writer.Complete();
            }
        }
    }
}
