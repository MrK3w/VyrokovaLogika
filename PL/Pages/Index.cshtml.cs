using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PL.Helpers;
using VyrokovaLogika;
using static VyrokovaLogika.Operator;

namespace PL.Pages
{
    public partial class IndexModel : PageModel
    {
        public enum ButtonType
        {
            None,
            DAG,
            SyntaxTree,
            CheckTautology,
            CheckContradiction,
            Exercise,
            Draw,
            CheckTautologyDAG,
            CheckContradictionDAG,
            ExerciseDAG
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

        public string Arguments { get; set; }

        public int mIssueIndex { get; set; } = -1;

        public string ExerciseFormula { get; set; }
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

            DrawTree(engine.tree);
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        private void DrawTree(Tree tree)
        {
            htmlTree.Add("<li>");
            htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");

            if (tree.childNodeLeft != null)
            {
                htmlTree.Add("<ul>");
                DrawTree(tree.childNodeLeft);
                if (tree.childNodeRight != null)
                {
                    DrawTree(tree.childNodeRight);
                }
                htmlTree.Add("</ul>");
            }
            htmlTree.Add("</li>");
        }

        public IActionResult OnPostExercise()
        {
            foreach (var item in listItems)
            {
                item.Selected = false;
            }

            button = ButtonType.Exercise;
            ExerciseHelper.GeneratateNumber();
            Valid = true;
            int number = ExerciseHelper.number;
            string f = ExerciseHelper.formulaList[number].Item1;
            ExerciseType = ExerciseHelper.formulaList[number].Item2;
            Converter.ConvertLogicalOperators(ref f);
            Validator.ValidateSentence(ref f);
            ExerciseFormula = f;
            ExerciseHelper.formula = f;
            Engine engine = PrepareEngine(ExerciseFormula);
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
            foreach (var item in listItems)
            {
                item.Selected = false;
            }
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
            ExerciseFormula = ExerciseHelper.formula;
            ExerciseQuote = constructer.ExerciseQuote;
            htmlTreeTruth.Clear();
            PrintTree(truthTree);
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";
            return Page();
        }

        public IActionResult OnPostExerciseDAG()
        {
            foreach (var item in listItems)
            {
                item.Selected = false;
            }
            button = ButtonType.ExerciseDAG;
            ExerciseHelper.GeneratateNumber();
            int number = ExerciseHelper.number;
            string f = ExerciseHelper.formulaList[number].Item1;
            ExerciseType = ExerciseHelper.formulaList[number].Item2;
            Converter.ConvertLogicalOperators(ref f);
            Validator.ValidateSentence(ref f);
            ExerciseFormula = f;
            ExerciseHelper.formula = f;
            Valid = true;
            Engine engine = PrepareEngine(ExerciseFormula);
            if (ExerciseType == "Not Tautology" || ExerciseType == "Tautology")
            {
                IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            }
            if (ExerciseType == "Not Contradiction" || ExerciseType == "Contradiction")
            {
                IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
            }
            engine.ConvertTreeToDag();
            engine.PrepareDAG(true);
            TreeConnections = engine.TreeConnections;
            DAGNodes = engine.DAGNodes;
            return Page();
        }

        public IActionResult OnPostExerciseProcessDAG(string pDAGNodes, string DAGPath)
        {
            foreach (var item in listItems)
            {
                item.Selected = false;
            }
            int number = ExerciseHelper.number;
            ExerciseType = ExerciseHelper.formulaList[number].Item2;
            button = ButtonType.ExerciseDAG;
            List<JsonTreeNodes> nodeList = JsonConvert.DeserializeObject<List<JsonTreeNodes>>(pDAGNodes);
            List<JsonEdges> edgeList = JsonConvert.DeserializeObject<List<JsonEdges>>(DAGPath);
            ExerciseFormula = ExerciseHelper.formula;
           
            TreeConnections = edgeList.Select(edge => Tuple.Create(edge.From, edge.To)).ToList();
            DAGNodes = nodeList.Select(edge => edge.Label).ToList();
            TreeConnections = PrepareTreeConnections();
            
            TruthDagVerifier verifier;
            switch (ExerciseType)
            {
                case "Tautology":
                    verifier = new TruthDagVerifier(TreeConnections, DAGNodes, true, false);
                    break;
                case "Not Tautology":
                    verifier = new TruthDagVerifier(TreeConnections, DAGNodes,  true, true);
                    break;
                case "Contradiction":
                    verifier = new TruthDagVerifier(TreeConnections, DAGNodes, false, false);
                    break;
                case "Not Contradiction":
                    verifier = new TruthDagVerifier(TreeConnections, DAGNodes, false, true);
                    break;
                default:
                    return Page();
            }
            mIssueIndex = verifier.mIssueIndex;
            ExerciseQuote = verifier.ExerciseQuote;
            return Page();
        }

        private List<Tuple<string, string>> PrepareTreeConnections()
        {
            List<Tuple<string, string>> modifiedTries = new List<Tuple<string, string>>();
            for (int i = 0; i < TreeConnections.Count; i++)
            {
                var item1PartsFull = TreeConnections[i].Item1.Split(new[] { "=" }, 2, StringSplitOptions.None);
                var item2PartsFull = TreeConnections[i].Item2.Split(new[] { "=" }, 2, StringSplitOptions.None);
                string item1Parts = item1PartsFull[0];
                string item2Parts = item2PartsFull[0];
                for (int j = 0; j < DAGNodes.Count; j++)
                {
                    var dagNode = DAGNodes[j].Split(new[] { "=" }, 2, StringSplitOptions.None);
                    if (dagNode[0] == item1Parts)
                    {
                        item1Parts += "=" + dagNode[1];
                    }
                    else if (dagNode[0] == item2Parts)
                    {
                        item2Parts += "=" + dagNode[1];
                    }

                }
                modifiedTries.Add(new Tuple<string, string>(item1Parts, item2Parts));
            }
            return modifiedTries;
        }

        public IActionResult OnPostCreateDAG()
        {
            button = ButtonType.DAG;
            string mSentence = getFormula();
            if (!Valid) return Page();
            Engine engine = PrepareEngine(mSentence);
            engine.ConvertTreeToDag();
            engine.PrepareDAG();
            TreeConnections = engine.TreeConnections;
            DAGNodes = engine.DAGNodes;
            return Page();
        }

        public IActionResult OnPostCheckTautologyDAG()
        {
            button = ButtonType.CheckTautologyDAG;
            string mSentence = getFormula();
            if (!Valid) return Page();
            Engine engine = PrepareEngine(mSentence);
            engine.ConvertTreeToDag();
            IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            engine.PrepareDAG();
            TreeConnections = engine.TreeConnections;
            DAGNodes = engine.DAGNodes;
            distinctNodes = engine.distinctNodes;
            return Page();
        }

        public IActionResult OnPostCheckContradictionDAG()
        {
            button = ButtonType.CheckContradictionDAG;
            string mSentence = getFormula();
            if (!Valid) return Page();
            Engine engine = PrepareEngine(mSentence);
            engine.ConvertTreeToDag();
            IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
            engine.PrepareDAG();
            TreeConnections = engine.TreeConnections;
            DAGNodes = engine.DAGNodes;
            distinctNodes = engine.distinctNodes;
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
                        return vl;
                    }
                }
                return vl;
            }
            return null;
        }

        private void PrintTree(Tree tree)
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

            if (tree.childNodeLeft != null)
            {
                htmlTree.Add("<ul>");
                PrintTree(tree.childNodeLeft);
                if (tree.childNodeRight != null)
                {
                    PrintTree(tree.childNodeRight);
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