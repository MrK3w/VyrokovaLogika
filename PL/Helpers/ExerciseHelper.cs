using Newtonsoft.Json;
using System;
using System.Text.Json;

namespace PL.Helpers
{
    public class LogicalFormula
    {
        public string formula { get; set; }
        public string status { get; set; }
    }
    public static class ExerciseHelper
    {
        public static string formula { get; set; }

        public static int number { get; set; }
        static List<LogicalFormula> formulas;

        public static List<Tuple<string, string>> formulaList { get; set; } = new List<Tuple<string, string>>();

        public static void GeneratateNumber()
        {
            Random random = new Random();
            number = random.Next(0, formulaList.Count-1);
        }

        public static void GetFormulaList(IWebHostEnvironment env)
        {

            var filePath = Path.Combine(env.ContentRootPath, "Helpers", "formulas.json");

            try
            {
                var json = File.ReadAllText(filePath);
                formulas = JsonConvert.DeserializeObject<List<LogicalFormula>>(json);

                // Convert the list of LogicalFormula objects to a list of Tuples
                formulaList = formulas
                    .Select(f => Tuple.Create(f.formula, f.status))
                    .ToList();
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public static void SaveFormulaList(IWebHostEnvironment env, string formula, string formulaType)
        {
            GetFormulaList(env);
            var newExercise = new LogicalFormula { formula = formula, status = formulaType };
            formulas.Add(newExercise);
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = System.Text.Json.JsonSerializer.Serialize(formulas, options);
            var filePath = Path.Combine(env.ContentRootPath, "Helpers", "formulas.json");
            File.WriteAllText(filePath, jsonString);
            return;
        }
    }
}
