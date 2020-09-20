using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAtWork.Models
{
    public class UserChannel
    {
        public int UserChannelId { get; set; }
        public string ChannelName { get; set; }
        public int AppUserId { get; set; }
        public bool IsShared { get; set; }
        public bool IsSelectedForVid { get; set; }
    }

    public class UserChannelWithCounts : UserChannel
    {
        public int VideoCount { get; set; }
        public int PathCount { get; set; }
        public string CreatedBy { get; set; }
    }

    public class ChannelVideo
    {
        public int ChannelVideoId { get; set; }
        public Guid VideoId { get; set; }
        public int UserChannelId { get; set; }
    }

}