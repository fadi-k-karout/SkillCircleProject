namespace Application.DTOs;

public class VideoStatusResponse
{
    public Ingest Ingest { get; set; }
    public EncodingStatus Encoding { get; set; }
}

public class Ingest
{
    public string Status { get; set; }
}

public class EncodingStatus
{
    public bool Playable { get; set; }
}