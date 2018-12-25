using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace NAR.Tests.Confiability.Hardware
{
    public class Information
    {
        #region Variables

        private Processor _processor;
        private OperatingSystem _os;
        private IList<PhysicalMemory> _physicalMemories;
        
        #endregion

        #region Properties

        public Processor Processor
        {
            get { return _processor; }
        }
        public OperatingSystem OperatingSystem
        {
            get { return _os; }
        }
        public IList<PhysicalMemory> PhysicalMemories
        {
            get { return _physicalMemories; }
        }
        
        #endregion

        #region Constructors/Destructors
        public Information()
        {
            //ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            //string collectedInfo = ""; // here we will put the informa

            //foreach (ManagementObject share in searcher.Get())
            //{
            //    // first of all, the processorid
            //    collectedInfo += share.GetPropertyValue("ProcessorId").ToString();
            //}

            //searcher.Query = new ObjectQuery("select * from Win32_BIOS");
            //foreach (ManagementObject share in searcher.Get())
            //{
            //    //then, the serial number of BIOS
            //    collectedInfo += share.GetPropertyValue("SerialNumber").ToString();
            //}

            //searcher.Query = new ObjectQuery("select * from Win32_BaseBoard");
            //foreach (ManagementObject share in searcher.Get())
            //{
            //    //finally, the serial number of motherboard
            //    collectedInfo += share.GetPropertyValue("SerialNumber").ToString();
            //}

            //ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            //ManagementObjectCollection osDetailsCollection = objOSDetails.Get();

            //foreach (ManagementObject mo in osDetailsCollection)
            //{
            //    //_osName = mo["name"].ToString();// what other fields are there other than name
            //    //_osVesion = mo["version"].ToString();
            //    //_loginName = mo["csname"].ToString();
            //}

            //ComputerSystem
        }
        #endregion

        #region Methods
        public void Read()
        {
            _processor = GetProcessor();
            _os = GetOperatingSystem();
            _physicalMemories = GetPhysicalMemory();
        }

        private Processor GetProcessor()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");


            foreach (ManagementObject share in searcher.Get())
            {
                Processor processor = new Processor();
                processor.Manufacturer = share["Manufacturer"].ToString();
                processor.MaxClockSpeed = share["MaxClockSpeed"].ToString();
                processor.Name = share["Name"].ToString();
                processor.ProcessorID = share["ProcessorID"].ToString();
                processor.Revision = share["Revision"].ToString();

                return processor;
            }

            return null;

        }
        private OperatingSystem GetOperatingSystem()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");


            foreach (ManagementObject share in searcher.Get())
            {
                OperatingSystem os = new OperatingSystem();
                os.Caption = share["Caption"].ToString();
                os.FreePhysicalMemory = share["FreePhysicalMemory"].ToString();
                os.InstallDate = share["InstallDate"].ToString();
                os.ServicePackMajorVersion = share["ServicePackMajorVersion"].ToString();
                os.ServicePackMinorVersion = share["ServicePackMinorVersion"].ToString();
                os.Version = share["Version"].ToString();
                return os;
            }

            return null;

        }
        private IList<PhysicalMemory> GetPhysicalMemory()
        {
            IList<PhysicalMemory> list = new List<PhysicalMemory>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");


            foreach (ManagementObject share in searcher.Get())
            {
                PhysicalMemory physicalMemory = new PhysicalMemory();
                physicalMemory.Capacity = long.Parse(share["Capacity"].ToString());
                //physicalMemory.Manufacturer = share["Manufacturer"].ToString();

                list.Add(physicalMemory);
            }

            return list ;

        }
        

        #endregion
    }
}
