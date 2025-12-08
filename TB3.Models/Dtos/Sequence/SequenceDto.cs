namespace TB3.Models.Dtos.Sequence;

public class SequenceResponseDto
{
    public int Id { get; set; }

    public string Field { get; set; }

    public string Code { get; set; }

    public int Length { get; set; }

    public int Sequence { get; set; }
}

public class SequenceCreateDto
{
    public string Field { get; set; }

    public string Code { get; set; }

    public int Length { get; set; }
}
    
public class SequencePatchDto
{
    public string? Field { get; set; }

    public string? Code { get; set; }

    public int? Length { get; set; }

    public int? Sequence { get; set; }
}