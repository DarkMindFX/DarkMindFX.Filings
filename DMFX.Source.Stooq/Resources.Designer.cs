﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DMFX.Source.Stooq {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DMFX.Source.Stooq.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;companies&gt;
        ///  &lt;company ticker=&quot;MMM&quot; name=&quot;3M Company&quot; industry=&quot;Industrials&quot; subindustry=&quot;Industrial Conglomerates&quot; cik=&quot;66740&quot; /&gt;
        ///  &lt;company ticker=&quot;ABT&quot; name=&quot;Abbott Laboratories&quot; industry=&quot;Health Care&quot; subindustry=&quot;Health Care Equipment&quot; cik=&quot;1800&quot; /&gt;
        ///  &lt;company ticker=&quot;ABBV&quot; name=&quot;AbbVie Inc.&quot; industry=&quot;Health Care&quot; subindustry=&quot;Pharmaceuticals&quot; cik=&quot;1551152&quot; /&gt;
        ///  &lt;company ticker=&quot;ACN&quot; name=&quot;Accenture plc&quot; industry=&quot;Information Technology&quot; subindustry=&quot;IT Con [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SECCompanyList {
            get {
                return ResourceManager.GetString("SECCompanyList", resourceCulture);
            }
        }
    }
}
