using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace NovinDevHubStaffCore.Core
{
    public static class ApplicationInspector
    {
        public static string GetChromeUrlAddress(IntPtr handle)
        {

            // create automation element from MainWindowHandle
            AutomationElement elm = AutomationElement.FromHandle(handle);

            //Find EditBox inside Toolbar of chrome
            AutomationElement elmUrlEdit = null;
            try
            {
                //Find element contains Google Chrome
                var elm1 = elm.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));
                if (elm1 == null) { return ""; } // not the right chrome.exe
                                                // here, you can optionally check if Incognito is enabled:
                                                //bool bIncognito = TreeWalker.RawViewWalker.GetFirstChild(TreeWalker.RawViewWalker.GetFirstChild(elm1)) != null;

                //find Toolbar element inside elm1
                var walker = new TreeWalker(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));
                AutomationElement toolbarNode = walker.GetFirstChild(elm1);

                //find Edit element inside Toolbar
                walker = new TreeWalker(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
                elmUrlEdit = walker.GetFirstChild(toolbarNode);
            }
            catch
            {
                return "";
            }

            // make sure it's valid
            if (elmUrlEdit == null)
            {
                // it's not..
                return "";
            }

            // elmUrlEdit is now the URL Edit element. 
            // Make sure that it's out of keyboard focus to get a valid URL
            if ((bool)elmUrlEdit.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty))
            {
                return "";
            }

            // there might not be a valid pattern to use, so we have to make sure we have one
            AutomationPattern[] patterns = elmUrlEdit.GetSupportedPatterns();
            if (patterns.Length == 1)
            {
                string url = "";
                try
                {
                    url = ((ValuePattern)elmUrlEdit.GetCurrentPattern(patterns[0])).Current.Value;
                }
                catch { }
                if (url != "")
                {
                    // must match a domain name (and possibly "https://" in front)
                    if (Regex.IsMatch(url, @"^(https:\/\/)?[a-zA-Z0-9\-\.]+(\.[a-zA-Z]{2,4}).*$"))
                    {
                        // prepend http:// to the url, because Chrome hides it if it's not SSL
                        if (!url.StartsWith("http"))
                        {
                            url = "http://" + url;
                        }
                        return url;
                    }
                }
                return "";
            }

            return "";
        }

        public static string GetFireFoxUrlAddress(IntPtr handle)
        {
            // create automation element from MainWindowHandle
            AutomationElement elm = AutomationElement.FromHandle(handle);

            //Find EditBox inside Toolbar of chrome
            AutomationElement elmUrlEdit = null;
            try
            {
                //Find element with name Navigation(it's a toolbar)
                var elm1 = elm.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Navigation"));
                if (elm1 == null) { return ""; } // not the right chrome.exe
                                                // here, you can optionally check if Incognito is enabled:
                                                //bool bIncognito = TreeWalker.RawViewWalker.GetFirstChild(TreeWalker.RawViewWalker.GetFirstChild(elm1)) != null;

                //find ComboBox element inside elm1
                var walker = new TreeWalker(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ComboBox));
                AutomationElement toolbarNode = walker.GetFirstChild(elm1);

                //find Edit element inside ComboBox
                walker = new TreeWalker(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
                elmUrlEdit = walker.GetFirstChild(toolbarNode);
            }
            catch
            {
                return "";
            }

            // make sure it's valid
            if (elmUrlEdit == null)
            {
                // it's not..
                return "";
            }

            // elmUrlEdit is now the URL Edit element. 
            // Make sure that it's out of keyboard focus to get a valid URL
            if ((bool)elmUrlEdit.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty))
            {
                return "";
            }

            // there might not be a valid pattern to use, so we have to make sure we have one
            AutomationPattern[] patterns = elmUrlEdit.GetSupportedPatterns();
            foreach (var pattern in patterns)
                if (elmUrlEdit.GetCurrentPattern(pattern) is ValuePattern)
                {
                    string url = "";
                    try
                    {
                        url = ((ValuePattern)elmUrlEdit.GetCurrentPattern(pattern)).Current.Value;
                    }
                    catch { }
                    if (url != "")
                    {
                        // must match a domain name (and possibly "https://" in front)
                        if (Regex.IsMatch(url, @"^(https:\/\/)?[a-zA-Z0-9\-\.]+(\.[a-zA-Z]{2,4}).*$"))
                        {
                            // prepend http:// to the url, because Chrome hides it if it's not SSL
                            if (!url.StartsWith("http"))
                            {
                                url = "http://" + url;
                            }
                            return url;
                        }
                    }
                    continue;
                }

            return "";
        }
    }
}
