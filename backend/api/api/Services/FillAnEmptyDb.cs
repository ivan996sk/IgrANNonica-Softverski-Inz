using api.Interfaces;
using api.Models;
using MongoDB.Driver;

namespace api.Services
{
    public class FillAnEmptyDb : IHostedService
    {
        private readonly IFileService _fileService;
        private readonly IDatasetService _datasetService;
        private readonly IModelService _modelService;
        private readonly IExperimentService _experimentService;
        private readonly IPredictorService _predictorService;


        public FillAnEmptyDb(IUserStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);

            _fileService = new FileService(settings, mongoClient);
            _experimentService = new ExperimentService(settings, mongoClient);
            _datasetService = new DatasetService(settings, mongoClient, _experimentService);
            _modelService = new ModelService(settings, mongoClient);
            _predictorService = new PredictorService(settings, mongoClient);
        }



        //public void AddToEmptyDb()
        public Task StartAsync(CancellationToken cancellationToken)
        {


            if (_fileService.CheckDb())
            {

                FileModel file = new FileModel();

                string folderName = "UploadedFiles";
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName, "000000000000000000000000");
                var fullPath = Path.Combine(folderPath, "titanic.csv");

                file._id = "";
                file.type = ".csv";
                file.uploaderId = "000000000000000000000000";
                file.path = fullPath;
                file.date = DateTime.Now;

                _fileService.Create(file);



                Dataset dataset = new Dataset();

                dataset._id = "";
                dataset.uploaderId = "000000000000000000000000";
                dataset.name = "Titanik dataset (public)";
                dataset.description = "Titanik dataset";
                dataset.fileId = _fileService.GetFileId(fullPath);
                dataset.extension = ".csv";
                dataset.isPublic = true;
                dataset.accessibleByLink = true;
                dataset.dateCreated = DateTime.UtcNow;
                dataset.lastUpdated = DateTime.UtcNow;
                dataset.delimiter = ",";
                dataset.columnInfo = new[]
                {

                    new ColumnInfo( "PassengerId", true, 0, 446, 1, 891, 446, new string[]{ "1","599","588", "589", "590", "591" }, new int[] { 1, 1, 1, 1, 1, 1}, new float[] { 0.0011223345063626766f, 0.0011223345063626766f, 0.0011223345063626766f, 0.0011223345063626766f, 0.0011223345063626766f, 0.0011223345063626766f }, 223.5f, 668.5f ),
                    new ColumnInfo( "Survived", true, 0, 0.38383838534355164f, 0, 1, 0, new string[]{ "0", "1" }, new int[] { 549, 342}, new float[] { 0.6161616444587708f, 0.38383838534355164f}, 0f, 1f ),
                    new ColumnInfo( "Pclass", true, 0, 2.3086419105529785f, 1, 3, 3, new string[]{ "3", "1", "2" }, new int[] {491, 216, 184}, new float[] {0.5510662198066711f, 0.24242424964904785f, 0.2065095454454422f }, 2f, 3f ),
                    new ColumnInfo( "Name", false, 0, 0, 0, 0, 0, new string[]{"Braund, Mr. Owen Harris", "Boulos, Mr. Hanna", "Frolicher-Stehli, Mr. Maxmillian", "Gilinski, Mr. Eliezer", "Murdlin, Mr. Joseph", "Rintamaki, Mr. Matti"}, new int[] {1,1,1,1,1,1}, new float[] {0.0011223345063626766f, 0.0011223345063626766f, 0.0011223345063626766f, 0.0011223345063626766f, 0.0011223345063626766f, 0.0011223345063626766f }, 0f, 0f ),
                    new ColumnInfo( "Sex", false, 0, 0, 0, 0, 0, new string[]{ "male", "female" }, new int[] {577,314}, new float[] {0.6475870013237f, 0.35241302847862244f}, 0f,0f ),
                    new ColumnInfo( "Age", true, 177, 29.69911766052246f, 0.41999998688697815f, 80, 28, new string[]{ "nan", "24.0", "22.0", "18.0", "28.0", "30.0" }, new int[] {177,30,27,26,25,25}, new float[] {0.1986531913280487f,0.033670034259557724f,0.03030303120613098f,0.029180696234107018f,0.028058361262083054f,0.028058361262083054f}, 20.125f,38f ),
                    new ColumnInfo( "SibSp", true, 0, 0.523007869720459f, 0, 8, 0, new string[]{ "0", "1", "2", "3", "4", "3", "8" }, new int[] {608, 209, 28, 18, 16, 7}, new float[] {0.6823793649673462f, 0.23456789553165436f, 0.031425364315509796f, 0.020202020183205605f, 0.017957352101802826f, 0.007856341078877449f }, 0f, 1f ),
                    new ColumnInfo( "Parch", true, 0, 0.3815937042236328f, 0, 6, 0, new string[]{ "0", "1", "2", "5", "3", "4" }, new int[] { 678, 118, 80, 5, 5, 4}, new float[] {0.7609427571296692f, 0.13243547081947327f, 0.08978675305843353f, 0.005611672066152096f, 0.005611672066152096f, 0.0044893380254507065f }, 0f,0f ),
                    new ColumnInfo( "Ticket", false, 0, 0, 0, 0, 0, new string[]{ "347082", "CA. 2343", "1601", "3101295", "CA 2144", "347088" }, new int[] {7, 7, 7, 6, 6, 6}, new float[] {0.007856341078877449f, 0.007856341078877449f, 0.007856341078877449f, 0.006734006572514772f, 0.006734006572514772f, 0.006734006572514772f}, 0f,0f ),
                    new ColumnInfo( "Fare", true, 0, 32.20420837402344f, 0, 512.3292236328125f, 14.45419979095459f, new string[]{ "8.05", "13.0", "7.8958", "7.75", "26.0", "10.5"}, new int[] {43, 42, 38, 34, 31, 24}, new float[] {0.04826038330793381f, 0.047138046473264694f, 0.04264871031045914f, 0.03815937042236328f, 0.03479236736893654f, 0.02693602629005909f }, 7.910399913787842f,31f ),
                    new ColumnInfo( "Cabin", false, 687, 0, 0, 0, 0, new string[]{ "B96 B98", "G6", "C23 C25 C27", "C22 C26", "F33", "F2" }, new int[] {4, 4, 4, 3, 3, 3}, new float[] {0.0044893380254507065f, 0.0044893380254507065f, 0.0044893380254507065f, 0.003367003286257386f, 0.003367003286257386f, 0.003367003286257386f }, 0f,0f ),
                    new ColumnInfo( "Embarked", false, 2, 0, 0, 0, 0, new string[]{ "S", "C", "Q" }, new int[] {644, 168, 77}, new float[] {0.7227833867073059f, 0.18855218589305878f, 0.08641975373029709f}, 0f,0f ),
                };
                dataset.rowCount = 891;
                dataset.nullCols = 3;
                dataset.nullRows = 689;
                dataset.isPreProcess = true;
                dataset.cMatrix = new float[12][];
                dataset.cMatrix[0] = new float[] { 1, -0.005007f, -0.035144f, -0.038559f, 0.042939f, 0.036847f, -0.057527f, -0.001652f, -0.056554f, 0.012658f, -0.035077f, 0.013083f };
                dataset.cMatrix[1] = new float[] { -0.0050066607f, 1f, -0.33848104f, -0.057343315f, -0.54335135f, -0.077221096f, -0.0353225f, 0.08162941f, -0.16454913f, 0.25730652f, -0.25488788f, -0.16351666f };
                dataset.cMatrix[2] = new float[] { -0.035143994f, -0.33848104f, 1f, 0.052830875f, 0.13190049f, -0.369226f, 0.083081365f, 0.018442672f, 0.31986925f, -0.54949963f, 0.6841206f, 0.15711245f };
                dataset.cMatrix[3] = new float[] { -0.038558863f, -0.057343315f, 0.052830875f, 1f, 0.020313991f, 0.06258293f, -0.017230336f, -0.04910539f, 0.047348045f, -0.049172707f, 0.061959103f, -0.0045570857f };
                dataset.cMatrix[4] = new float[] { 0.04293888f, -0.54335135f, 0.13190049f, 0.020313991f, 1f, 0.093253575f, -0.11463081f, -0.24548896f, 0.059371985f, -0.18233283f, 0.09668117f, 0.104057096f };
                dataset.cMatrix[5] = new float[] { 0.036847197f, -0.077221096f, -0.369226f, 0.06258293f, 0.093253575f, 1f, -0.30824676f, -0.18911926f, -0.07593447f, 0.09606669f, -0.2523314f, -0.02525195f };
                dataset.cMatrix[6] = new float[] { -0.057526834f, -0.0353225f, 0.083081365f, -0.017230336f, -0.11463081f, -0.30824676f, 1f, 0.4148377f, 0.079461284f, 0.15965104f, 0.043592583f, 0.06665404f };
                dataset.cMatrix[7] = new float[] { -0.0016520123f, 0.08162941f, 0.018442672f, -0.04910539f, -0.24548896f, -0.18911926f, 0.4148377f, 1f, 0.020003473f, 0.21622494f, -0.02832425f, 0.038322248f };
                dataset.cMatrix[8] = new float[] { -0.056553647f, -0.16454913f, 0.31986925f, 0.047348045f, 0.059371985f, -0.07593447f, 0.079461284f, 0.020003473f, 1f, -0.013885464f, 0.24369627f, -0.0060414947f };
                dataset.cMatrix[9] = new float[] { 0.012658219f, 0.25730652f, -0.54949963f, -0.049172707f, -0.18233283f, 0.09606669f, 0.15965104f, 0.21622494f, -0.013885464f, 1f, -0.5033555f, -0.22122625f };
                dataset.cMatrix[10] = new float[] { -0.035077456f, -0.25488788f, 0.6841206f, 0.061959103f, 0.09668117f, -0.2523314f, 0.043592583f, -0.02832425f, 0.24369627f, -0.5033555f, 1f, 0.19320504f };
                dataset.cMatrix[11] = new float[] { 0.013083069f, -0.16351666f, 0.15711245f, -0.0045570857f, 0.104057096f, -0.02525195f, 0.06665404f, 0.038322248f, -0.0060414947f, -0.22122625f, 0.19320504f, 1f };

                _datasetService.Create(dataset);


                Model model = new Model();

                model._id = "";
                model.uploaderId = "000000000000000000000000";
                model.name = "Titanik model (public)";
                model.description = "Model Titanik (public)";
                model.dateCreated = DateTime.Now;
                model.lastUpdated = DateTime.Now;
                model.type = "binarni-klasifikacioni";
                model.optimizer = "Adam";
                model.lossFunction = "binary_crossentropy";
                model.hiddenLayers = 4;
                model.batchSize = "64";
                model.learningRate = "1";
                model.outputNeurons = 0;
                model.layers = new[]
                {
                    new Layer ( 0,"sigmoid", 3,"l1", "0" ),
                    new Layer ( 1,"sigmoid", 3,"l1", "0" ),
                    new Layer ( 2,"sigmoid", 3,"l1", "0" ),
                    new Layer ( 3,"sigmoid", 3,"l1", "0" ),
                };
                model.outputLayerActivationFunction = "sigmoid";
                model.metrics = new string[] { };
                model.epochs = 50;
                model.randomOrder = true;
                model.randomTestSet = true;
                model.randomTestSetDistribution = 0.1f;
                model.isPublic = true;
                model.accessibleByLink = true;
                model.validationSize = 0.1f;//proveri

                _modelService.Create(model);


                Experiment experiment = new Experiment();

                experiment._id = "";
                experiment.name = "Titanik eksperiment (binarno-klasifikacioni)";
                experiment.description = "Binarno klasifikacioni, label";
                experiment.type = "binarni-klasifikacioni";
                experiment.ModelIds = new string[] { }.ToList();
                experiment.datasetId = _datasetService.GetDatasetId(dataset.fileId);
                experiment.uploaderId = "000000000000000000000000";
                experiment.inputColumns = new string[] { "Survived", "Pclass", "Sex", "Age", "SibSp", "Parch", "Ticket", "Fare", "Embarked" };
                experiment.outputColumn = "Survived";
                experiment.nullValues = "delete_rows";
                experiment.dateCreated = DateTime.Now;
                experiment.lastUpdated = DateTime.Now;
                experiment.nullValuesReplacers = new[]
                {
                    new NullValues ("Survived", "delete_rows", ""),
                    new NullValues ("Pclass", "delete_rows", ""),
                    new NullValues ("Sex", "delete_rows", ""),
                    new NullValues ("Age", "delete_rows", ""),
                    new NullValues ("SibSp", "delete_rows", ""),
                    new NullValues ("Parch", "delete_rows", ""),
                    new NullValues ("Ticket", "delete_rows", ""),
                    new NullValues ("Fare", "delete_rows", ""),
                    new NullValues ("Embarked", "delete_rows", "")
                };
                experiment.encodings = new[]
                {
                    new ColumnEncoding( "PassengerId", "label" ),
                    new ColumnEncoding( "Survived", "label" ),
                    new ColumnEncoding( "Pclass", "label" ),
                    new ColumnEncoding( "Name", "label" ),
                    new ColumnEncoding( "Sex", "label" ),
                    new ColumnEncoding( "Age", "label" ),
                    new ColumnEncoding( "SibSp", "label" ),
                    new ColumnEncoding( "Parch", "label" ),
                    new ColumnEncoding( "Ticket", "label" ),
                    new ColumnEncoding( "Fare", "label" ),
                    new ColumnEncoding( "Cabin", "label" ),
                    new ColumnEncoding("Embarked", "label" )
                };
                experiment.columnTypes = new string[] { "numerical", "categorical", "categorical", "categorical", "categorical", "numerical", "categorical", "numerical", "categorical", "numerical", "categorical", "categorical" };

                _experimentService.Create(experiment);

                /*
                Predictor predictor = new Predictor();
                
                predictor._id = "";
                predictor.uploaderId = "000000000000000000000000";
                predictor.inputs = new string[] { "Embarked" };
                predictor.output = "Survived";
                predictor.isPublic = true;
                predictor.accessibleByLink = true;
                predictor.dateCreated = DateTime.Now;
                predictor.experimentId = experiment._id;//izmeni experiment id
                predictor.modelId = _modelService.getModelId("000000000000000000000000");
                predictor.h5FileId = ;
                predictor.metrics = new Metric[] { };
                predictor.finalMetrics = new Metric[] { };

                _predictorService.Create(predictor);*/

                //--------------------------------------------------------------------

                file = new FileModel();

                fullPath = Path.Combine(folderPath, "diamonds.csv");
                file._id = "";
                file.type = ".csv";
                file.uploaderId = "000000000000000000000000";
                file.path = fullPath;
                file.date = DateTime.Now;

                _fileService.Create(file);


                dataset = new Dataset();

                dataset._id = "";
                dataset.uploaderId = "000000000000000000000000";
                dataset.name = "Diamonds dataset (public)";
                dataset.description = "Diamonds dataset(public)";
                dataset.fileId = _fileService.GetFileId(fullPath);
                dataset.extension = ".csv";
                dataset.isPublic = true;
                dataset.accessibleByLink = true;
                dataset.dateCreated = DateTime.Now;
                dataset.lastUpdated = DateTime.Now;
                dataset.delimiter = ",";
                dataset.columnInfo = new[]
                 {
                    new ColumnInfo( "Unnamed: 0", true, 0, 26969.5f, 0, 53939, 26969.5f, new string[]{ "0", "35977", "35953", "35954", "35955", "35956" }, new int[] {1,1,1,1,1,1}, new float[] {0.000018539118173066527f, 0.000018539118173066527f, 0.000018539118173066527f, 0.000018539118173066527f, 0.000018539118173066527f, 0.000018539118173066527f}, 13484.75f,40454.25f ),
                    new ColumnInfo( "carat", true, 0, 0.7979397773742676f, 0.20000000298023224f, 5.010000228881836f, 0.699999988079071f, new string[]{ "0.3", "0.31", "1.01", "0.7", "0.32", "1.0" }, new int[] {2604, 2249, 2242, 1981, 1840, 1558}, new float[] {0.04827586188912392f, 0.04169447720050812f, 0.0415647029876709f, 0.03672599047422409f, 0.034111976623535156f, 0.02888394519686699f}, 0.4000000059604645f,1.0399999618530273f ),
                    new ColumnInfo( "cut", false, 0, 0, 0, 0, 0, new string[]{ "Ideal", "Premium", "Very Good", "Good", "Fair" }, new int[] {}, new float[] {}, 0.01f,0.1f ),
                    new ColumnInfo( "color", false, 0, 0, 0, 0, 0, new string[]{"G", "E", "F", "H", "D", "I", "I", "J"}, new int[] {}, new float[] {}, 0.01f,0.1f ),
                    new ColumnInfo( "clarity", false, 0, 0, 0, 0, 0, new string[]{ "SI1", "VS2","SI2", "VS1", "VVS2", "VVS1", "IF", "I1"  }, new int[] {}, new float[] {}, 0.01f,0.1f ),
                    new ColumnInfo( "depth", true, 0, 61.74940490722656f, 43, 79, 61.79999923706055f, new string[]{ }, new int[] {}, new float[] {}, 0.01f,0.1f ),
                    new ColumnInfo( "table", true, 0, 57.457183837890625f, 43, 95, 57, new string[]{ }, new int[] {}, new float[] {}, 0.01f,0.1f ),
                    new ColumnInfo( "price", true, 0, 3932.7998046875f, 326, 18823, 2401, new string[]{ }, new int[] {}, new float[] {}, 0.01f,0.1f ),
                    new ColumnInfo( "x", true, 0, 5.731157302856445f, 0, 10.739999771118164f, 5.699999809265137f, new string[]{  }, new int[] {}, new float[] {}, 0.01f,0.1f ),
                    new ColumnInfo( "y", true, 0, 5.73452615737915f, 0, 58.900001525878906f, 5.710000038146973f, new string[]{ }, new int[] {}, new float[] {}, 0.01f,0.1f ),
                    new ColumnInfo( "z", true, 0, 3.538733720779419f, 0, 31.799999237060547f, 3.5299999713897705f, new string[]{ }, new int[] {}, new float[] {}, 0.01f,0.1f )
                    };
                dataset.rowCount = 53940;
                dataset.nullCols = 0;
                dataset.nullRows = 0;
                dataset.isPreProcess = true;

                dataset.cMatrix = new float[12][];
                dataset.cMatrix[0] = new float[] { 1f, -0.37798348f, -0.023327231f, -0.095097944f, 0.12513599f, -0.03480023f, -0.10083032f, -0.30687317f, -0.40544048f, -0.39584267f, -0.39920828f };
                dataset.cMatrix[1] = new float[] { -0.37798348f, 1f, 0.017123736f, 0.29143676f, -0.21429037f, 0.028224314f, 0.18161754f, 0.9215913f, 0.9750942f, 0.9517222f, 0.9533874f };
                dataset.cMatrix[2] = new float[] { -0.023327231f, 0.017123736f, 1f, 0.0003042479f, 0.028235365f, -0.19424856f, 0.15032703f, 0.03986029f, 0.022341928f, 0.027572025f, 0.0020373568f };
                dataset.cMatrix[3] = new float[] { -0.095097944f, 0.29143676f, 0.0003042479f, 1f, -0.027795495f, 0.047279235f, 0.026465202f, 0.17251092f, 0.27028668f, 0.2635844f, 0.26822686f };
                dataset.cMatrix[4] = new float[] { 0.12513599f, -0.21429037f, 0.028235365f, -0.027795495f, 1f, -0.05308011f, -0.08822266f, -0.07153497f, -0.22572145f, -0.2176158f, -0.22426307f };
                dataset.cMatrix[5] = new float[] { -0.03480023f, 0.028224314f, -0.19424856f, 0.047279235f, -0.05308011f, 1f, -0.2957785f, -0.010647405f, -0.025289247f, -0.029340671f, 0.09492388f };
                dataset.cMatrix[6] = new float[] { -0.10083032f, 0.18161754f, 0.15032703f, 0.026465202f, -0.08822266f, -0.2957785f, 1f, 0.1271339f, 0.19534428f, 0.18376015f, 0.15092869f };
                dataset.cMatrix[7] = new float[] { -0.30687317f, 0.9215913f, 0.03986029f, 0.17251092f, -0.07153497f, -0.010647405f, 0.1271339f, 1f, 0.8844352f, 0.8654209f, 0.86124945f };
                dataset.cMatrix[8] = new float[] { -0.40544048f, 0.9750942f, 0.022341928f, 0.27028668f, -0.22572145f, -0.025289247f, 0.19534428f, 0.8844352f, 1f, 0.97470146f, 0.9707718f };
                dataset.cMatrix[9] = new float[] { -0.39584267f, 0.9517222f, 0.027572025f, 0.2635844f, -0.2176158f, -0.029340671f, 0.18376015f, 0.8654209f, 0.97470146f, 1f, 0.95200574f };
                dataset.cMatrix[10] = new float[] { -0.035077456f, -0.25488788f, 0.6841206f, 0.061959103f, 0.09668117f, -0.2523314f, 0.043592583f, -0.02832425f, 0.24369627f, -0.5033555f, 1f, 0.19320504f };
                dataset.cMatrix[11] = new float[] { -0.39920828f, 0.9533874f, 0.0020373568f, 0.26822686f, -0.22426307f, 0.09492388f, 0.15092869f, 0.86124945f, 0.9707718f, 0.95200574f, 1f };



                _datasetService.Create(dataset);



                model = new Model();

                model._id = "";
                model.uploaderId = "000000000000000000000000";
                model.name = "Diamonds model (public)";
                model.description = "Diamonds model (public)";
                model.dateCreated = DateTime.Now;
                model.lastUpdated = DateTime.Now;
                model.type = "multi-klasifikacioni";
                model.optimizer = "Adam";
                model.lossFunction = "sparse_categorical_crossentropy";
                model.hiddenLayers = 5;
                model.batchSize = "64";
                model.learningRate = "1";
                model.outputNeurons = 0;
                model.layers = new[]
                {
                    new Layer ( 0,"softmax", 3,"l1", "0" ),
                    new Layer ( 1,"softmax", 3,"l1", "0" ),
                    new Layer ( 2,"softmax", 3,"l1", "0" ),
                    new Layer ( 3,"softmax", 3,"l1", "0" ),
                    new Layer ( 4,"softmax", 3,"l1", "0" )
                };
                model.outputLayerActivationFunction = "softmax";
                model.metrics = new string[] { };
                model.epochs = 50;
                model.randomOrder = true;
                model.randomTestSet = true;
                model.randomTestSetDistribution = 0.10000000149011612f;
                model.isPublic = true;
                model.accessibleByLink = true;
                model.validationSize = 0.1f;//proveri

                _modelService.Create(model);


                experiment = new Experiment();

                experiment._id = "";
                experiment.name = "Diamonds eksperiment (multi-klasifikacioni)";
                experiment.description = "Diamonds eksperiment";
                experiment.type = "multi-klasifikacioni";
                experiment.ModelIds = new string[] { }.ToList();
                experiment.datasetId = _datasetService.GetDatasetId(dataset.fileId);
                experiment.uploaderId = "000000000000000000000000";
                experiment.inputColumns = new string[] { "carat", "cut", "color", "clarity", "depth", "table", "price" };
                experiment.outputColumn = "cut";
                experiment.nullValues = "delete_rows";
                experiment.dateCreated = DateTime.Now;
                experiment.lastUpdated = DateTime.Now;
                experiment.nullValuesReplacers = new NullValues[] { };
                experiment.encodings = new[]
                 {
                    new ColumnEncoding( "Unnamed: 0", "label" ),
                    new ColumnEncoding( "carat", "label" ),
                    new ColumnEncoding( "cut", "label" ),
                    new ColumnEncoding( "color", "label" ),
                    new ColumnEncoding( "clarity", "label" ),
                    new ColumnEncoding( "depth", "label" ),
                    new ColumnEncoding( "table", "label" ),
                    new ColumnEncoding( "price", "label" ),
                    new ColumnEncoding( "x", "label" ),
                    new ColumnEncoding( "y", "label" ),
                    new ColumnEncoding( "z", "label" )
                };
                experiment.columnTypes = new string[] { "numerical", "numerical", "categorical", "categorical", "categorical", "numerical", "numerical", "numerical", "numerical", "numerical", "numerical" };

                _experimentService.Create(experiment);
                /*
                predictor._id = "";
                predictor.uploaderId = "000000000000000000000000";
                predictor.inputs = new string[] { "Unnamed: 0", "carat", "cut", "color", "clarity", "depth", "table", "x", "y", "z" };
                predictor.output = "price";
                predictor.isPublic = true;
                predictor.accessibleByLink = true;
                predictor.dateCreated = DateTime.Now;
                predictor.experimentId = experiment._id;//izmeni experiment id
                predictor.modelId = _modelService.getModelId("000000000000000000000000");
                predictor.h5FileId = ;
                predictor.metrics = new Metric[] { }
                predictor.finalMetrics = new Metric[] { };
                
                 _predictorService.Create(predictor);
                 */

                //--------------------------------------------------------------------


                file = new FileModel();

                fullPath = Path.Combine(folderPath, "IMDB-Movie-Data.csv");
                file._id = "";
                file.type = ".csv";
                file.uploaderId = "000000000000000000000000";
                file.path = fullPath;
                file.date = DateTime.Now;

                _fileService.Create(file);


                dataset = new Dataset();

                dataset._id = "";
                dataset.uploaderId = "000000000000000000000000";
                dataset.name = "IMDB-Movie-Data Dataset (public)";
                dataset.description = "IMDB-Movie-Data Dataset (public)";
                dataset.fileId = _fileService.GetFileId(fullPath);
                dataset.extension = ".csv";
                dataset.isPublic = true;
                dataset.accessibleByLink = true;
                dataset.dateCreated = DateTime.Now;
                dataset.lastUpdated = DateTime.Now;
                dataset.delimiter = ",";
                dataset.columnInfo = new[]
                 {
                    new ColumnInfo( "Rank", true, 0, 500.5f, 1, 1000, 500.5f, new string[]{ "1", "672", "659", "660", "661", "662" }, new int[] {1,1,1,1,1,1}, new float[] { 0.0010000000474974513f, 0.0010000000474974513f, 0.0010000000474974513f, 0.0010000000474974513f, 0.0010000000474974513f, 0.0010000000474974513f}, 250.75f,750.25f ),
                    new ColumnInfo( "Title", false, 0, 0, 0, 0, 0, new string[]{ "The Host", "Guardians of the Galaxy", "The Hurt Locker", "The Daughter", "Pineapple Express", "The First Time" }, new int[] {2, 1, 1, 1, 1, 1}, new float[] { 0.0020000000949949026f, 0.0010000000474974513f, 0.0010000000474974513f, 0.0010000000474974513f, 0.0010000000474974513f, 0.0010000000474974513f}, 0, 0 ),
                    new ColumnInfo( "Genre", false, 0, 0, 0, 0, 0, new string[]{ "Action,Adventure,Sci-Fi", "Drama", "Comedy,Drama,Romance", "Comedy", "Drama,Romance", "Animation,Adventure,Comedy" }, new int[] {50, 48, 35, 32, 31, 27}, new float[] { 0.05000000074505806f, 0.04800000041723251f, 0.03500000014901161f, 0.03200000151991844f, 0.03099999949336052f, 0.027000000700354576f}, 0f,0f ),
                    new ColumnInfo( "Description", false, 0, 0, 0, 0, 0, new string[]{ "A group of intergalactic criminals are forced to work together to stop a fanatical warrior from taking control of the universe.", "A disgraced member of the Russian military police investigates a series of child murders during the Stalin-era Soviet Union.", "A Russian teenager living in London who dies during childbirth leaves clues to a midwife in her journal that could tie her child to a rape involving a violent Russian mob family.", "The story follows a man who returns home to discover a long-buried family secret, and whose attempts to put things right threaten the lives of those he left home years before.", "A process server and his marijuana dealer wind up on the run from hitmen and a corrupt police officer after he witnesses his dealer's boss murder a competitor while trying to serve papers on him.", "A shy senior and a down-to-earth junior fall in love over one weekend."}, new int[] {1,1,1,1,1,1}, new float[] {0.0010000000474974513f, 0.0010000000474974513f, 0.0010000000474974513f, 0.0010000000474974513f, 0.0010000000474974513f, 0.0010000000474974513f}, 0f,0f ),
                    new ColumnInfo( "Director", false, 0, 0, 0, 0, 0, new string[]{ "Ridley Scott", "David Yates", "M. Night Shyamalan", "Paul W.S. Anderson", "Michael Bay", "Zack Snyder" }, new int[] {8, 6, 6, 6, 6,5}, new float[] {0.00800000037997961f, 0.006000000052154064f, 0.006000000052154064f, 0.006000000052154064f, 0.006000000052154064f, 0.004999999888241291f}, 0, 0 ),
                    new ColumnInfo( "Actors", false, 0, 0, 0, 0, 0, new string[]{ "Jennifer Lawrence, Josh Hutcherson, Liam Hemsworth, Woody Harrelson", "Daniel Radcliffe, Emma Watson, Rupert Grint, Michael Gambon", "Shia LaBeouf, Megan Fox, Josh Duhamel, Tyrese Gibson", "Gerard Butler, Aaron Eckhart, Morgan Freeman,Angela Bassett", "Chris Pratt, Vin Diesel, Bradley Cooper, Zoe Saldana", "Ethan Hawke, David Thewlis, Emma Watson,Dale Dickey"}, new int[] {2,2,2,2,1,1}, new float[] {0.0020000000949949026f,0.0020000000949949026f, 0.0020000000949949026f, 0.0020000000949949026f,0.0010000000474974513f, 0.0010000000474974513f}, 0, 0 ),
                    new ColumnInfo( "Year", true, 0, 2012.782958984375f, 2006, 2016, 2014, new string[]{ "2016", "2015", "2014", "2013", "2012", "2011"}, new int[] {297, 127, 98, 91, 64, 63}, new float[] {0.296999990940094f, 0.12700000405311584f, 0.09799999743700027f, 0.09099999815225601f, 0.06400000303983688f, 0.06300000101327896f}, 2010, 2016 ),
                    new ColumnInfo( "Runtime (Minutes)", true, 0, 113.1719970703125f, 66, 191, 111, new string[]{ "108", "100", "117", "118", "106", "110" }, new int[] {31,28,27,26,26,26}, new float[] { 0.03099999949336052f, 0.02800000086426735f, 0.027000000700354576f, 0.026000000536441803f, 0.026000000536441803f, 0.026000000536441803f}, 100, 123 ),
                    new ColumnInfo( "Rating", true, 0, 6.723199844360352f, 1.899999976158142f, 9f, 6.800000190734863f, new string[]{ "7.1", "6.7", "7.0", "6.3", "7.3", "7.2"}, new int[] {52, 48, 46, 44, 42, 42}, new float[] {0.052000001072883606f, 0.04800000041723251f, 0.04600000008940697f, 0.04399999976158142f, 0.041999999433755875f, 0.041999999433755875f}, 6.199999809265137f,7.400000095367432f ),
                    new ColumnInfo( "Votes", true, 0, 169808.25f, 61, 1791916, 110799, new string[]{ "97141", "291", "1427", "757074", "5796", "168875"}, new int[] {2,2,2,1,1,1}, new float[] {0.0020000000949949026f,0.0020000000949949026f,0.0020000000949949026f,0.0010000000474974513f,0.0010000000474974513f,0.0010000000474974513f}, 36309f,239909.75f ),
                    new ColumnInfo( "Revenue (Millions)", true, 128, 82.95637512207031f, 0, 936.6300048828125f, 47.98500061035156f, new string[]{ "nan", "0.03", "0.01", "0.02", "0.04", "0.05"}, new int[] {128, 7, 5, 4,4,4}, new float[] {0.12800000607967377f, 0.007000000216066837f, 0.004999999888241291f, 0.004000000189989805f, 0.004000000189989805f, 0.004000000189989805f}, 13.270000457763672f,113.71499633789062f ),
                    new ColumnInfo( "Metascore", true, 64, 58.985042572021484f, 11, 100, 59.5f, new string[]{ "nan", "66.0", "68.0", "72.0", "64.0", "57.0"}, new int[] {64, 25, 25, 25,24,23}, new float[] { 0.06400000303983688f, 0.02500000037252903f, 0.02500000037252903f, 0.02500000037252903f, 0.024000000208616257f, 0.023000000044703484f}, 47,72 )
                    };
                dataset.rowCount = 1000;
                dataset.nullCols = 2;
                dataset.nullRows = 0;
                dataset.isPreProcess = true;

                //IMDB ODRADIIII
                dataset.cMatrix = new float[12][];
                dataset.cMatrix[0] = new float[] { 1f, -0.37798348f, -0.023327231f, -0.095097944f, 0.12513599f, -0.03480023f, -0.10083032f, -0.30687317f, -0.40544048f, -0.39584267f, -0.39920828f };
                dataset.cMatrix[1] = new float[] { -0.37798348f, 1f, 0.017123736f, 0.29143676f, -0.21429037f, 0.028224314f, 0.18161754f, 0.9215913f, 0.9750942f, 0.9517222f, 0.9533874f };
                dataset.cMatrix[2] = new float[] { -0.023327231f, 0.017123736f, 1f, 0.0003042479f, 0.028235365f, -0.19424856f, 0.15032703f, 0.03986029f, 0.022341928f, 0.027572025f, 0.0020373568f };
                dataset.cMatrix[3] = new float[] { -0.095097944f, 0.29143676f, 0.0003042479f, 1f, -0.027795495f, 0.047279235f, 0.026465202f, 0.17251092f, 0.27028668f, 0.2635844f, 0.26822686f };
                dataset.cMatrix[4] = new float[] { 0.12513599f, -0.21429037f, 0.028235365f, -0.027795495f, 1f, -0.05308011f, -0.08822266f, -0.07153497f, -0.22572145f, -0.2176158f, -0.22426307f };
                dataset.cMatrix[5] = new float[] { -0.03480023f, 0.028224314f, -0.19424856f, 0.047279235f, -0.05308011f, 1f, -0.2957785f, -0.010647405f, -0.025289247f, -0.029340671f, 0.09492388f };
                dataset.cMatrix[6] = new float[] { -0.10083032f, 0.18161754f, 0.15032703f, 0.026465202f, -0.08822266f, -0.2957785f, 1f, 0.1271339f, 0.19534428f, 0.18376015f, 0.15092869f };
                dataset.cMatrix[7] = new float[] { -0.30687317f, 0.9215913f, 0.03986029f, 0.17251092f, -0.07153497f, -0.010647405f, 0.1271339f, 1f, 0.8844352f, 0.8654209f, 0.86124945f };
                dataset.cMatrix[8] = new float[] { -0.40544048f, 0.9750942f, 0.022341928f, 0.27028668f, -0.22572145f, -0.025289247f, 0.19534428f, 0.8844352f, 1f, 0.97470146f, 0.9707718f };
                dataset.cMatrix[9] = new float[] { -0.39584267f, 0.9517222f, 0.027572025f, 0.2635844f, -0.2176158f, -0.029340671f, 0.18376015f, 0.8654209f, 0.97470146f, 1f, 0.95200574f };
                dataset.cMatrix[10] = new float[] { -0.035077456f, -0.25488788f, 0.6841206f, 0.061959103f, 0.09668117f, -0.2523314f, 0.043592583f, -0.02832425f, 0.24369627f, -0.5033555f, 1f, 0.19320504f };
                dataset.cMatrix[11] = new float[] { -0.39920828f, 0.9533874f, 0.0020373568f, 0.26822686f, -0.22426307f, 0.09492388f, 0.15092869f, 0.86124945f, 0.9707718f, 0.95200574f, 1f };



                _datasetService.Create(dataset);



                model = new Model();

                model._id = "";
                model.uploaderId = "000000000000000000000000";
                model.name = "IMDB model  (public)";
                model.description = "IMDB model  (public)";
                model.dateCreated = DateTime.Now;
                model.lastUpdated = DateTime.Now;
                model.type = "regresioni";
                model.optimizer = "Adam";
                model.lossFunction = "mean_absolute_error";
                model.hiddenLayers = 3;
                model.batchSize = "64";
                model.learningRate = "1";
                model.outputNeurons = 0;
                model.layers = new[]
                {
                    new Layer ( 0,"relu", 3,"l1", "0" ),
                    new Layer ( 1,"relu", 3,"l1", "0" ),
                    new Layer ( 2,"relu", 3,"l1", "0" )
                };
                model.outputLayerActivationFunction = "relu";
                model.metrics = new string[] { };
                model.epochs = 50;
                model.randomOrder = true;
                model.randomTestSet = true;
                model.randomTestSetDistribution = 0.10000000149011612f;
                model.isPublic = true;
                model.accessibleByLink = true;
                model.validationSize = 0.1f;//proveri

                _modelService.Create(model);


                experiment = new Experiment();

                experiment._id = "";
                experiment.name = "IMDB eksperiment (regresioni)";
                experiment.description = "IMDB eksperiment (regresioni)";
                experiment.type = "regresioni";
                experiment.ModelIds = new string[] { }.ToList();
                experiment.datasetId = _datasetService.GetDatasetId(dataset.fileId);
                experiment.uploaderId = "000000000000000000000000";
                experiment.inputColumns = new string[] { "Rank", "Title", "Genre", "Description", "Director", "Actors", "Year", "Runtime(Minutes)", "Rating", "Votes", "Revenue (Millions)", "Metascore" };
                experiment.outputColumn = "Revenue (Millions)";
                experiment.nullValues = "delete_rows";
                experiment.dateCreated = DateTime.Now;
                experiment.lastUpdated = DateTime.Now;
                experiment.nullValuesReplacers = new[]
                {
                    new NullValues( "Rank", "delete_rows", "" ),
                    new NullValues( "Title", "delete_rows", "" ),
                    new NullValues( "Genre", "delete_rows", "" ),
                    new NullValues( "Description", "delete_rows", "" ),
                    new NullValues( "Director", "delete_rows", "" ),
                    new NullValues( "Actors", "delete_rows", "" ),
                    new NullValues( "Year", "delete_rows", "" ),
                    new NullValues( "Runtime(Minutes)", "delete_rows", "" ),
                    new NullValues( "Rating", "delete_rows", "" ),
                    new NullValues( "Votes", "delete_rows", "" ),
                    new NullValues( "Revenue (Millions)", "delete_rows", "" ),
                    new NullValues( "Metascore", "delete_rows", "" )

                };
                experiment.encodings = new[]
                 {
                    new ColumnEncoding( "Rank", "label" ),
                    new ColumnEncoding( "Title", "label" ),
                    new ColumnEncoding( "Genre", "label" ),
                    new ColumnEncoding( "Description", "label" ),
                    new ColumnEncoding( "Director", "label" ),
                    new ColumnEncoding( "Actors", "label" ),
                    new ColumnEncoding( "Year", "label" ),
                    new ColumnEncoding( "Runtime(Minutes)", "label" ),
                    new ColumnEncoding( "Rating", "label" ),
                    new ColumnEncoding( "Votes", "label" ),
                    new ColumnEncoding( "Revenue (Millions)", "label" ),
                    new ColumnEncoding( "Metascore", "label" )
                };
                experiment.columnTypes = new string[] { "numerical", "categorical", "categorical", "categorical", "categorical", "categorical", "categorical", "numerical", "numerical", "numerical", "numerical", "numerical" };

                _experimentService.Create(experiment);




                file = new FileModel();

                fullPath = Path.Combine(folderPath, "churn.csv");
                file._id = "";
                file.type = ".csv";
                file.uploaderId = "000000000000000000000000";
                file.path = fullPath;
                file.date = DateTime.Now;

                _fileService.Create(file);


                dataset = new Dataset();

                dataset._id = "";
                dataset.uploaderId = "000000000000000000000000";
                dataset.name = "Churn dataset (public)";
                dataset.description = "Churn dataset(public)";
                dataset.fileId = _fileService.GetFileId(fullPath);
                dataset.extension = ".csv";
                dataset.isPublic = true;
                dataset.accessibleByLink = true;
                dataset.dateCreated = DateTime.Now;
                dataset.lastUpdated = DateTime.Now;
                dataset.delimiter = ",";
                dataset.columnInfo = new[]
                 {
                    new ColumnInfo( "Unnamed: 0", true, 0, 4999.5f, 0, 9999, 4999.5f, new string[]{ "0", "6670", "6663", "6664", "6665", "6666" }, new int[] {1,1,1,1,1,1}, new float[] { 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f}, 2499.75f, 7499.25f ),

                    new ColumnInfo( "RowNumber", true, 0, 5000.5f, 1, 10000, 5000.5f, new string[]{ "1", "6671", "6664", "6665", "6666", "6667" }, new int[] {1, 1, 1, 1, 1, 1}, new float[] { 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f}, 2500.75f, 7500.25f ),
                    new ColumnInfo( "CustomerId", true, 0, 15690941, 15565701, 15815690, 15690738, new string[]{ "15634602", "15667932", "15766185", "15667632", "15599024", "15798709" }, new int[] {1, 1, 1, 1, 1, 1}, new float[] { 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f}, 15628528, 15753234 ),
                    new ColumnInfo( "Surname", false, 0, 0, 0, 0, 0, new string[]{ "Smith", "Scott", "Martin", "Walker", "Brown", "Yeh" }, new int[] {32, 29, 29, 28, 26, 25}, new float[] { 0.0031999999191612005f, 0.002899999963119626f, 0.002899999963119626f, 0.00279999990016222f, 0.0026000000070780516f, 0.0024999999441206455f}, 0f,0f ),
                    new ColumnInfo( "CreditScore", false, 0, 650.52880859375f, 350, 850, 652, new string[]{ "850", "678", "655", "705", "667", "684" }, new int[] {223, 63, 54, 53, 53, 52}, new float[] { 0.02329999953508377f, 0.006300000008195639f, 0.005400000140070915f, 0.0052999998442828655f, 0.0052999998442828655f, 0.005200000014156103f}, 584f,718f ),
                    new ColumnInfo( "Geography", false, 0, 0, 0, 0, 0, new string[]{ "France", "Germany", "Spain" }, new int[] {5014, 2509, 2477}, new float[] { 0.5013999938964844f, 0.250900000333786f, 0.24770000576972961f }, 0f,0f ),
                    new ColumnInfo( "Gender", false, 0, 0, 0, 0, 0, new string[]{ "Male", "Female"}, new int[] {5457, 4543}, new float[] { 0.5457000136375427f, 0.4542999863624573f }, 0f,0f ),
                    new ColumnInfo( "Age", true, 0, 38.92179870605469f, 18, 92, 37, new string[]{ "37", "38", "35", "36", "34", "33" }, new int[] { 478, 477, 474, 456, 447, 442}, new float[] { 0.04780000075697899f, 0.04769999906420708f, 0.04740000143647194f, 0.04560000076889992f, 0.04470000043511391f, 0.044199999421834946f}, 32f, 44f ),
                    new ColumnInfo( "Tenure", true, 0, 5.012800216674805f, 0, 10, 5, new string[]{ "2", "1", "7", "8", "5", "3" }, new int[] {1048, 1035, 1028, 1025, 1012, 1009}, new float[] { 0.10480000078678131f, 0.10350000113248825f, 0.10279999673366547f, 0.10249999910593033f, 0.10119999945163727f, 0.10090000182390213f}, 3f,7f ),
                    new ColumnInfo( "Balance", true, 0, 76485.890625f, 0, 250898.09375f, 97198.5390625f, new string[]{ "0.0", "130170.82", "105473.74", "85304.27", "159397.75", "144238.7"  }, new int[] {3617, 2, 2, 1, 1, 1}, new float[] { 0.36169999837875366f, 0.00019999999494757503f, 0.00019999999494757503f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f}, 0f,127644.2421875f ),
                    new ColumnInfo( "NumOfProducts", true, 0, 1.5302000045776367f, 1, 4, 1, new string[]{ "1", "2", "3", "4" }, new int[] {5084, 4590, 266, 60}, new float[] { 0.508400022983551f, 0.45899999141693115f, 0.026599999517202377f, 0.006000000052154064f }, 1f, 2f ),
                    new ColumnInfo( "HasCrCard", true, 0, 0.7055000066757202f, 0, 1, 1, new string[]{ "0", "1" }, new int[] { 7055, 2945}, new float[] { 0.7055000066757202f, 0.2944999933242798f}, 0f,1f ),
                    new ColumnInfo( "IsActiveMember", true, 0, 0.5151000022888184f, 0, 1, 1, new string[]{ "1", "0" }, new int[] {5151, 4849}, new float[] { 0.5151000022888184f, 0.48489999771118164f }, 0f,1f ),
                    new ColumnInfo( "EstimatedSalary", true, 0, 100090.2421875f, 11.579999923706055f, 199992.484375f, 100193.9140625f, new string[]{ "24924.92", "101348.88", "55313.44", "72500.68", "182692.8", "4993.94" }, new int[] {2, 1, 1, 1, 1, 1}, new float[] { 0.00019999999494757503f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f, 0.00009999999747378752f}, 51002.109375f, 149388.25f ),
                    new ColumnInfo( "Exited", true, 0, 0.2037000060081482f, 0, 1, 0, new string[]{ "0", "1" }, new int[] {7963, 2037}, new float[] { 0.7962999939918518f, 0.2037000060081482f }, 0f,0f )
                    };
                dataset.rowCount = 1000;
                dataset.nullCols = 0;
                dataset.nullRows = 0;
                dataset.isPreProcess = true;

                dataset.cMatrix = new float[15][];
                dataset.cMatrix[0] = new float[] { 1f, 1f, 0.0042017936f, 0.0016822532f, 0.005840162f, -0.010357857f, 0.018196361f, 0.0007826142f, -0.0064947405f, -0.009066685f, 0.007246245f, 0.0005987465f, 0.01204439f, -0.005988457f, -0.01657137f };
                dataset.cMatrix[1] = new float[] { 1f, 1f, 0.0042017936f, 0.0016822532f, 0.005840162f, -0.010357857f, 0.018196361f, 0.0007826142f, -0.0064947405f, -0.009066685f, 0.007246245f, 0.0005987465f, 0.01204439f, -0.005988457f, -0.01657137f };
                dataset.cMatrix[2] = new float[] { 0.0042017936f, 0.0042017936f, 1f, 0.0056890887f, 0.0053079007f, 0.0065158387f, -0.0026411626f, 0.009496868f, -0.014882554f, -0.0124187f, 0.016971877f, -0.01402513f, 0.0016649648f, 0.015270681f, -0.0062479866f };
                dataset.cMatrix[3] = new float[] { 0.0016822532f, 0.0016822532f, 0.0056890887f, 1f, 0.007488916f, -0.022877663f, -0.0020492766f, 0.005549851f, -0.017412309f, 0.002657281f, -0.01646001f, -0.008993098f, 0.0014833941f, 0.011849712f, -0.010821913f };
                dataset.cMatrix[4] = new float[] { 0.005840162f, 0.005840162f, 0.0053079007f, 0.007488916f, 1f, 0.007888128f, -0.0028566201f, -0.0039649056f, 0.0008419418f, 0.0062683816f, 0.012237879f, -0.005458482f, 0.025651323f, -0.0013842928f, -0.027093539f };
                dataset.cMatrix[5] = new float[] { -0.010357857f, -0.010357857f, 0.0065158387f, -0.022877663f, 0.007888128f, 1f, 0.004718526f, 0.022811523f, 0.003738785f, 0.06940812f, 0.003972134f, -0.008522772f, 0.006724301f, -0.0013685637f, 0.035942953f };
                dataset.cMatrix[6] = new float[] { 0.018196361f, 0.018196361f, -0.0026411626f, -0.0020492766f, -0.0028566201f, 0.004718526f, 1f, -0.027543992f, 0.014733053f, 0.012086568f, -0.021858567f, 0.0057661245f, 0.022544324f, -0.008112339f, -0.10651249f };
                dataset.cMatrix[7] = new float[] { 0.0007826142f, 0.0007826142f, 0.009496868f, 0.005549851f, -0.0039649056f, 0.022811523f, -0.027543992f, 1f, -0.009996826f, 0.02830837f, -0.030680088f, -0.011721029f, 0.085472144f, -0.0072010425f, 0.28532302f };
                dataset.cMatrix[8] = new float[] { -0.0064947405f, -0.0064947405f, -0.014882554f, -0.017412309f, 0.0008419418f, 0.003738785f, 0.014733053f, -0.009996826f, 1f, -0.012253926f, 0.013443756f, 0.022582868f, -0.028362079f, 0.0077838255f, -0.014000612f };
                dataset.cMatrix[9] = new float[] { -0.009066685f, -0.009066685f, -0.0124187f, 0.002657281f, 0.0062683816f, 0.06940812f, 0.012086568f, 0.02830837f, -0.012253926f, 1f, -0.30417973f, -0.014858345f, -0.0100841f, 0.012797496f, 0.11853277f };
                dataset.cMatrix[10] = new float[] { 0.007246245f, 0.007246245f, 0.016971877f, -0.01646001f, 0.012237879f, 0.003972134f, -0.021858567f, -0.030680088f, 0.013443756f, -0.30417973f, 1f, 0.003183146f, 0.009611876f, 0.014204195f, -0.047819864f };
                dataset.cMatrix[11] = new float[] { 0.0005987465f, 0.0005987465f, -0.01402513f, -0.008993098f, -0.005458482f, -0.008522772f, 0.0057661245f, -0.011721029f, 0.022582868f, -0.014858345f, 0.003183146f, 1f, -0.011865637f, -0.009933415f, -0.0071377656f };
                dataset.cMatrix[12] = new float[] { 0.01204439f, 0.01204439f, 0.0016649648f, 0.0014833941f, 0.025651323f, 0.006724301f, 0.022544324f, 0.085472144f, -0.028362079f, -0.0100841f, 0.009611876f, -0.011865637f, 1f, -0.011421431f, -0.15612827f };
                dataset.cMatrix[13] = new float[] { -0.005988457f, -0.005988457f, 0.015270681f, 0.011849712f, -0.0013842928f, -0.0013685637f, -0.008112339f, -0.0072010425f, 0.0077838255f, 0.012797496f, 0.014204195f, -0.009933415f, -0.011421431f, 1f, 0.012096861f };
                dataset.cMatrix[14] = new float[] { -0.01657137f, -0.01657137f, -0.0062479866f, -0.010821913f, -0.027093539f, 0.035942953f, -0.10651249f, 0.28532302f, -0.014000612f, 0.11853277f, -0.047819864f, -0.0071377656f, -0.15612827f, 0.012096861f, 1f };



                _datasetService.Create(dataset);



                model = new Model();

                model._id = "";
                model.uploaderId = "000000000000000000000000";
                model.name = "Churn model (public)";
                model.description = "Churn model (public)";
                model.dateCreated = DateTime.Now;
                model.lastUpdated = DateTime.Now;
                model.type = "binarni-klasifikacioni";
                model.optimizer = "Adam";
                model.lossFunction = "binary_crossentropy";
                model.hiddenLayers = 4;
                model.batchSize = "64";
                model.learningRate = "1";
                model.outputNeurons = 0;
                model.layers = new[]
                {
                    new Layer ( 0,"sigmoid", 3,"l1", "0" ),
                    new Layer ( 1,"sigmoid", 3,"l1", "0" ),
                    new Layer ( 2,"sigmoid", 3,"l1", "0" ),
                    new Layer ( 3,"sigmoid", 3,"l1", "0" )
                };
                model.outputLayerActivationFunction = "sigmoid";
                model.metrics = new string[] { };
                model.epochs = 50;
                model.randomOrder = true;
                model.randomTestSet = true;
                model.randomTestSetDistribution = 0.10000000149011612f;
                model.isPublic = true;
                model.accessibleByLink = true;
                model.validationSize = 0.1f;//proveri

                _modelService.Create(model);


                experiment = new Experiment();

                experiment._id = "";
                experiment.name = "Churn eksperiment (binarno-klasifikacioni)";
                experiment.description = "Churn eksperiment (binarno-klasifikacioni)";
                experiment.type = "binarni-klasifikacioni";
                experiment.ModelIds = new string[] { }.ToList();
                experiment.datasetId = _datasetService.GetDatasetId(dataset.fileId);
                experiment.uploaderId = "000000000000000000000000";
                experiment.inputColumns = new string[] { "Surname", "CreditScore", "Age", "Tenure", "Balance", "NumOfProducts", "HasCrCard", "IsActiveMember", "EstimatedSalary", "Exited" };
                experiment.outputColumn = "Gender";
                experiment.nullValues = "delete_rows";
                experiment.dateCreated = DateTime.Now;
                experiment.lastUpdated = DateTime.Now;
                experiment.nullValuesReplacers = new[]
                {
                    new NullValues( "Surname", "delete_rows", "" ),
                    new NullValues( "CreditScore", "delete_rows", "" ),
                    new NullValues( "Geography", "delete_rows", "" ),
                    new NullValues( "Gender", "delete_rows", "" ),
                    new NullValues( "Age", "delete_rows", "" ),
                    new NullValues( "Tenure", "delete_rows", "" ),
                    new NullValues( "Balance", "delete_rows", "" ),
                    new NullValues( "NumOfProducts", "delete_rows", "" ),
                    new NullValues( "HasCrCard", "delete_rows", "" ),
                    new NullValues( "IsActiveMember", "delete_rows", "" ),
                    new NullValues( "EstimatedSalary", "delete_rows", "" ),
                    new NullValues( "Exited", "delete_rows", "" )

                };
                experiment.encodings = new[]
                 {
                    new ColumnEncoding( "Unnamed: 0", "label" ),
                    new ColumnEncoding( "RowNumber", "label" ),
                    new ColumnEncoding( "CustomerId", "label" ),
                    new ColumnEncoding( "Surname", "label" ),
                    new ColumnEncoding( "CreditScore", "label" ),
                    new ColumnEncoding( "Geography", "label" ),
                    new ColumnEncoding( "Gender", "label" ),
                    new ColumnEncoding( "Age", "label" ),
                    new ColumnEncoding( "Tenure", "label" ),
                    new ColumnEncoding( "Balance", "label" ),
                    new ColumnEncoding( "NumOfProducts", "label" ),
                    new ColumnEncoding( "HasCrCard", "label" ),
                    new ColumnEncoding( "IsActiveMember", "label" ),
                    new ColumnEncoding( "EstimatedSalary", "label" ),
                    new ColumnEncoding( "Exited", "label" )
                };
                experiment.columnTypes = new string[] { "numerical", "numerical", "numerical", "categorical", "numerical", "categorical", "categorical", "numerical", "numerical", "numerical", "numerical", "numerical", "numerical", "numerical", "numerical" };

                _experimentService.Create(experiment);



                file = new FileModel();

                fullPath = Path.Combine(folderPath, "winequality.csv");
                file._id = "";
                file.type = ".csv";
                file.uploaderId = "000000000000000000000000";
                file.path = fullPath;
                file.date = DateTime.Now;

                _fileService.Create(file);


                dataset = new Dataset();

                dataset._id = "";
                dataset.uploaderId = "000000000000000000000000";
                dataset.name = "Winequality dataset (public)";
                dataset.description = "Winequality dataset(public)";
                dataset.fileId = _fileService.GetFileId(fullPath);
                dataset.extension = ".csv";
                dataset.isPublic = true;
                dataset.accessibleByLink = true;
                dataset.dateCreated = DateTime.Now;
                dataset.lastUpdated = DateTime.Now;
                dataset.delimiter = ",";
                dataset.columnInfo = new[]
                 {
                    new ColumnInfo( "fixed acidity", true, 0, 8.319637298583984f, 4.599999904632568f, 15.899999618530273f, 7.900000095367432f, new string[]{ "7.2", "7.1", "7.8", "7.5", "7.0", "7.7" }, new int[] {67,57,53,52,50,49}, new float[] { 0.04190118983387947f, 0.035647280514240265f, 0.0331457145512104f, 0.03252032399177551f, 0.03126954287290573f, 0.03064415231347084f}, 7.099999904632568f, 9.199999809265137f ),
                    new ColumnInfo( "volatile acidity", true, 0, 0.5278205275535583f, 0.11999999731779099f, 1.5800000429153442f, 0.5199999809265137f, new string[]{ "0.6", "0.5", "0.43", "0.59", "0.36", "0.58" }, new int[] {47, 46, 43, 39, 38,38}, new float[] { 0.02939337119460106f, 0.028767980635166168f, 0.026891807094216347f, 0.024390242993831635f, 0.023764852434396744f, 0.023764852434396744f}, 0.38999998569488525f, 0.6399999856948853f ),
                    new ColumnInfo( "citric acid", true, 0, 0.27097561955451965f, 0, 1, 0.25999999046325684f, new string[]{ "0.0", "0.49", "0.24", "0.02", "0.26", "0.1" }, new int[] { 132, 68, 51, 50, 38, 35}, new float[] { 0.08255159109830856f, 0.04252658039331436f, 0.03189493343234062f, 0.03126954287290573f, 0.023764852434396744f, 0.02188868075609207f }, 0.09000000357627869f, 0.41999998688697815f ),
                    new ColumnInfo( "residual sugar", true, 0, 2.5388054847717285f, 0.8999999761581421f, 15.5f, 2.200000047683716f, new string[]{ "2.0", "2.2", "1.8", "2.1", "1.9", "2.3" }, new int[] { 156, 131, 129, 128, 117, 109}, new float[] { 0.09756097197532654f, 0.08192620426416397f, 0.08067542314529419f, 0.080050028860569f, 0.0731707289814949f, 0.06816760450601578f}, 1.899999976158142f, 2.5999999046325684f ),
                    new ColumnInfo( "chlorides", true, 0, 0.0874665379524231f, 0.012000000104308128f, 0.6110000014305115f, 0.07900000363588333f, new string[]{ "0.08", "0.74", "0.76", "0.78", "0.84", "0.71" }, new int[] { 66, 55, 51, 51, 49, 47}, new float[] { 0.04127579554915428f, 0.03439649939537048f, 0.03189493343234062f, 0.03189493343234062f, 0.03064415231347084f, 0.02939337119460106f}, 0.07000000029802322f, 0.09000000357627869f ),
                    new ColumnInfo( "free sulfur dioxide", true, 0, 15.874921798706055f, 1, 72, 14, new string[]{ "6.0", "5.0", "10.0", "15.0", "12.0", "7.0" }, new int[] { 138, 104, 79, 78, 75, 71}, new float[] { 0.0863039419054985f, 0.06504064798355103f, 0.04940588027238846f, 0.04878048598766327f, 0.0469043143093586f, 0.044402752071619034f}, 7f, 21f ),
                    new ColumnInfo( "total sulfur dioxide", true, 0, 46.46779251098633f, 6f, 289f, 38f, new string[]{ "28.0", "24.0", "15.0", "18.0", "23.0", "14.0" }, new int[] {43, 36, 35, 35, 34, 33}, new float[] { 0.026891807094216347f, 0.022514071315526962f, 0.02188868075609207f, 0.02188868075609207f, 0.02126329019665718f, 0.02063789777457714f}, 22f, 62f ),
                    new ColumnInfo( "density", true, 0, 0.9967466592788696f, 0.9900699853897095f, 1.0036900043487549f, 0.996749997138977f, new string[]{ "0.9972", "0.9968", "0.9976", "0.998", "0.9962", "0.9978" }, new int[] { 36, 35, 35, 29, 28, 26}, new float[] { 0.022514071315526962f, 0.02188868075609207f, 0.02188868075609207f, 0.018136335536837578f, 0.017510944977402687f, 0.016260161995887756f}, 0.9955999851226807f, 0.9978349804878235f ),
                    new ColumnInfo( "pH", true, 0, 3.311113119125366f, 2.740000009536743f, 4.010000228881836f, 3.309999942779541f, new string[]{ "3.3", "3.36", "3.26", "3.38", "3.39", "3.29" }, new int[] { 57, 56, 53, 48, 48, 46}, new float[] { 0.035647280514240265f, 0.035021889954805374f, 0.0331457145512104f, 0.03001876175403595f, 0.03001876175403595f, 0.028767980635166168f}, 3.2100000381469727f, 3.4000000953674316f ),
                    new ColumnInfo( "sulphates", true, 0, 0.6581488251686096f, 0.33000001311302185f, 2f, 0.6200000047683716f, new string[]{ "0.06", "0.58", "0.54", "0.62", "0.56", "0.57" }, new int[] { 69, 68, 68, 61, 60, 55}, new float[] { 0.04315197095274925f, 0.04252658039331436f, 0.04252658039331436f, 0.03814884275197983f, 0.03752345219254494f, 0.03439649939537048f }, 0.550000011920929f, 0.7300000190734863f ),
                    new ColumnInfo( "alcohol", true, 0, 10.422983169555664f, 8.399999618530273f, 14.899999618530273f, 10.199999809265137f, new string[]{ "9.5", "9.4", "9.8", "9.2", "10.0", "10.5" }, new int[] { 139, 103, 78, 72, 67, 67}, new float[] { 0.0869293287396431f, 0.06441526114940643f, 0.04878048598766327f, 0.045028142631053925f, 0.04190118983387947f, 0.04190118983387947f}, 9.5f, 11.100000381469727f ),
                    new ColumnInfo( "quality", true, 0, 5.636022567749023f, 3f, 8f, 6f, new string[]{ "5", "6", "7", "4", "8", "3" }, new int[] { 681, 638, 199, 53, 18, 10}, new float[] { 0.4258911907672882f, 0.3989993631839752f, 0.12445278465747833f, 0.0331457145512104f, 0.011257035657763481f, 0.006253908853977919f}, 5f, 6f ),
                    //new ColumnInfo( "", true, 0, , , , , new string[]{ "", "", "", "", "", "" }, new int[] {}, new float[] { }, 2500.75f, 7500.25f ),
                 };
                dataset.rowCount = 1599;
                dataset.nullCols = 0;
                dataset.nullRows = 0;
                dataset.isPreProcess = true;

                dataset.cMatrix = new float[12][];
                dataset.cMatrix[0] = new float[] { 1f, -0.2561309f, 0.67170346f, 0.11477672f, 0.093705185f, -0.1537942f, -0.11318144f, 0.6680473f, -0.6829782f, 0.18300566f, -0.06166827f, 0.124051645f };
                dataset.cMatrix[1] = new float[] { -0.2561309f, 1f, -0.55249566f, 0.001917882f, 0.061297774f, -0.010503827f, 0.07647f, 0.022026232f, 0.2349373f, -0.2609867f, -0.20228803f, -0.39055777f };
                dataset.cMatrix[2] = new float[] { 0.67170346f, -0.55249566f, 1f, 0.14357716f, 0.20382291f, -0.06097813f, 0.035533022f, 0.36494717f, -0.54190415f, 0.31277004f, 0.109903246f, 0.22637251f };
                dataset.cMatrix[3] = new float[] { 0.11477672f, 0.001917882f, 0.14357716f, 1f, 0.055609535f, 0.187049f, 0.20302789f, 0.35528338f, -0.085652426f, 0.0055271215f, 0.042075437f, 0.013731637f };
                dataset.cMatrix[4] = new float[] { 0.093705185f, 0.061297774f, 0.20382291f, 0.055609535f, 1f, 0.005562147f, 0.047400467f, 0.20063233f, -0.26502612f, 0.3712605f, -0.22114055f, -0.12890656f };
                dataset.cMatrix[5] = new float[] { -0.1537942f, -0.010503827f, -0.06097813f, 0.187049f, 0.005562147f, 1f, 0.66766644f, -0.02194583f, 0.0703775f, 0.051657572f, -0.06940836f, -0.050656058f };
                dataset.cMatrix[6] = new float[] { -0.11318144f, 0.07647f, 0.035533022f, 0.20302789f, 0.047400467f, 0.66766644f, 1f, 0.071269475f, -0.06649456f, 0.042946838f, -0.20565395f, -0.18510029f };
                dataset.cMatrix[7] = new float[] { 0.6680473f, 0.022026232f, 0.36494717f, 0.35528338f, 0.20063233f, -0.02194583f, 0.071269475f, 1f, -0.34169933f, 0.14850642f, -0.49617976f, -0.17491923f };
                dataset.cMatrix[8] = new float[] { -0.6829782f, 0.2349373f, -0.54190415f, -0.085652426f, -0.26502612f, 0.0703775f, -0.06649456f, -0.34169933f, 1f, -0.1966476f, 0.20563251f, -0.05773139f };
                dataset.cMatrix[9] = new float[] { 0.18300566f, -0.2609867f, 0.31277004f, 0.0055271215f, 0.3712605f, 0.051657572f, 0.042946838f, 0.14850642f, -0.1966476f, 1f, 0.09359475f, 0.25139707f };
                dataset.cMatrix[10] = new float[] { -0.06166827f, -0.20228803f, 0.109903246f, 0.042075437f, -0.22114055f, -0.06940836f, -0.20565395f, -0.49617976f, 0.20563251f, 0.09359475f, 1f, 0.47616634f };
                dataset.cMatrix[11] = new float[] { 0.124051645f, -0.39055777f, 0.22637251f, 0.013731637f, -0.12890656f, -0.050656058f, -0.18510029f, -0.17491923f, -0.05773139f, 0.25139707f, 0.47616634f, 1f };



                _datasetService.Create(dataset);



                model = new Model();

                model._id = "";
                model.uploaderId = "000000000000000000000000";
                model.name = "Winequality model (public)";
                model.description = "Winequality model (public)";
                model.dateCreated = DateTime.Now;
                model.lastUpdated = DateTime.Now;
                model.type = "regresioni";
                model.optimizer = "Adam";
                model.lossFunction = "mean_absolute_error";
                model.hiddenLayers = 2;
                model.batchSize = "64";
                model.learningRate = "1";
                model.outputNeurons = 0;
                model.layers = new[]
                {
                    new Layer ( 0,"relu", 3,"l1", "0" ),
                    new Layer ( 1,"relu", 3,"l1", "0" )
                };
                model.outputLayerActivationFunction = "relu";
                model.metrics = new string[] { };
                model.epochs = 50;
                model.randomOrder = true;
                model.randomTestSet = true;
                model.randomTestSetDistribution = 0.10000000149011612f;
                model.isPublic = true;
                model.accessibleByLink = true;
                model.validationSize = 0.1f;//proveri

                _modelService.Create(model);


                experiment = new Experiment();

                experiment._id = "";
                experiment.name = "Winequality eksperiment(regresioni)";
                experiment.description = "Winequality eksperiment(regresioni)";
                experiment.type = "regresioni";
                experiment.ModelIds = new string[] { }.ToList();
                experiment.datasetId = _datasetService.GetDatasetId(dataset.fileId);
                experiment.uploaderId = "000000000000000000000000";
                experiment.inputColumns = new string[] { "fixed acidity", "volatile acidity", "citric acid", "residual sugar", "chlorides", "free sulfur dioxide", "total sulfur dioxide", "density", "pH", "sulphates", "alcohol", "quality" };
                experiment.outputColumn = "quality";
                experiment.nullValues = "delete_rows";
                experiment.dateCreated = DateTime.Now;
                experiment.lastUpdated = DateTime.Now;
                experiment.nullValuesReplacers = new[]
                {
                    new NullValues( "fixed acidity", "delete_rows", "" ),
                    new NullValues( "volatile acidity", "delete_rows", "" ),
                    new NullValues( "citric acid", "delete_rows", "" ),
                    new NullValues( "residual sugar", "delete_rows", "" ),
                    new NullValues( "chlorides", "delete_rows", "" ),
                    new NullValues( "free sulfur dioxide", "delete_rows", "" ),
                    new NullValues( "total sulfur dioxide", "delete_rows", "" ),
                    new NullValues( "density", "delete_rows", "" ),
                    new NullValues( "pH", "delete_rows", "" ),
                    new NullValues( "sulphates", "delete_rows", "" ),
                    new NullValues( "alcohol", "delete_rows", "" ),
                    new NullValues( "quality", "delete_rows", "" )

                };
                experiment.encodings = new[]
                 {
                    new ColumnEncoding( "fixed acidity", "label" ),
                    new ColumnEncoding( "volatile acidity", "label" ),
                    new ColumnEncoding( "citric acid", "label" ),
                    new ColumnEncoding( "residual sugar", "label" ),
                    new ColumnEncoding( "chlorides", "label" ),
                    new ColumnEncoding( "free sulfur dioxide", "label" ),
                    new ColumnEncoding( "total sulfur dioxide", "label" ),
                    new ColumnEncoding( "density", "label" ),
                    new ColumnEncoding( "pH", "label" ),
                    new ColumnEncoding( "sulphates", "label" ),
                    new ColumnEncoding( "alcohol", "label" ),
                    new ColumnEncoding( "quality", "label" ),
                 };
                experiment.columnTypes = new string[] { "numerical", "numerical", "numerical", "numerical", "numerical", "numerical", "numerical", "numerical", "numerical", "numerical", "numerical", "numerical" };

                _experimentService.Create(experiment);




                /*
                predictor._id = "";
                predictor.uploaderId = "000000000000000000000000";
                predictor.inputs = new string[] { "Unnamed: 0", "carat", "cut", "color", "clarity", "depth", "table", "x", "y", "z" };
                predictor.output = "price";
                predictor.isPublic = true;
                predictor.accessibleByLink = true;
                predictor.dateCreated = DateTime.Now;
                predictor.experimentId = experiment._id;//izmeni experiment id
                predictor.modelId = _modelService.getModelId("000000000000000000000000");
                predictor.h5FileId = ;
                predictor.metrics = new Metric[] { }
                predictor.finalMetrics = new Metric[] { };
                
                 _predictorService.Create(predictor);
                 */

                //========================================



                /*
                file = new FileModel();

                fullPath = Path.Combine(folderPath, "iris.csv");
                file._id = "";
                file.type = ".csv";
                file.uploaderId = "000000000000000000000000";
                file.path = fullPath;
                file.date = DateTime.Now;

                _fileService.Create(file);

                
                dataset = new Dataset();

                dataset._id = "";
                dataset.uploaderId = "000000000000000000000000";
                dataset.name = "Iris dataset";
                dataset.description = "Iris dataset(public) ";
                dataset.fileId = _fileService.GetFileId(fullPath);
                dataset.extension = ".csv";
                dataset.isPublic = true;
                dataset.accessibleByLink = true;
                dataset.dateCreated = DateTime.Now;
                dataset.lastUpdated = DateTime.Now;
                dataset.delimiter = ",";
                dataset.columnInfo = new[]
                  {
                    new ColumnInfo( "sepal_length", true, 0, 5.8433332443237305f, 4.300000190734863f, 7.900000095367432f, 5.800000190734863f, new string[]{ }, new int[] {}, new float[] {}, 0.01f, 0.1f ),
                    new ColumnInfo( "sepal_width", true, 0, 3.053999900817871f, 2, 4.400000095367432f, 3, new string[]{ }, new int[] {}, new float[] {}, 0.01f,0.1f ),
                    new ColumnInfo( "petal_length", true, 0, 3.758666753768921f, 1, 6.900000095367432f, 4.349999904632568f, new string[]{ }, new int[] {}, new float[] {}, 0.01f,0.1f ),
                    new ColumnInfo( "petal_width", true, 0, 1.1986666917800903f, 0.10000000149011612f, 2.5f, 1.2999999523162842f, new string[]{}, new int[] {}, new float[] {}, 0.01f,0.1f ),
                    new ColumnInfo( "class", false, 0, 0, 0, 0, 0, new string[]{ "Iris-setosa", "Iris-versicolor", "Iris-virginica" }, new int[] {}, new float[] {}, 0.01f,0.1f ),
                };
                dataset.nullCols = 150;
                dataset.nullRows = 0;
                dataset.isPreProcess = true;

                dataset.cMatrix = new float[11][];
                dataset.cMatrix[0] = new float[] { 1f, -0.37798348f, -0.023327231f, -0.095097944f, 0.12513599f, -0.03480023f, -0.10083032f, -0.30687317f, -0.40544048f, -0.39584267f, -0.39920828f };
                dataset.cMatrix[1] = new float[] { -0.37798348f, 1f, 0.017123736f, 0.29143676f, -0.21429037f, 0.028224314f, 0.18161754f, 0.9215913f, 0.9750942f, 0.9517222f, 0.9533874f };
                dataset.cMatrix[2] = new float[] { -0.023327231f, 0.017123736f, 1f, 0.0003042479f, 0.028235365f, -0.19424856f, 0.15032703f, 0.03986029f, 0.022341928f, 0.027572025f, 0.0020373568f };
                dataset.cMatrix[3] = new float[] { -0.095097944f, 0.29143676f, 0.0003042479f, 1f, -0.027795495f, 0.047279235f, 0.026465202f, 0.17251092f, 0.27028668f, 0.2635844f, 0.26822686f };
                dataset.cMatrix[4] = new float[] { 0.12513599f, -0.21429037f, 0.028235365f, -0.027795495f, 1f, -0.05308011f, -0.08822266f, -0.07153497f, -0.22572145f, -0.2176158f, -0.22426307f };
                dataset.cMatrix[5] = new float[] { -0.03480023f, 0.028224314f, -0.19424856f, 0.047279235f, -0.05308011f, 1f, -0.2957785f, -0.010647405f, -0.025289247f, -0.029340671f, 0.09492388f };
                dataset.cMatrix[6] = new float[] { -0.10083032f, 0.18161754f, 0.15032703f, 0.026465202f, -0.08822266f, -0.2957785f, 1f, 0.1271339f, 0.19534428f, 0.18376015f, 0.15092869f };
                dataset.cMatrix[7] = new float[] { -0.30687317f, 0.9215913f, 0.03986029f, 0.17251092f, -0.07153497f, -0.010647405f, 0.1271339f, 1f, 0.8844352f, 0.8654209f, 0.86124945f };
                dataset.cMatrix[8] = new float[] { -0.40544048f, 0.9750942f, 0.022341928f, 0.27028668f, -0.22572145f, -0.025289247f, 0.19534428f, 0.8844352f, 1f, 0.97470146f, 0.9707718f };
                dataset.cMatrix[9] = new float[] { -0.39584267f, 0.9517222f, 0.027572025f, 0.2635844f, -0.2176158f, -0.029340671f, 0.18376015f, 0.8654209f, 0.97470146f, 1f, 0.95200574f };
                dataset.cMatrix[10] = new float[] { -0.035077456f, -0.25488788f, 0.6841206f, 0.061959103f, 0.09668117f, -0.2523314f, 0.043592583f, -0.02832425f, 0.24369627f, -0.5033555f, 1f, 0.19320504f };
                dataset.cMatrix[11] = new float[] { -0.39920828f, 0.9533874f, 0.0020373568f, 0.26822686f, -0.22426307f, 0.09492388f, 0.15092869f, 0.86124945f, 0.9707718f, 0.95200574f, 1f };


                _datasetService.Create(dataset);


                model = new Model();

                model._id = "";
                model.uploaderId = "000000000000000000000000";
                model.name = "Model Iris";
                model.description = "Model Iris";
                model.dateCreated = DateTime.Now;
                model.lastUpdated = DateTime.Now;
                model.type = "multi-klasifikacioni";
                model.optimizer = "Adam";
                model.lossFunction = "sparse_categorical_crossentropy";
                model.hiddenLayers = 3;
                model.batchSize = "64";
                model.outputNeurons = 0;
                model.outputLayerActivationFunction = "softmax";
                model.metrics = new string[] { };
                model.epochs = 1;
                model.isPublic = true;
                model.accessibleByLink = true;
                model.validationSize = 0.1f;//proveri

                _modelService.Create(model);


                experiment = new Experiment();

                experiment._id = "";
                experiment.name = "Iris eksperiment";
                experiment.description = "Iris eksperiment";
                experiment.ModelIds = new string[] { }.ToList();
                experiment.datasetId = _datasetService.GetDatasetId(dataset.fileId);
                experiment.uploaderId = "000000000000000000000000";
                experiment.inputColumns = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "class" };
                experiment.outputColumn = "class";
                experiment.dateCreated = DateTime.Now;
                experiment.lastUpdated = DateTime.Now;
                experiment.nullValues = "delete_rows";
                experiment.nullValuesReplacers = new NullValues[] { };
                experiment.encodings = new[]
                 {
                    new ColumnEncoding( "sepal_length", "label" ),
                    new ColumnEncoding("sepal_width", "label" ),
                    new ColumnEncoding( "petal_length", "label" ),
                    new ColumnEncoding( "petal_width", "label" ),
                    new ColumnEncoding( "class", "label" )
                };
                experiment.columnTypes = new string[] { "categorical", "numerical", "numerical", "numerical", "categorical" };


                _experimentService.Create(experiment);*/
                /*
                predictor._id = "";
                predictor.uploaderId = "000000000000000000000000";
                predictor.inputs = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width" };
                predictor.output = "class";
                predictor.isPublic = true;
                predictor.accessibleByLink = true;
                predictor.dateCreated = DateTime.Now;
                predictor.experimentId = experiment._id;//izmeni experiment id
                predictor.modelId = _modelService.getModelId("000000000000000000000000");
                predictor.h5FileId = ;
                predictor.metrics = new Metric[] { };
                predictor.finalMetrics = new Metric[] { };
                
                 _predictorService.Create(predictor);
                 */

            }


            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}
