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
            AddNewFormula,
            DrawTautology,
            DrawContradiction,
            InteractiveTree
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

        public string yourFormula {get;set;}

        public List<string> Steps { get; set; } = new List<string> { };

        public List<SelectListItem> AllExerciseFormulas { get; set; }


        IWebHostEnvironment mEnv;
        public IndexModel(IWebHostEnvironment env)
        {
            mEnv = env;
            PrepareList();
        }

        public IActionResult OnPostCreateTree()
        {
            button = ButtonType.SyntaxTree;
            string mSentence = getFormula();
            if (!Valid)
            {
                if(mSentence != null)
                {
                    yourFormula = mSentence;
                }
                return Page();
            }
                Engine engine = PrepareEngine(mSentence);

            PrintTree(engine.tree, false);
           
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        public IActionResult OnPostInteractiveTree()
        {
            button = ButtonType.InteractiveTree;
            string mSentence = getFormula();
            if (!Valid)
            {
                if (mSentence != null)
                {
                    yourFormula = mSentence;
                   
                }
                return Page();
            }
            Formula = mSentence;
            Engine engine = PrepareEngine(mSentence);
            PrintTreeInteractive(engine.tree);

            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        public IActionResult OnPostInteractiveTreeProcess(string tree, string originalTree)
        {
            button = ButtonType.InteractiveTree;

            ExerciseTreeConstructer constructer = new ExerciseTreeConstructer(tree);
            constructer.ProcessTreeForInteractiveDrawing();
            Formula = originalTree;
            var interactiveTree = constructer.interactiveTree;
            Engine engine = PrepareEngine(originalTree);
            PrintTreeInteractiveCheck(engine.tree, interactiveTree);
            if (Steps.Count == 0) Steps.Add("Máš to správně!");
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTree = d + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        private void PrintTreeInteractiveCheck(Tree tree, Tree userTree)
        {
            htmlTree.Add("<li>");
            if (Operator.GetEnumDescription(tree.Item.mOperator) == userTree.Item.mSentence)
            {
                htmlTree.Add("<span class=tf-nc>"+userTree.Item.mSentence+"</span>");
            }
            else
            {
                if (Validator.isVariable(userTree.Item.mSentence))
                {
                    htmlTree.Add("<span class=tf-nc>" + userTree.Item.mSentence + "</span>");
                }
                else if (userTree.Item.mSentence == " ")
                {
                    htmlTree.Add("<span class=tf-nc> </span>");
                }
                else
                {
                    Steps.Add("Chyba! Špatně přiřazen operátor " +  userTree.Item.mSentence);
                    htmlTree.Add("<span class=tf-nc style='color: red;'>" + userTree.Item.mSentence + "</span>");
                }
            }
            if (tree.childNodeLeft != null)
            {
                htmlTree.Add("<ul>");
                PrintTreeInteractiveCheck(tree.childNodeLeft, userTree.childNodeLeft);
                if (tree.childNodeRight != null)
                {
                    PrintTreeInteractiveCheck(tree.childNodeRight,userTree.childNodeRight);
                }
                htmlTree.Add("</ul>");
            }
            htmlTree.Add("</li>");
        }

        private void PrintTreeInteractive(Tree tree)
        {
            htmlTree.Add("<li>");

            if (tree.Item.mOperator != Operator.OperatorEnum.EMPTY)
            {
                htmlTree.Add("<span class=tf-nc> </span>");
            }
            else
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");
            }
            if (tree.childNodeLeft != null)
            {
                htmlTree.Add("<ul>");
                PrintTreeInteractive(tree.childNodeLeft);
                if (tree.childNodeRight != null)
                {
                    PrintTreeInteractive(tree.childNodeRight);
                }
                htmlTree.Add("</ul>");
            }
            htmlTree.Add("</li>");
        }

        public IActionResult OnPostAddNewFormula()
        {
           button = ButtonType.AddNewFormula;
           TypesOfExercises = ListItemsHelper.ExerciseTypes;
           ExerciseHelper.GetFormulaList(mEnv);
           AllExerciseFormulas = ExerciseHelper.formulaList.Select(x => new SelectListItem
            {
                Value = x.Item1 + " | " + x.Item2 ,
                Text = x.Item1 + " | " + x.Item2
           }).ToList();
            return Page();
        }

        public IActionResult OnPostAddNewFormulaPost()
        {
            if (Request.Form.ContainsKey("addNewFormulaButton"))
            {
                button = ButtonType.AddNewFormula;
                string formula = Request.Form["FormulaInput"];
                string selectedValue = Request.Form["typeOfExercise"];
                if (!Validator.ValidateSentence(ref formula))
                {
                    ErrorMessage = "Špatný formát";
                    Valid = false;
                    return Page();
                }
                Engine engine = PrepareEngine(formula);
                if (selectedValue == "je tautologie" || selectedValue == "není tautologie")
                {
                    IsTautologyOrContradiction = engine.ProofSolver("Tautology");
                    if(selectedValue != "je tautologie" && IsTautologyOrContradiction)
                    {
                        ErrorMessage = "Špatný typ formule";
                        Valid = false;
                        return Page();
                    }
                    if (selectedValue != "není tautologie" && !IsTautologyOrContradiction)
                    {
                        ErrorMessage = "Špatný typ formule";
                        Valid = false;
                        return Page();
                    }
                }
                else
                {
                    IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
                    if (selectedValue != "je kontradikce" && IsTautologyOrContradiction)
                    {
                        ErrorMessage = "Špatný typ formule";
                        Valid = false;
                        return Page();
                    }
                    if (selectedValue != "není kontradikce" && !IsTautologyOrContradiction)
                    {
                        ErrorMessage = "Špatný typ formule";
                        Valid = false;
                        return Page();
                    }
                }
             
                ExerciseHelper.SaveFormulaList(mEnv, formula, selectedValue);
            }
            else if (Request.Form.ContainsKey("removeFormulaButton"))
            {
                string selectedValue = Request.Form["MyFormulas"];
                ExerciseHelper.RemoveFromFormulaList(mEnv, selectedValue);
            }
            button = ButtonType.None;
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
            if (!Valid)
            {
                if (mSentence != null)
                {
                    yourFormula = mSentence;
                }
                return Page();
            }
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

        public IActionResult OnPostDrawTreeTautology(string buttonValue)
        {
            button = ButtonType.DrawTautology;
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
            if (!Valid)
            {
                if (mSentence != null)
                {
                    yourFormula = mSentence;
                }
                return Page();
            }
            Engine engine = PrepareEngine(mSentence);
            var depth = engine.tree.MaxDepth();
            if (buttonValue == "Přidej úroveň" && level < depth) level++;
            else if (buttonValue == "Sniž úroveň" && level > 0) level--;
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            if (level == 0) Steps.Add("Hledáme tautologii proto jako první hodnotu dosadíme 0");
            distinctNodes = engine.distinctNodes;
           
            PrintLevelOrder(engine.counterModel, level);
          
            if (level >= depth)
            {
                if (IsTautologyOrContradiction)
                {
                    Steps.Add("Našli jsme semántický spor, proto toto může být tautologie.");
                    var contradiction = GetContradiction();
                    DrawTree(engine.counterModel, level, 0, contradiction);
                }
                else
                {
                    Steps.Add("Ke sporu jsme nedošli, proto toto není tautologie!");
                    DrawTree(engine.counterModel, level, 0);
                }
            }
            else
            {
                DrawTree(engine.counterModel, level);
            }
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        public IActionResult OnPostDrawTreeContradiction(string buttonValue)
        {
            button = ButtonType.DrawContradiction;
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
            if (!Valid)
            {
                if (mSentence != null)
                {
                    yourFormula = mSentence;
                }
                return Page();
            }
            Engine engine = PrepareEngine(mSentence);
            var depth = engine.tree.MaxDepth();
            if (buttonValue == "Přidej úroveň" && level < depth) level++;
            else if (buttonValue == "Sniž úroveň" && level > 0) level--;
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
            if (level == 0) Steps.Add("Hledáme kontradikci proto jako první hodnotu dosadíme 1");
            distinctNodes = engine.distinctNodes;
            PrintLevelOrder(engine.counterModel, level);
            
            if (level >= depth)
            {
                if (IsTautologyOrContradiction)
                {
                    Steps.Add("Našli jsme semántický spor, proto toto může být kontradikce.");
                    var contradiction = GetContradiction();
                    DrawTree(engine.counterModel, level, 0, contradiction);
                }
                else
                {
                    Steps.Add("Ke sporu jsme nedošli, proto toto není kontradikce!");
                    DrawTree(engine.counterModel, level, 0);
                }
            }
            else
            {
                DrawTree(engine.counterModel, level);
            }
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        public IActionResult OnPostExercise()
        {
            foreach (var item in listItems)
            {
                item.Selected = false;
            }
            ExerciseHelper.GetFormulaList(mEnv);
            button = ButtonType.Exercise;
            if (ExerciseHelper.formulaList.Count == 0)
            {
                ErrorMessage = "Nejsou nahrána žádna cvičení!";
                Valid = false;
                return Page();
            }
            ExerciseHelper.GeneratateNumber();

            int number = ExerciseHelper.number;
           
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
            if (ExerciseHelper.formulaList.Count == 0)
            {
                ErrorMessage = "Nejsou nahrána žádna cvičení!";
                Valid = false;
                return Page();
            }
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

        public IActionResult OnPostCreateDAG()
        {
            button = ButtonType.DAG;
            string mSentence = getFormula();
            if (!Valid) {
                if (mSentence != null)
                {
                    yourFormula = mSentence;
                }
                return Page();
            }
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
            if (!Valid)
            {
                if (mSentence != null)
                {
                    yourFormula = mSentence;
                }
                return Page();
            }
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
            if (!Valid) {
                if (mSentence != null)
                {
                    yourFormula = mSentence;
                }
                return Page();
            }
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
            if (!Valid)
            {
                if (mSentence != null)
                {
                    yourFormula = mSentence;
                }
                return Page();
            }
            Engine engine = PrepareEngine(mSentence);

            IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            distinctNodes = engine.distinctNodes;
            var contradiction = GetContradiction();
            PrintTree(engine.counterModel, false, contradiction);
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";

            return Page();
        }

        public IActionResult OnPostCheckContradiction()
        {
            button = ButtonType.CheckContradiction;
            string mSentence = getFormula();
            if (!Valid)
            {
                if (mSentence != null)
                {
                    yourFormula = mSentence;
                }
                return Page();
            }
            Engine engine = PrepareEngine(mSentence);

            IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
            distinctNodes = engine.distinctNodes;
            var contradiction = GetContradiction();
            PrintTree(engine.counterModel, false, contradiction);
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";
            return Page();
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

        private List<string> GetContradiction()
        {
            var filteredTuples = distinctNodes.GroupBy(t => t.Item1)
                                   .Where(g => g.Select(t => t.Item2).Distinct().Count() > 1)
                                   .SelectMany(g => g).ToList();
            return filteredTuples.Select(t => t.Item1)
                                     .Distinct()
                                     .ToList();
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
                    yourFormula = vl1;
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

        public void PrintLevelOrder(Tree tree, int startLevel = 0)
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

        private void PrintTree(TruthTree tree, bool exercise = false, List<string> contradictionValues = null)
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
                    if (contradictionValues != null && contradictionValues.Contains(tree.literal))
                    {

                        spanValue = "<span class=tf-nc style='color: red;'> ";
                        htmlTreeTruth.Add(spanValue + tree.literal + "=" + tree.Item + contradiction + "</span>");
                    }
                    else
                    {
                        htmlTreeTruth.Add(spanValue + tree.literal + "=" + tree.Item + contradiction + "</span>");
                    }
                  
                }
            }
            if (tree.ChildNodeLeft != null)
            {
                htmlTreeTruth.Add("<ul>");
                PrintTree(tree.ChildNodeLeft, exercise, contradictionValues);
                if (tree.ChildNodeRight != null)
                {
                    PrintTree(tree.ChildNodeRight, exercise, contradictionValues);
                }
                htmlTreeTruth.Add("</ul>");
            }
            htmlTreeTruth.Add("</li>");
        }

        private void DrawTree(Tree tree, int maxLevel = 0, int level = 0)
        {
            htmlTree.Add("<li>");
            if ((tree.childNodeLeft == null && tree.childNodeRight == null) && maxLevel - 1 == level)
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");
            }
            else if ((tree.childNodeLeft != null && tree.childNodeRight != null) && maxLevel - 1 == level)
                htmlTree.Add("<span class=tf-nc>" + tree.childNodeLeft.Item.mSentence + "<font color='red'>" + GetEnumDescription(tree.Item.mOperator) + "</font>" + tree.childNodeRight.Item.mSentence + "</span>");
            else if (maxLevel - 1 == level && tree.childNodeRight == null)
            {
                htmlTree.Add("<span class=tf-nc>" + "<font color='red'>" + GetEnumDescription(tree.Item.mOperator) + "</font>" + tree.childNodeLeft.Item.mSentence + "</span>");
            }
            else
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");
            }
            if (tree.childNodeLeft != null && level < maxLevel)
            {
                htmlTree.Add("<ul>");
                DrawTree(tree.childNodeLeft, maxLevel, level + 1);
                if (tree.childNodeRight != null)
                {

                    DrawTree(tree.childNodeRight, maxLevel, level + 1);
                }
                htmlTree.Add("</ul>");
            }
            htmlTree.Add("</li>");
        }

        private void DrawTree(TruthTree tree, int maxLevel = 0, int level = 0, List<string> contradiction = null)
        {
            if (maxLevel >= level)
            {
                htmlTree.Add("<li>");
                if (tree.literal == null)
                    htmlTree.Add("<span class=tf-nc>" + GetEnumDescription(tree.mOperator) + "=" + tree.Item + "</span>");
                else
                {
                    if (contradiction != null && contradiction.Contains(tree.literal))
                    {

                        var spanValue = "<span class=tf-nc style='color: red;'> ";
                        htmlTree.Add(spanValue + tree.literal + "=" + tree.Item + "</span>");
                    }
                    else
                    {
                        htmlTree.Add("<span class=tf-nc>" + tree.literal + "=" + tree.Item + "</span>");
                    }
                }
            }
            else
            {
                htmlTree.Add("<li>");
                if (tree.literal == null)
                    htmlTree.Add("<span class=tf-nc>" + GetEnumDescription(tree.mOperator) + "= </span>");
                else
                {
                    htmlTree.Add("<span class=tf-nc>" + tree.literal + "= </span>");
                }
            }
            if (tree.ChildNodeLeft != null)
            {
                htmlTree.Add("<ul>");
                DrawTree(tree.ChildNodeLeft, maxLevel, level + 1, contradiction);
                if (tree.ChildNodeRight != null)
                {
                    DrawTree(tree.ChildNodeRight, maxLevel, level + 1, contradiction);
                }
                htmlTree.Add("</ul>");
            }
            htmlTree.Add("</li>");
        }

        public void PrintLevelOrder(TruthTree tree, int startLevel = 2)
        {
            if (tree == null)
            {
                return;
            }
            Queue<(TruthTree, int)> queue = new Queue<(TruthTree, int)>();
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

                        if (tree.mOperator != OperatorEnum.EMPTY && tree.ChildNodeRight != null)
                        {
                            Steps.Add((j + ") - Pokud operátor " + GetEnumDescription(tree.mOperator) + " má hodnotu " + tree.Item + " tak můžeme zkusit dosadit " + tree.ChildNodeLeft.Item + " a " + tree.ChildNodeRight.Item + "<br>"));
                            j++;
                        }
                        else if (tree.mOperator != OperatorEnum.EMPTY && tree.ChildNodeRight == null)
                        {
                            Steps.Add((j + ") - Pokud operátor " + GetEnumDescription(tree.mOperator) + " má hodnotu " + tree.Item + " tak dosadíme " + tree.ChildNodeLeft.Item + "<br>"));
                            j++;
                        }
                    }
                    if (tree.ChildNodeLeft != null)
                    {
                        queue.Enqueue((tree.ChildNodeLeft, level + 1));
                    }
                    if (tree.ChildNodeRight != null)
                    {
                        queue.Enqueue((tree.ChildNodeRight, level + 1));
                    }
                }
            }
        }
    }
}