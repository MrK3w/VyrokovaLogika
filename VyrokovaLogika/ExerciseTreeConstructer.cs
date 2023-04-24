﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class ExerciseTreeConstructer
    {
        string mHtmlTree;
        TruthTree tree;
        public Tree interactiveTree { get; set; }
        private bool mSemanticContradiction;
        private bool mTautology;
        public bool Green { get; set; }
        public string ExerciseQuote { get; set; }

        public ExerciseTreeConstructer(string hmtlTree) 
        { 
            mHtmlTree = hmtlTree;
        }

        public void IsTreeOkay()
        {
            TruthTreeVerifierForExercises verifier = new TruthTreeVerifierForExercises(mTautology, tree, mSemanticContradiction);
            if (verifier.Verify())
            {
                ExerciseQuote = verifier.ExerciseQuote;
                Green = verifier.green;
                return;
            }
            ExerciseQuote = verifier.ExerciseQuote;
        }

        public TruthTree ProcessTree(bool tautology, bool contradiction)
        {
            mSemanticContradiction = contradiction;
            mTautology = tautology;
            var strippedTags = StripTree();
            CreateTree(strippedTags);
            return tree;
        }

        public Tree ProcessTreeForInteractiveDrawing()
        {
            var strippedTags = StripTree();
            CreateTreeForInteractiveDrawing(strippedTags);
            return interactiveTree;
        }

        private void CreateTree(List<string> strippedTags)
        {
            bool itIsItem = false;
            bool ThereWasLi = false;
            foreach (string tag in strippedTags)
            {
                if(tag == "</li>") { ThereWasLi = true;
                    continue;
                }
                if (itIsItem)
                {
                    if (Validator.ContainsOperator(tag[0].ToString()) || Validator.ContainsNegationOnFirstPlace(tag[0].ToString()))
                    {
                        tree.mOperator = Operator.GetOperator(tag[0].ToString());
                    }
                    else
                    {
                        tree.literal = tag[0].ToString();
                    }
                    tree.Item = Int32.Parse(tag[2].ToString());
                    if (tag.Length == 5 && tag[4] == 'x') tree.contradiction = true;
                    itIsItem = false;
                    continue;
                }
                else if (tag == "<start>")
                {
                    tree = new TruthTree();
                }
                else if (tag == "</item>") continue;
                else if (tag == "<li>" && ThereWasLi)
                {
                    tree = tree.Parent;
                    tree.AddChild("right");
                    tree = tree.ChildNodeRight;
                    ThereWasLi = false;
                }
                else if (tag == "</ul>")
                {
                    tree = tree.Parent;
                }
                else if (tag == "<ul>")
                {
                    tree.AddChild("left");
                    tree =tree.ChildNodeLeft;
                }
                else if (tag == "<item>")
                        {
                            itIsItem = true;
                            continue;
                        }
            }
        }

        private void CreateTreeForInteractiveDrawing(List<string> strippedTags)
        {
            bool itIsItem = false;
            bool ThereWasLi = false;
            foreach (string tag in strippedTags)
            {
                if (tag == "</li>")
                {
                    ThereWasLi = true;
                    continue;
                }
                if (itIsItem)
                {
                    interactiveTree.Item = new Node(tag);
                    itIsItem = false;
                    continue;
                }
                else if (tag == "<start>")
                {
                    interactiveTree = new Tree();
                }
                else if (tag == "</item>") continue;
                else if (tag == "<li>" && ThereWasLi)
                {
                    interactiveTree = interactiveTree.Parent;
                    interactiveTree.AddChild("right");
                    interactiveTree = interactiveTree.childNodeRight;
                    ThereWasLi = false;
                }
                else if (tag == "</ul>")
                {
                    interactiveTree = interactiveTree.Parent;
                }
                else if (tag == "<ul>")
                {
                    interactiveTree.AddChild("left");
                    interactiveTree = interactiveTree.childNodeLeft;
                }
                else if (tag == "<item>")
                {
                    itIsItem = true;
                    continue;
                }
            }
        }

        private List<string> StripTree()
        {

            mHtmlTree = mHtmlTree.Replace("<span class=\"tf-nc\">", "<item>").Replace("</span>","</item>").Replace("<span class=\"tf-nc\" style=\"color: red;\">","<item>").Replace("<span class=\"tf-nc\" style=\"color: green;\">", "<item>");
            string[] delimiters = { "<li>", "</li>", "<item>", "</item>", "<ul>", "</ul>" };

            // Split the input string by the delimiters
            return SplitWithDelimiters(mHtmlTree, delimiters);
        }

        static List<string> SplitWithDelimiters(string input, string[] delimiters)
        {
            // Replace delimiters with a unique marker
            for (int i = 0; i < delimiters.Length; i++)
            {
                input = input.Replace(delimiters[i], $"|DELIMITER{i}|");
            }

            // Split the input string by the unique marker
            var tempList = input.Split('|').ToList();
            List<string> result = new List<string>
            {
                "<start>",
            };
            result.AddRange(tempList);
            // Replace the unique marker with the original delimiters
            for (int i = 0; i < result.Count; i++)
            {
                for (int j = 0; j < delimiters.Length; j++)
                {
                    result[i] = result[i].Replace($"DELIMITER{j}", delimiters[j]);
                }
            }

            return result.Where(s => !string.IsNullOrEmpty(s)).ToList();
        }
    }
}
