using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TB3.Database.AppDbContextModels;
using TB3.WebApi.Services.Sequence;

namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SequenceController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SequenceController(AppDbContext db, ISequenceService sequenceService)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetSequences()
        {
            List<SequenceResponseDto> lts = _db.TblSequences
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => new SequenceResponseDto()
                {
                    Id = x.Id,
                    Field = x.Field,
                    Code = x.Code,
                    Length = x.Length,
                    Sequence = x.Sequence
                })
                .ToList();
            
            return Ok(lts);
        }

        [HttpGet("{id}")]
        public IActionResult GetSequence(int id)
        {
            var sequence = _db.TblSequences
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
                .FirstOrDefault();

            if (sequence is null)
            {
                return NotFound("Sequence not found");
            }
            
            return Ok(sequence);
        }

        [HttpPost]
        public IActionResult CreateSequence(SequenceCreateDto request)
        {
            var exists = _db.TblSequences
                .Any(x => x.Field == request.Field);
            
            if (exists)
            {
                return BadRequest("Sequence for this field already exists");
            }
            
            _db.TblSequences.Add(new TblSequence()
            {
                Field = request.Field,
                Code = request.Code,
                Length = request.Length,
                Sequence = 0
            });
            
            int result = _db.SaveChanges();
            string message = result > 0 ? "Saving successful" : "Saving failed";
            
            return Ok(message);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateSequence(int id, SequencePatchDto request)
        {
            var sequence = _db.TblSequences
                .FirstOrDefault(x => x.Id == id);
            
            if (sequence is null)
            {
                return NotFound("Sequence not found");
            }
            
            if (!string.IsNullOrEmpty(request.Field))
            {
                var exists = _db.TblSequences
                    .Any(x => x.Field == request.Field && x.Id != id);

                if (exists)
                {
                    return BadRequest("Another sequence with this field already exists");
                }

                sequence.Field = request.Field;
            }
            if (!string.IsNullOrEmpty(request.Code))
                sequence.Code = request.Code;
            if (request.Length is not null && request.Length > 0)
                sequence.Length = request.Length ?? 0;
            if (request.Sequence is not null && request.Sequence > 0)
                sequence.Sequence = request.Sequence ?? 0;
            
            int result = _db.SaveChanges();
            string message = result > 0 ? "Patching successful" : "Patching failed";
            
            return Ok(message);
        }
    }

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

        public int Sequence { get; set; }
    }
    
    public class SequencePatchDto
    {
        public string? Field { get; set; }

        public string? Code { get; set; }

        public int? Length { get; set; }

        public int? Sequence { get; set; }
    }
}
