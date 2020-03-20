using System;
using System.Collections.Generic;
using V1.BrowserTabs;

namespace BrowserTabs.V1
{
    public class BrowserUtilities
    {
        public static List<BrowserWindow> ListTabs(string processName) {
            IBrowser browser = GetInstance(processName);

            return browser.ListTabsFromAllWindows();
        }

        private static IBrowser GetInstance(string processName)
        {
            switch (processName)
            {
                case BrowserProcessName.GoogleChrome:
                    return new Browser_Chrome();
                case BrowserProcessName.Firefox:
                    return new Browser_Firefox();
                default:
                    throw new ArgumentException("Invalid Process Name");
            }
        }
    }
}
