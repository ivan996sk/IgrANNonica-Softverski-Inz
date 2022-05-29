using System.Net.Http.Headers;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperimentController : ControllerBase
    {

        private readonly IExperimentService _experimentService;
        private IJwtToken jwtToken;
        private readonly IMlConnectionService _mlConnectionService;
        private readonly IFileService _fileService;

        public ExperimentController(IExperimentService experimentService, IConfiguration configuration, IJwtToken Token, IMlConnectionService mlConnectionService, IFileService fileService)
        {
            _experimentService = experimentService;
            jwtToken = Token;
            _mlConnectionService = mlConnectionService;
            _fileService = fileService;
        }
        public string getUserId()
        {
            string uploaderId;
            var header = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(header, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;
                uploaderId = jwtToken.TokenToId(parameter);
                if (uploaderId == null)
                    return null;
            }
            else
                return null;

            return uploaderId;
        }

        [HttpPost("add")]
        [Authorize(Roles = "User,Guest")]
        public async Task<ActionResult<Experiment>> Post([FromBody] Experiment experiment)
        {
            string uploaderId = getUserId();

            if (uploaderId == null)
                return BadRequest();

            experiment.uploaderId = uploaderId;

            var existingExperiment = _experimentService.Get(uploaderId, experiment.name);
            if(existingExperiment != null)
                return NotFound($"Experiment with this name exists");
            _experimentService.Create(experiment);
            return Ok(experiment);
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = "User,Guest")]
        public async Task<ActionResult<Experiment>> Get(string id)
        {
            string uploaderId = getUserId();

            if (uploaderId == null)
                return BadRequest();

            var experiment = _experimentService.Get(id);
            if(experiment.uploaderId!=uploaderId)
                return BadRequest("Not your experiment");

            return Ok(experiment);
        }

        [HttpGet("getMyExperiments")]
        [Authorize(Roles = "User,Guest")]
        public async Task<ActionResult<List<Experiment>>> getMyExperiments()
        {
            string uploaderId = getUserId();

            if (uploaderId == null)
                return BadRequest();

            var experiments=_experimentService.GetMyExperiments(uploaderId);
            return Ok(experiments);
        }

        // PUT api/<ExperimentController>/{name}
        [HttpPut("{id}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult Put(string id, [FromBody] Experiment experiment)
        {
            string uploaderId = getUserId();

            if (uploaderId == null)
                return BadRequest();

            var existingExperiment = _experimentService.GetOneExperiment(uploaderId, id);

            //ne mora da se proverava
            if (existingExperiment == null)
                return NotFound($"Experiment with ID = {id} or user with ID = {uploaderId} not found");

            experiment.lastUpdated = DateTime.UtcNow;


            return Ok(_experimentService.Update(uploaderId, id, experiment));
        }

        // DELETE api/<ExperimentController>/name
        [HttpDelete("{id}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult Delete(string id)
        {
            string uploaderId = getUserId();

            if (uploaderId == null)
                return BadRequest();

            var experiment = _experimentService.GetOneExperiment(uploaderId, id);

            if (experiment == null)
                return NotFound($"Experiment with ID = {id} or user with ID = {uploaderId} not found");

            _experimentService.Delete(experiment.uploaderId, experiment._id);

            return Ok($"Experiment with ID = {id} deleted");

        }

        public void DeleteHelper(string uploaderId, string experimentId)
        {
            _experimentService.Delete(uploaderId, experimentId);
        }

    }
}
