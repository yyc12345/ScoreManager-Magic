using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMMLib.Data {

    [Flags]
    public enum SM_Priority {
        None = 0,
        User = 0b1,
        Live = 0b10,
        Speedrun = 0b100,
        Admin = 0b1000
    }

    public class StandardResponse {
        public StandardResponse(bool a, string b) {
            IsSuccess = a;
            Description = b;
        }
        public bool IsSuccess = true;
        public string Description = "";
    }
}
