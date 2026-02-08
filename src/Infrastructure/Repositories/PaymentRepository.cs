using Application.Common.Interfaces.repos;
using Application.DTOs.Payment;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<Payment> CreatePaymentAsync(RequestPaymentDto paymentDto)
    {

        var course = _context.Courses.FindAsync(paymentDto.CourseId);
        if (course.Result is  null)
            throw new InvalidOperationException($"Course with ID: {paymentDto.CourseId} not found");
        
        var amount = course.Result.Price;
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            CourseId = paymentDto.CourseId,
            UserId = paymentDto.UserId,
            Amount = amount,
            IsPaid = false, 
            CreatedAt = DateTime.UtcNow,
            PaidAt = DateTime.UtcNow 
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return payment;
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByUserIdAsync(Guid userId)
    {
        return await _context.Payments
            .AsNoTracking()
            .Include(p => p.Course) 
            .Where(p => p.UserId == userId)
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                CourseId = p.CourseId,
                UserId = p.UserId,
                Amount = p.Amount,
                IsPaid = p.IsPaid,
                CreatedAt = p.CreatedAt,
                PaidAt = p.PaidAt,
                CourseSlug = p.Course.Slug 
            })
            .ToListAsync();
    }

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }

    public async Task<PaymentDto> UpdatePaymentAsync(Guid paymentId, UpdatePaymentDto paymentDto)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null) return null;

        payment.CourseId = paymentDto.CourseId;
        payment.UserId = paymentDto.UserId;
        payment.Amount = paymentDto.Amount;

        await _context.SaveChangesAsync();

        return new PaymentDto
        {
 
            CourseId = payment.CourseId,
            UserId = payment.UserId,
            Amount = payment.Amount,
            IsPaid = payment.IsPaid,
            CreatedAt = payment.CreatedAt,
            PaidAt = payment.PaidAt,
            CourseSlug = payment.Course.Slug 
        };
    }

    public async Task<bool> DeletePaymentAsync(Guid paymentId)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null) return false;

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<List<Payment>> GetByIds(List<Guid> paymentIds)
    {
        return await _context.Payments
            .Where(p => paymentIds.Contains(p.Id))
            .ToListAsync();
    }
}
