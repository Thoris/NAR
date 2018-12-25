using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace NAR.Forms.Tests.Controls
{

    public sealed class TypeLoader
    {
        #region Methods
        /// <summary>
        /// Dynamically loads a type given a fully qualified type name
        /// </summary>
        /// <param name="fullTypeName">The fully qualified type name of the object</param>
        /// <returns>The initialized object</returns>
        public static object CreateTypeByFullTypeName(string fullTypeName)
        {
            return CreateTypeByFullTypeName(fullTypeName, null);
        }
        /// <summary>
        /// Dynamically loads a type given a fully qualified type name and intializes
        /// its constructor.
        /// </summary>
        /// <param name="fullTypeName">The fully qualified type name of the object</param>
        /// <param name="parameters">Parameters to pass into the constructor of the object</param>
        /// <returns>The initialized object</returns>
        public static object CreateTypeByFullTypeName(string fullTypeName, object[] parameters)
        {
            if (string.IsNullOrEmpty(fullTypeName))
                throw new ArgumentNullException("fullTypeName");

            string[] splitType = SplitFullType(fullTypeName);
            string typeName = splitType[0];
            string assembly = splitType[1];

            return CreateType(assembly, typeName, parameters);
        }
        /// <summary>
        /// Dynamically loads a type given an assembly name and the type name
        /// </summary>
        /// <param name="assembly">The assembly the type resides in</param>
        /// <param name="typeName">The type name of the object to create</param>
        /// <returns>The initalized object</returns>
        public static object CreateType(string assembly, string typeName)
        {
            return CreateType(assembly, typeName, null);
        }
        /// <summary>
        /// Dynamically loads a type given an assembly name and the type name and initializes
        /// its constructor with the passed in parameters.
        /// </summary>
        /// <param name="assembly">The assembly the type resides in</param>
        /// <param name="typeName">The type name of the object to create</param>
        /// <param name="parameters">List of parameters to initialize the constructor with</param>
        /// <returns>The initalized object</returns>
        public static object CreateType(string assembly, string typeName, object[] parameters)
        {
            Type t = GetDynamicType(assembly, typeName);
            return Activator.CreateInstance(t, parameters);
        }
        /// <summary>
        /// Loads a type given an assembly and type name
        /// </summary>
        /// <param name="assembly">The assembly the type resides in</param>
        /// <param name="typeName">The type name of the object to create</param>
        /// <returns>The initalized object</returns>
        private static Type GetDynamicType(string assembly, string typeName)
        {
            if (string.IsNullOrEmpty(assembly))
                throw new ArgumentNullException("fullTypeName");

            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentNullException("fullTypeName");

            Assembly oAssem = Assembly.Load(assembly);
            if (oAssem == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Error to load the assembly", assembly), "assembly");
            }

            Type dynamicType = null;
            try
            {
                dynamicType = oAssem.GetType(typeName, true, true);
            }
            catch
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Could not load the type", typeName, assembly), "typeName");
            }

            return dynamicType;
        }
        /// <summary>
        /// Splits a full type name into assembly, type and version info
        /// </summary>
        /// <param name="fullType">The fulltype name to split</param>
        /// <returns>A string array of the split values</returns>
        private static string[] SplitFullType(string fullType)
        {
            if (string.IsNullOrEmpty(fullType))
                throw new ArgumentNullException("fullType");

            string[] splitType = new string[2];

            string[] parts = fullType.Split(',');

            if (parts.Length == 1)
            {
                splitType[0] = fullType;
            }
            else if (parts.Length == 2)
            {
                splitType[0] = parts[0].Trim();
                splitType[1] = parts[1].Trim();
            }
            else if (parts.Length == 5)
            {
                //  set the object type name
                splitType[0] = parts[0].Trim();

                //  set the object assembly name
                splitType[1] = String.Concat(parts[1].Trim() + ",",
                    parts[2].Trim() + ",",
                    parts[3].Trim() + ",",
                    parts[4].Trim());
            }
            else
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid Full Type Name"), "fullType");
            }

            return splitType;
        }
        #endregion
    }
}
