using SHDocVw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;
using System.Windows.Controls;

namespace BrowserTabs
{
    public class BrowserTabs
    {
        private delegate void SafeCallDelegate(string text);

        [STAThread]
        public ListBox getAllRunningIEURLs()
        {
            try
            {
                var list = new ListBox();
                foreach (InternetExplorer browser in new ShellWindows())
                {
                    list.Items.Add(browser.LocationURL.ToString());
                }
                return list;
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        //private var foo; public void SearchForFooCallbackMethod(IAsyncResult ar) { foo = GetFooFromAsyncResult(ar); Thread thread = new Thread(ProcessInkPresenter); thread.SetApartmentState(ApartmentState.sta); thread.Start(); thread.Join(); }
        //private void ProcessInkPresenter() { var inkPresenter = XamlReader.Parse(foo.Xaml) as InkPresenter; }


        public void ListAllTabs()
        {
            Process[] procsChrome = Process.GetProcessesByName("chrome");
            if (procsChrome.Length <= 0)
            {
                Console.WriteLine("Chrome is not running");
            }
            else
            {
                foreach (Process proc in procsChrome)
                {
                    // the chrome process must have a window 
                    if (proc.MainWindowHandle == IntPtr.Zero)
                    {
                        continue;
                    }


                    AutomationElement root2 = AutomationElement.FromHandle(proc.MainWindowHandle);
                    Condition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);
                    var tabs = root2.FindAll(TreeScope.Descendants, condition);

                    //// to find the tabs we first need to locate something reliable - the 'New Tab' button 
                    //AutomationElement root = AutomationElement.FromHandle(proc.MainWindowHandle);
                    //Condition condNewTab = new PropertyCondition(AutomationElement.NameProperty, "Nueva Pestaña");
                    //AutomationElement elmNewTab = root.FindFirst(TreeScope.Descendants, condNewTab);
                    //// get the tabstrip by getting the parent of the 'new tab' button 
                    //TreeWalker treewalker = TreeWalker.ControlViewWalker;
                    //AutomationElement elmTabStrip = treewalker.GetParent(elmNewTab);
                    //// loop through all the tabs and get the names which is the page title 
                    //Condition condTabItem = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);
                    Console.WriteLine($"Active tab: {root2.Current.Name}");
                    foreach (AutomationElement tabitem in tabs)/*elmTabStrip.FindAll(TreeScope.Children, condTabItem))*/
                    {
                        Console.WriteLine(tabitem.Current.Name);
                    }
                }
            }
        }
    }
}
