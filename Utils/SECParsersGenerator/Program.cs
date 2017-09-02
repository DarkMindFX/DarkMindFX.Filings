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
            string classTemplate = Resources.Class10QTemplate;
            string xmlTemplate = Resources.CompanyTagsTemplate;

            string projectRoot = "..\\..\\..\\DMFX.SECParser\\";

            foreach (var c in companies)
            {
                Console.WriteLine("Generating: " + c.Code + " (" + c.Name + ")");
                string classFileName = Path.Combine(projectRoot, "SEC10Q", c.Code + ".cs");
                if (!File.Exists(classFileName))
                {
                    string content = classTemplate.Replace("{{CODE}}", c.Code.Replace(".", "_"));
                    File.WriteAllText(classFileName, content);

                }

                string xmlFileName = Path.Combine(projectRoot, "Resources", c.Code + ".xml");
                if (!File.Exists(xmlFileName))
                {
                    string content = xmlTemplate.Replace("{{CODE}}", c.Code.ToLowerInvariant());
                    File.WriteAllText(xmlFileName, content);

                }
            }
             
        }
    }
}
