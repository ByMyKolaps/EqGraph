namespace EqGraph
{
    public class Equation
    {
        public string IRI { get; set; }
        // Поменять на список, т.к названий может быть много
#pragma warning disable CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        public string? Label { get; set; }
#pragma warning restore CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
        public List<string> Types { get; set; }

        public string OMPClass { get; set; }
        public string StringForm { get; set; }
        // Поменять на список, т.к ссылок может быть много
        public string EqWorldRef { get; set; }

        // Реализовать
        /*public string LatexForm { get; set; }
        public string HTMLForm { get; set; }*/

        public Equation()
        {
            
        }

        public Equation(string iri, IEnumerable<string> types, string stringForm, string eqWorldRef, string label)
        {
            IRI = iri;
            Label = label;
            Types = types.ToList();
            StringForm = stringForm;
            EqWorldRef = eqWorldRef;
        }
    }
}
