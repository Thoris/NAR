using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Base
{
    public class ListBaseTest : BaseTest
    {
        #region Constants

        public const int TotalTests = 10;
        public const string ImageListFolder = ".\\Resources\\List";
        public const string FileFilter = "*.*";

        #endregion

        #region Variables

        private int _totalTests;
        private IList<FileResultData> _results = null;
        private IList<NAR.Model.IImage> _imageList = new List<NAR.Model.IImage>();
        private IList<string> _fileNames = new List<string>();
        private string _imageFolderList = ImageListFolder;
        private NAR.ImageProcessing.ICommand[] _newCommands;

        #endregion

        #region Properties

        protected IList<NAR.Model.IImage> ImageList
        {
            get { return _imageList; }
            set { _imageList = value; }
        }
        protected IList<string> FileNames
        {
            get { return _fileNames; }
        }
        public IList<FileResultData> Results
        {
            get { return _results; }
        }
        public NAR.ImageProcessing.ICommand[] NewCommands
        {
            get { return _newCommands; }
            set { _newCommands = value; }
        }

        #endregion

        #region Constructors/Destructors

        public ListBaseTest(int totalTests, string imageFolderList)
        {
            _totalTests = totalTests;
            _imageFolderList = imageFolderList;
            
        }
        
        #endregion

        #region Methods

        private IList<NAR.Model.IImage> ReadImageList(string folder, string filter)
        {
            IList<NAR.Model.IImage> result = new List<NAR.Model.IImage>();

            _fileNames.Clear();

            string[] files = System.IO.Directory.GetFiles(folder, filter);

            foreach (string file in files)
            {
                _fileNames.Add(file);
                result.Add (base.OpenImage(file));
            }

            return result;
        }
        public void ExecuteList()
        {
            _results = ExecuteList(_command, _newCommands);
        }
        protected virtual IList<FileResultData> ExecuteList(NAR.ImageProcessing.ICommand command, params NAR.ImageProcessing.ICommand [] newCommands )
        {

            IList<FileResultData> result = new List<FileResultData>();

            int pos = 0;
            foreach (Model.IImage image in _imageList)
            {
                IList<ResultData> resultData = new List<ResultData>();
                NAR.Model.IImage modified = null;
                NAR.Model.IImage newResult = null;

                for (int c = 0; c < _totalTests; c++)
                {
                    

                    ResultData singleResult = Test(command, _fileNames[pos], image, c == 0,  newCommands);

                    if (c == 0)
                    {
                        modified = singleResult.Result;
                        newResult = singleResult.NewFinalResult;
                    }

                    resultData.Add(singleResult);
                }


                result.Add (new FileResultData(this.GetType().Name, _fileNames[pos], resultData, image, modified, newResult, base.TimeImagePrepared)); 

                pos++;
            }

            return result;

        }
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            _imageList = ReadImageList(_imageFolderList, FileFilter);

            base.Initialize(ref command);
        }
        public new void Initialize()
        {
            this.Initialize(ref _command);
        }

        #endregion
    }
}
