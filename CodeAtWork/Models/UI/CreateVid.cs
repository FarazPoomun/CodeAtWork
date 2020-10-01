using System;
using System.Web;

namespace CodeAtWork.Models.UI
{
    public class CreateVid
    {
        public string VideoURL { get; set; }
        public Boolean IsLocal { get; set; }
        public string VideoAuthor { get; set; }
        public string VideoDescription { get; set; }
        public string RelatedTopicIds { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }

    }
}