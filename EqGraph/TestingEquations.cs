using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EqGraph
{
    public static class TestingEquations
    {
        static List<Equation> testEquations = new List<Equation>()
        {
            new Equation(Guid.NewGuid().ToString(), new List<string>{ "Algebraic equations" }, "ax + b = 0", "https://eqworld.ipmnet.ru/en/solutions/ae/ae0101.pdf", "Линейное уравнение"),
            new Equation(Guid.NewGuid().ToString(), new List<string>{ "Algebraic equations" }, "ax^2 + bx + c = 0", "https://eqworld.ipmnet.ru/en/solutions/ae/ae0102.pdf","Квадратное уравнение"),
            new Equation(Guid.NewGuid().ToString(), new List<string>{ "Algebraic equations" }, "ax^3 + bx^2 + cx + d = 0", "https://eqworld.ipmnet.ru/en/solutions/ae/ae0103.pdf","Кубическое уравнение"),
            new Equation(Guid.NewGuid().ToString(), new List<string>{ "Algebraic equations" }, "ax^4 + bx^2 + c = 0", "https://eqworld.ipmnet.ru/en/solutions/ae/ae0104.pdf","Биквадратное уравнение"),
            new Equation(Guid.NewGuid().ToString(), new List<string>{ "Algebraic equations" }, "ax^4 + bx^3 + cx^2 + bx + a = 0", "https://eqworld.ipmnet.ru/en/solutions/ae/ae0105.pdf","Возвратное (алгебраическое) уравнение"),
        };
    }
}
