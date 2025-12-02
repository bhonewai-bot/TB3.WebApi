namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SequenceController : ControllerBase
    {
        private readonly ISequenceService _sequenceService;

        public SequenceController(ISequenceService sequenceService)
        {
            _sequenceService = sequenceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSequences()
        {
            var list = await _sequenceService.GetSequencesAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSequence(int id)
        {
            var sequence = await _sequenceService.GetSequenceAsync(id);

            if (sequence is null)
            {
                return NotFound("Sequence not found");
            }

            return Ok(sequence);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSequence(SequenceCreateDto request)
        {
            var sequence = await _sequenceService.CreateSequenceAsync(request);

            if (sequence is null)
            {
                return BadRequest("Sequence for this field already exists or failed to save");
            }

            return Ok(sequence);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateSequence(int id, SequencePatchDto request)
        {
            var sequence = await _sequenceService.UpdateSequenceAsync(id, request);

            if (sequence is null)
            {
                return BadRequest("Sequence not found or field already used by another sequence");
            }

            return Ok(sequence);
        }
    }
}
