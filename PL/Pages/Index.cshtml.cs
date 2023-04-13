using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VyrokovaLogika;

namespace PL.Pages
{
    public partial class IndexModel : PageModel
    {
        private string vl;
        private string vl1;
        private List<string> htmlTree = new List<string>();
        public string ConvertedTree { get; set; }
        public List<string> DAGNodes { get; set; } = new List<string>();
        public List<Tuple<string, string>> TreeConnections { get; set; } = new List<Tuple<string, string>>();

        public bool DAG { get; set; } = false;
        public string TautologyDecision { get; set; }

        public List<SelectListItem> listItems { get; set; } = new List<SelectListItem>();
        public bool Valid { get; private set; } = true;

        public IndexModel()
        {
            PrepareList();
        }

        private void PrepareList()
        {
            string mPropositionalSentence = "(p>q)≡(-q>-p)";
            Converter.ConvertLogicalOperators(ref mPropositionalSentence);

            string mPropositionalSentence1 = "(((-x|b)&(x|a)) | (x&B)) >((a|b)&(b&c))";
            Converter.ConvertLogicalOperators(ref mPropositionalSentence1);

            string mPropositionalSentence2 = "((((-x|b)&(x|a))))";
            Converter.ConvertLogicalOperators(ref mPropositionalSentence2);

            SelectListItem item1 = new SelectListItem(mPropositionalSentence, mPropositionalSentence);
            SelectListItem item2 = new SelectListItem(mPropositionalSentence1, mPropositionalSentence1);
            SelectListItem item3 = new SelectListItem(mPropositionalSentence2, mPropositionalSentence2);
            listItems.Add(item1);
            listItems.Add(item2);
            listItems.Add(item3);
        }

        public void OnPost(string submit)
        {
            switch (submit)
            {
                case "Create tree":
                    break;
                case "Create DAG":
                    DAG = true;
                    break;
                default:
                    throw new Exception();
            }
            vl = Request.Form["formula"];
            vl1 = Request.Form["UserInput"];
            Engine engine = new Engine("");
            if (vl == "" && vl1 == "") return;
            htmlTree.Clear();

           
            if (vl1 != "")
            {
                listItems.Add(new SelectListItem(vl1, vl1));
                var selected = listItems.Where(x => x.Value == vl1).First();
                selected.Selected = true;
                engine = new Engine(vl1);
            }

            else if (vl != "")
            {
                foreach (var item in listItems)
                {
                    item.Selected = false;
                    if (vl == item.Value) item.Selected = true;
                }
                engine = new Engine(vl);
            }
            Valid = engine.ProcessSentence();
            if (Valid)
            {
                if (DAG)
                {
                    engine.PrepareDAG();
                    TreeConnections = engine.TreeConnections;
                    DAGNodes = engine.DAGNodes;
                }
                else
                {
                    PrintTree(engine.tree);
                    string div = "<div class='tf-tree tf-gap-lg'>".Replace("'", "\"");
                    ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
                }
            }
           
            if (engine.Tautology)
            {
                TautologyDecision = "Propositional sentence is Tautology";
            }
            else TautologyDecision = "Propositional sentence is not Tautology";
        }

        private void PrintTree(Tree tree)
        {
            htmlTree.Add("<li>");
            if (tree.Item.mOperator != Operator.OperatorEnum.EMPTY)
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.mOperator + "</span>");
            }
            else
            {
                htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");
            }
            //htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");
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
    }
}