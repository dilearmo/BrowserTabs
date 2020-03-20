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

            BrowserTabs.BrowserTabs bt = new BrowserTabs.BrowserTabs();
           // BrowserTabs_v1.otro();
            BrowserTabs_v1 btv1 = new BrowserTabs_v1();

            var res = btv1.ListTabsFromAllWindows(BrowserProcessName.GoogleChrome);

            bt.ListAllTabs();
            bt.getAllRunningIEURLs();

            Console.Read();
        }
    }
}
