using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Interfaces
{
    public enum EErrorType
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }
    public enum EErrorCodes
    {
        Success = 0,
        InvalidParserParams = 1,
        FileNotFound = 2,
        ParserError = 3,
        ImporterError = 4,
        InvalidSourceParams = 5,
        SubmissionNotFound = 6,
        GeneralError = 99999
    }
}
