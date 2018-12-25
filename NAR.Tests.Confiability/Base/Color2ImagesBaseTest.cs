using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Base
{
    public class Color2ImagesBaseTest : ColorBaseTest
    {
        #region Constants
        public const string ComparedImage = "Resources\\Sample2.bmp";
        #endregion

        #region Variables
        private string _compareFileName;
        private NAR.Model.IImage _compared;
        #endregion

        #region Constructors/Destructors
        public Color2ImagesBaseTest(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            base.Initialize(ref command);

            _compareFileName = ComparedImage;

            string fileInfo = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _compareFileName);

            _compared = base.OpenImage(fileInfo);


            ((NAR.ImageProcessing.Base.BaseFileCompared)command).ImageBase = _compared;

            
        }

        public new void Initialize()
        {
            this.Initialize(ref _command);
        }
        #endregion
    }
}
