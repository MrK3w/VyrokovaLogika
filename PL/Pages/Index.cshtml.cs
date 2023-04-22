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
            ExerciseDAG,
            AddNewFormula
        }
        private string vl;
        private string vl1;
        public ButtonType button { get; set; }

        private List<string> htmlTree = new List<string>();

        private List<string> htmlTreeTruth = new List<string>();
        public string ConvertedTree { get; set; }
        public string ConvertedTreeTruth { get; set; }

        public int level { get; set; } = 0;
        public List<string> DAGNodes { get; set; } = new List<string>();
        public List<Tuple<string, string>> TreeConnections { get; set; } = new List<Tuple<string, string>>();
        private bool Green;
        public List<Tuple<string, int>> distinctNodes { get; set; } = new List<Tuple<string, int>>();
        public bool IsTautologyOrContradiction { get; set; }
        public string ErrorMessage;

        public List<SelectListItem> listItems { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TypesOfExercises { get; set; } = new List<SelectListItem>();
        public bool Valid { get; private set; } = true;

        public string ExerciseType { get; set; }

        public string ExerciseQuote { get; set; }

        public string Arguments { get; set; }

        public int mIssueIndex { get; set; } = -1;

        public string Formula { get; set; }
        public string ExerciseFormula { get; set; }

        public List<string> Steps { get; set; } = new List<string> { };

        IWebHostEnvironment mEnv;
        public IndexModel(IWebHostEnvironment env)
        {
            mEnv = env;
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

            PrintTree(engine.tree, true);
           
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        public IActionResult OnPostAddNewFormula()
        {
           button = ButtonType.AddNewFormula;
           TypesOfExercises = ListItemsHelper.ExerciseTypes;
           return Page();
        }

        public IActionResult OnPostAddNewFormulaPost()
        {
            button = ButtonType.AddNewFormula;
            string formula = Request.Form["FormulaInput"];
            string selectedValue = Request.Form["typeOfExercise"];
            if (!Validator.ValidateSentence(ref formula))
            {
                Valid = false;
                return Page();
            }
            ExerciseHelper.SaveFormulaList(mEnv,formula, selectedValue);
            return Page();
        }

        public IActionResult OnPostDrawTree(string buttonValue)
        {
            button = ButtonType.Draw;
            string mSentence;
            int outLevel;
            if (!int.TryParse(Request.Form["level"], out outLevel))
            {
                level = 0;
            }
            else
            {
                level = outLevel;
            }
            if (Request.Form.ContainsKey("tree"))
            {
                mSentence = Request.Form["tree"];
            }
            else
            {
                mSentence = getFormula();
            }
            Formula = mSentence;
            if (!Valid) return Page();
            
            Engine engine = PrepareEngine(mSentence);
            var depth = engine.tree.MaxDepth();
            if (buttonValue == "Přidej úroveň" && level < depth) level++;
            else if (buttonValue == "Sniž úroveň" && level > 0) level--;
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            DrawTree(engine.tree, level);
            PrintLevelOrder(engine.tree, level);
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
          
            return Page();
        }

        private void DrawTree(Tree tree, int maxLevel = 0,int level = 0, char letter = 'a')
        {
            htmlTree.Add("<li>");
            if ((tree.childNodeLeft == null && tree.childNodeRight == null )&& maxLevel-1 == level)
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");
            }
            else if ((tree.childNodeLeft != null && tree.childNodeRight != null) && maxLevel - 1 == level)
                htmlTree.Add("<span class=tf-nc>" + tree.childNodeLeft.Item.mSentence + "<font color='red'>" + GetEnumDescription(tree.Item.mOperator) + "</font>" + tree.childNodeRight.Item.mSentence + "</span>");
            else if(maxLevel -1 == level &&  tree.childNodeRight == null)
            {
                htmlTree.Add("<span class=tf-nc>"  + "<font color='red'>" + GetEnumDescription(tree.Item.mOperator) + "</font>" + tree.childNodeLeft.Item.mSentence + "</span>");
            }
            else
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");
            }
                if (tree.childNodeLeft != null && level < maxLevel)
                {
                    htmlTree.Add("<ul>");
                if (letter == 'b') DrawTree(tree.childNodeLeft, maxLevel, level + 1);
                else DrawTree(tree.childNodeLeft, maxLevel, level + 1);

                    if (tree.childNodeRight != null)
                    {
                    if (letter == 'b') DrawTree(tree.childNodeRight, maxLevel, level + 1);
                    else DrawTree(tree.childNodeRight, maxLevel, level +1);
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
            ExerciseHelper.GetFormulaList(mEnv);
            button = ButtonType.Exercise;
            ExerciseHelper.GeneratateNumber();

            int number = ExerciseHelper.number;
            if(ExerciseHelper.formulaList.Count == 0)
            {
                Valid = false;
                return Page();
            }
            string f = ExerciseHelper.formulaList[number].Item1;
            Valid = true;
            ExerciseType = ExerciseHelper.formulaList[number].Item2;
            Converter.ConvertLogicalOperators(ref f);
            Validator.ValidateSentence(ref f);
            ExerciseFormula = f;
            ExerciseHelper.formula = f;
            Engine engine = PrepareEngine(ExerciseFormula);
            if (ExerciseType == "není tautologie" || ExerciseType == "je tautologie")
            {
                IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            }
            if (ExerciseType == "není kontradikce" || ExerciseType == "je kontradikce")
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
            if (ExerciseHelper.formulaList.Count == 0)
            {
                Valid = false;
                return Page();
            }
            ExerciseType = ExerciseHelper.formulaList[number].Item2;
            button = ButtonType.Exercise;
            ExerciseTreeConstructer constructer = new ExerciseTreeConstructer(tree);
            TruthTree truthTree = new TruthTree();
            if (ExerciseType == "je tautologie")
            {
                truthTree = constructer.ProcessTree(true,false);
            }
            //there must be some contradiction
            else if(ExerciseType == "není tautologie")
            {
                truthTree = constructer.ProcessTree(true, true);
            }
            else if (ExerciseType == "je kontradikce")
            {
                truthTree = constructer.ProcessTree(false, false);
            }
            else if (ExerciseType == "není kontradikce")
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
            ExerciseHelper.GetFormulaList(mEnv);
            button = ButtonType.ExerciseDAG;
            ExerciseHelper.GeneratateNumber();
            if (ExerciseHelper.formulaList.Count == 0)
            {
                Valid = false;
                return Page();
            }
            int number = ExerciseHelper.number;
            string f = ExerciseHelper.formulaList[number].Item1;
            ExerciseType = ExerciseHelper.formulaList[number].Item2;
            Converter.ConvertLogicalOperators(ref f);
            Validator.ValidateSentence(ref f);
            ExerciseFormula = f;
            ExerciseHelper.formula = f;
            Valid = true;
            Engine engine = PrepareEngine(ExerciseFormula);
            if (ExerciseType == "není tautologie" || ExerciseType == "je tautologie")
            {
                IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            }
            if (ExerciseType == "není kontradikce" || ExerciseType == "je kontradikce")
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
            if (ExerciseHelper.formulaList.Count == 0)
            {
                Valid = false;
                return Page();
            }
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
                case "je tautologie":
                    verifier = new TruthDagVerifier(TreeConnections, DAGNodes, true, false);
                    break;
                case "není tautologie":
                    verifier = new TruthDagVerifier(TreeConnections, DAGNodes,  true, true);
                    break;
                case "je kontradikce":
                    verifier = new TruthDagVerifier(TreeConnections, DAGNodes, false, false);
                    break;
                case "není kontradikce":
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

        public string? getFormula()
        {
            vl = Request.Form["formula"];
            vl1 = Request.Form["UserInput"];
            if (vl == "" && vl1 == "")
            {
                Valid = false;
                ErrorMessage = "Nevybral jsi žádnou formuli!";
                return null;
            }
            if (vl1 != "")
            {
                if (!Validator.ValidateSentence(ref vl1))
                {
                    ErrorMessage = Validator.ErrorMessage;
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
                    ErrorMessage = Validator.ErrorMessage;
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

        public void PrintLevelOrder(Tree tree, int startLevel = 2)
        {
            if (tree == null)
            {
                return;
            }
            Queue<(Tree, int)> queue = new Queue<(Tree, int)>();
            queue.Enqueue((tree, 1));

            while (queue.Count > 0)
            {
                int j = 1;
                int levelSize = queue.Count;
                for (int i = 0; i < levelSize; i++)
                {
                    (tree, int level) = queue.Dequeue();
                    if (level == startLevel)
                    { 

                        if (tree.Item.mOperator != OperatorEnum.EMPTY)
                        {
                            Steps.Add((j + ") - Rozdělujeme podle: " + GetEnumDescription(tree.Item.mOperator) + "<br>"));
                            j++;
                        }
                    }
                    if (tree.childNodeLeft != null)
                    {
                        queue.Enqueue((tree.childNodeLeft, level + 1));
                    }
                    if (tree.childNodeRight != null)
                    {
                        queue.Enqueue((tree.childNodeRight, level + 1));
                    }
                }
            }
        }

        private void PrintTree(Tree tree, bool fullTree = false)
            {
            htmlTree.Add("<li>");

            if (!fullTree)
            {
                if (tree.Item.mOperator != Operator.OperatorEnum.EMPTY)
                {
                    htmlTree.Add("<span class=tf-nc>" + GetEnumDescription(tree.Item.mOperator) + "</span>");
                }
                else
                {
                    htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");
                }
            }
            else
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");
            }
            if (tree.childNodeLeft != null)
            {
                htmlTree.Add("<ul>");
                PrintTree(tree.childNodeLeft, fullTree);
                if (tree.childNodeRight != null)
                {
                    PrintTree(tree.childNodeRight, fullTree);
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
                string contradiction = "";
                if(tree.contradiction)
                {
                    contradiction = " x";
                }
                if (tree.literal == null)
                    htmlTreeTruth.Add(spanValue + GetEnumDescription(tree.mOperator) + "=" + tree.Item + contradiction + "</span>");
                else
                {
                    htmlTreeTruth.Add(spanValue + tree.literal + "=" + tree.Item + contradiction + "</span>");
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