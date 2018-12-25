using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Tests.Confiability.Commands
{
    public class EchoCommand : NAR.ImageProcessing.ICommand
    {
        #region Constructors/Destructors
        public EchoCommand()
        {
        }
        #endregion

        #region ICommand Members
        public NAR.Model.IImage Execute(NAR.Model.IImage image)
        {
            return image;
        }
        #endregion
    }
}
