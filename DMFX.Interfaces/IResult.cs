using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Interfaces
{
    public interface IResult
    {
        bool Success
        {
            get;
            set;
        }

        List<Error> Errors
        {
            get;
            set;
        }

        void AddError(EErrorCodes code, EErrorType type, string message);

        void AddError(Error error);

        bool HasErrors
        {
            get;
        }

        bool HasWarnings
        {
            get;
        }

        bool HasInfo
        {
            get;            
        }
    }
}
