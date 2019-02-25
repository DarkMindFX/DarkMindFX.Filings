﻿using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetPortfolio/{Name}/{SessionToken}", "GET")]
    [Route("/GetPortfolio", "POST")]
    public class GetPortfolio : RequestBase, IReturn<GetPortfolioResult>
    {
        public string Name
        {
            get;
            set;
        }
    }

    public class GetPortfolioResult : ResponseBase
    {
        public Portfolio Portfolio
        {
            get;
            set;
        }
    }
}
