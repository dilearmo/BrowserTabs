using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrowserTabs.V1
{
    public class AllTogether
    {
        [DllImport("user32.dll")]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public static void PrintBrowserTabName()
        {
            var browsersList = new List<string>
            {
                "chrome",
                "firefox",
                "iexplore",
                "safari",
                "opera",
                "edge"
            };

            foreach (var singleBrowser in browsersList)
            {
                var processes = Process.GetProcessesByName(singleBrowser);
                if (processes.Length > 0)
                {
                    foreach (Process p in processes)
                    {
                        IntPtr hWnd = p.MainWindowHandle;
                        int length = GetWindowTextLength(hWnd);

                        StringBuilder text = new StringBuilder(length + 1);
                        GetWindowText(hWnd, text, text.Capacity);
                        Console.WriteLine(text.ToString());
                    }
                }
            }
        }
    }
}
