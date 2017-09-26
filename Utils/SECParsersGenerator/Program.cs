using DMFX.Dictionaries;
using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SECParsersGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            FileDictionary fd = new FileDictionary();

            List<CompanyInfo> companies = fd.GetCompaniesByRegulator("SEC");

            // class template
            string classTemplate10Q = Resources.Class10QTemplate;
            string classTemplate10K = Resources.Class10KTemplate;
            string xmlTemplate = Resources.CompanyTagsTemplate;

            string projectRoot = "..\\..\\..\\DMFX.SECParser\\";

            string comp10QCatalogRegister = "new TypeCatalog(";
            string comp10KCatalogRegister = "new TypeCatalog(";
            string csvNames = "";

            foreach (var c in companies)
            {
                // 10-Q report classes
                {
                    Console.WriteLine("Generating: " + c.Code + " (" + c.Name + ")");
                    string className = c.Code.Replace(".", "_");
                    string classFileName = Path.Combine(projectRoot, "SEC10Q", c.Code + ".cs");

                    comp10QCatalogRegister += string.Format("typeof(SEC10Q.{0}),\r\n", className);
                    csvNames += string.Format("result.Add(\"{0}\");\r\n", c.Code);

                    if (!File.Exists(classFileName))
                    {
                        string content = classTemplate10Q.Replace("{{CODE}}", className);
                        File.WriteAllText(classFileName, content);
                    }
                }

                // 10-K report classes
                {
                    Console.WriteLine("Generating: " + c.Code + " (" + c.Name + ")");
                    string className = c.Code.Replace(".", "_");
                    string classFileName = Path.Combine(projectRoot, "SEC10K", c.Code + ".cs");

                    comp10KCatalogRegister += string.Format("typeof(SEC10K.{0}),\r\n", className);
                    csvNames += string.Format("result.Add(\"{0}\");\r\n", c.Code);

                    if (!File.Exists(classFileName))
                    {
                        string content = classTemplate10K.Replace("{{CODE}}", className);
                        File.WriteAllText(classFileName, content);
                    }
                }

                // Parser XML resources
                {

                    string xmlFileName = Path.Combine(projectRoot, "Resources", c.Code + ".xml");
                    if (!File.Exists(xmlFileName))
                    {
                        string content = xmlTemplate.Replace("{{CODE}}", c.Code.ToLowerInvariant());
                        File.WriteAllText(xmlFileName, content);

                    }
                }
            }

            comp10QCatalogRegister += ")";
            comp10KCatalogRegister += ")";


        }
    }
}
