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
        /// <param name="content">content to be saved</param>
        EErrorCodes Save(string regulatorCode, string companyCode, string filingName, string contentName, List<byte> content);

        /// <summary>
        /// Loads content of the filing
        /// </summary>
        /// <param name="regulatorCode"></param>
        /// <param name="companyCode"></param>
        /// <param name="filingName"></param>
        /// <param name="contentName"></param>
        /// <param name="content">returns extracted content</param>
        /// <returns></returns>
        EErrorCodes Load(string regulatorCode, string companyCode, string filingName, string contentName, List<byte> content);

        /// <summary>
        /// Returns list of the items in the given filing
        /// </summary>
        /// <param name="regulatorCode"></param>
        /// <param name="companyCode"></param>
        /// <param name="filingName"></param>
        /// <param name="items">returns list of items in the filing</param>
        /// <returns></returns>
        EErrorCodes ListItems(string regulatorCode, string companyCode, string filingName, List<string> items);

        IStorageParams CreateStorageParams();
    }
}
