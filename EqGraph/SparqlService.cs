using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Ontology;
using VDS.RDF.Query;
using static System.Net.WebRequestMethods;

namespace EqGraph
{
    public class SparqlService
    {
        public Uri EndpointUri = new Uri("http://localhost:8890/sparql/");
        public static string DefaultGraphUri = "http://localhost:8890/OntoMathPro";
        public static string OntologyIRI = "http://ontomathpro.org/omp2#";
        public static string OntologyPrefix = "omp2";
        public SparqlRemoteEndpoint endpoint;
        

        public SparqlService()
        {
            endpoint = new SparqlRemoteEndpoint(EndpointUri, DefaultGraphUri);

        }

#pragma warning disable CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        public string? GetOMPClassIRIByLabel(string label)
#pragma warning restore CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        {
            var results = endpoint.QueryWithResultSet(
                    $"PREFIX omp2: <http://ontomathpro.org/omp2#> " +
                    $"SELECT ?s WHERE " +
                    $"{{" +
                        $"?s rdfs:subClassOf*/rdfs:subClassOf omp2:E1891 ." +
                        $"?s rdfs:label \"{label}\"@ru ." +
                    $"}} "
                    );
            if (results.IsEmpty)
                return null;
            var ompClass = results[0].Value("s");
            return ompClass.ToString();
        }
    }
}
