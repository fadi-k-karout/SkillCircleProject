using Application.DTOs.Payment;
using Domain.Models;

namespace Application.Common.Interfaces.repos;

public interface IPaymentRepository
{
    Task<int> SaveChangesAsync();
    Task<Payment> CreatePaymentAsync(RequestPaymentDto paymentDto);
    Task<IEnumerable<PaymentDto>> GetPaymentsByUserIdAsync(Guid userId);
    Task<PaymentDto> UpdatePaymentAsync(Guid paymentId, UpdatePaymentDto paymentDto);
    Task UpdateAsync(Payment payment);
    Task<bool> DeletePaymentAsync(Guid paymentId);
    Task<List<Payment>> GetByIds(List<Guid> paymentIds);
}
