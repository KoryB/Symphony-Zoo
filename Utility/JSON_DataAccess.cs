using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Symphony_Zoo_New.Models;

namespace Symphony_Zoo_New.Utility
{
    public class JSON_DataAccess
    {
        public void LoadJson()
        {
            using (StreamReader r = new StreamReader("file.json"))
            {
                string json = r.ReadToEnd();
                List<Measure> items = JsonConvert.DeserializeObject<List<Measure>>(json);
            }
        }
    }
}
