using HUBT_Social_Core.Models.DTOs.EmailDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Email_Service.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailRequest request);

    }
}
