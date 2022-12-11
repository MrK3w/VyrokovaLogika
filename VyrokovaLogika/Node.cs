using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VyrokovaLogika
{
    public class Node
    {
        public string mSentence { get; set; }
        public Operator.OperatorEnum mOperator { get; set; }
        int upperEquals;
        public int level;
        public bool isFinal = false;

        public Node(string sentence, int level = 1)
        {
            mSentence = sentence;
            this.level = level;
        }

        public override string ToString()
        {
            return mSentence;
        }
    }
}
