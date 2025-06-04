using HUBT_Social_Core.Models.DTOs.EmailDTO;
using HUBT_Social_Core.Settings;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using Org.BouncyCastle.Asn1.Ocsp;
using HUBT_Social_Email_Service.ASP_Extentions;
using Microsoft.Extensions.Options;


namespace HUBT_Social_Email_Service.Services
{
    internal class EmailPostCodeService(IOptions<SMPTSetting> setting) : IEmailPostCodeService 
    {
        private readonly SMPTSetting _emailSetting = setting.Value;

        public async Task<bool> SendPostCodeAsync(SendPostCodeRequest request) => await CreatePostCodeEmailMessage(request).SendEmailAsync(_emailSetting);
        
        private MimeMessage CreatePostCodeEmailMessage(SendPostCodeRequest emailRequest)
        {

            var emailMessage = new MimeMessage();
            string emailHtmlContent;
            // Người gửi và người nhận
            emailMessage.From.Add(new MailboxAddress("HUBT Social", _emailSetting.Email));
            emailMessage.To.Add(new MailboxAddress(emailRequest.ToEmail, emailRequest.ToEmail));
            emailMessage.Subject = emailRequest.Subject;
            try
            {
                // Đọc HTML template
                var assembly = Assembly.GetExecutingAssembly();
                using Stream? stream = assembly.GetManifestResourceStream("HUBT_Social_Email_Service.HTML_Template.OTPVerify.html") 
                    ?? throw new FileNotFoundException("Template resource not found");
                using StreamReader reader = new(stream);
                emailHtmlContent = reader.ReadToEnd();

                // Thay thế thông tin trong template
                emailHtmlContent = emailHtmlContent
                    .Replace("{{name}}", emailRequest.FullName.Length != 0 ? emailRequest.FullName : emailRequest.ToEmail)
                    .Replace("{{device}}", emailRequest.Device)
                    .Replace("{{location}}", emailRequest.Location)
                    .Replace("{{time}}", emailRequest.DateTime)
                    .Replace("{{text0}}", LocalValue.Get(KeyStore.Email2Text0))
                    .Replace("{{text1}}", LocalValue.Get(KeyStore.Email2Text1))
                    .Replace("{{text2}}", LocalValue.Get(KeyStore.Email2Text2))
                    .Replace("{{text3}}", LocalValue.Get(KeyStore.Email2Text3))
                    .Replace("{{text4}}", LocalValue.Get(KeyStore.Email2Text4))
                    .Replace("{{text5}}", LocalValue.Get(KeyStore.Email2Text5))
                    .Replace("{{text6}}", LocalValue.Get(KeyStore.Email2Text6))
                    .Replace("{{text7}}", LocalValue.Get(KeyStore.Email2Text7))
                    .Replace("{{text8}}", LocalValue.Get(KeyStore.Email2Text8))
                    .Replace("{{text9}}", LocalValue.Get(KeyStore.Email2Text9))
                    .Replace("{{text10}}", LocalValue.Get(KeyStore.Email2Text10))
                    .Replace("{{text11}}", LocalValue.Get(KeyStore.Email2Text11))
                    .Replace("{{text12}}", LocalValue.Get(KeyStore.Email2Text12))
                    .Replace("{{text13}}", LocalValue.Get(KeyStore.Email2Text13));
            }
            catch
            {
                emailHtmlContent = LocalValue.Get(KeyStore.EmailTemplate);

                emailHtmlContent = emailHtmlContent
                .Replace("{{RecipientName}}", emailRequest.FullName.Length != 0 ? emailRequest.FullName : emailRequest.ToEmail)
                .Replace("{{content-top0}}", LocalValue.Get(KeyStore.EmailContentOTP0))
                .Replace("{{content-top1}}", LocalValue.Get(KeyStore.EmailContentOTP1))
                .Replace("{{content-top2}}", LocalValue.Get(KeyStore.EmailContentOTP2))
                .Replace("{{content-bottom2}}", LocalValue.Get(KeyStore.EmailContentOTP4))
                .Replace("{{content-bottom3}}", LocalValue.Get(KeyStore.EmailContentOTP5))
                .Replace("{{content-bottom4}}", LocalValue.Get(KeyStore.EmailContentOTP6))
                .Replace("{{footer1}}", LocalValue.Get(KeyStore.EmailContentFooter1))
                .Replace("{{footer2}}", LocalValue.Get(KeyStore.EmailContentFooter2))
                .Replace("{{footer3}}", LocalValue.Get(KeyStore.EmailContentFooter3))
                .Replace("{{footer4}}", LocalValue.Get(KeyStore.EmailContentFooter4));
            }


            for (int i = 0; i < 6; i++)
            {
                string placeholder = $"{{{{value-{i}}}}}";

                string value = emailRequest.Code[i].ToString();
                emailHtmlContent = emailHtmlContent.Replace(placeholder, value);
            }

            // Tạo body của email
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = emailHtmlContent,
                TextBody = $"Hi {emailRequest.ToEmail},\n\nYour OTP is {string.Join("", emailRequest.Code)}.\n\nBest Regards,\nHUBT Social Team"
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }



    }
}
