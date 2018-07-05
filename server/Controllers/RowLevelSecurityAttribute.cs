using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Controllers
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RowLevelSecurityAttribute:Attribute
    {
        public string KeyName { get; set; }
    }
}
