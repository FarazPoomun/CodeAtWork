using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAtWork.Models
{
    public class SubscribedChannelUser
    {
        public int ChannelSubscribedUserId { get; set; }
        public int UserChannelId { get; set; }
        public string Email { get; set; }

    }
}