﻿namespace ChatService.AppSettingsOptions
{
    public class SecurityOptions
    {
        public string PublicKeyFilePath { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
