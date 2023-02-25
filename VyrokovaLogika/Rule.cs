using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VyrokovaLogika.Operator;

namespace VyrokovaLogika
{
    public static class Rule
    {
        //B&A a vyslo 1
        public static (int, int) GetValuesOfBothSides(int parentMustBe, OperatorEnum op)
        {
            if(op == OperatorEnum.IMPLICATION)
            {
                if (parentMustBe == 0) return (1, 0);
                else return (0, 0);
            }
            
            if(op == OperatorEnum.AND)
            {
                if (parentMustBe == 1) return (0, 1);
            }    
            return (0, 0);
        }
    }
}
