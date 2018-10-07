using System;
using System.Collections.Generic;
using System.Text;

namespace WhereAreYouApp.Messaging.Contexts
{
    public class StatefulContextSnapshot
    {
        public string TypeName { get; set; }
        public string NextMethodName { get; set; }
        public string StatusJson { get; set; }
    }
}
