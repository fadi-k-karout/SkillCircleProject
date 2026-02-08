namespace Application.DTOs.Payment;

public class RequestPaymentDto
{
    public Guid CourseId { get; set; }
    public Guid UserId { get; set; }
    
}
