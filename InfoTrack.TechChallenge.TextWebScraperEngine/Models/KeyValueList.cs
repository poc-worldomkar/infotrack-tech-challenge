﻿using System.Collections.Generic;

namespace InfoTrack.TechChallenge.WebScraperEngine.Models
{
    public class KeyValueList<TKey, TValue> : List<KeyValuePair<TKey, TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }
    }
}
