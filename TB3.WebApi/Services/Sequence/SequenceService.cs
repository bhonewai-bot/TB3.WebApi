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

    public async Task<SequenceResponseDto?> CreateSequenceAsync(SequenceCreateDto request)
    {
        var exists = await _db.TblSequences
            .AnyAsync(x => x.Field == request.Field);

        if (exists)
        {
            return null;
        }

        var sequence = new TblSequence
        {
            Field = request.Field,
            Code = request.Code,
            Length = request.Length,
            Sequence = request.Sequence 
        };

        _db.TblSequences.Add(sequence);
        var result = await _db.SaveChangesAsync();

        return new SequenceResponseDto()
        {
            Id = sequence.Id,
            Field = sequence.Field,
            Code = sequence.Code,
            Length = sequence.Length,
            Sequence = sequence.Sequence
        };
    }

    public async Task<SequenceResponseDto?> UpdateSequenceAsync(int id, SequencePatchDto request)
    {
        var sequence = await _db.TblSequences
            .FirstOrDefaultAsync(x => x.Id == id);

        if (sequence is null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(request.Field))
        {
            var exists = await _db.TblSequences
                .AnyAsync(x => x.Field == request.Field && x.Id != id);

            if (exists)
            {
                return null;
            }

            sequence.Field = request.Field;
        }

        if (!string.IsNullOrEmpty(request.Code))
            sequence.Code = request.Code;

        if (request.Length is not null && request.Length > 0)
            sequence.Length = request.Length.Value;

        if (request.Sequence is not null && request.Sequence >= 0)
            sequence.Sequence = request.Sequence.Value;

        await _db.SaveChangesAsync();

        return new SequenceResponseDto()
        {
            Id = sequence.Id,
            Field = sequence.Field,
            Code = sequence.Code,
            Length = sequence.Length,
            Sequence = sequence.Sequence
        };;
    }
    
    public async Task<string> GenerateCode(string field)
    {
        var sequence = await _db.TblSequences
            .FirstOrDefaultAsync(x => x.Field == field);

        if (sequence is null)
        {
            throw new Exception($"Sequence not found for field: {field}");
        }

        sequence.Sequence += 1;
        string generatedCode = $"{sequence.Code}{sequence.Sequence.ToString().PadLeft(sequence.Length, '0')}";

        await _db.SaveChangesAsync();
        
        return generatedCode;
    }
}