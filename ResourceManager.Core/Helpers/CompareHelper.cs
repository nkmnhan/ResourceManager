using System.Collections.Generic;
using System.Linq;

namespace ResourceManager.Core.Helpers
{
    public static class CompareHelper
    {
        public static (IEnumerable<T> NotInA, IEnumerable<T> NotInB) GetDifference<T>(IEnumerable<T> collectionA, IEnumerable<T> collectionB)
        {
            return (collectionB.Except(collectionA), collectionA.Except(collectionB));
        }
    }
}
