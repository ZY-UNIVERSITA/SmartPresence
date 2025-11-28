namespace SmartPresence.Services.Users.Queries
{
    public class UserIdentificationRequest
    {
        public string Email { get; set; }

        public UserIdentificationRequest(string email)
        {
            this.Email = email;
        }
    }
}
