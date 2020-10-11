using CodeAtWork.Models.Misc;
using System.Collections.Generic;

namespace CodeAtWork.Models
{
    public class Path
    {
        public int PathId { get; set; }
        public string Name { get; set; }
        public LevelsEnum Level { get; set; }
    }


    public class PathDetail : Path
    {
        public string Description { get; set; }
        public List<string> Prerequisites = new List<string>();
        public List<string> Outcomes = new List<string>();
    }
}