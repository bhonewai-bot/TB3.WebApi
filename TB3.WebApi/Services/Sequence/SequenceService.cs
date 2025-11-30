using TB3.Database.AppDbContextModels;

namespace TB3.WebApi.Services.Sequence;

public class SequenceService : ISequenceService
{
    private readonly AppDbContext _db;

    public SequenceService(AppDbContext db)
    {
        _db = db;
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