using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability
{
    public class FileResultData
    {
        #region Variables

        private IList<ResultData> _results;
        private string _fileName;
        private NAR.Model.IImage _source;
        private NAR.Model.IImage _modified;
        private NAR.Model.IImage _resultFinal;
        private TimeSpan _average;
        private string _testName;
        private TimeSpan _timeToPrepare;
        
        #endregion

        #region Properties

        public IList<ResultData> Results
        {
            get { return _results; }
        }
        public string FileName
        {
            get { return _fileName; }
        }
        public NAR.Model.IImage Source
        {
            get { return _source; }
        }
        public NAR.Model.IImage Modified
        {
            get { return _modified; }
        }
        public NAR.Model.IImage ResultFinal
        {
            get { return _resultFinal; }
        }
        public TimeSpan TimeToPrepare
        {
            get { return _timeToPrepare; }
        }
        public TimeSpan Average
        {
            get
            {
                //if (_average == null)
                    CalculateAverage();

                return _timeToPrepare + _average;
            }
        }
        public int FPS
        {
            get
            {
                

                TimeSpan sec = new TimeSpan(0, 0, 1);
                TimeSpan avg = _timeToPrepare + _average;

                if (avg.Ticks == 0)
                    return 0;

                double result = sec.Ticks / avg.Ticks;

                TimeSpan res = new TimeSpan((long)result * 10000000);

                int total = res.Hours  * 60 * 60 + res.Minutes * 60 + res.Seconds;

                return total;
            }
        }
        public string TestName
        {
            get { return _testName; }
        }

        #endregion

        #region Constructors/Destructors
        public FileResultData(string testName, string fileName, IList<ResultData> results, NAR.Model.IImage source, NAR.Model.IImage modified, NAR.Model.IImage resultFinal, TimeSpan timeToPrepare)
        {
            _fileName = fileName;
            _results = results;
            _source = source;
            _modified = modified;
            _testName = testName;
            _resultFinal = resultFinal;
            _timeToPrepare = timeToPrepare;
        }
        #endregion

        #region Methods

        private void CalculateAverage()
        {
            
            int total = _results.Count;
            TimeSpan totalTime = new TimeSpan();

            for (int c = 0; c < total; c++)
            {
                totalTime += _results[c].Time;
            }

            _average = new TimeSpan(totalTime.Ticks / total);
        }

        #endregion
    }
}
