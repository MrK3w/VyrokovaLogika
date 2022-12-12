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
        public static (int, int) Getxy(int parentMustBe, OperatorEnum op)
        {
            if(op == OperatorEnum.IMPLICATION)
            {
                if (parentMustBe == 0) return (1, 0);
            }
            return (0, 0);
        }
    }
}
