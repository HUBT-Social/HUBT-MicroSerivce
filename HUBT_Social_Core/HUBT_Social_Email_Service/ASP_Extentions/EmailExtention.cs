using HUBT_Social_Core.Models.DTOs.EmailDTO;
using HUBT_Social_Core.Settings;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace HUBT_Social_Email_Service.ASP_Extentions
{
    public static class EmailExtention
    {
        public static async Task<bool> SendEmailAsync(this MimeMessage mailContent, SMPTSetting emailSetting)
        {
            if (emailSetting is null)
                throw new InvalidOperationException("Email setting is not initialized. Call InitEmailExtention() first.");

            try
            {
                using var smtpClient = new SmtpClient();

                await smtpClient.ConnectAsync(
                    emailSetting.Host,
                    int.Parse(emailSetting.Port),
                    SecureSocketOptions.StartTls
                );

                await smtpClient.AuthenticateAsync(emailSetting.Email, emailSetting.Password);
                await smtpClient.SendAsync(mailContent);
                await smtpClient.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Error] {ex.Message}");
                return false;
            }
        }

        public static string StripHtmlTags(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return "";

            // Regex để loại bỏ HTML tags
            return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", "");
        }
    }
}
