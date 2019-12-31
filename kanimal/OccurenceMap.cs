﻿using System.Collections.Generic;

namespace kanimal
{
    using AnimHashTable = Dictionary<int, string>;

    // Spriter's object names are numbered sequentially from 0.
    // This is arguably useful if
    // multiple of the same sprite is included in a single animation frame
    public class OccurenceMap : Dictionary<SpriteName, int>
    {
        public void Update(KAnim.Element element, AnimHashTable animHashes)
        {
            var name = element.FindName(animHashes);
            if (!ContainsKey(name))
                this[name] = 0;
            else
                this[name] += 1;
        }

        public SpriterObjectName FindOccurenceName(KAnim.Element element, AnimHashTable animHashes)
        {
            var name = element.FindName(animHashes);
            return name.ToSpriterObjectName(this[name]);
        }
    }
}