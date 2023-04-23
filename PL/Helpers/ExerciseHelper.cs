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
            if(formulas == null) formulas = new List<LogicalFormula> { newExercise };
            else formulas.Add(newExercise);
            JsonWriteToFIle(env);
            return;
        }

        private static void JsonWriteToFIle(IWebHostEnvironment env)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = System.Text.Json.JsonSerializer.Serialize(formulas, options);
            var filePath = Path.Combine(env.ContentRootPath, "Helpers", "formulas.json");
            File.WriteAllText(filePath, jsonString);
        }

        internal static void RemoveFromFormulaList(IWebHostEnvironment env, string selectedValue)
        {
            GetFormulaList(env);
            selectedValue = selectedValue.Trim();
            var parts = selectedValue.Split('|');
            parts[0] = parts[0].Trim();
            parts[1] = parts[1].Trim();
            var removedFormula = new LogicalFormula { formula = parts[0], status = parts[1] };
            formulas.RemoveAll(f => f.formula == removedFormula.formula && f.status == removedFormula.status);
            JsonWriteToFIle(env);
        }
    }
}
