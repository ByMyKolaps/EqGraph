using HtmlAgilityPack;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Storage;
using VDS.RDF.Update;
using VDS.RDF;

namespace EqGraph
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Чтение URL-адресов...");
            string path = "urls.txt";
            var urls = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
#pragma warning disable CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
                string? url;
#pragma warning restore CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
                while ((url = await reader.ReadLineAsync()) != null)
                {
                    urls.Add(url);
                }
            }

            Console.WriteLine("Парсинг...");
            var equations = EqWorldParser.ParseSite(urls);

            SparqlService sparqlService = new SparqlService();

            Console.WriteLine("Разметка в терминах онтологии...");
            foreach (var equation in equations)
            {
                if (equation.Label != "No label")
                {
                    var result = sparqlService.GetOMPClassIRIByLabel(equation.Label);
                    if (result != null)
                    {
                        equation.OMPClass = result;
                        continue;
                    }
                    foreach (var eqType in equation.Types)
                    {
                        result = sparqlService.GetOMPClassIRIByLabel(eqType);
                        if (result != null)
                        {
                            equation.OMPClass = result;
                            break;
                        }
                    }
                    if (equation.OMPClass == null)
                    {
                        equation.OMPClass = "http://ontomathpro.org/omp2#E1891";
                    }
                }
                else
                {
                    foreach (var eqType in equation.Types)
                    {
                        var result = sparqlService.GetOMPClassIRIByLabel(eqType);
                        if (result != null)
                        {
                            equation.OMPClass = result;
                            break;
                        }
                    }
                    if (equation.OMPClass == null)
                    {
                        equation.OMPClass = "http://ontomathpro.org/omp2#E1891";
                    }
                }
            }

            Console.WriteLine("Сохранение XML-файла...");
            EqWorldParser.SaveAsXML(equations);
            Console.WriteLine("Генерация RDF-триплетов и создание графа знаний...");
            EqWorldParser.CreateTriples();

            SparqlParameterizedString update = new SparqlParameterizedString();
            update.CommandText = "LOAD <file:/output.ttl> INTO <http://localhost:8890/EqGraph>";

            SparqlRemoteUpdateEndpoint endpoint = new SparqlRemoteUpdateEndpoint("http://localhost:8890/sparql/");

            endpoint.Update(update.ToString());
            Console.WriteLine("Граф знаний создан");

        }
    }
}