using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;

namespace EqGraph
{
    public class NodeParser
    {
        public static string Parse(HtmlNode formula)
        {
            StringBuilder sb = new StringBuilder();
            ProcessNode(formula, ref sb);
            var result = sb.Replace("&minus;", "-")
                .Replace("&prime;", "'")
                .Replace("&lambda;", "\u03BB")
                .Replace("&alpha;", "\u03B1")
                .Replace("&beta;", "\u03B2")
                .Replace("&gamma;", "\u03B3")
                .Replace("&sigma;", "\u03C3")
                .Replace("&nu;", "\u03BD")
                .Replace("&mu;", "\u03BC")
                .Replace("&psi;", "\u03C8")
                .Replace("&nbsp;", "")
                .Replace("\r\n", "")
                .Replace(" ", "")
                .ToString();

            return result;
        }

        static void ProcessNode(HtmlNode node, ref StringBuilder res)
        {
            foreach (var child in node.ChildNodes)
            {
                if (child.Name == "sup")
                    ProcessIndex(child, ref res, '^');
                else if (child.Name == "sub")
                    ProcessIndex(child, ref res, '_');
                else if (child.HasChildNodes)
                    ProcessNode(child, ref res);
                else
                    res.Append(child.InnerText);
            }
        }

        static void ProcessIndex(HtmlNode node, ref StringBuilder res, char index)
        {
            res.Append(index);
            if (node.HasChildNodes)
            {
                res.Append("(");
                ProcessNode(node, ref res);
                res.Append(")");
            }
        }

    }
}
