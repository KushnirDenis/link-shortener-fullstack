﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LinkShortener.Localization {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ErrorMessages_ru {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorMessages_ru() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("LinkShortener.Localization.ErrorMessages_ru", typeof(ErrorMessages_ru).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string UserAlreadyExists {
            get {
                return ResourceManager.GetString("UserAlreadyExists", resourceCulture);
            }
        }
        
        internal static string WrongMailFormat {
            get {
                return ResourceManager.GetString("WrongMailFormat", resourceCulture);
            }
        }
        
        internal static string ShortPassword {
            get {
                return ResourceManager.GetString("ShortPassword", resourceCulture);
            }
        }
        
        internal static string IncorrectLoginOrPassword {
            get {
                return ResourceManager.GetString("IncorrectLoginOrPassword", resourceCulture);
            }
        }
        
        internal static string NoAccess {
            get {
                return ResourceManager.GetString("NoAccess", resourceCulture);
            }
        }
        
        internal static string AlreadyShortenedLink {
            get {
                return ResourceManager.GetString("AlreadyShortenedLink", resourceCulture);
            }
        }
        
        internal static string LinkNotFound {
            get {
                return ResourceManager.GetString("LinkNotFound", resourceCulture);
            }
        }
        
        internal static string NoClickedOnLink {
            get {
                return ResourceManager.GetString("NoClickedOnLink", resourceCulture);
            }
        }
        
        internal static string InvalidLink {
            get {
                return ResourceManager.GetString("InvalidLink", resourceCulture);
            }
        }
    }
}
