namespace TB3.WebApi.Services.Sequence;

public class SequenceService : ISequenceService
{
    private readonly AppDbContext _db;

    public SequenceService(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<Result<List<SequenceResponseDto>>> GetSequences(int pageNo, int pageSize)
    {
        try
        {
            if (pageNo <= 0)
                return Result<List<SequenceResponseDto>>.ValidationError("Page No must be greater than zero");
            
            if (pageSize <= 0)
                return Result<List<SequenceResponseDto>>.ValidationError("Page Size must be greater than zero");
            
            var lts = await _db.TblSequences
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SequenceResponseDto
                {
                    Id = x.Id,
                    Field = x.Field,
                    Code = x.Code,
                    Length = x.Length,
                    Sequence = x.Sequence
                })
                .ToListAsync();

            return Result<List<SequenceResponseDto>>.Success(lts);
        }
        catch (Exception ex)
        {
            return Result<List<SequenceResponseDto>>.SystemError(ex.Message);
        }
    }

    public async Task<Result<SequenceResponseDto>> GetSequence(int id)
    {
        try
        {
            var sequence = await _db.TblSequences
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
            
            if (sequence is null)
                return Result<SequenceResponseDto>.ValidationError("Sequence not found");

            return Result<SequenceResponseDto>.Success(sequence);
        }
        catch (Exception ex)
        {
            return Result<SequenceResponseDto>.SystemError(ex.Message);
        }
    }

    public async Task<Result<SequenceResponseDto>> CreateSequence(SequenceCreateDto request)
    {
        try
        {
            var exists = await _db.TblSequences
                .AnyAsync(x => x.Field == request.Field);

            if (exists)
                return Result<SequenceResponseDto>.ValidationError("Sequence for this field already exists");
            
            if (string.IsNullOrWhiteSpace(request.Field))
                return Result<SequenceResponseDto>.ValidationError("Field is required");
            
            if (string.IsNullOrWhiteSpace(request.Code))
                return Result<SequenceResponseDto>.ValidationError("Code is required");
            
            if (request.Length <= 0)
                return Result<SequenceResponseDto>.ValidationError("Length is required");

            var sequence = new TblSequence
            {
                Field = request.Field,
                Code = request.Code,
                Length = request.Length,
                Sequence = 0 
            };

            _db.TblSequences.Add(sequence);
            await _db.SaveChangesAsync();

            var dto =  new SequenceResponseDto()
            {
                Id = sequence.Id,
                Field = sequence.Field,
                Code = sequence.Code,
                Length = sequence.Length,
                Sequence = sequence.Sequence
            };
            
            return Result<SequenceResponseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<SequenceResponseDto>.SystemError(ex.Message);
        }
    }

    public async Task<Result<SequenceResponseDto>> UpdateSequence(int id, SequencePatchDto request)
    {
        try
        {
            var sequence = await _db.TblSequences
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sequence is null)
                return Result<SequenceResponseDto>.ValidationError("Sequence not found");

            bool isUpdated = false;
            
            if (!string.IsNullOrWhiteSpace(request.Field))
            {
                var exists = await _db.TblSequences
                    .AnyAsync(x => x.Field == request.Field && x.Id != id);

                if (exists)
                    return Result<SequenceResponseDto>.ValidationError("Sequence for this field already exists");

                sequence.Field = request.Field;
                isUpdated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                sequence.Code = request.Code;
                isUpdated = true;
            }

            if (request.Length is not null && request.Length > 0)
            {
                sequence.Length = request.Length.Value;
                isUpdated = true;
            }

            if (request.Sequence is not null && request.Sequence >= 0)
            {
                int max = await _db.TblSequences
                    .Where(x => x.Field == request.Field)
                    .Select(x => x.Length)
                    .DefaultIfEmpty(0)
                    .MaxAsync();
                
                if (request.Sequence.Value < max)
                    return Result<SequenceResponseDto>.ValidationError($"Sequence cannot be less than the current maximum used value ({max})");
                
                sequence.Sequence = request.Sequence.Value;
                isUpdated = true;
            }

            if (!isUpdated)
                return Result<SequenceResponseDto>.ValidationError("Invalid action");

            await _db.SaveChangesAsync();

            var sequenceDto = new SequenceResponseDto()
            {
                Id = sequence.Id,
                Field = sequence.Field,
                Code = sequence.Code,
                Length = sequence.Length,
                Sequence = sequence.Sequence
            };

            return Result<SequenceResponseDto>.Success(sequenceDto);
        }
        catch (Exception ex)
        {
            return Result<SequenceResponseDto>.SystemError(ex.Message);
        }
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