using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mx_csharp.Models
{
    public class AdRequest
    {
        public string Id { get; set; }
        public App App { get; set; }
        public Device Device { get; set; }
    }
}
