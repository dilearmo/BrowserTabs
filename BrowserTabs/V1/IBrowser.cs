using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace BrowserTabs.V1
{
    interface IBrowser
    {
        List<BrowserWindow> ListTabsFromAllWindows();
        /*BrowserWindow ListTabs(AutomationElement rootWindow, Process process, string windowName);
        string FindWindowUrl(Process process);*/

    }
}
