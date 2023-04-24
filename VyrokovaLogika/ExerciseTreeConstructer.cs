using System;
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
        public Tree InteractiveTree { get; set; }
        private bool mSemanticContradiction;
        private bool mTautology;
        public bool Green { get; set; }
        public string ExerciseQuote { get; set; }

        public ExerciseTreeConstructer(string hmtlTree) 
        { 
            mHtmlTree = hmtlTree;
        }

        //create new instance of TruthTreeVerifier and verify mistakes
        public void IsTreeOkay()
        {
            TruthTreeVerifierForExercises verifier = new(mTautology, tree, mSemanticContradiction);
            verifier.Verify();
            ExerciseQuote = verifier.ExerciseQuote;
            //green for exercises if we correctly marked contradiction
            Green = verifier.Green;
            return;
        }

        //create truthtree from html code
        public TruthTree ProcessTree(bool tautology, bool contradiction)
        {
            mSemanticContradiction = contradiction;
            mTautology = tautology;
            var strippedTags = StripTree();
            CreateTree(strippedTags);
            return tree;
        }

        //create tree from htmlCode
        public Tree ProcessTreeForInteractiveDrawing()
        {
            var strippedTags = StripTree();
            CreateTreeForInteractiveDrawing(strippedTags);
            return InteractiveTree;
        }

        //createTree from List of tag string
        private void CreateTree(List<string> strippedTags)
        {
            //bool value if next value is item
            bool itIsItem = false;
            //if previous tag was already </li>
            bool ThereWasLi = false;
            foreach (string tag in strippedTags)
            {
                if(tag == "</li>") { ThereWasLi = true;
                    continue;
                }
                //if it item we need to get this values
                if (itIsItem)
                {
                    //if on first place is operator we will save him to tree
                    if (Validator.ContainsOperator(tag[0].ToString()) || Validator.ContainsNegationOnFirstPlace(tag[0].ToString()))
                    {
                        tree.MOperator = Operator.GetOperator(tag[0].ToString());
                    }
                    //else is there literal
                    else
                    {
                        tree.literal = tag[0].ToString();
                    }
                    //third char is truth value
                    tree.Item = int.Parse(tag[2].ToString());
                    if (tag.Length == 5 && tag[4] == 'x') tree.contradiction = true;
                    itIsItem = false;
                    continue;
                }
                //tag start we will create new truthTree
                else if (tag == "<start>")
                {
                    tree = new TruthTree();
                }
                //if there is </item> we will just skip this
                else if (tag == "</item>") continue;
                //if <li> and there was already <\li>
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
                    InteractiveTree.Item = new Node(tag);
                    itIsItem = false;
                    continue;
                }
                else if (tag == "<start>")
                {
                    InteractiveTree = new Tree();
                }
                else if (tag == "</item>") continue;
                else if (tag == "<li>" && ThereWasLi)
                {
                    InteractiveTree = InteractiveTree.Parent;
                    InteractiveTree.AddChild("right");
                    InteractiveTree = InteractiveTree.childNodeRight;
                    ThereWasLi = false;
                }
                else if (tag == "</ul>")
                {
                    InteractiveTree = InteractiveTree.Parent;
                }
                else if (tag == "<ul>")
                {
                    InteractiveTree.AddChild("left");
                    InteractiveTree = InteractiveTree.childNodeLeft;
                }
                else if (tag == "<item>")
                {
                    itIsItem = true;
                    continue;
                }
            }
        }

        //strip html string to list
        private List<string> StripTree()
        {
            //replace all occurences of span to item
            mHtmlTree = mHtmlTree.Replace("<span class=\"tf-nc\">", "<item>").Replace("</span>","</item>").Replace("<span class=\"tf-nc\" style=\"color: red;\">","<item>").Replace("<span class=\"tf-nc\" style=\"color: green;\">", "<item>");
            //split by this delimeter
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
            List<string> result = new()
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
