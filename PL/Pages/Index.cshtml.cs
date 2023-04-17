using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PL.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VyrokovaLogika;
using static VyrokovaLogika.Operator;

namespace PL.Pages
{
    public partial class IndexModel : PageModel
    {
        public enum ButtonType
        {
            DAG,
            SyntaxTree,
            CheckTautology,
            CheckContradiction,
            Exercise,
            Draw
        }
        private string vl;
        private string vl1;
        public ButtonType button { get; set; }
        private List<string> htmlTree = new List<string>();
        private List<string> htmlTreeTruth = new List<string>();
        public string ConvertedTree { get; set; }
        public string ConvertedTreeTruth { get; set; }
        public List<string> DAGNodes { get; set; } = new List<string>();
        public List<Tuple<string, string>> TreeConnections { get; set; } = new List<Tuple<string, string>>();
        private bool Green;
        public List<Tuple<string, int>> distinctNodes { get; set; } = new List<Tuple<string, int>>();
        public bool IsTautologyOrContradiction { get; set; }

        public List<SelectListItem> listItems { get; set; } = new List<SelectListItem>();
        public bool Valid { get; private set; } = true;

        public string ExerciseType { get; set; }

        public string ExerciseQuote { get; set; }

        public string xd { get; set; }

        public string formula { get; set; }
        public IndexModel()
        {
            PrepareList();
        }

        private Engine PrepareEngine(string mSentence)
        {
            Engine engine = new Engine(mSentence);
            htmlTree.Clear();
            htmlTreeTruth.Clear();
            engine.ProcessSentence();
            return engine;
        }

        private void PrepareList()
        {
            listItems = ListItemsHelper.ListItems;
        }

        public IActionResult OnPostCreateTree()
        {
            button = ButtonType.SyntaxTree;
            string mSentence = getFormula();
            if (!Valid) return Page();
            Engine engine = PrepareEngine(mSentence);

            PrintTree(engine.tree);
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        public IActionResult OnPostDrawTree()
        {
            button = ButtonType.Draw;
            string mSentence = getFormula();
            if (!Valid) return Page();
            Engine engine = PrepareEngine(mSentence);

            PrintTree(engine.tree);
            PrepareTree(engine.tree);
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        private void PrepareTree(Tree tree)
        {
            xd += tree.Item.mSentence + "<br>";
            if(tree.childNodeLeft != null)
            {
                PrepareTree(tree.childNodeLeft);
            }
            if(tree.childNodeRight != null)
            {
                PrepareTree(tree.childNodeRight);
            }
        }

        public IActionResult OnPostExercise()
        {
            button = ButtonType.Exercise;
            ExerciseHelper.GeneratateNumber();
            int number = ExerciseHelper.number;
            string f = ExerciseHelper.formulaList[number].Item1;
            ExerciseType = ExerciseHelper.formulaList[number].Item2;
            Converter.ConvertLogicalOperators(ref f);
            Validator.ValidateSentence(ref f);
            formula = f;
            ExerciseHelper.formula = f;
            Engine engine = PrepareEngine(formula);
            Console.WriteLine(f);
            if (ExerciseType == "Not Tautology" || ExerciseType == "Tautology")
            {
                IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            }
            if (ExerciseType == "Not Contradiction" || ExerciseType == "Contradiction")
            {
                IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
            }
            PrintTree(engine.counterModel,true);
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";
           
            return Page();
        }

        public IActionResult OnPostExerciseProcess(string tree)
        {
            int number = ExerciseHelper.number;
            ExerciseType = ExerciseHelper.formulaList[number].Item2;
            button = ButtonType.Exercise;
            ExerciseTreeConstructer constructer = new ExerciseTreeConstructer(tree);
            TruthTree truthTree = new TruthTree();
            if (ExerciseType == "Tautology")
            {
                truthTree = constructer.ProcessTree(true,false);
            }
            else if(ExerciseType == "Not Tautology")
            {
                truthTree = constructer.ProcessTree(true, true);
            }
            else if (ExerciseType == "Contradiction")
            {
                truthTree = constructer.ProcessTree(false, false);
            }
            else if (ExerciseType == "Not Contradiction")
            {
                truthTree = constructer.ProcessTree(false, true);
            }
            
            constructer.IsTreeOkay();
            Green = constructer.Green;
            formula = ExerciseHelper.formula;
            ExerciseQuote = constructer.ExerciseQuote;
            htmlTreeTruth.Clear();
            PrintTree(truthTree);
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";
            return Page();
        }
    
        public IActionResult OnPostCreateDAG()
        {
            button = ButtonType.DAG;
            string mSentence = getFormula();
            if (!Valid) return Page();
            Engine engine = PrepareEngine(mSentence);
            engine.ConvertTreeToDag();
            IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            engine.PrepareDAG();
            TreeConnections = engine.TreeConnections;
            DAGNodes = engine.DAGNodes;
            return Page();
        }

        public IActionResult OnPostCheckTautology()
        {
            button = ButtonType.CheckTautology;
            string mSentence = getFormula();
            if (!Valid) return Page();
            Engine engine = PrepareEngine(mSentence);

            IsTautologyOrContradiction = engine.ProofSolver("Tautology");

            distinctNodes = engine.distinctNodes;
            PrintTree(engine.counterModel);
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";

            return Page();
        }

        public IActionResult OnPostCheckContradiction()
        {
            button = ButtonType.CheckContradiction;
            string mSentence = getFormula();
            if (!Valid) return Page();
            Engine engine = PrepareEngine(mSentence);

            IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
            distinctNodes = engine.distinctNodes;
            PrintTree(engine.counterModel);
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";
            return Page();
        }

        public string getFormula()
        {
            vl = Request.Form["formula"];
            vl1 = Request.Form["UserInput"];
            if (vl == "" && vl1 == "")
            {
                Valid = false;
                return null;
            }
            if (vl1 != "")
            {
                if (!Validator.ValidateSentence(ref vl1))
                {
                    Valid = false;
                    return null;
                }
                Converter.ConvertLogicalOperators(ref vl1);
                ListItemsHelper.SetListItems(vl1);
                var selected = listItems.Where(x => x.Value == vl1).First();
                selected.Selected = true;
                return vl1;
            }

            else if (vl != "")
            {
                if (!Validator.ValidateSentence(ref vl))
                {
                    Valid = false;
                    return null;
                }
                foreach (var item in listItems)
                {
                    item.Selected = false;
                    if (vl == item.Value)
                    {
                        item.Selected = true;
                    }
                }
                return vl;
            }
            return null;
        }

        private void PrintTree(Tree tree, int i = 0)
        {
            htmlTree.Add("<li>");
            if (tree.Item.mOperator != Operator.OperatorEnum.EMPTY)
            {
                htmlTree.Add("<span class=tf-nc>" + GetEnumDescription(tree.Item.mOperator) + "</span>");
            }
            else
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");
            }
            if (tree.childNodeLeft != null && i < 1000)
            {
                htmlTree.Add("<ul>");
                PrintTree(tree.childNodeLeft, i + 1);
                if (tree.childNodeRight != null)
                {
                    PrintTree(tree.childNodeRight, i + 1);
                }
                htmlTree.Add("</ul>");
            }
            htmlTree.Add("</li>");
        }

        private void PrintTree(TruthTree tree, bool exercise = false)
        {
            htmlTreeTruth.Add("<li>");
            if(exercise)
            {
               
                if (tree.literal == null)
                    htmlTreeTruth.Add("<span class=tf-nc>" + GetEnumDescription(tree.mOperator) + "=" + "0" + "</span>");
                else
                {
                    htmlTreeTruth.Add("<span class=tf-nc>" + tree.literal + "=" + "0" + "</span>");
                }
            }
            else 
            {
                string spanValue;
                if (tree.invalid)
                {
                   if(!Green) spanValue = "<span class=tf-nc style='color: red;'>";
                   else
                    {
                        spanValue = "<span class=tf-nc style='color: green;'>";
                    }
                }
                else
                {
                    spanValue = "<span class=tf-nc>";
                }
                if (tree.literal == null)
                    htmlTreeTruth.Add(spanValue + GetEnumDescription(tree.mOperator) + "=" + tree.Item + "</span>");
                else
                {
                    htmlTreeTruth.Add(spanValue + tree.literal + "=" + tree.Item + "</span>");
                }
            }
            if (tree.ChildNodeLeft != null)
            {
                htmlTreeTruth.Add("<ul>");
                PrintTree(tree.ChildNodeLeft, exercise);
                if (tree.ChildNodeRight != null)
                {
                    PrintTree(tree.ChildNodeRight, exercise);
                }
                htmlTreeTruth.Add("</ul>");
            }
            htmlTreeTruth.Add("</li>");
        }
    }
}