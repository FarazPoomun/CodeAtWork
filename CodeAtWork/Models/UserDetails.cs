using CodeAtWork.Models.Misc;
using System.Collections.Generic;

namespace CodeAtWork.Models.Session
{
    public class UserDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class UserDetailsWithId: UserDetails
    {
        public int AppUserId { get; set; }
        public int UserDetailId { get; set; }
    }

    public class FullUserDetail: UserDetailsWithId
    {
        public string DisplayName => $"{FirstName} {LastName}";
        public string Company { get; set; }
        public int YrsOfXP { get; set; }
        public int Title { get; set; }
        public int Role { get; set; }
        public int OrgLevel { get; set; }
    }

    public class JobRole
    {
        static readonly Dictionary<int, string> Roles = new Dictionary<int, string>
            {
                { 1, "Back-end Web Dev" },
                { 2, "Data-Focused Dev" },
                { 3, "Desktop Dev" },
                { 4, "Embedded App Dev" },
                { 5, "Entry Level Services Dev" },
                { 6, "Front-end Dev" },
                { 7, "Full-stack Game Dev" },
                { 8, "Full-stack Web Dev" },
                { 9, "Game Dev" },
                { 10, "Manager / Director / Executive" },
                { 11, "Mobile Dev" },
                { 12, "Quality Assurance / Tester" },
                { 13, "Other" }
            };

        public static string GetJobRoleById(int id)
        {
            return Roles[id];
        }
    }
}