public class PasswordResetEmailModel
{
    public string UserName { get; set; }
    public string ResetLink { get; set; }
    public string Subject { get; set; } = "Password Reset Request";
}