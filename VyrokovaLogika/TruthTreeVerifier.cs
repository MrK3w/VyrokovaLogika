using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class TruthTreeVerifier
    {
        bool mNotTautology = false;
        TruthTree mTree { get; set; }
        public string ExerciseQuote { get; set; }
        public TruthTreeVerifier(bool notTautology, TruthTree tree)
        {
            mTree = tree;
            mNotTautology = notTautology;
        }

        public bool Verify()
        {
            if (mNotTautology)
            {
                if (mTree.Item != 0)
                {
                    ExerciseQuote = "You must be searching evaluation where formula is 0";
                    return false;
                }
                if (CheckContradiction())
                {
                    ExerciseQuote = "You have in your equation contradiction!";
                    return false;
                }

            }
            ExerciseQuote = "You have equation correctly!";
            return true;
        }

        private bool CheckContradiction()
        {
            var elementalNodes = mTree.GetLeafNodes();
            foreach (var node in elementalNodes)
            {

                foreach (var node1 in elementalNodes)
                {
                    if (node1.literal == node.literal && node1.Item != node.Item)
                        return true;
                }
            }
            return false;
        }
    }
}
