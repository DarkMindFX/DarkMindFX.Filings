﻿using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetCompanies/{RegulatorCode}")]
    public class GetCompanies : RequestBase, IReturn<GetCompaniesResponse>
    {
        public string RegulatorCode
        {
            get;
            set;
        }
    }

    public class CompanyInfo
    {
        public string Name
        {
            get;
            set;
        }

        public string Code
        {
            get;
            set;
        }

        public DateTime? LastUpdate
        {
            get;
            set;
        }
        
    }

    public class GetCompaniesResponse : ResponseBase
    {
        public GetCompaniesResponse()
        {
            Companies = new List<CompanyInfo>();
        }
        public string RegulatorCode
        {
            get;
            set;
        }
        public List<CompanyInfo> Companies
        {
            get;
            set;
        }
    }
}
