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
        public int number;
        public Operator.OperatorEnum mOperator { get; set; }
        public int valueMustBe { get; set; }
        public int level;

        public Node(string sentence, int level = 1)
        {
            if (level == 1) valueMustBe = 0;
            mSentence = sentence;
            this.level = level;
        }

        public override string ToString()
        {
            return mSentence;
        }
    }
}
