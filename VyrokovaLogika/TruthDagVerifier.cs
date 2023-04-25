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
        readonly List<Tuple<string, string>> mConnections;
        readonly bool mTautology;
        readonly bool mContradiction;
        readonly List<string> mDAGNodes;
        public int MIssueIndex { get; set; } = -1;
        public string ExerciseQuote { get; set; }

        //If we find contraction where we are trying to find Tautology is it okay
        public TruthDagVerifier(List<Tuple<string,string>> connections, List<string> DAGNodes , bool tautology, bool contradiction) 
        {
            mConnections = connections;
            mTautology = tautology;
            mContradiction = contradiction;
            mDAGNodes = DAGNodes;
            Verify();
        }

        public bool Verify()
        {
            //if root of dag don't have right truth value return false
            if (!CheckFirstValue())
            {
                return false;
            }
            //check evaluations on all sides of dag
            if (!CheckEvaluations())
            {
                return false;
            }
            //check contradiction in dag
            if (CheckContradiction())
            {
                if (mContradiction)
                {
                    ExerciseQuote = "Máš to správně";
                }

            }
            return true;
        }
        //check evaluations based on list values
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

        //check values on one line
        private bool CheckValuesOnOneLine(int i)
        {
            //split item and by '=' and check values
            var leftSideItem = mConnections[i].Item1.Split("=").Select(s => s.Trim()).ToList();
            var rightSideItem = mConnections[i].Item2.Split("=").Select(s => s.Trim()).ToList();

            return true;
        }

        private bool CheckValues(int i)
        {
            //we need to split first item by = to get formula and truth value
            var item = mConnections[i].Item1.Split("=").Select(s => s.Trim()).ToList();
            //in case we have issue to know which line it is
            string issue = mConnections[i].Item1;
            //we need to get operator from first item
            Node node = new(item[0]);
            Splitter splitter = new(node);
            splitter.Split();
            node.MOperator = splitter.MNode.MOperator;

            //we need to get truth value
            var MainValue = int.Parse(item[1]);
            //from items we need to get also truth values
            var firstValue = mConnections[i].Item2.Split("=").Select(s => s.Trim()).ToList();
            var secondValue = mConnections[i + 1].Item2.Split("=").Select(s => s.Trim()).ToList();
            int firstValueV = 0;
            int secondValueV = 0;
            int firstValueVV = -1;
            int secondValueVV = -1;
            List<(int, int)> values = Rule.GetValuesOfBothSides(MainValue, node.MOperator);
            if (firstValue.Count == 2 && secondValue.Count == 2)
            {
                //we are controlling main truth value with items to check if evaluation is alright
                firstValueV = int.Parse(firstValue[1]);
                secondValueV = int.Parse(secondValue[1]);
                foreach (var value in values)
                {
                    if (value.Item1 == firstValueV && value.Item2 == secondValueV) return true;
                }
                ExerciseQuote = $"Pokud {node.MOperator} má hodnotu {MainValue} jeho potomci nemůžou mít hodnotu {firstValueV} a {secondValueV}";
                MIssueIndex = mDAGNodes.FindIndex(str => str == issue);
                return false;
            }
            else if (firstValue.Count == 3 && secondValue.Count == 2)
            {
                firstValueV = int.Parse(firstValue[1]);
                secondValueV = int.Parse(secondValue[1]);
                firstValueVV = int.Parse(firstValue[2]);
                foreach (var value in values)
                {
                    if (value.Item1 == firstValueV && value.Item2 == secondValueV || value.Item1 == firstValueVV && value.Item2 == secondValueV) return true;
                }
                ExerciseQuote = $"Pokud {node.MOperator} má hodnotu {MainValue} jeho potomci nemůžou mít hodnotu {firstValueV} {firstValueVV} a {secondValueV}";
                MIssueIndex = mDAGNodes.FindIndex(str => str == issue);
                return false;
            }
            else if (firstValue.Count == 2 && secondValue.Count == 3)
            {
                firstValueV = int.Parse(firstValue[1]);
                secondValueV = int.Parse(secondValue[1]);
                secondValueVV = int.Parse(secondValue[2]);
                foreach (var value in values)
                {
                    if (value.Item1 == firstValueV && value.Item2 == secondValueV || value.Item1 == firstValueV && value.Item2 == secondValueVV) return true;
                }
                ExerciseQuote = $"Pokud {node.MOperator} má hodnotu {MainValue} jeho potomci nemůžou mít hodnotu {firstValueV} a {secondValueV} {secondValueVV}";
                MIssueIndex = mDAGNodes.FindIndex(str => str == issue);
                return false;
            }
            else if(firstValue.Count == 3 && secondValue.Count == 3)
            {
                firstValueV = int.Parse(firstValue[1]);
                secondValueV = int.Parse(secondValue[1]);
                firstValueVV = int.Parse(firstValue[2]);
                secondValueVV = int.Parse(secondValue[2]);
                foreach (var value in values)
                {
                    if (value.Item1 == firstValueV && value.Item2 == secondValueV || value.Item1 == firstValueV && value.Item2 == secondValueVV
                        || value.Item1 == firstValueVV && value.Item2 == secondValueV || value.Item1 == firstValueVV && value.Item2 == secondValueVV) return true;
                }
                ExerciseQuote = $"Pokud {node.MOperator} má hodnotu {MainValue} jeho potomci nemůžou mít hodnotu {firstValueV} {firstValueVV} a {secondValueV} {secondValueVV}";
                MIssueIndex = mDAGNodes.FindIndex(str => str == issue);
                return false;
            }
            return true;

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
            MIssueIndex = 0;
            if(mTautology)
            ExerciseQuote = "Formule musí mít hodnotu 0 pokud chceš hledat sémantický spor ve formuli, která je tautologii.";

            else
            {
                ExerciseQuote = "Formule musí mít hodnotu 1 pokud chceš hledat sémantický spor ve formuli, která je kontradikci";
            }
            //Verify tautology
            var firstPart = mConnections[0].Item1.Split("=");
            if (firstPart.Length != 2)
            {
                ExerciseQuote = "Formule nemůže mít dvě hodnoty.";
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
            ExerciseQuote = "Máš to správně!";
            MIssueIndex = -1;
            return true;
        }
    }
}
