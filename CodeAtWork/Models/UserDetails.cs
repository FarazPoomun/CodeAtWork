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
}