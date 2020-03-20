using BrowserTabs;
using BrowserTabs.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V1.BrowserTabs;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {

            //AllTogether.PrintBrowserTabName();

            //BrowserTabs.BrowserTabs bt = new BrowserTabs.BrowserTabs();
            // BrowserTabs_v1.otro();
            //BrowserTabsFirefox btv1 = new BrowserTabsFirefox();

            //BrowserTabsChrome btv1 = new BrowserTabsChrome();

            var res = BrowserUtilities.ListTabs(BrowserProcessName.GoogleChrome);
            var res2 = BrowserUtilities.ListTabs(BrowserProcessName.Firefox);

            Console.Read();
        }
    }
}
