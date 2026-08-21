using System.Runtime.CompilerServices;

namespace loremiscExpansion.CWTs
{
    public static class SpearCWT
    {

        public static readonly ConditionalWeakTable<Spear, DataClass> spearCWT = new();
        public static bool TryGetData(Spear key, out DataClass data)
        {
            if (key != null)
            {
                data = spearCWT.GetOrCreateValue(key);
            }
            else data = null;

            return data != null;
        }
        public class DataClass
        {
            public bool thrownByProtag = false;
        }
    }
}
