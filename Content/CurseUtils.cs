using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMP.Content
{
    public class CurseUtils
    {
        public string EncriptCode(string[] code)
        {
            //string[] code = new string[8] { "Covid20", "1", "true", "true", "false", "false", "false", "600" };
            string code2 = "Covid20:111000600";
            string code3;
            int colon = code2.IndexOf(":");
            code3 = code2[..colon] + ((int)code2[colon + 1] + 5) + ((int)code2[colon + 2] + 3) + ((int)code2[colon + 3] + 4) + ((int)code2[colon + 4] + 1) + ((int)code2[colon+ 5] + 7) + ((int)code2[colon + 6] + 5) + ((int)(code2[colon+7]) + 43);



            return code3;
        }
    }
}
