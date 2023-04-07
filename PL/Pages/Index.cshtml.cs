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
        public string TautologyDecision { get; set; }

        public List<string> DAGNodes { get; set; } = new List<string>();
        public List<Tuple<int, int>> TreeConnectionsNumbered { get; set; } = new List<Tuple<int, int>>();

        public List<Tuple<string, string>> TreeConnections { get; set; } = new List<Tuple<string, string>>();

        public List<Tuple<string, int>> DAGNodesNumbered { get; set; } = new List<Tuple<string, int>>();
        public List<SelectListItem> listItems { get; set; } = new List<SelectListItem>();

        public IndexModel()
        {
            string mPropositionalSentence = "(B > A) > A";
            Converter.ConvertSentence(ref mPropositionalSentence);

            string mPropositionalSentence1 = "(((-x|b)&(x|a)) | (x&B)) >((a|b)&(b&c))";
            Converter.ConvertSentence(ref mPropositionalSentence1);

            string mPropositionalSentence2 = "((((-x|b)&(x|a))))";
            Converter.ConvertSentence(ref mPropositionalSentence2);

            SelectListItem item1 = new SelectListItem(mPropositionalSentence, mPropositionalSentence);
            SelectListItem item2 = new SelectListItem(mPropositionalSentence1, mPropositionalSentence1);
            SelectListItem item3 = new SelectListItem(mPropositionalSentence2, mPropositionalSentence2);
            listItems.Add(item1);
            listItems.Add(item2);
            listItems.Add(item3);
        }

        public void OnPost()
        {
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

            engine.ProcessSentence();
            var tree = engine.tree;
            var dag = engine.Dag;
            PrintTree(tree);
            PrepareDAGNodesList(dag);
            DAGNodes = DAGNodes.Distinct().ToList();

            PrepareDAGNodesListConnection(dag);
            RemoveDuplicates();
            ReplaceConnectionNumbersForString();
            string div = "<div class='tf-tree tf-gap-lg'>".Replace("'", "\"");
            ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
            if (engine.Tautology)
            {
                TautologyDecision = "Propositional sentence is Tautology";
            }
            else TautologyDecision = "Propositional sentence is not Tautology";
        }

        private void ReplaceConnectionNumbersForString()
        {
            foreach (var connection in TreeConnectionsNumbered)
            {
                TreeConnections.Add(new Tuple<string, string>(SearchByNumber(connection.Item1),SearchByNumber(connection.Item2)));
            }
        }

        // Method to search for a corresponding tuple by number
        public string SearchByNumber(int number)
        {
            return DAGNodesNumbered.FirstOrDefault(t => t.Item2 == number).Item1;
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

        private void PrepareDAGNodesList(DAGNode tree)
        {
            DAGNodesNumbered.Add((new Tuple<string, int>(tree.Item.mSentence, tree.Item.number)));
            DAGNodes.Add(tree.Item.mSentence);
            if (tree.LeftChild != null)
            {
                PrepareDAGNodesList(tree.LeftChild);
                if (tree.RightChild != null)
                {
                    PrepareDAGNodesList(tree.RightChild);
                }
            }
        }

        private void PrepareDAGNodesListConnection(DAGNode tree)
        {
            if (tree.LeftChild != null)
            {
                TreeConnectionsNumbered.Add(new Tuple<int, int>(tree.Item.number, tree.LeftChild.Item.number));
            }
            if (tree.RightChild != null)
            {
                TreeConnectionsNumbered.Add(new Tuple<int, int>(tree.Item.number, tree.RightChild.Item.number));
            }
            if (tree.LeftChild != null)
            {
                PrepareDAGNodesListConnection(tree.LeftChild);
                if (tree.RightChild != null)
                {
                    PrepareDAGNodesListConnection(tree.RightChild);
                }
            }
        }

        private void RemoveDuplicates()
        {
            TreeConnectionsNumbered = TreeConnectionsNumbered.Distinct(new TupleEqualityComparer<int, int>()).ToList();
        }
    }
}