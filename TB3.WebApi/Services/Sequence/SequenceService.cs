using TB3.Database.AppDbContextModels;

namespace TB3.WebApi.Services.Sequence;

public class SequenceService : ISequenceService
{
    private readonly AppDbContext _db;

    public SequenceService(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<List<SequenceResponseDto>> GetSequencesAsync()
    {
        var list = await _db.TblSequences
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Select(x => new SequenceResponseDto
            {
                Id = x.Id,
                Field = x.Field,
                Code = x.Code,
                Length = x.Length,
                Sequence = x.Sequence
            })
            .ToListAsync();

        return list;
    }

    public async Task<SequenceResponseDto?> GetSequenceAsync(int id)
    {
        var seq = await _db.TblSequences
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new SequenceResponseDto
            {
                Id = x.Id,
                Field = x.Field,
                Code = x.Code,
                Length = x.Length,
                Sequence = x.Sequence
            })
            .FirstOrDefaultAsync();

        return seq;
    }

    public async Task<bool> CreateSequenceAsync(SequenceCreateDto request)
    {
        var exists = await _db.TblSequences
            .AnyAsync(x => x.Field == request.Field);

        if (exists)
        {
            return false;
        }

        var entity = new TblSequence
        {
            Field = request.Field,
            Code = request.Code,
            Length = request.Length,
            Sequence = 0 // ignore request.Sequence, start from 0
        };

        _db.TblSequences.Add(entity);
        var result = await _db.SaveChangesAsync();

        return result > 0;
    }

    public async Task<bool> UpdateSequenceAsync(int id, SequencePatchDto request)
    {
        var sequence = await _db.TblSequences
            .FirstOrDefaultAsync(x => x.Id == id);

        if (sequence is null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(request.Field))
        {
            var exists = await _db.TblSequences
                .AnyAsync(x => x.Field == request.Field && x.Id != id);

            if (exists)
            {
                // let controller handle message
                return false;
            }

            sequence.Field = request.Field;
        }

        if (!string.IsNullOrEmpty(request.Code))
            sequence.Code = request.Code;

        if (request.Length is not null && request.Length > 0)
            sequence.Length = request.Length.Value;

        if (request.Sequence is not null && request.Sequence >= 0)
            sequence.Sequence = request.Sequence.Value;

        var result = await _db.SaveChangesAsync();

        return result > 0;
    }
    
    public string GenerateCode(string field)
    {
        var sequence = _db.TblSequences.FirstOrDefault(x => x.Field == field);

        if (sequence is null)
        {
            throw new Exception($"Sequence not found for field: {field}");
        }

        sequence.Sequence += 1;

        string generatedCode = $"{sequence.Code}{sequence.Sequence.ToString().PadLeft(sequence.Length, '0')}";

        _db.SaveChanges();
        
        return generatedCode;
    }
}