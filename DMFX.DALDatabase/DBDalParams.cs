using DMFX.Interfaces;
using DMFX.Interfaces.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.DALDatabase
{
    public class DBDalParams : IDalParams
    {
        public DBDalParams()
        {
            Parameters = new Dictionary<string, string>();
        }
        public Dictionary<string, string> Parameters
        {
            get;
            private set;
        }
    }
}
