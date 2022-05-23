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

        public static int Levenshtein(string source1, string source2)
        {
            var source1Length = source1.Length;
            var source2Length = source2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
                return source2Length;

            if (source2Length == 0)
                return source1Length;

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
            for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

            // Calculate rows and collumns distances
            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            // return result
            return matrix[source1Length, source2Length];
        }
    }
}