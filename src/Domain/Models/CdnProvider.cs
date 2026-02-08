namespace Domain.Models;

public class CdnProvider
{ 
        public Guid Id { get; set; }
        public string ProviderName { get; set;} 
        public string ResourceType { get; set;} 
        public string ResourceId { get; set;}
        public DateTime CreatedAt { get; set;} = DateTime.UtcNow; 
        
        private readonly List<Video> _videos = new();
        public IReadOnlyCollection<Video> Videos => _videos.AsReadOnly();
}