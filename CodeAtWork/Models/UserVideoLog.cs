using System;

namespace CodeAtWork.Models
{
    public class UserVideoLog
    {
        public int UserVideoLogId { get; set; }
        public Guid VideoId { get; set; }
        public int AppUserId { get; set; }
        public float LastPlayedDuration { get; set; }
        public DateTime LastModifiedTimestamp { get; set; }
        public bool IsFinished { get; set; }
    }
}