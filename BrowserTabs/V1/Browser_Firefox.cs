using BrowserTabs.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation;

namespace V1.BrowserTabs
{
    class Browser_Firefox : IBrowser
    {

        private string processName = BrowserProcessName.Firefox;

        public List<BrowserWindow> ListTabsFromAllWindows()
        {
            List<BrowserWindow> result = new List<BrowserWindow>();

            Process[] processes = Process.GetProcessesByName("firefox");
            List<IntPtr> mainWindowHandleList = new List<IntPtr>();

            foreach (Process process in processes)
            {
                if (process.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }

                AutomationElement rootElement = AutomationElement.FromHandle(process.MainWindowHandle);

                Condition condDocAll = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document);
                AutomationElementCollection tabElements = rootElement.FindAll(TreeScope.Descendants, condDocAll);
                List<BrowserTab> tabs = ListTabs(tabElements, rootElement.Current.Name);

                BrowserWindow window = new BrowserWindow
                {
                    CurrentTab = tabs?.Where((x) => x.IsCurrentTab)?.FirstOrDefault(),
                    Tabs = tabs,
                    IsMinimized = tabs.Count == 0 // falta setearlo bien
                };

                result.Add(window);
            }
            return result;
        }

        private List<BrowserTab> ListTabs(AutomationElementCollection tabElements, string windowName)
        {
            List<BrowserTab> result = new List<BrowserTab>();
            foreach (AutomationElement tab in tabElements)
            {
                result.Add(new BrowserTab
                {
                    Name = tab.Current.Name,
                    Url = GetUrl(tab),
                    IsCurrentTab = windowName.Contains(tab.Current.Name)
                });
            }
            return result;
        }

        private string GetUrl(AutomationElement docElement)
        {
            foreach (AutomationPattern pattern in docElement.GetSupportedPatterns())
            {
                if (docElement.GetCurrentPattern(pattern) is ValuePattern)
                {
                    var url = (docElement.GetCurrentPattern(pattern) as ValuePattern).Current.Value.ToString();
                    return url;
                }
            }
            return string.Empty;
        }
    }
}
