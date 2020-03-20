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
    public class BrowserTabs_v1
    {
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

        public List<BrowserWindow> ListTabsFromAllWindows(string processName)
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
                List<BrowserTab> tabs = ListTabs(root);
                BrowserWindow window = new BrowserWindow
                {
                    CurrentTab = tabs?.Where((x) => x.IsCurrentTab)?.FirstOrDefault(),
                    Tabs = tabs,
                    IsMinimized = false // falta setearlo bien
                };

                result.Add(window);
            }

            return result;
        }

        private List<BrowserTab> ListTabs(AutomationElement root)
        {
            List<BrowserTab> result = new List<BrowserTab>();
            Condition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);
            var tabs = root.FindAll(TreeScope.Descendants, condition);

            Console.WriteLine($"Active tab: {root.Current.Name}");
            var elmUrlBar = root.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

            foreach (AutomationElement tabitem in tabs)
            {
                Console.WriteLine(tabitem.Current.Name);

                BrowserTab tab = new BrowserTab
                {
                    Name = tabitem.Current.Name,
                    Url = "",
                    IsCurrentTab = tabitem.Current.IsEnabled // falta setearlo bien
                };

                result.Add(tab);
            }

            return result;
        }



        public static void otro()
        {
            Process[] procsChrome = Process.GetProcessesByName("chrome");
            foreach (Process chrome in procsChrome)
            {
                if (chrome.MainWindowHandle == IntPtr.Zero)
                    continue;

                AutomationElement element = AutomationElement.FromHandle(chrome.MainWindowHandle);
                if (element != null)
                {
                    Condition conditions = new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, chrome.Id),
                        new PropertyCondition(AutomationElement.IsControlElementProperty, true),
                        new PropertyCondition(AutomationElement.IsContentElementProperty, true),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

                    AutomationElement elementx = element.FindFirst(TreeScope.Descendants, conditions);
                    var res = ((ValuePattern)elementx.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
                    Console.WriteLine(res);
                }
            }
        }
    }
}
