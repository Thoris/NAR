using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability
{
    public class ResultData
    {
        #region Variables

        private string _fileName;
        private DateTime _startDate;
        private DateTime _endDate;
        private TimeSpan _time;
        private NAR.ImageProcessing.ICommand _command;
        private NAR.Model.IImage _result;
        private NAR.Model.IImage _newFinalResult;

        #endregion

        #region Properties

        public string FileName
        {
            get { return _fileName; }
        }            
        public DateTime StartDate
        {
            get { return _startDate; }
        }
        public DateTime EndDate
        {
            get { return _endDate; }
        }
        public TimeSpan Time
        {
            get { return _time; }
        }
        public NAR.ImageProcessing.ICommand Command
        {
            get { return _command; }
        }
        public NAR.Model.IImage Result
        {
            get { return _result; }
        }
        public NAR.Model.IImage NewFinalResult
        {
            get { return _newFinalResult; }
        }

        #endregion

        #region Constructors/Destructors
        public ResultData(string fileName, DateTime startDate, DateTime endDate, TimeSpan time, NAR.ImageProcessing.ICommand command, NAR.Model.IImage result, NAR.Model.IImage newFinalResult)
        {
            _fileName = fileName;
            _startDate = startDate;
            _endDate = endDate;
            _time = time;
            _command = command;
            _result = result;
            _newFinalResult = newFinalResult;
        }
        #endregion
    }
}
