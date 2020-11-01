using CodeAtWork.Models.Misc;
using System;

namespace CodeAtWork.Models
{
    public class VideoRepository
    {
        public Guid VideoId { get; set; }
        public string VideoURL { get; set; }
        public bool IsLocal { get; set; }
        public string VideoAuthor { get; set; }
        public string VideoDescription { get; set; }
        public bool IsBookMarked { get; set; }
        public LevelsEnum Level { get; set; }

        public VideoRepository() {}
    }

    public class VideoWithTime : VideoRepository
    {
        public float SeekTo { get; set; }
    }

    public class VideoWithSequence: VideoRepository
    {
        public int Sequence { get; set; }
    }
}