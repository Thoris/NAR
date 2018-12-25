using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability
{
    public interface IReport
    {
        string CommandName { get; set; }
        string ModuleName { get; set; }
        bool Save(IList<FileResultData> files);
    }
}
