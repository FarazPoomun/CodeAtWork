using CodeAtWork.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAtWork.Common
{
    //We Don't really need extension methods, but it will be fun to use
    public static class CodeAtWorkExtensions
    {
        public static HtmlString ConvertVidToHtmlString (this List<VideoRepository> vids)
        {
            string resultStr = "";
            vids.ForEach(v =>
            {
                resultStr += $@"<iframe id='player_iframe' class='vidFrame'
                src = '{vids.First().VideoURL}' ></iframe>";
            });

            return new HtmlString(resultStr);
        }

    }
}