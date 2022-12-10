using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public static class Operator
    {
        public enum OperatorEnum
        {
            EMPTY, AND, OR, IMPLICATION
        }
  
        public static OperatorEnum GetOperator(string sign)
        {
            switch (sign)
            {
                case "|":
                    return OperatorEnum.OR;
                case "&":
                    return OperatorEnum.AND;
                case ">":
                    return OperatorEnum.IMPLICATION;
                default:
                    return OperatorEnum.EMPTY;
            }
        }
    }
}
