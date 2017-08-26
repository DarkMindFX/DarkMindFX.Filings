using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMFX.Interfaces;
using System.ComponentModel.Composition.Hosting;

namespace DMFX.SECParser
{
    [Export("SEC", typeof(IParsersRepository))]
    class SECParsersRepository : IParsersRepository
    {
        CompositionContainer _compContainer = null;
        public SECParsersRepository()
        {
            Init();
        }

        private void Init()
        {
            AggregateCatalog catalog = new AggregateCatalog();

            // adding parsers to catalog
            Prepare10QParsers(catalog);
            Prepare10KParsers(catalog);

            _compContainer = new CompositionContainer(catalog);

        }

        private void Prepare10QParsers(AggregateCatalog catalog)
        {
            var catalog10Q = new TypeCatalog(
                typeof(SEC10Q.AAPL),
                typeof(SEC10Q.AMZN),
                typeof(SEC10Q.BAC),
                typeof(SEC10Q.BRK_B),
                typeof(SEC10Q.FB),
                typeof(SEC10Q.GOOG),
                typeof(SEC10Q.JNJ),
                typeof(SEC10Q.JPM),
                typeof(SEC10Q.MSFT),
                typeof(SEC10Q.PG),
                typeof(SEC10Q.TSLA),
                typeof(SEC10Q.XON)
                );

            catalog.Catalogs.Add(catalog10Q);

        }

        private void Prepare10KParsers(AggregateCatalog catalog)
        {
            var catalog10K = new TypeCatalog(
                typeof(SEC10K.AAPL)
                );

            catalog.Catalogs.Add(catalog10K);
        }


        public IFilingParser GetParser(string company, string type)
        {
            IFilingParser result = null;

            var parsers = _compContainer.GetExports<IFilingParser>(company);
            if (parsers != null)
            {
                var parser = parsers.FirstOrDefault(p => p.Value.ReportType == type);
                if (parser != null)
                {
                    result = parser.Value;
                }
            }

            return result;
        }
    }
}
