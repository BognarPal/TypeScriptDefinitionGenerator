﻿using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;

namespace TypeScriptDefinitionGenerator
{
    public class OptionsDialogPage : DialogPage
    {
        internal const bool _defCamelCaseEnumerationValues = true;
        internal const bool _defCamelCasePropertyNames = true;
        internal const bool _defCamelCaseTypeNames = true;
        internal const bool _defClassInsteadOfInterface = true;
        internal const bool _defStringInsteadOfEnum = false;

        [Category("Casing")]
        [DisplayName("Camel case enum values")]
        [DefaultValue(_defCamelCaseEnumerationValues)]
        public bool CamelCaseEnumerationValues { get; set; } = _defCamelCaseEnumerationValues;

        [Category("Casing")]
        [DisplayName("Camel case property names")]
        [DefaultValue(_defCamelCasePropertyNames)]
        public bool CamelCasePropertyNames { get; set; } = _defCamelCasePropertyNames;

        [Category("Casing")]
        [DisplayName("Camel case type names")]
        [DefaultValue(_defCamelCaseTypeNames)]
        public bool CamelCaseTypeNames { get; set; } = _defCamelCaseTypeNames;

        [Category("Settings")]
        [DisplayName("Class instead of Interface")]
        [Description("Controls whether to generate a class or an interface: default is an Interface")]
        [DefaultValue(_defClassInsteadOfInterface)]
        public bool ClassInsteadOfInterface { get; set; } = _defClassInsteadOfInterface;

        [Category("Settings")]
        [DisplayName("String enumeration instead of Enum")]
        [Description("Controls whether to generate an enum or a string ('a' | 'b' | 'c'): default is an Interface")]
        [DefaultValue(_defStringInsteadOfEnum)]
        public bool StringInsteadOfEnum { get; set; } = _defStringInsteadOfEnum;
      
    }

    public class Options
    {
        const string OVERRIDE_FILE_NAME = "tsdefgen.json";
        static OptionsOverride overrides { get; set; } = null;
        static public bool CamelCaseEnumerationValues
        {
            get
            {
                return overrides != null ? overrides.CamelCaseEnumerationValues : DtsPackage.Options.CamelCaseEnumerationValues;
            }
        }

        static public bool CamelCasePropertyNames
        {
            get
            {
                return overrides != null ? overrides.CamelCasePropertyNames : DtsPackage.Options.CamelCasePropertyNames;
            }
        }

        static public bool CamelCaseTypeNames
        {
            get
            {
                return overrides != null ? overrides.CamelCaseTypeNames : DtsPackage.Options.CamelCaseTypeNames;
            }
        }

        static public bool ClassInsteadOfInterface
        {
            get
            {
                return overrides != null ? overrides.ClassInsteadOfInterface : DtsPackage.Options.ClassInsteadOfInterface;
            }
        }

        static public bool StringInsteadOfEnum
        {
            get
            {
                return overrides != null ? overrides.StringInsteadOfEnum : DtsPackage.Options.StringInsteadOfEnum;
            }
        }

        static public bool WebEssentials2015 => false;

        public static void ReadOptionOverrides(ProjectItem sourceItem, bool display = true)
        {
            Project proj = sourceItem.ContainingProject;

            string jsonName = "";

            foreach (ProjectItem item in proj.ProjectItems)
            {
                if (item.Name.ToLower() == OVERRIDE_FILE_NAME.ToLower())
                {
                    jsonName = item.FileNames[0];
                    break;
                }
            }

            if (!string.IsNullOrEmpty(jsonName))
            {
                // it has been modified since last read - so read again
                try
                {
                    overrides = JsonConvert.DeserializeObject<OptionsOverride>(File.ReadAllText(jsonName));
                    if (display)
                    {
                        VSHelpers.WriteOnOutputWindow(string.Format("Override file processed: {0}", jsonName));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Override file processed: {0}", jsonName));
                    }
                }
                catch (Exception e) when (e is Newtonsoft.Json.JsonReaderException || e is Newtonsoft.Json.JsonSerializationException)
                {
                    overrides = null; // incase the read fails
                    VSHelpers.WriteOnOutputWindow(string.Format("Error in Override file: {0}", jsonName));
                    VSHelpers.WriteOnOutputWindow(e.Message);
                    throw;
                }
            }
            else
            {
                if (display)
                {
                    VSHelpers.WriteOnOutputWindow("Using Global Settings");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Using Global Settings");
                }
                overrides = null;
            }
        }

    }

    internal class OptionsOverride
    {
        //        [JsonRequired]
        public bool CamelCaseEnumerationValues { get; set; } = OptionsDialogPage._defCamelCaseEnumerationValues;

        //        [JsonRequired]
        public bool CamelCasePropertyNames { get; set; } = OptionsDialogPage._defCamelCasePropertyNames;

        //        [JsonRequired]
        public bool CamelCaseTypeNames { get; set; } = OptionsDialogPage._defCamelCaseTypeNames;

        //        [JsonRequired]
        public bool ClassInsteadOfInterface { get; set; } = OptionsDialogPage._defClassInsteadOfInterface;

        //        [JsonRequired]
        public bool StringInsteadOfEnum { get; set; } = OptionsDialogPage._defStringInsteadOfEnum;


    }

}
