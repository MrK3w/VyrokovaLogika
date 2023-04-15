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
        public ExerciseTreeConstructer(string hmtlTree) 
        { 
            mHtmlTree = hmtlTree;
        }

        public bool IsTreeOkay()
        {
            throw new NotImplementedException();
        }

        public TruthTree ProcessTree()
        {
            var strippedTags = StripTree();
            CreateTree(strippedTags);
            return tree;
        }

        private void CreateTree(List<string> strippedTags)
        {
            bool itIsItem = false;
            bool firstLi = true;
            bool isUl = false;
            foreach (string tag in strippedTags)
            {
                if (tag == "</li>") continue;
                if (itIsItem)
                {
                    if (Validator.ContainsOperator(tag[0].ToString()))
                    {
                        tree.mOperator = Operator.GetOperator(tag[0].ToString());
                    }
                    else
                    {
                        tree.literal = tag[0].ToString();
                    }
                    tree.Item = Int32.Parse(tag[2].ToString());
                    itIsItem = false;
                    continue;
                }
                else if (tag == "<start>")
                {
                    continue;
                }
                else if (tag == "</item>") continue;
                else if (tag == "<li>")
                {
                    
                    isUl = false;
                   
                    if(firstLi == true)
                    {
                        if (tree == null)
                        {
                            tree = new TruthTree();
                        }
                        else
                        {
                            tree = tree.AddChild("left");
                            firstLi = false;
                        } 
                    }
                    else
                    {
                        tree = tree.AddChild("right");
                        firstLi = true;
                    }

                }
                else if (tag == "</ul>")
                {
                    tree = tree.Parent;
                    firstLi = false;
                }
                else if (tag == "<ul>")
                {
                    firstLi = true;
                    isUl = true;
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

            mHtmlTree = mHtmlTree.Replace("<span class=\"tf-nc\">", "<item>").Replace("</span>","</item>");
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
