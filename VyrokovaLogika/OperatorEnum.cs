using System;
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
            [System.ComponentModel.Description("")]
            EMPTY,
            [System.ComponentModel.Description("∧")]
            AND,
            [System.ComponentModel.Description("∨")]
            OR,
            [System.ComponentModel.Description("⇒")]
            IMPLICATION,
            [System.ComponentModel.Description("¬")]
            NEGATION,
            [System.ComponentModel.Description("≡")]
            EQUIVALENCE,
            [System.ComponentModel.Description("¬¬")]
            DOUBLENEGATION
        }

        //get Enum value like operator
        public static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false) is System.ComponentModel.DescriptionAttribute[] attributes && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            return value.ToString();
        }

        public static OperatorEnum GetOperator(string sign)
        {
            return sign switch
            {
                "∨" => OperatorEnum.OR,
                "∧" => OperatorEnum.AND,
                "⇒" => OperatorEnum.IMPLICATION,
                "¬" => OperatorEnum.NEGATION,
                "≡" => OperatorEnum.EQUIVALENCE,
                "¬¬" => OperatorEnum.DOUBLENEGATION,
                _ => OperatorEnum.EMPTY,
            };
        }
    }
}
