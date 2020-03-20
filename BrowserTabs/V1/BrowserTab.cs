using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserTabs.V1
{
    public class BrowserTab
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsCurrentTab { get; set; }
    }
}
