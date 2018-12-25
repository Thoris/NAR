using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace NAR.Tests.Confiability.Reports
{
    public class HtmlReport : FileReportBase, IReport
    {
        #region Constants
        public const string TemplateIndexFile = ".\\Templates\\HtmlIndex.htm";
        public const string TemplateFileName = ".\\Templates\\HtmlReportTemplate.htm";
        private const string TagTitle = "[%title%]";
        private const string TagOriginalImage = "[%original%]";
        private const string TagModified = "[%modified%]";
        private const string TagResults = "[%results%]";
        public const string TagImageResult = "[%imageresult%]";

        public const string TagProcessorManufacturer = "[%processormanufacturer%]";
        public const string TagProcessorName = "[%processorname%]";
        public const string TagProcessorMaxClockSpeed = "[%processormaxclockSpeed%]";
        public const string TagProcessorProcessorID = "[%processorprocessorID%]";
        public const string TagProcessorRevision = "[%processorrevision%]";


        public const string TagOperatingSystemCaption = "[%operatingsystemcaption%]";
        public const string TagOperatingSystemServicePackMajorVersion = "[%operatingsystemmajorversion%]";
        public const string TagOperatingSystemServicePackMinorVersion = "[%operatingsystemminorversion%]";
        public const string TagOperatingSystemInstallDate = "[%operatingsysteminstalldate%]";
        public const string TagOperatingSystemVersion = "[%operatingsystemversion%]";
        public const string TagOperatingSystemFreePhysicalMemory = "[%operatingsystemfreephysicalmemory%]";

        public const string TagMemoryRam = "[%memoryram%]";




        #endregion

        #region Variables
        private string _templateFile;
        #endregion

        #region Constructors/Destructors
        public HtmlReport(string folder)
            : base(folder)
        {
        }
        #endregion

        #region Methods
        private string ReplaceTag(string original, string tag, string value)
        {
            return original.Replace (tag, value);
        }
        public static void CreateIndexPage(string folder, string file, Hardware.Information hardware, IList<Base.ReportBase> files)
        {
            

            StreamReader reader = new StreamReader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,TemplateIndexFile));
            string fileData = reader.ReadToEnd();
            reader.Close();

            fileData = fileData.Replace (TagTitle, "Index");

            fileData = fileData.Replace (TagProcessorManufacturer, hardware.Processor.Manufacturer);
            fileData = fileData.Replace(TagProcessorName, hardware.Processor.Name);
            fileData = fileData.Replace(TagProcessorMaxClockSpeed, hardware.Processor.MaxClockSpeed);
            fileData = fileData.Replace(TagProcessorProcessorID, hardware.Processor.ProcessorID);
            fileData = fileData.Replace(TagProcessorRevision, hardware.Processor.Revision);
            fileData = fileData.Replace (TagOperatingSystemCaption, hardware.OperatingSystem.Caption);
            fileData = fileData.Replace(TagOperatingSystemServicePackMajorVersion, hardware.OperatingSystem.ServicePackMajorVersion);
            fileData = fileData.Replace(TagOperatingSystemServicePackMinorVersion, hardware.OperatingSystem.ServicePackMinorVersion);
            fileData = fileData.Replace(TagOperatingSystemInstallDate, hardware.OperatingSystem.InstallDate);
            fileData = fileData.Replace(TagOperatingSystemVersion, hardware.OperatingSystem.Version);
            fileData = fileData.Replace(TagOperatingSystemFreePhysicalMemory, hardware.OperatingSystem.FreePhysicalMemory);


            long ram = 0;
            for (int c = 0; c < hardware.PhysicalMemories.Count; c++)
            {
                ram += hardware.PhysicalMemories[c].Capacity / 1024 / 1024;
            }

            fileData = fileData.Replace(TagMemoryRam, ram.ToString());

            fileData = fileData.Replace(TagOperatingSystemFreePhysicalMemory, hardware.OperatingSystem.FreePhysicalMemory);


            string data = "";

            int fileCount = 0;

            foreach (FileResultData result in files[0].Results)
            {

                FileInfo info = new FileInfo(result.FileName);
                string sourceFile = System.IO.Path.Combine(folder, info.Name);

                if (System.IO.File.Exists(sourceFile))
                    System.IO.File.Delete(sourceFile);

                System.IO.File.Copy(result.FileName, sourceFile);


                data +=
                    "<table width=\"100%\">\n" +
                    "   <tr>\n" +
                    "       <td valign=\"top\">" + info.Name + "<br /><img src=\"" + info.Name + "\" /></td>\n" +
                    "       <td>\n";

                data +=
                    "           <table width=\"100%\">\n" +
                    "               <tr>\n" +
                    "                   <td>Command Name</td>\n" +
                    "                   <td>Total Tests</td>\n" +
                    "                   <td>Average</td>\n" +
                    "                   <td>FPS</td>\n" +
                    "               </tr>\n";

                for (int c = 0; c < files.Count; c++)
                {
                    string htmlFileName = "" + files[c].Command.GetType().Namespace + "\\" +
                        files[c].GetType().Name + "_" + 
                        info.Name.Replace (info.Extension, "") + ".html";


                    data += 
                        "               <tr>\n" +
                        "                   <td><a href=\"" + htmlFileName + "\">" + 
                                files[c].GetType ().FullName + "</a></td>\n" +
                        "                   <td>" + files[c].Results[fileCount].Results.Count + "</td>\n" +
                        "                   <td>" + files[c].Results[fileCount].Average.ToString() + "</td>\n" +
                        "                   <td>" + files[c].Results[fileCount].FPS.ToString() + "</td>\n" +

                        "               </tr>\n";

                }

                data += "           </table>\n" +
                    "       </td>\n" +
                    "   </tr>\n" + 
                    ""
                    ;


                data += 
                    "</table>\n";

                fileCount++;
            }

            fileData = fileData.Replace(TagResults, data);

            if (System.IO.File.Exists (file))
                System.IO.File.Delete(file);

            StreamWriter newFile = new StreamWriter(file);

            try
            {
                newFile.WriteLine(fileData);
            }
            finally
            {
                newFile.Close();
            }

        }
        #endregion

        #region IReport Members

        public bool Save(IList<FileResultData> files)
        {



            StreamReader reader = new StreamReader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TemplateFileName));
            string template = reader.ReadToEnd ();
            reader.Close();

            int pos = 0;
            foreach (FileResultData fileResultData in files)
            {

                System.IO.FileInfo info = new FileInfo(fileResultData.FileName);

                string imageName = info.Name.Replace(info.Extension, "");
                string imageOriginalName = base.CommandName + "_" + imageName + "_original" + ".jpg";
                string fullFileOriginal = System.IO.Path.Combine(base.FullFolder, imageOriginalName);

                string imageModifiedName = base.CommandName + "_" + imageName + "_modified" + ".jpg";
                string fullFileModified = System.IO.Path.Combine(base.FullFolder, imageModifiedName);

                string imageResultName = base.CommandName + "_" + imageName + "_result" + ".jpg";
                string fullFileResult = System.IO.Path.Combine(base.FullFolder, imageResultName);


                _templateFile = template;
                _templateFile = ReplaceTag(_templateFile, TagTitle, fileResultData.TestName + ": " + base.CommandName  + " - " + imageName);
                _templateFile = ReplaceTag(_templateFile, TagOriginalImage, imageOriginalName);
                _templateFile = ReplaceTag(_templateFile, TagModified, imageModifiedName);

                if (fileResultData.ResultFinal == null)
                    _templateFile = ReplaceTag(_templateFile, TagImageResult, "");
                else
                {
                    string newResultFile =
                        "<tr>\n" +
                        "    <td>Result</td>\n" +
                        "</tr>\n" + 
                        "<tr>\n" +
                        "    <td><img alt=\"\" src=\"" + imageResultName + "\" /></td>\n" +
                        "</tr>\n";

                    _templateFile = ReplaceTag(_templateFile, TagImageResult, newResultFile);


                    if (!System.IO.Directory.Exists(base.FullFolder))
                        System.IO.Directory.CreateDirectory(base.FullFolder);


                    fileResultData.ResultFinal.Image.Save(fullFileResult);
                    //System.Drawing.Image newImage = (System.Drawing.Image) fileResultData.ResultFinal.Image.Clone();
                    //newImage.Save(fullFileResult);
                    //Bitmap bm = new Bitmap(fileResultData.ResultFinal.Image);
                    //bm.Save(fullFileResult, ImageFormat.Jpeg);

                }

                string fileName = System.IO.Path.Combine ( base.FullFolder, files[pos].TestName) + "_" + imageName + ".html";

                if (!System.IO.Directory.Exists(base.FullFolder))
                    System.IO.Directory.CreateDirectory(base.FullFolder);



                if (System.IO.File.Exists(fullFileOriginal))
                    System.IO.File.Delete(fullFileOriginal);

                fileResultData.Source.Image.Save(fullFileOriginal);

                if (System.IO.File.Exists(fullFileModified))
                    System.IO.File.Delete(fullFileModified);


                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);

                StreamWriter writer = new StreamWriter(fileName, true);

                try
                {

                    string tableResults =
                        "<table width=\"100%\">\n" +
                        "   <tr>\n" +
                        "       <td>Order</td>\n" +
                        "       <td>Start Time</td>\n" +
                        "       <td>End Time</td>\n" +
                        "       <td>Duration</td>\n" +
                        "       <td>Total</td>\n" +
                        "       <td>FPS</td>\n" +
                        "   </tr>\n";
                    

                    for (int c = 0; c < fileResultData.Results.Count; c++)
                    {
                        if (c == 0)                      
                            fileResultData.Results[c].Result.Image.Save(fullFileModified);

                        long fps = 0;
                        TimeSpan total = (fileResultData.TimeToPrepare + fileResultData.Results[c].Time);

                        if (total.Ticks != 0)
                        {
                            TimeSpan res = new TimeSpan((long)(new TimeSpan(0, 0, 1).Ticks / total.Ticks) * 10000000);
                            fps = res.Hours * 60 * 60 + res.Minutes * 60 + res.Seconds;
                        }
                        
                        
                        tableResults += 
                            "   <tr>\n" +
                            "       <td>" + (c + 1).ToString("0000") + "</td>\n" +
                            "       <td>" + fileResultData.Results[c].StartDate.ToString("dd/MM/yy HH:mm:ss") + "</td>\n" +
                            "       <td>" + fileResultData.Results[c].EndDate.ToString("dd/MM/yy HH:mm:ss") + "</td>\n" +
                            "       <td>" + fileResultData.Results[c].Time.ToString() + "</td>\n" +
                            "       <td>" + (fileResultData.TimeToPrepare + fileResultData.Results[c].Time).ToString() + "</td>\n" +
                            "       <td>" + fps + "</td>\n" +
                            "   </tr>\n";


                    }

                    tableResults +=
                        "</table>";



                    _templateFile = ReplaceTag(_templateFile, TagResults, tableResults);

                    writer.WriteLine(_templateFile);

                }
                finally
                {
                    writer.Close();
                }

                pos++;
            }


            return true;
        }

        #endregion
    }
}


     
