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

        public static string GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false) as System.ComponentModel.DescriptionAttribute[];
            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            return value.ToString();
        }

        public static OperatorEnum GetOperator(string sign)
        {
            switch (sign)
            {
                case "∨":
                    return OperatorEnum.OR;
                case "∧":
                    return OperatorEnum.AND;
                case "⇒":
                    return OperatorEnum.IMPLICATION;
                case "¬":
                    return OperatorEnum.NEGATION;
                case "≡":
                    return OperatorEnum.EQUIVALENCE;
                case "¬¬":
                    return OperatorEnum.DOUBLENEGATION;
                default:
                    return OperatorEnum.EMPTY;
            }
        }
    }
}
