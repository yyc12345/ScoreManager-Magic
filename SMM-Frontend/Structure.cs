using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMM_Frontend.Structure {

    [Flags]
    public enum SM_Priority {
        None = 0,
        User = 0b1,
        Live = 0b10,
        Speedrun = 0b100,
        Admin = 0b1000
    }

    public class Structure_Salt {
        public int rnd { get; set; }
    }

    public class Structure_Login {
        public string token { get; set; }
        public int priority { get; set; }
    }

    public class Structure_Version {
        public string ver { get; set; }
    }

}
