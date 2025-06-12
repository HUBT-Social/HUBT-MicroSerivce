using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.Requests.Firebase
{
    public class NotificationRecipients
    {
        public List<string> FcmTokens { get; set; } = [];
        public List<string> PhoneNumbers { get; set; } = [];
        public List<string> Emails { get; set; } = [];
        public bool Any() => FcmTokens.Count != 0 || PhoneNumbers.Count != 0 || Emails.Count != 0;
        public int Count =>  Math.Max(FcmTokens.Count, Math.Max(PhoneNumbers.Count, Emails.Count));

    }

    public class RecipientFilterRequest
    {
        public bool IncludeEmails { get; set; }
        public bool IncludePhoneNumbers { get; set; }
        public bool IncludeFcmTokens { get; set; }
    }
}
