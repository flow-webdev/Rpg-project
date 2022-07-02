using System.Collections.Generic;

namespace RPG.Stats {

    public interface IModifierProvider {

        // Enumerable permit us to do a foreach loop, IEnumerator does not
        IEnumerable<float> GetAdditiveModifiers(Stat stat);
        IEnumerable<float> GetPercentageModifiers(Stat stat);
    }

}

