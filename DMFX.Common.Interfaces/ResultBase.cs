using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Interfaces
{
    public class ResultBase : IResult
    {
        public ResultBase()
        {
            Success = true;
            Errors = new List<Error>();
        }
        public List<Error> Errors
        {
            get;
            set;
        }

        public bool Success
        {
            get;
            set;
        }

        public bool HasErrors
        {
            get
            {
                return Errors.Any(e => e.Type == EErrorType.Error);
            }
        }

        public bool HasWarnings
        {
            get
            {
                return Errors.Any(e => e.Type == EErrorType.Warning);
            }
        }

        public bool HasInfo
        {
            get
            {
                return Errors.Any(e => e.Type == EErrorType.Info);
            }
        }

        public void AddError(EErrorCodes code, EErrorType type, string message)
        {
            Error err = new Error() { Code = code, Message = message, Type = type };
            AddError(err);
        }

        public void AddError(Error error)
        {
            Errors.Add(error);
        }
    }
}
