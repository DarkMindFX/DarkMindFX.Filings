using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Interfaces
{
    public interface IStorageParams
    {
        Dictionary<string, object> Parameters
        {
            get;
            set;
        }
    }

    public interface IStorage
    {
        /// <summary>
        /// Initializes the storage
        /// </summary>
        /// <param name="stgParams"></param>
        void Init(IStorageParams stgParams);

        /// <summary>
        /// Stores content of the filing
        /// </summary>
        /// <param name="regulatorCode"></param>
        /// <param name="companyCode"></param>
        /// <param name="filingName"></param>
        /// <param name="contentName"></param>
        /// <param name="content"></param>
        void Save(string regulatorCode, string companyCode, string filingName, string contentName, byte[] content);

        IStorageParams CreateStorageParams();
    }
}
