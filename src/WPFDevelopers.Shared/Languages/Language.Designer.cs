

namespace WPFDevelopers.Languages 
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Language {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Language() {
        }
       
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WPFDevelopers.Languages.Language", typeof(Language).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 取消 的本地化字符串。
        /// </summary>
        public static string Cancel {
            get {
                return ResourceManager.GetString("Cancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 跳至 的本地化字符串。
        /// </summary>
        public static string Goto {
            get {
                return ResourceManager.GetString("Goto", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 条 的本地化字符串。
        /// </summary>
        public static string Items {
            get {
                return ResourceManager.GetString("Items", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 否 的本地化字符串。
        /// </summary>
        public static string No {
            get {
                return ResourceManager.GetString("No", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 确定 的本地化字符串。
        /// </summary>
        public static string OK {
            get {
                return ResourceManager.GetString("OK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 页 的本地化字符串。
        /// </summary>
        public static string Page {
            get {
                return ResourceManager.GetString("Page", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 共 的本地化字符串。
        /// </summary>
        public static string Total {
            get {
                return ResourceManager.GetString("Total", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 是 的本地化字符串。
        /// </summary>
        public static string Yes {
            get {
                return ResourceManager.GetString("Yes", resourceCulture);
            }
        }
    }
}
