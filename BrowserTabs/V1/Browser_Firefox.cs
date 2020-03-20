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
                AutomationElementCollection elements = rootElement.FindAll(TreeScope.Descendants, condDocAll);

                foreach (AutomationElement docElement in elements)
                {
                    List<BrowserTab> tabs = ListTabs(docElement);
                    BrowserWindow window = new BrowserWindow
                    {
                        CurrentTab = tabs?.Where((x) => x.IsCurrentTab)?.FirstOrDefault(),
                        Tabs = tabs,
                        IsMinimized = tabs.Count == 0 // falta setearlo bien
                    };

                    result.Add(window);
                }
            }
            return result;
        }

        private List<BrowserTab> ListTabs(AutomationElement docElement)
        {
            List<BrowserTab> result = new List<BrowserTab>();
            foreach (AutomationPattern pattern in docElement.GetSupportedPatterns())
            {
                if (docElement.GetCurrentPattern(pattern) is ValuePattern)
                {
                    var elementName = (docElement.GetCurrentPattern(pattern) as ValuePattern).Current.Value.ToString();
                    Console.WriteLine(elementName);
                    string windowName = docElement.Current.Name;
                    result.Add(new BrowserTab
                    {
                        Name = elementName,
                        Url = windowName.Contains(elementName) ? FindWindowUrl(Process.GetProcessById(docElement.Current.ProcessId)) : string.Empty,
                        IsCurrentTab = windowName.Contains(elementName)
                    });
                }
            }
            return result;
        }

        private string FindWindowUrl(Process process)
        {
            AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
            if (element != null)
            {
                Condition conditions = new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id),
                    new PropertyCondition(AutomationElement.IsControlElementProperty, true),
                    new PropertyCondition(AutomationElement.IsContentElementProperty, true),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

                AutomationElement elementx = element.FindFirst(TreeScope.Descendants, conditions);
                var res = ((ValuePattern)elementx.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
                return res;
            }

            return string.Empty;
        }
    }
}
