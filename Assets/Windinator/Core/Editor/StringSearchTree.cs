using System;
using System.Collections.Generic;
using UnityEngine;

namespace WindinatorEditorUtils
{
    public struct ValueName
    {
        public string Name;
        public object Value;
    }

    public class StringSearchTree
    {
        List<ValueName> cache = new List<ValueName>();

        List<ValueName> m_names = new List<ValueName>();

        public StringSearchTree(Type @enum)
        {
            var values = Enum.GetValues(@enum);
            var names = Enum.GetNames(@enum);

            for (int i = 0; i < names.Length; i++)
            {
                m_names.Add(new ValueName
                {
                    Name = names[i],
                    Value = values.GetValue(i)
                });
            }
        }

        public List<ValueName> GetPossibleResults(string query)
        {
            query = query.ToLower();
            cache.Clear();

            int c = m_names.Count;
            int ql = query.Length;

            m_names.Sort((a, b) =>
            {
                int ad = Mathf.Abs(a.Name.Length - ql);
                int bd = Mathf.Abs(b.Name.Length - ql);

                if (ad > bd) return 1;
                else if (ad < bd) return -1;
                return 0;
            });

            for (int i = 0; i < c; i++)
            {
                if (cache.Count >= 10) break;

                var value = m_names[i];
                var name = value.Name;

                if (name.Contains(query))
                    cache.Add(value);
            }

            return cache;
        }
    }
}