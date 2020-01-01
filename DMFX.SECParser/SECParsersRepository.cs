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
            PrepareForm4Parsers(catalog);
            //PrepareForm13FParsers(catalog); // TODO: Uncomment when fully ready

            _compContainer = new CompositionContainer(catalog);

        }

        private void Prepare10QParsers(AggregateCatalog catalog)
        {
            var catalog10Q = new TypeCatalog(typeof(SEC10Q.SEC10QDefaultParser),
                                                typeof(SEC10Q.AAPL)
                                            );
            catalog.Catalogs.Add(catalog10Q);

        }

        private void Prepare10KParsers(AggregateCatalog catalog)
        {
            var catalog10K = new TypeCatalog(typeof(SEC10K.SEC10KDefaultParser),
                                                typeof(SEC10K.AAPL)
                                            );
            catalog.Catalogs.Add(catalog10K);
        }

        private void PrepareForm4Parsers(AggregateCatalog catalog)
        {
            var catalogForm4 = new TypeCatalog(typeof(SECForm4.SECForm4DefaultParser));
            catalog.Catalogs.Add(catalogForm4);
        }

        private void PrepareForm13FParsers(AggregateCatalog catalog)
        {
            var catalogForm13F = new TypeCatalog(typeof(SECForm13F.SECForm13FDefaultParser));
            catalog.Catalogs.Add(catalogForm13F);
        }


        public IFilingParser GetParser(string company, string type)
        {
            IFilingParser result = null;

            // first trying to get company-specific parser
            var parser = _compContainer.GetExports<IFilingParser>(company).FirstOrDefault(p => p.Value.ReportType == type);
            if (parser == null)
            {
                // no company-specific parser were found - trying to get default
                parser = _compContainer.GetExports<IFilingParser>("Default").FirstOrDefault(p => p.Value.ReportType == type);
            }
            if (parser != null)
            {
                result = parser.Value;
            }

            return result;
        }
    }
}
