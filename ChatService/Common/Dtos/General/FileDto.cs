﻿namespace ChatService.Common.Dtos.General
{
    public class FileDto
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[]? Bytes { get; set; }
    }
}
