using System.Net.Http.Headers;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;


namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private string[] permittedExtensions = { ".csv",".json",".xls",".xlsx" };
        private string[] permittedExtensionsH5 = { ".h5" };//niz da bi dodali h5 itd
        private readonly IConfiguration _configuration;
        private IJwtToken _token;
        private readonly IFileService _fileservice;

        public FileController(IConfiguration configuration,IFileService fileService,IJwtToken token)
        {
            _configuration = configuration;
            _token = token;
            _fileservice = fileService;
        }

        public string getUserId()
        {
            string uploaderId;
            var header = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(header, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                var parameter = headerValue.Parameter;
                uploaderId = _token.TokenToId(parameter);
                if (uploaderId == null)
                    return null;
            }
            else
                return null;

            return uploaderId;
        }

        [HttpPost("h5")]
        public async Task<ActionResult<string>> H5Upload([FromForm] IFormFile file,[FromForm] string uploaderId)
        {

            //get username from jwtToken
            
            string folderName="PredictorFiles";



            //Check filetype
            var filename = file.FileName;
            var ext = Path.GetExtension(filename).ToLowerInvariant();
            var name = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensionsH5.Contains(ext))
            {
                return BadRequest("Wrong file type");
            }
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName,uploaderId);
            //Check Directory
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            //Index file if same filename
            var fullPath = Path.Combine(folderPath, filename);
            int i = 0;

            while (System.IO.File.Exists(fullPath))
            {
                i++;
                fullPath = Path.Combine(folderPath, name + i.ToString() + ext);
            }


            //Write file
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            FileModel fileModel = new FileModel();
            fileModel.type = ".h5";
            fileModel.path = fullPath;
            fileModel.uploaderId = uploaderId;
            fileModel.date = DateTime.Now.ToUniversalTime();
            fileModel = _fileservice.Create(fileModel);


            return Ok(fileModel._id);
        }

        [HttpGet("csvread/{fileId}/{skipRows}/{takeRows}")]
        [Authorize(Roles = "User,Guest")]
        public ActionResult<string> CsvRead(string fileId, int skipRows = 1, int takeRows = 11)
        {

            string uploaderId = getUserId();

            if (uploaderId == null)
                return BadRequest();

            //String csvContent = System.IO.File.ReadAllText(fileModel.path);
            string filePath = _fileservice.GetFilePath(fileId, uploaderId);



            return String.Join("\n", System.IO.File.ReadLines(filePath).Skip(skipRows+1).Take(takeRows));
        }


        [HttpPost("Csv")]
        [Authorize(Roles = "User,Guest")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<string>> CsvUpload([FromForm]IFormFile file)
        {

            //get username from jwtToken
            
            string folderName;

            string uploaderId = getUserId();

            if (uploaderId == null)
                return BadRequest();

            if (uploaderId == "")
            {
                folderName = "TempFiles";
            }
            else
            {
                folderName = "UploadedFiles";
            }
            

            //Check filetype
            var filename=file.FileName;
            var ext=Path.GetExtension(filename).ToLowerInvariant();
            var name = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || ! permittedExtensions.Contains(ext)) {
                return BadRequest("Wrong file type");
            }
            var folderPath=Path.Combine(Directory.GetCurrentDirectory(),folderName, uploaderId);
            //Check Directory
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            //Index file if same filename
            var fullPath = Path.Combine(folderPath, filename);
            int i=0;

            while (System.IO.File.Exists(fullPath)) {
                i++;
                fullPath = Path.Combine(folderPath,name+i.ToString()+ext);
            }


            //Write file
            using (var stream=new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            FileModel fileModel= new FileModel();
            fileModel.type = ext;
            fileModel.path=fullPath;
            fileModel.uploaderId= uploaderId;
            fileModel.date = DateTime.Now.ToUniversalTime();
            fileModel =_fileservice.Create(fileModel);

            
            /*
            using (var streamReader = new StreamReader(fullPath))
            {
                using(var csvReader=new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    var records = csvReader.GetRecords<dynamic>().ToList();
                }
            }
            */

                return Ok(fileModel);
        }


        //msm generalno moze da se koristi Download samo
        [HttpGet("downloadh5")]
        [Authorize(Roles = "User,Guest")]
        public async Task<ActionResult> DownloadH5(string id)
        {
            //Get Username
            string uploaderId = getUserId();

            if (uploaderId == null)
                return BadRequest();

            string filePath = _fileservice.GetFilePath(id, uploaderId);
            if (filePath == null)
                return BadRequest();

            return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", Path.GetFileName(filePath));

        }

        [HttpGet("Download")]
        [Authorize(Roles = "User,Guest")]
        public async Task<ActionResult> DownloadFile(string id)
        {
            //Get Username
            string uploaderId = getUserId();

            if (uploaderId == null)
                return BadRequest();

            string filePath = _fileservice.GetFilePath(id, uploaderId);
            if (filePath == null)
                return BadRequest();

            return File(System.IO.File.ReadAllBytes(filePath),"application/octet-stream", Path.GetFileName(filePath));

        }

    }
}
