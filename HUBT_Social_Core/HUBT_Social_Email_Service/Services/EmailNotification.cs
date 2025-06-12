using HUBT_Social_Core.Models.DTOs.EmailDTO;
using HUBT_Social_Core.Settings;
using HUBT_Social_Email_Service.ASP_Extentions;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Email_Service.Services
{
    internal class EmailNotification(IOptions<SMPTSetting> setting) : IEmailNotification
    {

        private readonly SMPTSetting _emailSetting = setting.Value;

        public async Task<bool> SendNotificationAsync(SendNotificationMailRequest emailRequest)
        {
            if (emailRequest.ToEmails == null || !emailRequest.ToEmails.Any())
                return false;

            return await CreateNotificationMessage(emailRequest).SendEmailAsync(_emailSetting);
        }
        private MimeMessage CreateNotificationMessage(SendNotificationMailRequest emailRequest)
        {
            var emailMessage = new MimeMessage();
            string emailHtmlContent;

            // Người gửi
            emailMessage.From.Add(new MailboxAddress("HUBT Social", _emailSetting.Email));

            // Thêm nhiều người nhận TO
            if (emailRequest.ToEmails != null && emailRequest.ToEmails.Any())
            {
                foreach (var email in emailRequest.ToEmails)
                {
                    emailMessage.To.Add(new MailboxAddress("", email));
                    Console.WriteLine(emailMessage.To.Count);
                }
            }

            emailMessage.Subject = emailRequest.Title;

            try
            {
                // Đọc HTML template
                var assembly = Assembly.GetExecutingAssembly();
                using Stream? stream = assembly.GetManifestResourceStream("HUBT_Social_Email_Service.HTML_Template.Notification_Template.html")
                    ?? throw new FileNotFoundException("Template resource not found");

                var names = assembly.GetManifestResourceNames();
                Console.WriteLine(string.Join("\n", names));

                using StreamReader reader = new(stream);
                emailHtmlContent = reader.ReadToEnd();

                emailHtmlContent = emailHtmlContent
                    .Replace("---title---", emailRequest.Title ?? "")
                    .Replace("---body---", emailRequest.Body ?? "")
                    .Replace("---date---", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading template: {ex.Message}");
                emailHtmlContent = CreateFallbackTemplate(emailRequest);
            }

            // Tạo nội dung email
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = emailHtmlContent,
                TextBody = emailHtmlContent.StripHtmlTags() // Hoặc tạo plain text version
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        // Helper method để tạo fallback template
        private string CreateFallbackTemplate(SendNotificationMailRequest request)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <title>{request.Title}</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 40px; background-color: #f5f5f5; }}
                        .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 8px; margin-bottom: 20px; }}
                        .content {{ line-height: 1.6; color: #333; }}
                        .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; font-size: 12px; color: #666; text-align: center; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>{request.Title ?? "Thông báo"}</h1>
                        </div>
                        <div class='content'>
                            {request.Body ?? "Nội dung thông báo"}
                        </div>
                        <div class='footer'>
                            <p>Ngày gửi: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>
                            <p>© 2025 HUBT Social. Đây là email tự động, vui lòng không trả lời.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

    }
}