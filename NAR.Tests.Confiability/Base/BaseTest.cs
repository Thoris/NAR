using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NAR.Tests.Confiability.Base
{
    public class BaseTest
    {
        #region Constants

        public string FileSample = "\\Resources\\Sample.bmp";
        
        #endregion

        #region Variables

        protected NAR.ImageProcessing.ICommand _command;
        private TimeSpan _timeImagePrepared;
              
        #endregion

        #region Properties

        protected TimeSpan TimeImagePrepared
        {
            get { return _timeImagePrepared; }
            set { _timeImagePrepared = value; }
        }

        #endregion

        #region Constructors/Destructors

        public BaseTest()
        {
        }
        
        #endregion

        #region Methods

        protected Model.IImage OpenImage(string file)
        {
            string currentFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);


            Bitmap image = (Bitmap)Bitmap.FromFile(currentFolder, false);

            return new NAR.Model.ImageBitmap(image);
        
        }
        protected Model.IImage GetSample()
        {
            return OpenImage(FileSample);
        }
        protected virtual ResultData Test(NAR.ImageProcessing.ICommand command, string fileName, NAR.Model.IImage image, bool executeFullTest, params NAR.ImageProcessing.ICommand [] commandResults)
        {
            //Executing the test in the initial image getting the time result
            DateTime startDate = DateTime.Now;

            Model.IImage result = _command.Execute(image);

            DateTime endDate = DateTime.Now;

            TimeSpan time = endDate.Subtract(startDate);

            //Creating the new image which contains the final result
            NAR.Model.IImage newResult = (NAR.Model.IImage)result.Clone();

            if (commandResults != null && executeFullTest)
            {
                foreach (NAR.ImageProcessing.ICommand newCommand in commandResults)
                {
                    newResult = newCommand.Execute(newResult);
                }
            }

            //Adding to the list
            ResultData resultData = null;
            
            if (executeFullTest)
                resultData = new ResultData(fileName, startDate, endDate, time, command, result, newResult);
            else
                resultData = new ResultData(fileName, startDate, endDate, time, command, null, null);
            


            return resultData;

        }
        protected virtual void Initialize(ref NAR.ImageProcessing.ICommand command)
        {
            if (command == null)
                command = new Commands.EchoCommand();
        }
        protected virtual void Finalize(NAR.ImageProcessing.ICommand command)
        {
            command = null;
        }
        public void Initialize()
        {
            Initialize(ref _command);
        }

        #endregion
    }
}
