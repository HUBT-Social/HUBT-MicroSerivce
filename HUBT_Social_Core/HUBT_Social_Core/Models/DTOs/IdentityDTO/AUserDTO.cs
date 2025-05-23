﻿using HUBT_Social_Core.Settings.@enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.IdentityDTO
{
    public class AUserDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? AvataUrl { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? FCMToken { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
