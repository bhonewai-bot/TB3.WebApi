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
        public async Task<IActionResult> GetSequences(int pageNo = 1, int pageSize = 10)
        {
            var result = await _sequenceService.GetSequences(pageNo, pageSize);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSequence(int id)
        {
            var result = await _sequenceService.GetSequence(id);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSequence(SequenceCreateDto request)
        {
            var result = await _sequenceService.CreateSequence(request);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);

            return Ok(result.Data);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateSequence(int id, SequencePatchDto request)
        {
            var result = await _sequenceService.UpdateSequence(id, request);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);

            return Ok(result.Data);
        }
    }
}
