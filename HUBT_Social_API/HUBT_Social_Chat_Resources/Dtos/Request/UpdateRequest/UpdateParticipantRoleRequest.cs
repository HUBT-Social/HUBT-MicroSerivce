using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest
{
    public class UpdateParticipantRoleRequest
    {
        public string groupId {get;set;} = string.Empty;
        public string changed {get;set;} = string.Empty;
        public ParticipantRole participantRole {get;set;}
    }
}
