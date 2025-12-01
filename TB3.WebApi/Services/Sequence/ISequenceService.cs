namespace TB3.WebApi.Services.Sequence;

public interface ISequenceService
{
    Task<List<SequenceResponseDto>> GetSequencesAsync();
    Task<SequenceResponseDto?> GetSequenceAsync(int id);
    Task<bool> CreateSequenceAsync(SequenceCreateDto request);
    Task<bool> UpdateSequenceAsync(int id, SequencePatchDto request);
    
    string GenerateCode(string field);
}