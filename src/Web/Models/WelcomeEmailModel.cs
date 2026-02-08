public class WelcomeEmailModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Subject { get; set; } = "Welcome to Our Platform!";
    public string ContactEmail { get; set; } = "support@platform.com"; // Example email
}