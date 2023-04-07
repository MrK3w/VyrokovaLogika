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
        public static List<(int, int)> GetValuesOfBothSides(int parentMustBe, OperatorEnum op)
        {
            List<(int,int)> valuesList = new List<(int, int)>();
            if(op == OperatorEnum.IMPLICATION)
            {
                if (parentMustBe == 0)
                {
                   valuesList.Add((1, 0));
                }
                else
                {
                    valuesList.Add((0, 1));
                    valuesList.Add((1, 1));
                    valuesList.Add((0, 0));
                }
                return valuesList;
            }
            else if(op == OperatorEnum.OR)
            {
                if (parentMustBe == 0)
                {
                    valuesList.Add((0, 0));
                }
                else
                {
                    valuesList.Add((0, 1));
                    valuesList.Add((1, 0));
                    valuesList.Add((1, 1));
                }
            }
            else if (op == OperatorEnum.AND)
            {
                if (parentMustBe == 1)
                {
                    valuesList.Add((1, 1));
                }
                else
                {
                    valuesList.Add((0, 1));
                    valuesList.Add((1, 0));
                    valuesList.Add((0, 0));
                }
            }
            else if (op == OperatorEnum.AND)
            {
                if (parentMustBe == 1)
                {
                    valuesList.Add((1, 1));
                }
                else
                {
                    valuesList.Add((0, 1));
                    valuesList.Add((1, 0));
                    valuesList.Add((0, 0));
                }
            }
            else if(op == OperatorEnum.EQUIVALENCE)
            {
                if(parentMustBe == 1)
                {
                    valuesList.Add((1, 1));
                    valuesList.Add((0, 0));
                }
                else
                {
                    valuesList.Add((0, 1));
                    valuesList.Add((1, 0));
                }
                    
            }
            return valuesList;
        }
    }
}
