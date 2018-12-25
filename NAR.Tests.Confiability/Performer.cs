using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability
{
    public class Performer
    {
        #region Constants / Enumerations
        public const string ParamTotalTests = "-t";
        public const string ParamReportFolder = "-o";
        public const string ParamListImageTests = "-l";
        public const string ParamReportType = "-f";
        public const char ParamDivisor = '=';

        public enum ParamFormatType
        {
            Text = 1,
            Html = 2,
        }
        #endregion

        #region Variables

        private IList<Base.ReportBase> _execution = new List<Base.ReportBase>();
        
        private string[] _allowedParams = new string[] {
                ParamListImageTests, 
                ParamReportFolder, 
                ParamReportType, 
                ParamTotalTests };


        private int _totalTests = 10;
        private string _folder = ".\\";
        private string _listFolderFiles = ".\\Resources\\List\\";
        private bool _isHtml = true;

        private Hardware.Information _info = new Hardware.Information();
        

        #endregion

        #region Constructors/Destructors
        public Performer()
        {
        }
        #endregion

        #region Methods

        private bool IsParamAllowed(string value)
        {
            for (int c = 0; c < _allowedParams.Length; c++)
            {
                if (string.Compare (value, _allowedParams[c]) == 0)
                    return true;
            }
            return false;
        }
        private bool ValidateParams(params string[] args)
        {
            if (args == null)
                return true;

            for (int c = 0; c < args.Length; c++)
            {
                //Checking the format of the paramters: <type>:<value>
                string[] splitter = args[c].Split(new char[] { ParamDivisor });
                if (splitter.Length != 2)
                {
                    Console.WriteLine("ERROR: [{0}] Invalid Parameter Format", args[c]);
                    return false;
                }

                //Checking if the parameter is available
                if (!IsParamAllowed(splitter[0]))
                {
                    Console.WriteLine("ERROR: [{0}] Invalid Paramter type", args[c]);
                    return false;
                }

                switch (splitter[0])
                {
                    case ParamTotalTests:

                        try
                        {
                            _totalTests = int.Parse(splitter[1]);
                        }
                        catch 
                        {
                            Console.WriteLine("ERROR: Cannot convert [{0}]", splitter[1]);
                            return false;
                        }

                        break;


                    case ParamReportFolder:

                        if (string.IsNullOrEmpty(splitter[1]))
                        {
                            Console.WriteLine("ERROR: Folder is empty in parameter [{0}]", args[c]);
                            return false;
                        }

                        _folder = splitter[1];

                        break;


                    case ParamListImageTests:

                        if (string.IsNullOrEmpty(splitter[1]))
                        {
                            Console.WriteLine("ERROR: Folder is empty in parameter [{0}]", args[c]);
                            return false;
                        }

                        _listFolderFiles = splitter[1];

                        break;

                    case ParamReportType:

                        try
                        {
                            int value = int.Parse(splitter[1]);

                            switch (value)
                            {
                                case (int)ParamFormatType.Html:
                                    _isHtml = true;
                                    break;


                                case (int)ParamFormatType.Text:
                                    _isHtml = false;
                                    break;

                                default:
                                    Console.WriteLine("ERROR: Invalid Value [{0}]", args[c]);
                                    return false;
                            }
                        }
                        catch
                        {
                            Console.WriteLine("ERROR: Invalid parameter [{0}]", args[c]);
                            return false;
                        }
                        

                        break;
                }


            }

            return true;
        }
        private void Initialize(string folder)
        {
            _folder = folder;

            _execution.Clear();

            string reportType = typeof(Reports.HtmlReport).FullName;

            if (_isHtml)
                reportType = typeof(Reports.HtmlReport).FullName;
            else
                reportType = typeof(Reports.TextReport).FullName;



            NAR.ImageProcessing.Borders.IBorderDetector borderDetector = new NAR.ImageProcessing.Borders.SobelCommand(true);

            _execution.Add(new Commands.EchoCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));

            //MODEL
            //_execution.Add(new Commands.Model.ImageBitmapImageReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            //_execution.Add(new Commands.Model.ImageBitmapBytesReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));


            //BORDERS
            _execution.Add(new Commands.Borders.BooleanEdgeCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.CannyCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.DifferenceCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.EdgeDetectQuickCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.EmbossLaplacianCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            //_execution.Add(new Commands.Borders.EuclidianDistanceCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.HomogenityCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.HorizontalCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            //_execution.Add(new Commands.Borders.HumanCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.KirshCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.LaplaceCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.LaplaceOfGaussianCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            //_execution.Add(new Commands.Borders.MarrHildrethCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            //_execution.Add(new Commands.Borders.MultiFlashCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.PrewittCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.RichardsCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.RobinsonCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Borders.SobelCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));


            ////CONTOURS
            //_execution.Add(new Commands.Contours.GreedContoursCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder), borderDetector));
            //_execution.Add(new Commands.Contours.KassContoursCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder), borderDetector));
            ////_execution.Add(new Commands.Contours.MooreNeighborContoursCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder), borderDetector));
            ////_execution.Add(new Commands.Contours.RadialSweepContoursCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder), borderDetector));
            //_execution.Add(new Commands.Contours.SnakeContoursCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder), borderDetector));
            ////_execution.Add(new Commands.Contours.SquareContoursCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder), borderDetector));
            //_execution.Add(new Commands.Contours.TheoPavlidisContoursCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder), borderDetector));

            ////CORNERS

            //DISTANCE
            _execution.Add(new Commands.Distance.CalcDistanceCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));

            //EFECTS
            _execution.Add(new Commands.Effects.DilatationCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.DistanceFunctionCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.ErosionCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.FourierTransformCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.GaussianBlurCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.GaussianCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.MeanRemovalCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.MedianCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.MoireCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.PixelateCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.RandomJitterCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.SharpenCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.SmoothCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.SphereCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.SwirlCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.TimeWarpCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.WaterCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Effects.ZhangSuenCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));

            //IMAGES
            _execution.Add(new Commands.Images.AverageCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.BlackWhiteCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.BlackWhiteLimiarCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.BlueCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.GrayscaleCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.GrayscaleLuminanceCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.GreenCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            //_execution.Add(new Commands.Images.HysteresisThresholdCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.InvertCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.LuminosityCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.NormalizeCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.NormalizeGrayscaleCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.RedCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.RotateCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.SepiaCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));


            //BINARIZATION
            _execution.Add(new Commands.Images.Binarization.BayerDitheringCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.Binarization.BurkesDitheringCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.Binarization.FloydSteinbergDitheringCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.Binarization.JarvisJudiceNinkeDitheringCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.Binarization.OrderedDitheringCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.Binarization.SierraDitheringCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.Binarization.StuckiDitheringCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.Binarization.ThresholdCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Images.Binarization.Adaptative.BradleyLocalThresholdingCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));



            //OPERATIONS
            _execution.Add(new Commands.Operations.DivisionCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Operations.MultiplicationCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Operations.SubtractionCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Operations.SumCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));

            //OPERATORS
            _execution.Add(new Commands.Operators.AndCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Operators.NandCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Operators.NorCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Operators.NotCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Operators.OrCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Operators.XorCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));

            //PIXELS
            _execution.Add(new Commands.Pixels.CountPixelCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));

            //SEGMENTATION
            _execution.Add(new Commands.Segmentation.HoughLineTransformationCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder), borderDetector));
            _execution.Add(new Commands.Segmentation.HoughCircleTransformationCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder), borderDetector));

            //SIZE
            _execution.Add(new Commands.Size.SizeCalculatorCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));

            //TEXTURES
            _execution.Add(new Commands.Textures.CloudsTextureCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Textures.LabyrinthTextureCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Textures.MarbleTextureCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Textures.TextileTextureCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));
            _execution.Add(new Commands.Textures.WoodTextureCommandReport(_totalTests, _listFolderFiles, TypeLoader.CreateTypeReport(reportType, folder)));


            for (int c = 0; c < _execution.Count; c++)
            {
                _execution[c].Initialize();
            }
        }
        public void Run(params string[] args)
        {
            if (!ValidateParams(args))
                return;

            Initialize(_folder);

            _info.Read();


            for (int c = 0; c < _execution.Count; c++)
            {
                _execution[c].ExecuteList();
            }

            GenerateReport();

        }
        private void GenerateReport()
        {
            if (!System.IO.Directory.Exists(_folder))
                System.IO.Directory.CreateDirectory(_folder);


            for (int c = 0; c < _execution.Count; c++)
            {
                _execution[c].Save();
            }

            if (_isHtml)
                Reports.HtmlReport.CreateIndexPage(_folder,  System.IO.Path.Combine (_folder, "index.html"), _info, _execution);
        }
        
        #endregion
    }
}
