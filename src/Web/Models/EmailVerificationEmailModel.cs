public class EmailVerificationEmailModel
{
    public string UserName { get; set; }
    public string VerificationLink { get; set; }
    public string Subject { get; set; } = "Email Verification";
}