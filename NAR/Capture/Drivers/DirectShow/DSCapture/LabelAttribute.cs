// ------------------------------------------------------------------
// DirectX.Capture
//
// History:
//	2009-Feb-27	HV		- created
//  - Added Brian's Low 'december 2003' code
//
// Copyright (C) 2009 Hans Vosman
// ------------------------------------------------------------------
using System;

namespace NAR.Capture.Drivers.DirectShow.DSCapture
{
	using System;

	[AttributeUsage(AttributeTargets.All)]
	public class LabelAttribute : Attribute
    {
        #region Variables
        public readonly string Label;
        #endregion

        #region Constructors/Destructors
        public LabelAttribute(string label)
		{
			Label = label;
		}
        #endregion

        #region Methods
        public static string FromMember(object o)
		{
			return ((LabelAttribute)
				o.GetType().GetMember(o.ToString())[0].GetCustomAttributes(typeof(LabelAttribute), false)[0]).Label;
		}

		public static string FromType(object o)
		{
			return ((LabelAttribute)
				o.GetType().GetCustomAttributes(typeof(LabelAttribute), false)[0]).Label;
        }
        #endregion
    }

}
