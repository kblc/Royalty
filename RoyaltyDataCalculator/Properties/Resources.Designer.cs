﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RoyaltyDataCalculator.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RoyaltyDataCalculator.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter &apos;radiusLenght&apos; can&apos;t equals zero.
        /// </summary>
        internal static string AreaMap_Weight_RadiusLenghtCantEqualsZero {
            get {
                return ResourceManager.GetString("AreaMap_Weight_RadiusLenghtCantEqualsZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This columns not found in imported data: {0}.
        /// </summary>
        internal static string COLUMNS_NOT_FOUND_IN_IMPORT_FILE {
            get {
                return ResourceManager.GetString("COLUMNS_NOT_FOUND_IN_IMPORT_FILE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Account settings not contains values for folowing columns: {0}.
        /// </summary>
        internal static string DataCalculator_Preview_ColumnInSettingsNotSetted {
            get {
                return ResourceManager.GetString("DataCalculator_Preview_ColumnInSettingsNotSetted", resourceCulture);
            }
        }
    }
}
