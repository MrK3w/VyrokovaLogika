﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            CheckContradiction
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

        public List<Tuple<string, int>> distinctNodes { get; set; } = new List<Tuple<string, int>>();
        public bool IsTautologyOrContradiction { get; set; }

        public List<SelectListItem> listItems { get; set; } = new List<SelectListItem>();
        public bool Valid { get; private set; } = true;

        public IndexModel()
        {
            PrepareList();
        }


        private void PrepareList()
        {
            listItems = ListItemsHelper.ListItems;
        }

        public void OnPost(string submit)
        {
            Engine engine = new Engine("");
            vl = Request.Form["formula"];
            vl1 = Request.Form["UserInput"];
            if (vl == "" && vl1 == "")
            {
                Valid = false;
                return;
            }
            if (vl1 != "")
            {
                engine = new Engine(vl1);
                if (!engine.ValidateSentence())
                {
                    Valid = false;
                    return;
                }
                Converter.ConvertLogicalOperators(ref vl1);
                ListItemsHelper.SetListItems(vl1);
                var selected = listItems.Where(x => x.Value == vl1).First();
                selected.Selected = true;
               
            }

            else if (vl != "")
            {
                engine = new Engine(vl);
                if (!engine.ValidateSentence())
                {
                    Valid = false;
                    return;
                }
                foreach (var item in listItems)
                {
                    item.Selected = false;
                    if (vl == item.Value)
                    {
                        item.Selected = true;
                    }
                }
            }
            switch (submit)
            {
                case "Create tree":
                 button  = ButtonType.SyntaxTree;
                    break;
                case "Create DAG":
                    button = ButtonType.DAG;
                        break;
                case "Check Tautology":
                    button = ButtonType.CheckTautology;
                    break;
                case "Check Contradiction":
                    button = ButtonType.CheckContradiction;
                    break;
                default:
                    throw new Exception();
            }
         
            htmlTree.Clear();
            htmlTreeTruth.Clear();
            engine.ProcessSentence();
            if (Valid)
            {
                switch (button)
                {
                    case ButtonType.DAG:
                        engine.PrepareDAG();
                        TreeConnections = engine.TreeConnections;
                        DAGNodes = engine.DAGNodes;
                        break;
                    case ButtonType.SyntaxTree:
                        PrintTree(engine.tree, 1);
                        string div = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
                        ConvertedTree = div + string.Join("", htmlTree.ToArray()) + "</div>";
                        break;
                    case ButtonType.CheckTautology:
                        IsTautologyOrContradiction = engine.ProofSolver("Tautology");
                        if (!IsTautologyOrContradiction)
                        {
                            PrintTree(engine.tree);
                            string di = "<div class='tf-tree tf-gap-lg'>".Replace("'", "\"");
                            ConvertedTree = di + string.Join("", htmlTree.ToArray()) + "</div>";
                            distinctNodes = engine.distinctNodes;
                            PrintTree(engine.counterModel);
                            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
                            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";
                        }
                        break;
                    case ButtonType.CheckContradiction:
                       
                        IsTautologyOrContradiction = engine.ProofSolver("Contradiction");
                        if (!IsTautologyOrContradiction)
                        {
                            distinctNodes = engine.distinctNodes;
                            PrintTree(engine.counterModel);
                            string d = "<div class='tf-tree tf-gap-sm'>".Replace("'", "\"");
                            ConvertedTreeTruth = d + string.Join("", htmlTreeTruth.ToArray()) + "</div>";
                        }
                        break;
                }
            }   
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
            //htmlTree.Add("<span class=tf-nc>" + tree.Item.mSentence + "</span>");
            if (tree.childNodeLeft != null && i < 1000)
            {
                htmlTree.Add("<ul>");
                PrintTree(tree.childNodeLeft, i+1);
                if (tree.childNodeRight != null)
                {
                    PrintTree(tree.childNodeRight, i+1);
                }
                htmlTree.Add("</ul>");
            }
            htmlTree.Add("</li>");
        }

        private void PrintTree(TruthTree tree)
        {
            htmlTreeTruth.Add("<li>");
            if(tree.literal == null)
                htmlTreeTruth.Add("<span class=tf-nc>" + GetEnumDescription(tree.mOperator) + "=" + tree.Item + "</span>");
            else
            {
                htmlTreeTruth.Add("<span class=tf-nc>" + tree.literal + "=" + tree.Item + "</span>");
            }
            if (tree.ChildNodeLeft != null)
            {
                htmlTreeTruth.Add("<ul>");
                PrintTree(tree.ChildNodeLeft);
                if (tree.ChildNodeRight != null)
                {
                    PrintTree(tree.ChildNodeRight);
                }
                htmlTreeTruth.Add("</ul>");
            }
            htmlTreeTruth.Add("</li>");
        }
    }
}