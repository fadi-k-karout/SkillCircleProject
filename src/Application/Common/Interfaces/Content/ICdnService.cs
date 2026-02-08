namespace Application.Common.Interfaces.Content;

public interface ICdnService
{
    Task<string> GenerateUploadTokenAsync();
    Task<Dictionary<string, bool>> CheckUploadStatusAsync(List<string> resourceIds);
}
