using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BrowserTabs.V1
{
    interface IBrowserTabs
    {
        AutomationElement GetCurrentActiveWindow(string processName);
        List<BrowserWindow> ListTabsFromAllWindows(string processName);
        BrowserWindow ListTabs(AutomationElement rootWindow);

    }
}
