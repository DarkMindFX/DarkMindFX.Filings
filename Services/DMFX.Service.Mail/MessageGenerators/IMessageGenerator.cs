using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.Mail.MessageGenerators
{
    public interface IMessageGeneratorParams
    {
        Dictionary<string, object> Parameters
        {
            get;
            set;
        }

        string Template
        {
            get;
            set;
        }
    }

    public class BaseMessageGeneratorParams : IMessageGeneratorParams
    {
        public BaseMessageGeneratorParams()
        {
            Parameters = new Dictionary<string, object>();
        }
        public Dictionary<string, object> Parameters
        {
            get;
            set;
        }

        public string Template
        {
            get;
            set;
        }
    }

    public interface IMessageGenerator
    {
        string CreateMessage(IMessageGeneratorParams msgParams);

        IMessageGeneratorParams CreateParams();
    }
}
