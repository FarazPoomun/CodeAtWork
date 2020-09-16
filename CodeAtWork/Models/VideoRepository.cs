using System;

namespace CodeAtWork.Models
{
    public class VideoRepository
    {
        public Guid VideoId { get; set; }
        public string VideoURL { get; set; }
        public Boolean IsLocal { get; set; }
        public string VideoAuthor { get; set; }
        public string VideoDescription { get; set; }
        public bool IsBookMarked { get; set; }


    }
}