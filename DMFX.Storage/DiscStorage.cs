using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Storage
{
    public class DiscStorageParams : IStorageParams
    {
        public DiscStorageParams()
        {
            Parameters = new Dictionary<string, object>();
            Parameters.Add("RootFolder", null);
        }

        public Dictionary<string, object> Parameters
        {
            get;
            set;
        }
    }

    [Export("DiscStorage", typeof(IStorage))]
    public class DiscStorage : IStorage
    {
        private string _rootFolder = string.Empty;

        public IStorageParams CreateStorageParams()
        {
            return new DiscStorageParams();
        }

        public void Init(IStorageParams stgParams)
        {
            _rootFolder = (string)stgParams.Parameters["RootFolder"];
            if (!Directory.Exists(_rootFolder))
            {
                Directory.CreateDirectory(_rootFolder);
            }
        }

        public EErrorCodes Save(string regulatorCode, string companyCode, string filingName, string contentName, List<byte> content)
        {
            try
            {
                string filingFolder = PrepareFilingFolder(regulatorCode, companyCode, filingName);
                string fileName = Path.Combine(filingFolder, contentName);

                File.WriteAllBytes(fileName, content.ToArray());

                return EErrorCodes.Success;
            }
            catch (Exception)
            {
                return EErrorCodes.GeneralError;
            }
        }

        public EErrorCodes Load(string regulatorCode, string companyCode, string filingName, string contentName, List<byte> content)
        {
            try
            {
                string filePath = Path.Combine(_rootFolder, regulatorCode, companyCode, filingName, contentName);
                if (File.Exists(filePath))
                {
                    byte[] arr = File.ReadAllBytes(filePath);

                    content.AddRange(arr);

                    return EErrorCodes.Success;
                }
                else
                {
                    return EErrorCodes.FileNotFound;
                }
            }
            catch (Exception)
            {
                return EErrorCodes.GeneralError;
            }
        }

        public EErrorCodes ListItems(string regulatorCode, string companyCode, string filingName, List<string> items)
        {
            try
            {
                string filingDirPath = Path.Combine(_rootFolder, regulatorCode, companyCode, filingName);
                if (Directory.Exists(filingDirPath))
                {
                    items.AddRange( Directory.EnumerateFiles(filingDirPath) );

                    return EErrorCodes.Success;
                }
                else
                {
                    return EErrorCodes.FilingNotFound;
                }
            }
            catch (Exception)
            {
                return EErrorCodes.GeneralError;
            }
        }

        private string PrepareFilingFolder(string regulatorCode, string companyCode, string filingName)
        {
            string regFolder = Path.Combine(_rootFolder, regulatorCode);
            if (!Directory.Exists(regFolder))
            {
                Directory.CreateDirectory(regFolder);
            }

            string compFolder = Path.Combine(regFolder, companyCode);
            if (!Directory.Exists(compFolder))
            {
                Directory.CreateDirectory(compFolder);
            }

            string filingFolder = Path.Combine(compFolder, filingName);
            if (!Directory.Exists(filingFolder))
            {
                Directory.CreateDirectory(filingFolder);
            }

            return filingFolder;

        }

        
    }
}
