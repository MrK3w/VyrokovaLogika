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

        //generate random number of exercise
        public static void GeneratateNumber()
        {
            Random random = new Random();
            number = random.Next(0, formulaList.Count-1);
        }

        //get exercises from JSON
        public static void GetFormulaList(IWebHostEnvironment env)
        {
            //get path of that json
            var filePath = Path.Combine(env.ContentRootPath, "Helpers", "formulas.json");
            try
            {
                //read content of json
                var json = File.ReadAllText(filePath);
                //convert json to our formulas
                formulas = JsonConvert.DeserializeObject<List<LogicalFormula>>(json);

                formulaList = formulas
                    .Select(f => Tuple.Create(f.formula, f.status))
                    .ToList();
            }
            //if we dind't find that json 
            catch (Exception ex)
            {
                return;
            }
        }

        //save formula to json
        public static void SaveFormulaList(IWebHostEnvironment env, string formula, string formulaType)
        {
            //get all values from json
            GetFormulaList(env);
            //add new exercise
            var newExercise = new LogicalFormula { formula = formula, status = formulaType };
            // if list of exercises is empty create new list with that exercises
            if(formulas == null) formulas = new List<LogicalFormula> { newExercise };
            //otherwise just add formula to list
            else formulas.Add(newExercise);
            //write lsit to json file
            JsonWriteToFile(env);
            return;
        }

        //write list to JSON
        private static void JsonWriteToFile(IWebHostEnvironment env)
        {
            //options for json
            var options = new JsonSerializerOptions { WriteIndented = true };
            //serialize list to json
            var jsonString = System.Text.Json.JsonSerializer.Serialize(formulas, options);
            //get path of json
            var filePath = Path.Combine(env.ContentRootPath, "Helpers", "formulas.json");
            //write all text to json
            File.WriteAllText(filePath, jsonString);
        }

        //remove formula from json
        internal static void RemoveFromFormulaList(IWebHostEnvironment env, string selectedValue)
        {
            //get list from JSON
            GetFormulaList(env);
            //trim all whitespaces in input
            selectedValue = selectedValue.Trim();
            //split value by |
            var parts = selectedValue.Split('|');
            //first part is formula second one is type of exercise
            parts[0] = parts[0].Trim();
            parts[1] = parts[1].Trim();
            //create new object of formula
            var removedFormula = new LogicalFormula { formula = parts[0], status = parts[1] };
            //find that formula in list and remove
            formulas.RemoveAll(f => f.formula == removedFormula.formula && f.status == removedFormula.status);
            //write modified content to JSON
            JsonWriteToFile(env);
        }
    }
}
