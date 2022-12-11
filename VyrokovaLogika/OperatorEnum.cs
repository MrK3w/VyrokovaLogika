﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace VyrokovaLogika
{
    public static class Operator
    {
        public enum OperatorEnum
        {
            EMPTY, AND, OR, IMPLICATION, NEGATION
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
                case "-":
                    return OperatorEnum.NEGATION;
                default:
                    return OperatorEnum.EMPTY;
            }
        }
    }
}
