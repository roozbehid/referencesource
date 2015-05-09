//------------------------------------------------------------------------------
// <copyright file="VirtualPathUtility.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

/*
 * VirtualPathUtility class
 *
 * Copyright (c) 2004 Microsoft Corporation
 */

namespace System.Web {

    using System.Text;
using System.Web.Util;
using System.Security.Permissions;

/*
 * Code to perform virtual path operations
 */
public static class VirtualPathUtility {

#if MONO
        internal static string GetDirectory (string virtualPath, bool normalize) 
     { 
         if (normalize) 
             { // do nothing for now
}


             int vpLen = virtualPath.Length; 
             if (IsAppRelative (virtualPath) && vpLen < 3) { // "~" or "~/" 
                 virtualPath = ToAbsolute (virtualPath); 
                 vpLen = virtualPath.Length; 
             } 
              
             if (vpLen == 1 && virtualPath [0] == '/') // "/" 
                 return null; 
 

             int last = virtualPath.LastIndexOf ('/', vpLen - 2, vpLen - 2); 
             if (last > 0) 
                 return virtualPath.Substring (0, last + 1); 
             else 
                 return "/"; 
         } 
 
         internal static bool IsRooted (string virtualPath) 
         { 
             return IsAbsolute (virtualPath) || IsAppRelative (virtualPath); 
         } 


        internal static string Canonize (string path) 
         { 
             int index = -1; 
             for (int i=0; i < path.Length; i++) { 
                 if ((path [i] == '\\') || (path [i] == '/' && (i + 1) < path.Length && (path [i + 1] == '/' || path [i + 1] == '\\'))) { 
                     index = i; 
                     break; 
                 } 
             } 
             if (index < 0) 
                 return path; 
 

             StringBuilder sb = new StringBuilder (path.Length); 
             sb.Append (path, 0, index); 
 

             for (int i = index; i < path.Length; i++) { 
                 if (path [i] == '\\' || path [i] == '/') { 
                     int next = i + 1; 
                     if (next < path.Length && (path [next] == '\\' || path [next] == '/')) 
                         continue; 
                     sb.Append ('/'); 
                 } 
                 else { 
                     sb.Append (path [i]); 
                 } 
             } 
 

             return sb.ToString (); 
         } 

static char [] path_sep = { '/' };

internal static string Normalize (string path) 
         { 
             if (!IsRooted (path)) 
                 throw new ArgumentException (String.Format ("The relative virtual path '{0}' is not allowed here.", path)); 
 

             if (path.Length == 1) // '/' or '~' 
                 return path; 
 

             path = Canonize (path); 
 

             int dotPos = path.IndexOf ('.'); 
             while (dotPos >= 0) { 
                 if (++dotPos == path.Length) 
                     break; 
 

                 char nextChar = path [dotPos]; 
 

                 if ((nextChar == '/') || (nextChar == '.')) 
                     break; 
 

                 dotPos = path.IndexOf ('.', dotPos); 
             } 
 

             if (dotPos < 0) 
                 return path; 
 

             bool starts_with_tilda = false; 
             bool ends_with_slash = false; 
             string [] apppath_parts= null; 
 

             if (path [0] == '~') { 
                 if (path.Length == 2) // "~/" 
                     return "~/"; 
                 starts_with_tilda = true; 
                 path = path.Substring (1); 
             } 
             else if (path.Length == 1) { // "/" 
                 return "/"; 
             } 
 

             if (path [path.Length - 1] == '/') 
                 ends_with_slash = true; 
 

            string [] parts = path.Split (path_sep, StringSplitOptions.RemoveEmptyEntries); 
             int end = parts.Length; 
 

             int dest = 0; 
 

             for (int i = 0; i < end; i++) { 
                 string current = parts [i]; 
                 if (current == ".") 
                     continue; 
 

                 if (current == "..") { 
                     dest--; 
 

                     if(dest >= 0) 
                         continue; 
 

                     if (starts_with_tilda) { 
                         if (apppath_parts == null) { 
                             string apppath = HttpRuntime.AppDomainAppVirtualPath; 
                             apppath_parts = apppath.Split (path_sep, StringSplitOptions.RemoveEmptyEntries); 
                         } 
 

                         if ((apppath_parts.Length + dest) >= 0) 
                             continue; 
                     } 
                      
                     throw new HttpException ("Cannot use a leading .. to exit above the top directory."); 
                 } 
 

                 if (dest >= 0) 
                     parts [dest] = current; 
                 else 
                     apppath_parts [apppath_parts.Length + dest] = current; 
                  
                 dest++; 
             } 
 

             StringBuilder str = new StringBuilder(); 
             if (apppath_parts != null) { 
                 starts_with_tilda = false; 
                 int count = apppath_parts.Length; 
                 if (dest < 0) 
                     count += dest; 
                 for (int i = 0; i < count; i++) { 
                     str.Append ('/'); 
                     str.Append (apppath_parts [i]); 
                 } 
             } 
             else if (starts_with_tilda) { 
                 str.Append ('~'); 
             } 
 

             for (int i = 0; i < dest; i++) { 
                 str.Append ('/'); 
                 str.Append (parts [i]); 
             } 
 

             if (str.Length > 0) { 
                 if (ends_with_slash) 
                     str.Append ('/'); 
             } 
             else { 
                 return "/"; 
             } 
 

             return str.ToString (); 
         } 

#endif

    /* Discover virtual path type */

    public static bool IsAbsolute(string virtualPath) {
        VirtualPath virtualPathObject = VirtualPath.Create(virtualPath);
        return !virtualPathObject.IsRelative && virtualPathObject.VirtualPathStringIfAvailable != null;
    }

    public static bool IsAppRelative(string virtualPath) {
        VirtualPath virtualPathObject = VirtualPath.Create(virtualPath);
        return virtualPathObject.VirtualPathStringIfAvailable == null;
    }

    /* Convert between virtual path types */
    public static string ToAppRelative(string virtualPath) {
        VirtualPath virtualPathObject = VirtualPath.CreateNonRelative(virtualPath);
        return virtualPathObject.AppRelativeVirtualPathString;
    }

    public static string ToAppRelative(string virtualPath, string applicationPath) {
        VirtualPath virtualPathObject = VirtualPath.CreateNonRelative(virtualPath);

        // If it was already app relative, just return it
        if (virtualPathObject.AppRelativeVirtualPathStringIfAvailable != null)
            return virtualPathObject.AppRelativeVirtualPathStringIfAvailable;

        VirtualPath appVirtualPath = VirtualPath.CreateAbsoluteTrailingSlash(applicationPath);

        return UrlPath.MakeVirtualPathAppRelative(virtualPathObject.VirtualPathString,
            appVirtualPath.VirtualPathString, true /*nullIfNotInApp*/);
    }

    public static string ToAbsolute(string virtualPath) {
        VirtualPath virtualPathObject = VirtualPath.CreateNonRelative(virtualPath);
        return virtualPathObject.VirtualPathString;
    }

    public static string ToAbsolute(string virtualPath, string applicationPath) {
        VirtualPath virtualPathObject = VirtualPath.CreateNonRelative(virtualPath);

        // If it was already absolute, just return it
        if (virtualPathObject.VirtualPathStringIfAvailable != null)
            return virtualPathObject.VirtualPathStringIfAvailable;

        VirtualPath appVirtualPath = VirtualPath.CreateAbsoluteTrailingSlash(applicationPath);

        return UrlPath.MakeVirtualPathAppAbsolute(virtualPathObject.AppRelativeVirtualPathString,
            appVirtualPath.VirtualPathString);
    }


    /* Get pieces of virtual path */
    public static string GetFileName(string virtualPath) {
        VirtualPath virtualPathObject = VirtualPath.CreateNonRelative(virtualPath);
        return virtualPathObject.FileName;
    }

    public static string GetDirectory(string virtualPath) {
        VirtualPath virtualPathObject = VirtualPath.CreateNonRelative(virtualPath);

        virtualPathObject = virtualPathObject.Parent;
        if (virtualPathObject == null)
            return null;

        return virtualPathObject.VirtualPathStringWhicheverAvailable;
    }

    public static string GetExtension(string virtualPath) {
        VirtualPath virtualPathObject = VirtualPath.Create(virtualPath);
        return virtualPathObject.Extension;
    }

    /* Canonicalize virtual paths */
    public static string AppendTrailingSlash(string virtualPath) {
        return UrlPath.AppendSlashToPathIfNeeded(virtualPath);
    }

    public static string RemoveTrailingSlash(string virtualPath) {
        return UrlPath.RemoveSlashFromPathIfNeeded(virtualPath);
    }

// Removing Reduce per DevDiv 43118
#if OLD
    public static string Reduce(string virtualPath) {
        VirtualPath virtualPathObject = VirtualPath.CreateNonRelative(virtualPath);
        return virtualPathObject.VirtualPathString;
    }
#endif

    /* Work with multiple virtual paths */
    public static string Combine(string basePath, string relativePath) {
        VirtualPath virtualPath = VirtualPath.Combine(VirtualPath.CreateNonRelative(basePath),
            VirtualPath.Create(relativePath));
        return virtualPath.VirtualPathStringWhicheverAvailable;
    }

    public static string MakeRelative(string fromPath, string toPath) {
        return UrlPath.MakeRelative(fromPath, toPath);
    }
}


}
