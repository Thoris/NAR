using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NAR.Tests.Confiability.Reports
{
    public class TextReport : FileReportBase, IReport
    {
        #region Constructors/Destructors
        public TextReport(string folder)
            : base (folder)
        {
        }
        #endregion

        #region IReport Members

        public bool Save(IList<FileResultData> files)
        {
            string fileName = base.FullFileName + ".txt";

            if (!System.IO.Directory.Exists(base.FullFolder))
                System.IO.Directory.CreateDirectory(base.FullFolder);


            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);

            StreamWriter writer = new StreamWriter(fileName, true);

            try
            {

                foreach (FileResultData fileResultData in files)
                {
                    writer.WriteLine("------------------------------------------------------");
                    writer.WriteLine("      " + fileResultData.FileName);
                    writer.WriteLine("------------------------------------------------------");

                    writer.WriteLine("Order\tStart Date        \tEnd Date          \tDuration          ");
                    writer.WriteLine("-----\t------------------\t------------------\t------------------");
                        

                    for (int c = 0; c < fileResultData.Results.Count; c++)
                    {
                        
                        writer.WriteLine(
                            (c + 1).ToString("0000") + " \t" + 
                            fileResultData.Results[c].StartDate.ToString("dd/MM/yy HH:mm:ss") + " \t" +
                            fileResultData.Results[c].EndDate.ToString("dd/MM/yy HH:mm:ss") + " \t" +
                            fileResultData.Results[c].Time.ToString());


                    }
                }
            }
            finally
            {
                writer.Close();
            }




            return true;
        }

        #endregion
    }
}
