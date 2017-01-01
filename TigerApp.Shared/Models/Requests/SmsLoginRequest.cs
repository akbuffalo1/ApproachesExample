using System;
using System.ComponentModel;

namespace TigerApp.Shared.Models.Requests
{
    public class SmsLoginRequest
    {
        public string phone_number { get; set; }
        public string verify_code { get; set; }
    }
}