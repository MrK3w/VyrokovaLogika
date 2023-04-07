using System.Diagnostics.CodeAnalysis;

namespace PL.Pages
{
    public partial class IndexModel
    {
        // Custom equality comparer for Tuple<int, int>
        public class TupleEqualityComparer<T1, T2> : IEqualityComparer<Tuple<T1, T2>>
        {
            public bool Equals(Tuple<T1, T2> x, Tuple<T1, T2> y)
            {
                if (x == null && y == null)
                    return true;
                else if (x == null || y == null)
                    return false;
                else
                    return x.Item1.Equals(y.Item1) && x.Item2.Equals(y.Item2);
            }

            public int GetHashCode([DisallowNull] Tuple<T1, T2> obj)
            {
                return obj?.Item1.GetHashCode() ?? 0 ^ obj?.Item2.GetHashCode() ?? 0;
            }
        }
    }
}