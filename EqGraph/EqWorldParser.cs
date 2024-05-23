using HtmlAgilityPack;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;

namespace EqGraph
{
    public static class EqWorldParser
    {
        const string baseUrl = "https://eqworld.ipmnet.ru";
        const string virtuosoWebRootPath = @"C:\Program Files\OpenLink Software\Virtuoso OpenSource 7.2\vsp\output.ttl";


        public static List<Equation> ParseSite(IEnumerable<string> urls)
        {
            // Создаем класс, с помощью которого будем получать html-страницы
            HtmlWeb web = new HtmlWeb();

            // Меняем кодировку, на ту, которая используется на EqWorld
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            web.OverrideEncoding = Encoding.GetEncoding("windows-1251");

            List<Equation> result = new List<Equation>();

            // Обрабатываем каждый полученный url
            foreach (string url in urls)
            {
                // Загружаем html страницу 
                var htmlDoc = web.Load(url);

                // Получаем тэг html
                var html = htmlDoc.DocumentNode.SelectSingleNode("html");

                // Чистим текст главного контейнера от ненужных символов 
                ClearTextFrom(htmlDoc, "//div[@class='fixedwidth']", new string[] { "\r", "\n" });

                // Получаем главный тип описываемых на html странице уравнений
                string eqMainType = html.SelectSingleNode("//head/meta[@name='Description']")
                    .GetAttributeValue("content", "");

                // Получаем все списки
                var ols = html.SelectNodes("//ol");

                foreach (var ol in ols)
                {
                    var lis = ol.SelectNodes("li");

                    // Получаем подтип уравнений в списке, если он есть
                    var types = new List<string> { eqMainType };
                    var subType = ol.PreviousSibling;
                    if (subType.Name == "h3")
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var child in subType.ChildNodes)
                        {
                            if (child.Name == "span")
                                sb.Append(NodeParser.Parse(child));
                            else
                                sb.Append(child.InnerText);
                        }
                        sb.Remove(0, 5);
                        types.Add(sb.ToString());
                    }
                   
                    foreach (var li in lis)
                    {
                        // Получаем уравнение
                        var formulaSpan = li.SelectSingleNode("a/span[@class='math']");
                        if (formulaSpan == null)
                            continue;

                        // Вытаскиваем ссылку на pdf
                        var a = formulaSpan.ParentNode;
                        var eqWorldRef = a.GetAttributeValue("href", "").Replace("../../..", "").Insert(0, baseUrl);

                        // Получаем строковое представление уравнения
                        string stringForm = NodeParser.Parse(formulaSpan);

                        // Вытаскиваем название
                        var labelTag = li.SelectSingleNode("i");
                        if (labelTag == null)
                            labelTag = a.NextSibling;
#pragma warning disable CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
                        string? label = labelTag.InnerText.Replace("&nbsp;", "").Trim();
#pragma warning restore CS8632 // Аннотацию для ссылочных типов, допускающих значения NULL, следует использовать в коде только в контексте аннотаций "#nullable".
                        label = !string.IsNullOrEmpty(label) ? label : "No label";

                        result.Add(new Equation(Guid.NewGuid().ToString(), types, stringForm, eqWorldRef, label));
                    }
                }
            }
            return result;
        }

        // Метод для очистки текста тега от ненужных символов
        static void ClearTextFrom(HtmlDocument htmlDoc, string xPathToTag, IEnumerable<string> toClear)
        {
            var html = htmlDoc.DocumentNode;
            var text = html.SelectNodes($"{xPathToTag}/text()");
            foreach (var t in text)
            {
                StringBuilder innerHtml = new StringBuilder(t.InnerHtml);
                foreach (var c in toClear)
                    innerHtml.Replace(c, "");
                t.InnerHtml = innerHtml.ToString();
                t.InnerHtml = t.InnerHtml.Trim();
                if (t.InnerHtml == "")
                    t.Remove();
            }
        }

        public static void SaveAsXML(List<Equation> equations)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<Equation>));

            using (FileStream fs = new FileStream("../../../equations.xml", FileMode.Create))
            {
                formatter.Serialize(fs, equations);
            }
        }

        public static void CreateTriples()
        {
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.FileName = "java";
            procInfo.Arguments = $@"-jar rmlmapper.jar -m mapping.ttl -o ../../../output.ttl -s turtle";
            var process = Process.Start(procInfo);
            if (process != null)
            {
                process.WaitForExit();
                Console.WriteLine(process.ExitCode);
            }
            FileInfo triples = new FileInfo("output.ttl");
            triples.CopyTo(virtuosoWebRootPath);

        }
    }
}
