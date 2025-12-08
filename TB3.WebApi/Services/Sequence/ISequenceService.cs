namespace TB3.WebApi.Services.Sequence;

public interface ISequenceService
{
    Task<Result<List<SequenceResponseDto>>> GetSequences(int pageNo, int pageSize);
    Task<Result<SequenceResponseDto>> GetSequence(int id);
    Task<Result<SequenceResponseDto>> CreateSequence(SequenceCreateDto request);
    Task<Result<SequenceResponseDto>> UpdateSequence(int id, SequencePatchDto request);
    
    Task<string> GenerateCode(string field);
}