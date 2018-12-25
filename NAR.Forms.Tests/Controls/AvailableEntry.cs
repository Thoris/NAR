using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Forms.Tests.Controls
{
    public class AvailableEntry
    {
        #region Variables
        private object[] _parameters;
        private Type _command;
        #endregion

        #region Properties
        public object[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }
        public Type Command
        {
            get { return _command; }
            set { _command = value; }
        }
        #endregion

        #region Constructors/Destructors
        public AvailableEntry(Type command, params object [] parameters)
        {
            _command = command;
            _parameters = parameters;
        }
        #endregion

        #region Methods
        public NAR.ImageProcessing.ICommand CreateInstance()
        {

            object result = TypeLoader.CreateType(_command.Assembly.FullName, _command.FullName, _parameters);

            return (NAR.ImageProcessing.ICommand) result;
        }
        public override string ToString()
        {
            return _command.Name;
        }
        #endregion
    }
}
