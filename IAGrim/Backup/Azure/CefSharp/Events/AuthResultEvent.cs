using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Azure.CefSharp.Events {
    class AuthResultEvent : EventArgs {
        public string Token { get; set; }
    } 
}
