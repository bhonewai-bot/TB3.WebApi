namespace TB3.WebApi.Services.Sequence;

public interface ISequenceService
{
    Task<List<SequenceResponseDto>> GetSequencesAsync();
    Task<SequenceResponseDto?> GetSequenceAsync(int id);
    Task<SequenceResponseDto?> CreateSequenceAsync(SequenceCreateDto request);
    Task<SequenceResponseDto?> UpdateSequenceAsync(int id, SequencePatchDto request);
    
    Task<string> GenerateCode(string field);
}