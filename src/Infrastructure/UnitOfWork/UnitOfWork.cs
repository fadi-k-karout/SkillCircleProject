using Application.Common.Interfaces.repos;
using Infrastructure.Data;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private bool _disposed;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Courses = new CourseRepository(_context);
        Videos = new VideoRepository(_context);
        Skills = new SkillRepository(_context);
        Reviews = new ReviewRepository(_context);
        
    }

    public ICourseRepository Courses { get; }
    public IVideoRepository Videos { get; }
    public ISkillRepository Skills { get; }
    public IReviewRepository Reviews { get; }


    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _context.Dispose();
            }

            // Dispose unmanaged resources if any

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // Prevents finalization
    }
}
