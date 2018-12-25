using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Base
{
    public class GrayscaleBaseTest : ColorBaseTest
    {
        #region Variables

        private NAR.ImageProcessing.Images.GrayscaleCommand _grayscaleCommand;

        #endregion

        #region Constructors/Destructors
        public GrayscaleBaseTest(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests,  imageListFolder, reportGenerator)
        {
            _grayscaleCommand = new ImageProcessing.Images.GrayscaleCommand(false);
        }
        #endregion

        #region Methods
        protected override void Initialize(ref ImageProcessing.ICommand command)
        {
            base.Initialize(ref command);

            TimeSpan average = new TimeSpan();

            for (int c = 0; c < base.ImageList.Count; c++)
            {
                DateTime initial = DateTime.Now;

                base.ImageList[c] = _grayscaleCommand.Execute(base.ImageList[c]);

                average += DateTime.Now.Subtract(initial);
            }

            base.TimeImagePrepared += new TimeSpan(average.Ticks / base.ImageList.Count);
        }

        public new void Initialize()
        {
            this.Initialize(ref _command);
        }
        #endregion
    }
}
