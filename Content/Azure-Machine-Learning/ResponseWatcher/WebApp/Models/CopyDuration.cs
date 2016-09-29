using System;

namespace WebApp.Models
{
    public class CopyDuration
    {
        public long UtcUnixTimeSeconds { get; set; }

        public long FileSize { get; set; }

        public long DurationMs { get; set; }

        public CopyDuration()
        {
            UtcUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}