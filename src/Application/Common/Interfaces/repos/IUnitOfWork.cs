namespace Application.Common.Interfaces.repos;

public interface IUnitOfWork : IDisposable
{
    ICourseRepository Courses { get; }
    IVideoRepository Videos { get; }
    ISkillRepository Skills { get; }
    IReviewRepository Reviews { get; }


    Task<int> CommitAsync();
}