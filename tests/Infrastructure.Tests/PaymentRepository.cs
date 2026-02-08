using Application.DTOs.Payment;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repositories;

namespace Infrastructure.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class PaymentRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PaymentRepository _repository;
    
    private Guid _courseId = Guid.NewGuid();
    private Guid _userId = Guid.NewGuid();

    public PaymentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for each test
            .Options;

        _context = new ApplicationDbContext(options);

        _context.Users.Add(new User
        {
            Id = _userId,
            FirstName = "John",
            IsActive = true

        });

        // Seed some data for testing if necessary
        _context.Courses.Add(new Course
        {
            Id = _courseId,
            Title = "Test Course",
            Slug = "test-course",
            Price = 100,
            IsPrivate = false,
            IsPaid = true,
            CreatorId = _userId,
        });
        _context.SaveChanges();

        _repository = new PaymentRepository(_context);
    }

    [Fact]
    public async Task CreatePaymentAsync_ShouldCreatePayment_WhenCourseExists()
    {
        // Arrange
        var course = await _context.Courses.FirstAsync();
        var paymentDto = new RequestPaymentDto
        {
            CourseId = course.Id,
            UserId = Guid.NewGuid()
        };

        // Act
        var payment = await _repository.CreatePaymentAsync(paymentDto);

        // Assert
        Assert.NotNull(payment);
        Assert.Equal(course.Id, payment.CourseId);
        Assert.Equal(course.Price, payment.Amount);
        Assert.False(payment.IsPaid);
    }

    [Fact]
    public async Task CreatePaymentAsync_ShouldThrowException_WhenCourseDoesNotExist()
    {
        // Arrange
        var paymentDto = new RequestPaymentDto
        {
            CourseId = Guid.NewGuid(), // Non-existing CourseId
            UserId = Guid.NewGuid()
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.CreatePaymentAsync(paymentDto));
    }

    [Fact]
    public async Task GetPaymentsByUserIdAsync_ShouldReturnPayments_WhenPaymentsExistForUser()
    {
        // Arrange
        var userId = _userId;
        var course = await _context.Courses.FirstAsync();
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            CourseId = course.Id,
            UserId = userId,
            Amount = course.Price,
            IsPaid = true,
            CreatedAt = DateTime.UtcNow,
            PaidAt = DateTime.UtcNow
        };
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Act
        var payments = await _repository.GetPaymentsByUserIdAsync(userId);

        // Assert
        Assert.Single(payments);
        Assert.Equal(userId, payments.First().UserId);
        Assert.Equal(course.Slug, payments.First().CourseSlug);
    }

    [Fact]
    public async Task UpdatePaymentAsync_ShouldUpdatePayment_WhenPaymentExists()
    {
        // Arrange
        var course = await _context.Courses.FirstAsync();
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            CourseId = course.Id,
            UserId = Guid.NewGuid(),
            Amount = course.Price,
            IsPaid = false,
            CreatedAt = DateTime.UtcNow
        };
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        var updateDto = new UpdatePaymentDto
        {
            CourseId = payment.CourseId,
            UserId = payment.UserId,
            Amount = 200 // New amount
        };

        // Act
        var updatedPayment = await _repository.UpdatePaymentAsync(payment.Id, updateDto);

        // Assert
        Assert.NotNull(updatedPayment);
        Assert.Equal(200, updatedPayment.Amount);
    }

    [Fact]
    public async Task UpdatePaymentAsync_ShouldReturnNull_WhenPaymentDoesNotExist()
    {
        // Arrange
        var updateDto = new UpdatePaymentDto
        {
            CourseId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Amount = 150
        };

        // Act
        var result = await _repository.UpdatePaymentAsync(Guid.NewGuid(), updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeletePaymentAsync_ShouldDeletePayment_WhenPaymentExists()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Amount = 100,
            IsPaid = true,
            CreatedAt = DateTime.UtcNow,
            PaidAt = DateTime.UtcNow
        };
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeletePaymentAsync(payment.Id);

        // Assert
        Assert.True(result);
        Assert.Null(await _context.Payments.FindAsync(payment.Id));
    }

    [Fact]
    public async Task DeletePaymentAsync_ShouldReturnFalse_WhenPaymentDoesNotExist()
    {
        // Act
        var result = await _repository.DeletePaymentAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
