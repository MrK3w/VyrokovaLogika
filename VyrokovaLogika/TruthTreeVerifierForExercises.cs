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
        readonly bool mTautology = false;
        readonly bool mFindingContradiction = false;
        TruthTree MTree { get; set; }
        public bool Green { get; set; }
        public string ExerciseQuote { get; set; }
        bool mistake = false;
        bool MarkedContradiction = true;
        bool ContradictionMarkedBadly = false;
        public TruthTreeVerifierForExercises(bool tautology, TruthTree tree, bool findingContradiction)
        {
            MTree = tree;
            mTautology = tautology;
            mFindingContradiction = findingContradiction;
        }

        public void Verify()
        {
            //if we need to verify tautology
            if (mTautology)
            {
                //item must be 0
                if (MTree.Item != 0)
                {
                    ExerciseQuote = "Musíš hledat evaluaci, kde je formule 0, pokud chceš hledat tautologii.";
                    MTree.invalid = true;
                    return;
                }
            }
            //if we need to verify contradiction
            if (!mTautology)
            {
                //item must be 1
                if (MTree.Item != 1)
                {
                    ExerciseQuote = "Musíš hledat evaluaci, kde je formule 1, pokud chceš hledat kontradikci.";
                    MTree.invalid = true;
                    return;
                }
            }
            //check truth values for each logical operator and their children
            CheckEvaluation(MTree);
            if (mistake)
            {
                ExerciseQuote = "Máš chybu v potomcích! Podívej se na jejich hodnoty.";
                return;
            }
            //check if there is contradiction 
            if (CheckContradiction() )
            {
                //check if we need contradiction in formula
                if (mFindingContradiction)
                {
                    ExerciseQuote = "Máš ve formuli semantický spor! Takže to nemůže být tautologie.";
                    return;
                }
                else
                {
                    Green = true;
                }
            }            
            //check if we marked contradiction
            if(!MarkedContradiction)
            {
                ExerciseQuote = "Neoznačil jsi spor!";
                return;
            }
            if(ContradictionMarkedBadly)
            {
                ExerciseQuote = "Označil jsi spor na nesprávném místě!";
                return;
            }
            ExerciseQuote = "Máš to správně!";
            return;
        }



        private void CheckEvaluation(TruthTree tree)
        {
            //if there is already mistake don't continue
            if (mistake) return;
            //if it is leaf don't continue
            if (tree.literal != null) return;
          
            //tree has rightNode
            if (tree.ChildNodeRight != null)
            {
                //we need to get values
                var listValues = Rule.GetValuesOfBothSides(tree.Item, tree.MOperator);
                //set temporary mistake to true
                mistake = true;
                foreach (var listValue in listValues)
                {
                    //we found this values in children, there is not mistake
                    if(listValue.Item1 == tree.ChildNodeLeft.Item && listValue.Item2 == tree.ChildNodeRight.Item)
                    {
                        mistake = false;
                        break;
                    }
                   
                }
                //there is mistake
                if (mistake) tree.invalid = true;
            }
            else
            {
                //if operator is negation we need childNodeLeft to have different value
                if (tree.MOperator == Operator.OperatorEnum.NEGATION)
                {
                    if (tree.Item == tree.ChildNodeLeft.Item)
                    {
                        mistake = true;
                        tree.invalid = true;
                    }
                }
                //we need same value if it is double negation
                if(tree.MOperator == Operator.OperatorEnum.DOUBLENEGATION)
                {
                        if (tree.Item != tree.ChildNodeLeft.Item)
                        {
                            tree.invalid = true;
                            mistake = true;
                        }
                }
            }
            //checking child nodes leads to recursion
            if(tree.ChildNodeLeft != null) CheckEvaluation(tree.ChildNodeLeft);
            if (tree.ChildNodeRight != null) CheckEvaluation(tree.ChildNodeRight);

            return;
        }

        private bool CheckContradiction()
        {
            //get leaf nodes for tree
            var elementalNodes = MTree.GetLeafNodes();
            //check if there is contradiction
            foreach (var node in elementalNodes)
            {

                foreach (var nodeSecondary in elementalNodes)
                {
                    //check if literal is the same and value different - we have contradiction
                    if ((nodeSecondary.literal == node.literal && nodeSecondary.Item != node.Item))
                    {
                        //check if contradiction was marked
                        if (!nodeSecondary.contradiction || !node.contradiction)
                        {
                            MarkedContradiction = false;
                            return false;
                        }

                        //method to mark contradiction in tree
                        MTree.MarkContradiction(node.literal);

                        return true;
                    }
                    //check if we truly marked only contradiction on right places
                    else if((nodeSecondary.literal == node.literal && nodeSecondary.Item == node.Item) && (nodeSecondary.contradiction && node.contradiction) && (node.number != nodeSecondary.number))
                    {
                        ContradictionMarkedBadly = true;
                        return false;
                    }
                }
            }
            return false;

        }


    }
}
