using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictorController : Controller
    {
        private readonly IPredictorService _predictorService;
        private IJwtToken jwtToken;
        private readonly IMlConnectionService _mlConnectionService;
        private readonly IExperimentService _experimentService;
        private readonly IUserService _userService;
        private readonly IHubContext<ChatHub> _ichat;
        private readonly IModelService _modelService;

        public PredictorController(IPredictorService predictorService, IConfiguration configuration, IJwtToken Token, IMlConnectionService mlConnectionService, IExperimentService experimentService,IUserService userService, IHubContext<ChatHub> ichat,IModelService modelService)
        {
            _predictorService = predictorService;
            jwtToken = Token;
            _mlConnectionService = mlConnectionService;
            _experimentService = experimentService;
            _userService = userService;
            _ichat = ichat;
            _modelService = modelService;
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

        // GET: api/<PredictorController>/mypredictors
        [HttpGet("mypredictors")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult<List<Predictor>> Get()
        {
            string userId = getUserId();

            if (userId == null)
                return BadRequest();

            return _predictorService.GetMyPredictors(userId);
        }

        // GET: api/<PredictorController>/publicpredictors
        [HttpGet("publicpredictors")]
        public ActionResult<List<Predictor>> GetPublicPredictors()
        {
            return _predictorService.GetPublicPredictors();
        }

        //SEARCH za predictore (public ili private sa ovim imenom )
        // GET api/<PredictorController>/search/{name}

        //[HttpGet("search/{name}")]
        //[Authorize(Roles = "User")]
        //public ActionResult<List<Predictor>> Search(string name)
        //{
        //    string username = getUsername();
            
        //    if (username == null)
        //        return BadRequest();

        //    return _predictorService.SearchPredictors(name, username);
        //}

        // GET api/<PredictorController>/getpredictor/{name}
        [HttpGet("getpredictor/{id}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult<Predictor> GetPredictor(string id)
        {
            string userId = getUserId();

            if (userId == null)
                return BadRequest();

            Predictor predictor = _predictorService.GetPredictor(userId, id);

            if (predictor == null)
                return NotFound($"Predictor with id = {id} not found");

            return predictor;
        }

        // GET api/<PredictorController>/{name}
        [HttpGet("{name}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult<Predictor> Get(string id)
        {
            string userId = getUserId();

            if (userId == null)
                return BadRequest();

            //treba userId da se salje GetOnePredictor
            var predictor = _predictorService.GetOnePredictor(id);

            if (predictor == null)
                return NotFound($"Predictor with id = {id} not found or predictor is not public");

            return predictor;
        }
        // moze da vraca sve modele pa da se ovde odradi orderByDesc
        //odraditi to i u Datasetove i Predictore
        // GET: api/<PredictorController>/datesort/{ascdsc}/{latest}
        //asc - rastuce 1
        //desc - opadajuce 0
        //ako se posalje 0 kao latest onda ce da izlista sve u nekom poretku
        [HttpGet("datesort/{ascdsc}/{latest}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult<List<Predictor>> SortPredictors(bool ascdsc, int latest)
        {
            string userId = getUserId();

            if (userId == null)
                return BadRequest();

            List<Predictor> lista = _predictorService.SortPredictors(userId, ascdsc, latest);

            if(latest == 0)
                return lista;
            else
            {
                List<Predictor> novaLista = new List<Predictor>();

                for (int i = 0; i < latest; i++)
                    novaLista.Add(lista[i]);

                return novaLista;
            }
        }

        // POST api/<PredictorController>/add
        [HttpPost("add")]
        public async Task<ActionResult<Predictor>> Post([FromBody] Predictor predictor)
        {
            var user=_userService.GetUserById(predictor.uploaderId);
            predictor.dateCreated = DateTime.Now.ToUniversalTime();
            var model = _modelService.GetOneModel(predictor.modelId);
            if (model == null || user==null)
                return BadRequest("Model not found or user doesnt exist");
            Predictor p=_predictorService.Exists(predictor.modelId, predictor.experimentId);
            if (p == null)
                _predictorService.Create(predictor);
            else
                _predictorService.Update(p._id, predictor);
            if (ChatHub.CheckUser(user._id))
                foreach(var connection in ChatHub.getAllConnectionsOfUser(user._id))
                    await _ichat.Clients.Client(connection).SendAsync("NotifyPredictor", predictor._id,model.name);
            return CreatedAtAction(nameof(Get), new { id = predictor._id }, predictor);
            
        }

        // POST api/<PredictorController>/usepredictor {predictor,inputs}
        [HttpPost("usepredictor/{id}")]
        [Authorize(Roles = "User,Guest")]
        public async Task<ActionResult> UsePredictor(String id, [FromBody] PredictorColumns[] inputs)
        {
            string userId = getUserId();

            if (userId == null)
                return BadRequest();

            Predictor predictor = _predictorService.GetPredictor(userId, id);

            Experiment e = _experimentService.Get(predictor.experimentId);

            string result = await _mlConnectionService.Predict(predictor, e, inputs);

            //salji ml

            /*
            foreach(PredictorColumns i in inputs)
                Debug.WriteLine(i.value.ToString());*/
            return Ok(result);
        }

        // PUT api/<PredictorController>/{name}
        [HttpPut("{name}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult Put(string id, [FromBody] Predictor predictor)
        {
            string userId = getUserId();

            if (userId == null)
                return BadRequest();

            var existingPredictor = _predictorService.GetOnePredictor(id);

            //ne mora da se proverava
            if (existingPredictor == null)
                return NotFound($"Predictor with id = {id} or user with ID = {userId} not found");

            _predictorService.Update(id, predictor);

            return Ok($"Predictor with id = {id} updated");
        }

        // DELETE api/<PredictorController>/name
        [HttpDelete("{id}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult Delete(string id)
        {
            string userId = getUserId();

            if (userId == null)
                return BadRequest();

            var predictor = _predictorService.GetOnePredictor(id);

            if (predictor == null)
                return NotFound($"Predictor with id = {id} or user with ID = {userId} not found");

            _predictorService.Delete(id, userId);

            return Ok($"Predictor with id = {id} deleted");

        }
    }
}
