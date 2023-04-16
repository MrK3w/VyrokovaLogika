using System;

namespace PL.Helpers
{
    public static class ExerciseHelper
    {
        public static string formula { get; set; }

        public static int number { get; set; }

        public static List<Tuple<string, string>> formulaList { get; set; } = new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("(p∧(q>r))>((p∧q)∨(p∧r))", "Not Tautology"),
            new Tuple<string, string>("(p>q)=(-q>-p)", "Tautology"),
            new Tuple<string, string>("a>b", "Not Tautology"),
            new Tuple<string, string>("(A | B) & ((-A) & (-B))", "Not Contradiction"),
            new Tuple<string, string>("P&-P", "Contradiction")
        };

        public static void GeneratateNumber()
        {
            Random random = new Random();
            number = random.Next(0, formulaList.Count-1);
        }
    }
}
