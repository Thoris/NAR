using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Hardware
{
    public class OperatingSystem
    {
        public string Caption { get; set; }
        public string ServicePackMajorVersion { get; set; }
        public string ServicePackMinorVersion { get; set; }
        public string InstallDate { get; set; }
        public string Version { get; set; }
        public string FreePhysicalMemory { get; set; }
        
    }
}
