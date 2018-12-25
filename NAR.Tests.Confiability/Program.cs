using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability
{
    class Program
    {
        static void Main(string[] args)
        {
#if (DEBUG)
            args = new string[]{
                Performer.ParamTotalTests + Performer.ParamDivisor + "5",
                Performer.ParamReportFolder + Performer.ParamDivisor + ".\\Report\\",
                Performer.ParamReportType + Performer.ParamDivisor + ((int)Performer.ParamFormatType.Html).ToString(),
                Performer.ParamListImageTests + Performer.ParamDivisor + ".\\Resources\\List",
                
            };
#endif

            

            Performer performer = new Performer();
            performer.Run(args);
            
        }
    }
}
