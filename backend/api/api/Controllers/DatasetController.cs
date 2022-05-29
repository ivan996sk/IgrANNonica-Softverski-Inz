using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatasetController : ControllerBase
    {
        private readonly IDatasetService _datasetService;
        private readonly IMlConnectionService _mlConnectionService;
        private readonly IFileService _fileService;
        private IJwtToken jwtToken;

        public DatasetController(IDatasetService datasetService, IConfiguration configuration, IJwtToken Token, IMlConnectionService mlConnectionService, IFileService fileService)
        {
            _datasetService = datasetService;
            _mlConnectionService = mlConnectionService;
            _fileService = fileService;
            jwtToken = Token;
        }

        public string getUserId()
        {
            string userId;
            var header = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(header, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;
                userId = jwtToken.TokenToId(parameter);
                if (userId == null)
                    return null;
            }
            else
                return null;

            return userId;
        }

        // GET: api/<DatasetController>/mydatasets
        [HttpGet("mydatasets")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult<List<Dataset>> Get()
        {
            string userId = getUserId();

            if (userId == null)
                return BadRequest();

            if (userId == "")
                return _datasetService.GetGuestDatasets();

            //ako bude trebao ID, samo iz baze uzeti

            return _datasetService.GetMyDatasets(userId);
        }

        // GET: api/<DatasetController>/datesort/{ascdsc}/{latest}
        //asc - rastuce 1
        //desc - opadajuce 0
        //ako se posalje 0 kao latest onda ce da izlista sve u nekom poretku
        [HttpGet("datesort/{ascdsc}/{latest}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult<List<Dataset>> SortDatasets(bool ascdsc, int latest)
        {
            string userId = getUserId();

            if (userId == null)
                return BadRequest();

            List<Dataset> lista = _datasetService.SortDatasets(userId, ascdsc, latest);

            if (latest == 0)
                return lista;
            else
            {
                List<Dataset> novaLista = new List<Dataset>();
                for (int i = 0; i < latest; i++)
                    novaLista.Add(lista[i]);
                return novaLista;
            }
        }

        // GET: api/<DatasetController>/publicdatasets
        [HttpGet("publicdatasets")]
        public ActionResult<List<Dataset>> GetPublicDS()
        {
            return _datasetService.GetPublicDatasets();
        }

        //SEARCH za datasets (public ili private sa ovim imenom )
        // GET api/<DatasetController>/search/{name}
        [HttpGet("search/{name}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult<List<Dataset>> Search(string name)
        {
            return _datasetService.SearchDatasets(name);
        }


        // GET api/<DatasetController>/{name}
        //get odredjeni dataset
        [HttpGet("{name}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult<Dataset> Get(string name)
        {
            string userId = getUserId();

            if (userId == null)
                return BadRequest();

            var dataset = _datasetService.GetOneDatasetN(userId, name);
            if (dataset == null)
                return NotFound($"Dataset with name = {name} not found or dataset is not public or not preprocessed");

            return dataset;
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult<Dataset> GetDatasetById(string id)
        {
            string userId = getUserId();

            if (userId == null)
                return BadRequest();

            var dataset = _datasetService.GetOneDataset(userId, id);
            if (dataset == null)
                return NotFound($"Dataset with id = {id} not found or dataset is not public or not preprocessed");

            return Ok(dataset);
        }

        // POST api/<DatasetController>/add
        [HttpPost("add")]
        [Authorize(Roles = "User,Guest")]
        public async Task<ActionResult<Dataset>> Post([FromBody] Dataset dataset)
        {
            string uploaderId = getUserId();

            dataset.uploaderId = uploaderId;

            //da li ce preko tokena da se ubaci username ili front salje
            //dataset.username = usernameToken;
            //username = "" ako je GUEST DODAO
            var existingDataset = _datasetService.GetOneDatasetN(dataset.uploaderId, dataset.name);

            if (existingDataset != null)
                return NotFound($"Dataset with this name already exists");
            else
            {
                FileModel fileModel = _fileService.getFile(dataset.fileId);
                dataset.isPreProcess = false;
                _datasetService.Create(dataset);
                _mlConnectionService.PreProcess(dataset, fileModel.path, uploaderId);


                return Ok();
            }
        }
        // POST api/<DatasetController>/stealDs
        [HttpPost("stealDs")]
        [Authorize(Roles = "User,Guest")]
        public async Task<ActionResult<Dataset>> StealDs([FromBody] Dataset dataset)
        {
            string uploaderId = getUserId();

            dataset.uploaderId = uploaderId;

            //da li ce preko tokena da se ubaci username ili front salje
            //dataset.username = usernameToken;
            //username = "" ako je GUEST DODAO
            var existingDataset = _datasetService.GetOneDatasetN(dataset.uploaderId, dataset.name);

            if (existingDataset != null)
                return NotFound($"Dataset with this name already exists");
            else
            {
                dataset.dateCreated = DateTime.Now;
                dataset.lastUpdated = DateTime.Now;
                dataset.isPublic = false;
                
                FileModel fileModel = _fileService.getFile(dataset.fileId);

                string folderName = "UploadedFiles";
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName, uploaderId);
                
                string ext = ".csv";

                

                //Check Directory
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                //Index file if same filename
                var fullPath = Path.Combine(folderPath, dataset.name + ext);
                int i = 0;

                while (System.IO.File.Exists(fullPath))
                {
                    i++;
                    fullPath = Path.Combine(folderPath, dataset.name + i.ToString() + ext);
                }

                dataset.fileId = "";

                _fileService.CopyFile(fileModel.path, fullPath);

                
                FileModel fileModel1 = new FileModel();
                fileModel1.type = ext;
                fileModel1.path = fullPath;
                fileModel1.uploaderId = uploaderId;
                fileModel1.date = DateTime.Now.ToUniversalTime();
                fileModel1 = _fileService.Create(fileModel1);

                dataset.fileId = fileModel1._id;


                dataset.isPreProcess = true;
                _datasetService.Create(dataset);
                //_mlConnectionService.PreProcess(dataset, fileModel.path, uploaderId);
                return Ok();
            }
        }

        // PUT api/<DatasetController>/{name}
        [HttpPut("{id}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult Put(string id, [FromBody] Dataset dataset)
        {
            string uploaderId = getUserId();

            if (uploaderId == null)
                return BadRequest();

            var existingDataset = _datasetService.GetOneDataset(uploaderId, id);

            //ne mora da se proverava
            if (existingDataset == null)
                return NotFound($"Dataset with ID = {id} or user with ID = {uploaderId} not found");

            dataset.lastUpdated = DateTime.UtcNow;
            _datasetService.Update(uploaderId, id, dataset);

            if (!dataset.isPreProcess)
            {
                FileModel fileModel = _fileService.getFile(dataset.fileId);
                _mlConnectionService.PreProcess(dataset, fileModel.path, uploaderId);
            }

            return Ok($"Dataset with ID = {id} updated");
        }

        // DELETE api/<DatasetController>/name
        [HttpDelete("{id}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult Delete(string id)
        {
            string uploaderId = getUserId();

            if (uploaderId == null)
                return BadRequest();

            var dataset = _datasetService.GetOneDataset(uploaderId, id);

            if (dataset == null)
                return NotFound($"Dataset with ID = {id} or user with ID = {uploaderId} not found");

            _datasetService.Delete(dataset.uploaderId, dataset._id);

            return Ok($"Dataset with ID = {id} deleted");

        }
    }
}