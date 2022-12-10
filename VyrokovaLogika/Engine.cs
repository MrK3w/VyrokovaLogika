using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    class Engine
    {
        string mPropositionalSentence;
        public Engine(string propositionalSentence)
        {
            mPropositionalSentence = propositionalSentence;
        }

        public void ProcessSentence()
        {
            //Replace white spaces to better organize this sentence
            mPropositionalSentence = mPropositionalSentence.Replace(" ", string.Empty).ToLowerInvariant();
            Tree tree;
            //check if sentence is valid
            if (Validator.Check(mPropositionalSentence))
            {
                tree = new Tree(mPropositionalSentence);
                tree.Process();
            }
        }
    }
}
