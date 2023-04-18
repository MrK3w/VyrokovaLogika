using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public class TruthDagVerifier
    {
        List<Tuple<string, string>> mConnections;
        bool mTautology;
        bool mContradiction;
        public string ExerciseQuote { get; set; }

        //IF we find contraction where we are trying to find Tautology is it okay
        public TruthDagVerifier(List<Tuple<string,string>> connections, bool tautology, bool contradiction) 
        {
            mConnections = connections;
            mTautology = tautology;
            mContradiction = contradiction;
            Verify();
        }

        public bool Verify()
        {
            if (!CheckFirstValue())
            {
                return false;
            }
            if (CheckContradiction())
            {
                //if (mContradiction)
                //{
                //    ExerciseQuote = "There is contradiction";
                //}

            }
            if (CheckEvaluations())
            {

            }
            return true;
        }

        private bool CheckEvaluations()
        {
            int i = 0;
            while (i < mConnections.Count)
            {
                if (i < mConnections.Count - 1)
                {
                    if (mConnections[i].Item1 == mConnections[i + 1].Item1)
                    {
                        if (!CheckValues(i)) return false;
                        i += 2;
                    }
                    else
                    {
                        //this is pair where is negation 
                        if (!CheckValuesOnOneLine(i)) return false;
                        i++;
                    }
                }
                else
                {
                    //this is pair where is negation 
                    if (!CheckValuesOnOneLine(i)) return false;
                    i++;
                }
            }
            return true;
        }

        private bool CheckValuesOnOneLine(int i)
        {
            var item = mConnections[i].Item1.Split("=").Select(s => s.Trim()).ToList();
            var item2 = mConnections[i].Item2.Split("=").Select(s => s.Trim()).ToList();

            return true;
        }

        private bool CheckValues(int i)
        {
            //we need to split first item by = to get formula and truth value
            var item = mConnections[i].Item1.Split("=").Select(s => s.Trim()).ToList();
            
            //we need to get operator from first item
            Node node = new Node(item[0]);
            Splitter splitter = new Splitter(node);
            splitter.Split();
            node.mOperator = splitter.mNode.mOperator;

            //we need to get truth value
            var MainValue = int.Parse(item[1]);
            //from items we need to get also truth values
            var firstValue = mConnections[i].Item2.Split("=").Select(s => s.Trim()).ToList();
            var secondValue = mConnections[i + 1].Item2.Split("=").Select(s => s.Trim()).ToList();
            int firstValueV = 0;
            int secondValueV = 0;
            int firstValueVV = 0;
            int secondValueVV = 0;
            if (firstValue.Count == 2 && secondValue.Count == 2)
            {
                firstValueV = int.Parse(firstValue[1]);
                secondValueV = int.Parse(secondValue[1]);
            }
            //we are controlling main truth value with items to check if evaluation is alright
            List<(int, int)> values = Rule.GetValuesOfBothSides(MainValue, node.mOperator);
            foreach (var value in values)
            {
                if (value.Item1 == firstValueV && value.Item2 == secondValueV || value.Item1 == secondValueV && value.Item2 == firstValueV) return true;
            }
            ExerciseQuote = $"if {node.mOperator} has value {MainValue} his childrens can't have values {firstValueV} and {secondValueV}";
            return false;
        }

        private bool CheckContradiction()
        {
            foreach (var connection in mConnections)
            {
                var parts = connection.Item1.Split("=");
                if(parts.Length == 3) {
                    return true;
                }
                parts = connection.Item2.Split("=");
                if(parts.Length == 3) {
                    return true;
                }
            }
            return false;
        }

        private bool CheckFirstValue()
        {
            if(mTautology)
            ExerciseQuote = "Main node value must be 0 if you want to find contradiction in Tautology formula";
            else
            {
                ExerciseQuote = "Main node value must be 1 if you want to find contradiction in Contradiction formula";
            }
            //Verify tautology
            var firstPart = mConnections[0].Item1.Split("=");
            if (firstPart.Length != 2)
            {
                ExerciseQuote = "Main node can't have two values";
                return false;
            }
            firstPart[1] = firstPart[1].Replace(" ", "");
            if (firstPart[1] == "1" && mTautology)
            {
                return false;
            }
            if (firstPart[1] == "0" && !mTautology)
            {
                return false;
            }
            ExerciseQuote = "It is alright!";
            return true;
        }
    }
}
