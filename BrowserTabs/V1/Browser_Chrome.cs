using BrowserTabs.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;

namespace V1.BrowserTabs
{
    class Browser_Chrome : IBrowser
    {
        private string processName = BrowserProcessName.GoogleChrome;

        public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
        [DllImport("user32.dll")]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumWindowsProc ewp, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern uint GetWindowText(IntPtr hWnd, StringBuilder lpString, uint nMaxCount);

        [DllImport("user32.dll")]
        private static extern uint GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public List<BrowserWindow> ListTabsFromAllWindows()
        {
            List<BrowserWindow> result = new List<BrowserWindow>();

            Process[] process = Process.GetProcessesByName(processName.ToString());
            List<uint> processIds = process.Select(x => Convert.ToUInt32(x.Id)).ToList();

            List<IntPtr> windowHandles = new List<IntPtr>();

            EnumWindowsProc enumerateHandle = delegate (IntPtr hWnd, int lParam)
            {
                uint id;
                GetWindowThreadProcessId(hWnd, out id);

                if (processIds.Contains(id))
                {
                    var clsName = new StringBuilder(256);
                    var hasClass = GetClassName(hWnd, clsName, 256);
                    if (hasClass)
                    {
                        var maxLength = (int)GetWindowTextLength(hWnd);
                        var builder = new StringBuilder(maxLength + 1);
                        GetWindowText(hWnd, builder, (uint)builder.Capacity);

                        var text = builder.ToString();
                        var className = clsName.ToString();

                        if (!string.IsNullOrWhiteSpace(text) && className.Equals("Chrome_WidgetWin_1", StringComparison.OrdinalIgnoreCase))
                        {
                            windowHandles.Add(hWnd);
                        }
                    }
                }
                return true;
            };

            EnumDesktopWindows(IntPtr.Zero, enumerateHandle, 0);

            foreach (IntPtr ptr in windowHandles)
            {
                AutomationElement root = AutomationElement.FromHandle(ptr);
                Process rootProcess = Process.GetProcessById(root.Current.ProcessId);
                List<BrowserTab> tabs = ListTabs(root, rootProcess, root.Current.Name);
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

        protected List<BrowserTab> ListTabs(AutomationElement root, Process process, string windowName)
        {
            List<BrowserTab> result = new List<BrowserTab>();
            Condition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);
            var tabs = root.FindAll(TreeScope.Descendants, condition);

            Console.WriteLine($"Active tab: {root.Current.Name}");
            //var elmUrlBar = root.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

            //var buttons = root.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));

            foreach (AutomationElement tabitem in tabs)
            {
                Console.WriteLine(tabitem.Current.Name);

                BrowserTab tab = new BrowserTab
                {
                    Name = tabitem.Current.Name,
                    Url = windowName.Contains(tabitem.Current.Name) ? FindWindowUrl(process) : string.Empty,
                    IsCurrentTab = windowName.Contains(tabitem.Current.Name)
                };

                result.Add(tab);
            }

            return result;
        }



        protected string FindWindowUrl(Process process)
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
