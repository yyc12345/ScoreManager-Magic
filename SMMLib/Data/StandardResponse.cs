using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMMLib.Data {
    public class StandardResponse {
        public StandardResponse(bool a, string b) {
            IsSuccess = a;
            Description = b;
        }
        public bool IsSuccess = true;
        public string Description = "";
    }
}
