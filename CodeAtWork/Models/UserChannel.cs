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
    }
}