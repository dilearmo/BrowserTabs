using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserTabs.V1
{
    public class BrowserWindow
    {
        public BrowserTab CurrentTab { get; set; }
        public List<BrowserTab> Tabs { get; set; }
        public bool IsMinimized { get; set; }
    }
}
