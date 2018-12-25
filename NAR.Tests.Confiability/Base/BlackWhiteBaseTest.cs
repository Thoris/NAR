using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Base
{
    public class BlackWhiteBaseTest : GrayscaleBaseTest
    {
        #region Variables

        private NAR.ImageProcessing.Images.BlackWhiteCommand _bwCommand;

        #endregion

        #region Constructors/Destructors
        public BlackWhiteBaseTest(int totalTests, string imageListFolder, IReport reportGenerator)
            : base (totalTests, imageListFolder, reportGenerator)
        {
            _bwCommand = new ImageProcessing.Images.BlackWhiteCommand(true);
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

                base.ImageList[c] = _bwCommand.Execute(base.ImageList[c]);

                average += DateTime.Now.Subtract(initial);
            }

            base.TimeImagePrepared += new TimeSpan(average.Ticks / base.ImageList.Count);
        }
        #endregion
    }
}
