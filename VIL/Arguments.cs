using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace VIL
{
    //[DefaultMember("Item")]
    public class Arguments
    {
        // Fields
        private StringDictionary Parameters = new StringDictionary();

        // Methods
        public Arguments(string[] Args)
        {
            Regex regex = new Regex("^-{1,2}|^/|=|:",(RegexOptions) 9);
            Regex regex2 = new Regex("^['\"]?(.*?)['\"]?$", (RegexOptions)9);
            string str = null;
            foreach (string str2 in Args)
            {
                string[] strArray = regex.Split(str2, 3);
                switch (strArray.Length)
                {
                    case 1:
                        if (str != null)
                        {
                            if (!this.Parameters.ContainsKey(str))
                            {
                                strArray[0] = regex2.Replace(strArray[0], "$1");
                                this.Parameters.Add(str, strArray[0]);
                            }
                            str = null;
                        }
                        break;

                    case 2:
                        if ((str != null) && !this.Parameters.ContainsKey(str))
                        {
                            this.Parameters.Add(str, "true");
                        }
                        str = strArray[1];
                        break;

                    case 3:
                        if ((str != null) && !this.Parameters.ContainsKey(str))
                        {
                            this.Parameters.Add(str, "true");
                        }
                        str = strArray[1];
                        if (!this.Parameters.ContainsKey(str))
                        {
                            strArray[2] = regex2.Replace(strArray[2], "$1");
                            this.Parameters.Add(str, strArray[2]);
                        }
                        str = null;
                        break;
                }
            }
            if ((str != null) && !this.Parameters.ContainsKey(str))
            {
                this.Parameters.Add(str, "true");
            }
        }

        // Properties
        public string this[string Param]
        {
            get
            {
                return this.Parameters[Param];
            }
        }
    }




}
