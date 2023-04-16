using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class TruthTreeVerifierForExercises
    {
        bool mTautology = false;
        bool mFindingContradiction = false;
        TruthTree mTree { get; set; }
        public string ExerciseQuote { get; set; }
        bool mistake = false;
        public TruthTreeVerifierForExercises(bool tautology, TruthTree tree, bool findingContradiction)
        {
            mTree = tree;
            mTautology = tautology;
            mFindingContradiction = findingContradiction;
        }

        public bool Verify()
        {
            if (mTautology)
            {
                if (mTree.Item != 0)
                {
                    ExerciseQuote = "You must be searching evaluation where formula is 0, if you want to find tautology or show that this is not tautulogy";
                    mTree.invalid = true;
                    return false;
                }
            }
            if (!mTautology)
            {
                if (mTree.Item != 1)
                {
                    ExerciseQuote = "You must be searching evaluation where formula is 1, if you want to find Contradiction or show that this is not contradiction";
                    mTree.invalid = true;
                    return false;
                }
            }
            CheckEvaluation(mTree);
            if (mistake)
            {
                ExerciseQuote = "You have mistake in values of logical operators! Look on values of children!";
                return false;
            }
            if (CheckContradiction() )
            {
                if (mFindingContradiction)
                {
                    ExerciseQuote = "You have in your equation semantic contradiction! So this cannot be tautology or contradiction";
                    return false;
                }
            }            
            ExerciseQuote = "You have equation correctly!";
            return true;
        }

        private void CheckEvaluation(TruthTree tree)
        {
            if (mistake) return;
            if (tree.literal != null) return;
          

            if (tree.ChildNodeRight != null)
            {
                var listValues = Rule.GetValuesOfBothSides(tree.Item, tree.mOperator);
                mistake = true;
                foreach (var listValue in listValues)
                {
                    if(listValue.Item1 == tree.ChildNodeLeft.Item && listValue.Item2 == tree.ChildNodeRight.Item)
                    {
                        mistake = false;
                        break;
                    }
                   
                }
                if (mistake) tree.invalid = true;
            }
            else
            {
                if (tree.mOperator == Operator.OperatorEnum.NEGATION)
                {
                    if (tree.Item == tree.ChildNodeLeft.Item)
                    {
                        mistake = true;
                        tree.invalid = true;
                    }
                }
                if(tree.mOperator == Operator.OperatorEnum.DOUBLENEGATION)
                {
                        if (tree.Item != tree.ChildNodeLeft.Item)
                        {
                            tree.invalid = true;
                            mistake = true;
                        }
                }
            }
            if(tree.ChildNodeLeft != null) CheckEvaluation(tree.ChildNodeLeft);
            if (tree.ChildNodeRight != null) CheckEvaluation(tree.ChildNodeRight);

            return;
        }

        private bool CheckContradiction()
        {
            var elementalNodes = mTree.GetLeafNodes();
            foreach (var node in elementalNodes)
            {

                foreach (var node1 in elementalNodes)
                {
                    if (node1.literal == node.literal && node1.Item != node.Item)
                    {
                        mTree.MarkContradiction(node.literal);
                        return true;
                    }
                }
            }
            return false;

        }


    }
}
