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
        public bool green { get; set; }
        public string ExerciseQuote { get; set; }
        bool mistake = false;
        bool MarkedContradiction = true;
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
                    ExerciseQuote = "Musíš hledat evaluaci, kde je formule 0, pokud chceš hledat tautologii.";
                    mTree.invalid = true;
                    return false;
                }
            }
            if (!mTautology)
            {
                if (mTree.Item != 1)
                {
                    ExerciseQuote = "Musíš hledat evaluaci, kde je formule 1, pokud chceš hledat kontradikci.";
                    mTree.invalid = true;
                    return false;
                }
            }
            CheckEvaluation(mTree);
            if (mistake)
            {
                ExerciseQuote = "Máš chybu v potomcích! Podívej se na jejich hodnoty.";
                return false;
            }
            if (CheckContradiction() )
            {
                if (mFindingContradiction)
                {
                    ExerciseQuote = "Máš ve formuli semantickou kontradikci! Takže to nemůže být tautologie.";
                    return false;
                }
                else
                {
                    green = true;
                }
            }            
            if(!MarkedContradiction)
            {
                ExerciseQuote = "Neoznačil jsi spor!";
                return false;
            }
            ExerciseQuote = "Máš to správně!";
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
                    if ((node1.literal == node.literal && node1.Item != node.Item))
                    {
                        if (!node1.contradiction || !node.contradiction)
                        {
                            MarkedContradiction = false;
                            return false;
                        }
                            
                        mTree.MarkContradiction(node.literal);

                        return true;
                    }
                }
            }
            return false;

        }


    }
}
