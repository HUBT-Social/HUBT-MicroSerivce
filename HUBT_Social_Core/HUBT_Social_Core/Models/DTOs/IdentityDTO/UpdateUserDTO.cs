﻿using HUBT_Social_Core.Settings.@enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.IdentityDTO
{
    public class UpdateUserDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool? EnableTwoFactor { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
