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
        //button to each type of exercise
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
        public ButtonType Button { get; set; }
        private readonly List<string> htmlTree = new();

        private readonly List<string> htmlTreeTruth = new();
        public string ConvertedTree { get; set; }
        public string ConvertedTreeTruth { get; set; }

        public int Level { get; set; } = 0;
        public List<string> DAGNodes { get; set; } = new List<string>();
        public List<Tuple<string, string>> DagConnections { get; set; } = new List<Tuple<string, string>>();
        private bool Green;
        public List<Tuple<string, int>> DistinctNodes { get; set; } = new List<Tuple<string, int>>();
        public bool IsTautologyOrContradiction { get; set; }
        public string ErrorMessage;

        public List<SelectListItem> ListItems { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TypesOfExercises { get; set; } = new List<SelectListItem>();
        public bool Valid { get; private set; } = true;

        public string ExerciseType { get; set; }

        public string ExerciseQuote { get; set; }

        public string Arguments { get; set; }

        public int MIssueIndex { get; set; } = -1;

        public string Formula { get; set; }
        public string ExerciseFormula { get; set; }

        public string YourFormula { get; set; } = "";

        public List<string> Steps { get; set; } = new List<string> { };

        public List<SelectListItem> AllExerciseFormulas { get; set; }

        readonly IWebHostEnvironment mEnv;
        public IndexModel(IWebHostEnvironment env)
        {
            mEnv = env;
            PrepareList();
        }
        //Post method for basic drawing of tree
        public IActionResult OnPostCreateTree()
        {
            Button = ButtonType.SyntaxTree;
            //get formula from inputs
            string mSentence = GetFormula();
            //if it not valid save user input to YourFormula and return page
            if (!Valid)
            {
                if(mSentence != null)
                {
                    YourFormula = mSentence;
                }
                return Page();
            }
            //otherwise prepare engine with sentence we got
            Engine engine = PrepareEngine(mSentence);
            //prepare tree for css library treeflex
            PrintTree(engine.Tree, false);
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        public IActionResult OnPostInteractiveTree()
        {
            Button = ButtonType.InteractiveTree;
            string mSentence = GetFormula();
            if (!Valid)
            {
                if (mSentence != null)
                {
                    YourFormula = mSentence;
                   
                }
                return Page();
            }
            Formula = mSentence;
            Engine engine = PrepareEngine(mSentence);
            //method to prepare tree with empty nodes
            PrintTreeInteractive(engine.Tree);

            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        public IActionResult OnPostInteractiveTreeProcess(string tree, string originalTree)
        {
            Button = ButtonType.InteractiveTree;
            //contstruct tree which we get from html
            ExerciseTreeConstructer constructer = new(tree);
            //we need to construct that tree back
            constructer.ProcessTreeForInteractiveDrawing();
            //we get original formula and also construct tree for that
            Formula = originalTree;
            var interactiveTree = constructer.InteractiveTree;
            Engine engine = PrepareEngine(originalTree);
            //Print interactive tree check
            PrintTreeInteractiveCheck(engine.Tree, interactiveTree);
            //if we didn't found any mistake than it is ok
            if (Steps.Count == 0) Steps.Add("Máš to správně!");
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTree = d + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        private void PrintTreeInteractiveCheck(Tree tree, Tree userTree)
        {
            htmlTree.Add("<li>");
            //if user put right operator we will just print it back
            if (Operator.GetEnumDescription(tree.Item.MOperator) == userTree.Item.MSentence)
            {
                htmlTree.Add("<span class=tf-nc>"+userTree.Item.MSentence+"</span>");
            }
            else
            {
                //if it is literal we will print it back
                if (Validator.IsLiteral(userTree.Item.MSentence))
                {
                    htmlTree.Add("<span class=tf-nc>" + userTree.Item.MSentence + "</span>");
                }
                //if user didn't fill this node we will print it again as empty node
                else if (userTree.Item.MSentence == " ")
                {
                    htmlTree.Add("<span class=tf-nc> </span>");
                }
                //in other case user had to make mistake, so we will put his mistake to list of Steps and print that operator in red color
                else
                {
                    Steps.Add("Chyba! Špatně přiřazen operátor " +  userTree.Item.MSentence);
                    htmlTree.Add("<span class=tf-nc style='color: red;'>" + userTree.Item.MSentence + "</span>");
                }
            }
            //if tree has left node we will evaluate tree for this node
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

        //Method to print empty interactive tree for user, we will just print literals into it
        private void PrintTreeInteractive(Tree tree)
        {
            htmlTree.Add("<li>");

            if (tree.Item.MOperator != Operator.OperatorEnum.EMPTY)
            {
                htmlTree.Add("<span class=tf-nc> </span>");
            }
            else
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.MSentence + "</span>");
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

        //Prepare all available formulas for user, in case he wants to add new formula or remove previous ones 
        public IActionResult OnPostAddNewFormula()
        {
           Button = ButtonType.AddNewFormula;
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
            //if user wants to add new formula
            if (Request.Form.ContainsKey("addNewFormulaButton"))
            {
                Button = ButtonType.AddNewFormula;
                string formula = Request.Form["FormulaInput"];
                string selectedValue = Request.Form["typeOfExercise"];
                // we will get formula and validate if it is without mistakes 
                if (!Validator.ValidateSentence(ref formula))
                {
                    ErrorMessage = "Špatný formát";
                    Valid = false;
                    return Page();
                }
                
                //process formula to check if user used right type of exercise
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
                //if everything run correctly we will save this formula into json
                ExerciseHelper.SaveFormulaList(mEnv, formula, selectedValue);
            }
            //if user pressed removeFormula buttuon we will find that formula in json and remove it
            else if (Request.Form.ContainsKey("removeFormulaButton"))
            {
                string selectedValue = Request.Form["MyFormulas"];
                ExerciseHelper.RemoveFromFormulaList(mEnv, selectedValue);
            }
            //return back to main page
            Button = ButtonType.None;
            return Page();
        }

        //Option vykresli strom 
        public IActionResult OnPostDrawTree(string buttonValue)
        {
            Button = ButtonType.Draw;
            string mSentence;
            //on start is level 0
            if (!int.TryParse(Request.Form["level"], out int outLevel))
            {
                Level = 0;
            }
            else
            {
                Level = outLevel;
            }
            //if ther is already saved state of tree we will get it here
            if (Request.Form.ContainsKey("tree"))
            {
                mSentence = Request.Form["tree"];
            }
            //if it is new tree we will get it from text box or itemList
            else
            {
                mSentence = GetFormula();
            }
            Formula = mSentence;
            //if formula is not valid we will not continue
            if (!Valid)
            {
                if (mSentence != null)
                {
                    YourFormula = mSentence;
                }
                return Page();
            }
            //prepare engine for this formula
            Engine engine = PrepareEngine(mSentence);
            //get max depth of tree
            var depth = engine.Tree.MaxDepth();
            //if user clicked button Pridej uroven and we didn't already printed full tree we will add level
            if (buttonValue == "Přidej úroveň" && Level < depth) Level++;
            //if user pressed button Sniz uroven and level is higher than zero then we can decrease level
            else if (buttonValue == "Sniž úroveň" && Level > 0) Level--;
            //draw tree for user and also print steps on current level of tree
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            DrawTree(engine.Tree, Level);
            PrintLevelOrder(engine.Tree, Level);
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        //draw tree for tautology
        public IActionResult OnPostDrawTreeTautology(string buttonValue)
        {
            Button = ButtonType.DrawTautology;
            string mSentence;
            if (!int.TryParse(Request.Form["level"], out int outLevel))
            {
                Level = 0;
            }
            else
            {
                Level = outLevel;
            }
            if (Request.Form.ContainsKey("tree"))
            {
                mSentence = Request.Form["tree"];
            }
            else
            {
                mSentence = GetFormula();
            }
            Formula = mSentence;
            if (!Valid)
            {
                if (mSentence != null)
                {
                    YourFormula = mSentence;
                }
                return Page();
            }
            //prepare sentence in engine
            Engine engine = PrepareEngine(mSentence);
            //get depth of tree
            var depth = engine.Tree.MaxDepth();
            //buttons for levels
            if (buttonValue == "Přidej úroveň" && Level < depth) Level++;
            else if (buttonValue == "Sniž úroveň" && Level > 0) Level--;
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            //get tautology tree
            IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            //first step is to add to evaluation of formula 0
            if (Level == 0) Steps.Add("Hledáme tautologii proto jako první hodnotu dosadíme 0");
            //we will get distinct nodes
            DistinctNodes = engine.DistinctNodes;
            //print steps for current level of tree
            PrintLevelOrder(engine.CounterModel, Level);
            
            //if we are on end of tree
            if (Level >= depth)
            {
                //if it is tautology we will mark contracition values and draw tree with them
                if (IsTautologyOrContradiction)
                {
                    Steps.Add("Našli jsme semántický spor, proto toto může být tautologie.");
                    var contradiction = GetContradiction();
                    DrawTree(engine.CounterModel, Level, 0, contradiction);
                }
                else
                {
                    //if we didn't find contradiction, then it means that this formula cannot be tautology
                    Steps.Add("Ke sporu jsme nedošli, proto toto není tautologie!");
                    DrawTree(engine.CounterModel, Level, 0);
                }
            }
            //if we don't have full tree just print current level and steps
            else
            {
                DrawTree(engine.CounterModel, Level);
            }
            //for printing in tree
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        //method works like previous one just here we are finding contradiction
        public IActionResult OnPostDrawTreeContradiction(string buttonValue)
        {
            Button = ButtonType.DrawContradiction;
            string mSentence;
            if (!int.TryParse(Request.Form["level"], out int outLevel))
            {
                Level = 0;
            }
            else
            {
                Level = outLevel;
            }
            if (Request.Form.ContainsKey("tree"))
            {
                mSentence = Request.Form["tree"];
            }
            else
            {
                mSentence = GetFormula();
            }
            Formula = mSentence;
            if (!Valid)
            {
                if (mSentence != null)
                {
                    YourFormula = mSentence;
                }
                return Page();
            }
            Engine engine = PrepareEngine(mSentence);
            var depth = engine.Tree.MaxDepth();
            if (buttonValue == "Přidej úroveň" && Level < depth) Level++;
            else if (buttonValue == "Sniž úroveň" && Level > 0) Level--;
            string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
            if (Level == 0) Steps.Add("Hledáme kontradikci proto jako první hodnotu dosadíme 1");
            DistinctNodes = engine.DistinctNodes;
            PrintLevelOrder(engine.CounterModel, Level);
            
            if (Level >= depth)
            {
                if (IsTautologyOrContradiction)
                {
                    Steps.Add("Našli jsme semántický spor, proto toto může být kontradikce.");
                    var contradiction = GetContradiction();
                    DrawTree(engine.CounterModel, Level, 0, contradiction);
                }
                else
                {
                    Steps.Add("Ke sporu jsme nedošli, proto toto není kontradikce!");
                    DrawTree(engine.CounterModel, Level, 0);
                }
            }
            else
            {
                DrawTree(engine.CounterModel, Level);
            }
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            return Page();
        }

        //post method for exercise
        public IActionResult OnPostExercise()
        {
            foreach (var item in ListItems)
            {
                item.Selected = false;
            }
            //we will get some exercise from json file
            ExerciseHelper.GetFormulaList(mEnv);
            Button = ButtonType.Exercise;
            //if we don't have stored any exercises we will print error message that we don't have any exercises
            if (ExerciseHelper.formulaList.Count == 0)
            {
                ErrorMessage = "Nejsou nahrána žádna cvičení!";
                Valid = false;
                return Page();
            }
            //generate random exercise
            ExerciseHelper.GeneratateNumber();

            int number = ExerciseHelper.number;
           
            string f = ExerciseHelper.formulaList[number].Item1;
            Valid = true;
            ExerciseType = ExerciseHelper.formulaList[number].Item2;
            //convert logical operators for exercise and validate
            Converter.ConvertLogicalOperators(ref f);
            Validator.ValidateSentence(ref f);
            ExerciseFormula = f;
            ExerciseHelper.formula = f;
            //prepare tree for current exercise
            Engine engine = PrepareEngine(ExerciseFormula);
            if (ExerciseType == "není tautologie" || ExerciseType == "je tautologie")
            {
                //get tree for tautology / not tautology
                IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            }
            if (ExerciseType == "není kontradikce" || ExerciseType == "je kontradikce")
            {
                //get tree for contradiction / not contradiction
                IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
            }
            //print tree for current tree but let him set for 0
            PrintTree(engine.CounterModel,true);
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";
            return Page();
        }

        //post method for validating exercise
        public IActionResult OnPostExerciseProcess(string tree)
        {
            foreach (var item in ListItems)
            {
                item.Selected = false;
            }
            //we will get current exercise
            int number = ExerciseHelper.number;
            if (ExerciseHelper.formulaList.Count == 0)
            {
                Valid = false;
                return Page();
            }
            ExerciseType = ExerciseHelper.formulaList[number].Item2;
            Button = ButtonType.Exercise;
            //prepare tree from HTML code
            ExerciseTreeConstructer constructer = new(tree);
            TruthTree truthTree = new();
            //process tree in constucter
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
            //constructer will check if tree is filled right
            constructer.IsTreeOkay();
            //green is for correctly marked contradiction to change color in node
            Green = constructer.Green;
            //we will get formula and quote for user to tell him if tree is filled correctly
            ExerciseFormula = ExerciseHelper.formula;
            ExerciseQuote = constructer.ExerciseQuote;
            //clear htmlTree and prepare new one
            htmlTreeTruth.Clear();
            PrintTree(truthTree);
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";
            return Page();
        }
        //exercise on dag
        public IActionResult OnPostExerciseDAG()
        {
            //to deselect formula in itemList
            foreach (var item in ListItems)
            {
                item.Selected = false;
            }
            //get formula from json
            ExerciseHelper.GetFormulaList(mEnv);
            Button = ButtonType.ExerciseDAG;
            //check if we have some exercises
            if (ExerciseHelper.formulaList.Count == 0)
            {
                ErrorMessage = "Nejsou nahrána žádna cvičení!";
                Valid = false;
                return Page();
            }
            //generate random exercise
            ExerciseHelper.GeneratateNumber();
            int number = ExerciseHelper.number;
            string f = ExerciseHelper.formulaList[number].Item1;
            ExerciseType = ExerciseHelper.formulaList[number].Item2;
            //convert formula to right format
            Converter.ConvertLogicalOperators(ref f);
            Validator.ValidateSentence(ref f);
            ExerciseFormula = f;
            ExerciseHelper.formula = f;
            Valid = true;
            //prepare this formula 
            Engine engine = PrepareEngine(ExerciseFormula);
            if (ExerciseType == "není tautologie" || ExerciseType == "je tautologie")
            {
                IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            }
            if (ExerciseType == "není kontradikce" || ExerciseType == "je kontradikce")
            {
                IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
            }
            //convert tree to dag
            engine.ConvertTreeToDag();
            engine.PrepareDAG(true);
            //get treeConnections and DAGNodes
            DagConnections = engine.TreeConnections;
            DAGNodes = engine.DAGNodes;
            return Page();
        }

        //process dag from user
        public IActionResult OnPostExerciseProcessDAG(string pDAGNodes, string DAGPath)
        {
            //deselect items from itemList
            foreach (var item in ListItems)
            {
                item.Selected = false;
            }
            //get number of exercise
            int number = ExerciseHelper.number;
            //check if we have some exercises
            if (ExerciseHelper.formulaList.Count == 0)
            {
                Valid = false;
                return Page();
            }
            //get type of exercise
            ExerciseType = ExerciseHelper.formulaList[number].Item2;
            Button = ButtonType.ExerciseDAG;
            //get nodeList and dagConnections 
            List<JsonTreeNodes> nodeList = JsonConvert.DeserializeObject<List<JsonTreeNodes>>(pDAGNodes);
            List<JsonEdges> dagConnections = JsonConvert.DeserializeObject<List<JsonEdges>>(DAGPath);
            ExerciseFormula = ExerciseHelper.formula;
            //get connections and dagNodes
            DagConnections = dagConnections.Select(edge => Tuple.Create(edge.From, edge.To)).ToList();
            DAGNodes = nodeList.Select(edge => edge.Label).ToList();
            DagConnections = PrepareTreeConnections();
            //verify dag
            TruthDagVerifier verifier;
            switch (ExerciseType)
            {
                case "je tautologie":
                    verifier = new TruthDagVerifier(DagConnections, DAGNodes, true, false);
                    break;
                case "není tautologie":
                    verifier = new TruthDagVerifier(DagConnections, DAGNodes,  true, true);
                    break;
                case "je kontradikce":
                    verifier = new TruthDagVerifier(DagConnections, DAGNodes, false, false);
                    break;
                case "není kontradikce":
                    verifier = new TruthDagVerifier(DagConnections, DAGNodes, false, true);
                    break;
                default:
                    return Page();
            }
            //get where is problem and print quote where problem is
            MIssueIndex = verifier.MIssueIndex;
            ExerciseQuote = verifier.ExerciseQuote;
            return Page();
        }
        //create DAG for user
        public IActionResult OnPostCreateDAG()
        {
            //get formula from user inputs
            Button = ButtonType.DAG;
            string mSentence = GetFormula();
            if (!Valid) {
                if (mSentence != null)
                {
                    YourFormula = mSentence;
                }
                return Page();
            }
            //prepare that sentence in engine
            Engine engine = PrepareEngine(mSentence);
            //convert our tree to dag to be able to print him
            engine.ConvertTreeToDag();
            engine.PrepareDAG();
            DagConnections = engine.TreeConnections;
            DAGNodes = engine.DAGNodes;
            return Page();
        }

        //print DAG for tautology
        public IActionResult OnPostCheckTautologyDAG()
        {
            Button = ButtonType.CheckTautologyDAG;
            string mSentence = GetFormula();
            if (!Valid)
            {
                if (mSentence != null)
                {
                    YourFormula = mSentence;
                }
                return Page();
            }
            Engine engine = PrepareEngine(mSentence);
            engine.ConvertTreeToDag();
            IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            engine.PrepareDAG();
            DagConnections = engine.TreeConnections;
            DAGNodes = engine.DAGNodes;
            DistinctNodes = engine.DistinctNodes;
            return Page();
        }

        //print DAG for contradiction
        public IActionResult OnPostCheckContradictionDAG()
        {
            Button = ButtonType.CheckContradictionDAG;
            string mSentence = GetFormula();
            if (!Valid) {
                if (mSentence != null)
                {
                    YourFormula = mSentence;
                }
                return Page();
            }
            Engine engine = PrepareEngine(mSentence);
            engine.ConvertTreeToDag();
            IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
            engine.PrepareDAG();
            DagConnections = engine.TreeConnections;
            DAGNodes = engine.DAGNodes;
            DistinctNodes = engine.DistinctNodes;
            return Page();
        }

        //check tautology in Tree
        public IActionResult OnPostCheckTautology()
        {
            Button = ButtonType.CheckTautology;
            string mSentence = GetFormula();
            if (!Valid)
            {
                if (mSentence != null)
                {
                    YourFormula = mSentence;
                }
                return Page();
            }
            //prepare tree
            Engine engine = PrepareEngine(mSentence);
            //validate tautology in tree
            IsTautologyOrContradiction = engine.ProofSolver("Tautology");
            DistinctNodes = engine.DistinctNodes;
            //get nodes whee is contradiction
            var contradiction = GetContradiction();
            //prepare tree for printing using treeflex
            PrintTree(engine.CounterModel, false, contradiction);
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";
            return Page();
        }

        //check contradiction in tree
        public IActionResult OnPostCheckContradiction()
        {
            Button = ButtonType.CheckContradiction;
            //get formula from user inputs
            string mSentence = GetFormula();
            //check if it is valid
            if (!Valid)
            {
                if (mSentence != null)
                {
                    YourFormula = mSentence;
                }
                return Page();
            }
            //prepare tree in engine
            Engine engine = PrepareEngine(mSentence);
            //check tree for contradiction
            IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
            DistinctNodes = engine.DistinctNodes;
            var contradiction = GetContradiction();
            PrintTree(engine.CounterModel, false, contradiction);
            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";
            return Page();
        }
        //create instance of engine for work from sentence
        private Engine PrepareEngine(string mSentence)
        {
            //create new instance of engine
            Engine engine = new(mSentence);
            //clear previous htmlTrees
            htmlTree.Clear();
            htmlTreeTruth.Clear();
            //Process sentence to be able to create tree
            engine.ProcessSentence();
            return engine;
        }
        //set itemList of exercises
        private void PrepareList()
        {
            ListItems = ListItemsHelper.ListItems;
        }
        //get contradiction values
        private List<string> GetContradiction()
        {
            var filteredTuples = DistinctNodes.GroupBy(t => t.Item1)
                                   .Where(g => g.Select(t => t.Item2).Distinct().Count() > 1)
                                   .SelectMany(g => g).ToList();
            return filteredTuples.Select(t => t.Item1)
                                     .Distinct()
                                     .ToList();
        }
        //prepare tree connections for dag exercises
        private List<Tuple<string, string>> PrepareTreeConnections()
        {
            //get new list of tuples string string
            List<Tuple<string, string>> modifiedTrees = new();
            for (int i = 0; i < DagConnections.Count; i++)
            {
                var item1PartsFull = DagConnections[i].Item1.Split(new[] { "=" }, 2, StringSplitOptions.None);
                var item2PartsFull = DagConnections[i].Item2.Split(new[] { "=" }, 2, StringSplitOptions.None);
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
                modifiedTrees.Add(new Tuple<string, string>(item1Parts, item2Parts));
            }
            return modifiedTrees;
        }
        //get formula from user inputs
        public string? GetFormula()
        {
            vl = Request.Form["formula"];
            vl1 = Request.Form["UserInput"];
            //if user didn't use any of inputs invalidate request and throw errorMessage that user didn't choose formula
            if (vl == "" && vl1 == "")
            {
                Valid = false;
                ErrorMessage = "Nevybral jsi žádnou formuli!";
                return null;
            }
            //if user user userInput
            if (vl1 != "")
            {
                //validate his formula otherwise throw error message and save that formula so user can change it later
                if (!Validator.ValidateSentence(ref vl1))
                {
                    ErrorMessage = Validator.ErrorMessage;
                    Valid = false;
                    YourFormula = vl1;
                    return null;
                }
                //convert logical operators in case they are not in right format
                Converter.ConvertLogicalOperators(ref vl1);
                //add formula to listItem
                ListItemsHelper.SetListItems(vl1);
                //select that formula in itemList
                var selected = ListItems.Where(x => x.Value == vl1).First();
                selected.Selected = true;
                return vl1;
            }
            //if user used formula from listItem
            else if (vl != "")
            {
                //validate this sentece
                if (!Validator.ValidateSentence(ref vl))
                {
                    Valid = false;
                    ErrorMessage = Validator.ErrorMessage;
                    return null;
                }
                //select that formula in listItem
                foreach (var item in ListItems)
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

        //print steps in current level of tree for user
        public void PrintLevelOrder(Tree tree, int startLevel = 0)
        {
            //if we don't have tree then return
            if (tree == null)
            {
                return;
            }
            //create new queue for that tree
            Queue<(Tree, int)> queue = new();
            //and enqueue that tree on first place
            queue.Enqueue((tree, 1));
            //while till we have something in queue
            while (queue.Count > 0)
            {
                int j = 1;
                int levelSize = queue.Count;
                for (int i = 0; i < levelSize; i++)
                {
                    //deque current level
                    (tree, int level) = queue.Dequeue();
                    //if we are currently on level we need to print
                    if (level == startLevel)
                    { 
                        //if there is operator we will add step by which operator we are splitting
                        if (tree.Item.MOperator != OperatorEnum.EMPTY)
                        {
                            Steps.Add((j + ") - Rozdělujeme podle: " + GetEnumDescription(tree.Item.MOperator) + "<br>"));
                            j++;
                        }
                    }
                    //if tree has childNodeLeft we will add it to queue with level + 1
                    if (tree.childNodeLeft != null)
                    {
                        queue.Enqueue((tree.childNodeLeft, level + 1));
                    }
                    //if tree has childNodeRight we will add it to queue with level + 1
                    if (tree.childNodeRight != null)
                    {
                        queue.Enqueue((tree.childNodeRight, level + 1));
                    }
                }
            }
        }
        //print tree for user
        private void PrintTree(Tree tree, bool fullTree = false)
        {
            htmlTree.Add("<li>");
            //if we dont want tree with full formulas 
            if (!fullTree)
            {
                //if there is operator print that
                if (tree.Item.MOperator != Operator.OperatorEnum.EMPTY)
                {
                    htmlTree.Add("<span class=tf-nc>" + GetEnumDescription(tree.Item.MOperator) + "</span>");
                }
                //otherwise print Msentence which will be literal
                else
                {
                    htmlTree.Add("<span class=tf-nc>" + tree.Item.MSentence + "</span>");
                }
            }
            //if we want to print fullTree we will just print all formula in nodes
            else
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.MSentence + "</span>");
            }
            //if tree has childNodeLeft we will use recursion 
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

        //modified print tree method
        private void PrintTree(TruthTree tree, bool exercise = false, List<string>? contradictionValues = null)
        {
            htmlTreeTruth.Add("<li>");
            //if we want to print tree for new exercise we will print after operator 0
            if (exercise)
            {
                //if there is operator use this
                if (tree.literal == null)
                    htmlTreeTruth.Add("<span class=tf-nc>" + GetEnumDescription(tree.MOperator) + "=" + "0" + "</span>");
                //if there is literal use this
                else
                {
                    htmlTreeTruth.Add("<span class=tf-nc>" + tree.literal + "=" + "0" + "</span>");
                }
            }
            else
            {
                //span value is used for color in tree
                string spanValue;
                if (tree.invalid)
                {
                    //if we want to print nodes in red e.g to mark contradiction
                    if (!Green) spanValue = "<span class=tf-nc style='color: red;'>";
                    else
                    {
                        //if we want to print nodes in green e.g. exercise done correctly
                        spanValue = "<span class=tf-nc style='color: green;'>";
                    }
                }
                //else just print that node in black
                else
                {
                    spanValue = "<span class=tf-nc>";
                }
                //in case we need to print in exercise x for marked contradiction
                string contradiction = "";
                if (tree.contradiction)
                {
                    contradiction = " x";
                }
                //if tree don't have literal print color plus operator = value after that and x if user marked contradiction
                if (tree.literal == null)
                    htmlTreeTruth.Add(spanValue + GetEnumDescription(tree.MOperator) + "=" + tree.Item + contradiction + "</span>");
                else
                {
                    //if there is some contradiction and it is not exercise
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
            //recursion in tree
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

        //draw tree by using levels
        private void DrawTree(Tree tree, int maxLevel = 0, int level = 0)
        {
            htmlTree.Add("<li>");
            //print full formula if it is literal
            if ((tree.childNodeLeft == null && tree.childNodeRight == null) && maxLevel - 1 == level)
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.MSentence + "</span>");
            }
            //if it is not literal print left side operator by red and right side
            else if ((tree.childNodeLeft != null && tree.childNodeRight != null) && maxLevel - 1 == level)
                htmlTree.Add("<span class=tf-nc>" + tree.childNodeLeft.Item.MSentence + "<font color='red'>" + GetEnumDescription(tree.Item.MOperator) + "</font>" + tree.childNodeRight.Item.MSentence + "</span>");
            //in case there is negation print first operator in red and then print leftNode
            else if (maxLevel - 1 == level && tree.childNodeRight == null)
            {
                htmlTree.Add("<span class=tf-nc>" + "<font color='red'>" + GetEnumDescription(tree.Item.MOperator) + "</font>" + tree.childNodeLeft.Item.MSentence + "</span>");
            }
            //else print full formula
            else
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.MSentence + "</span>");
            }
            //if tree has childNodeLeft and our level is lower than max level we want to print to
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
        //works like previous one just in this case we want to print truth numbers in tree also
        private void DrawTree(TruthTree tree, int maxLevel = 0, int level = 0, List<string>? contradiction = null)
        {
            if (maxLevel >= level)
            {
                htmlTree.Add("<li>");
                if (tree.literal == null)
                    htmlTree.Add("<span class=tf-nc>" + GetEnumDescription(tree.MOperator) + "=" + tree.Item + "</span>");
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
                    htmlTree.Add("<span class=tf-nc>" + GetEnumDescription(tree.MOperator) + "= </span>");
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

        //print steps for truthTrees
        public void PrintLevelOrder(TruthTree tree, int startLevel = 2)
        {
            if (tree == null)
            {
                return;
            }
            Queue<(TruthTree, int)> queue = new();
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

                        if (tree.MOperator != OperatorEnum.EMPTY && tree.ChildNodeRight != null)
                        {
                            Steps.Add((j + ") - Pokud operátor " + GetEnumDescription(tree.MOperator) + " má hodnotu " + tree.Item + " tak můžeme zkusit dosadit " + tree.ChildNodeLeft.Item + " a " + tree.ChildNodeRight.Item + "<br>"));
                            j++;
                        }
                        else if (tree.MOperator != OperatorEnum.EMPTY && tree.ChildNodeRight == null)
                        {
                            Steps.Add((j + ") - Pokud operátor " + GetEnumDescription(tree.MOperator) + " má hodnotu " + tree.Item + " tak dosadíme " + tree.ChildNodeLeft.Item + "<br>"));
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