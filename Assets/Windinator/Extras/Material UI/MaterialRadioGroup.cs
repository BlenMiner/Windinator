using System.Collections.Generic;
using UnityEngine;

public class MaterialRadioGroup : MonoBehaviour
{
    List<MaterialRadio> m_radios = new List<MaterialRadio>();

    int m_selectedId = 0;

    public int SelectedID => m_selectedId;

    void Init()
    {
        m_radios.Clear();

        GetComponentsInChildren<MaterialRadio>(m_radios);

        int count = m_radios.Count;
        bool oneSelected = false;

        for (int i = 0; i < count; ++i)
        {
            var r = m_radios[i];

            r.onValueChangedRef.RemoveListener(ValueChanged);

            if (r.Value)
            {
                if (oneSelected)
                {
                    r.Value = false;
                }
                else
                {
                    m_selectedId = i;
                    oneSelected = true;
                }
            }
            else if (i == count - 1 && !oneSelected)
            {
                m_radios[0].Value = true;
            }

            r.onValueChangedRef.AddListener(ValueChanged);
        }
    }

    void ValueChanged(MaterialRadio radio, bool value)
    {
        if (!value) return;

        int index = m_radios.IndexOf(radio);

        if (index < 0) Init();
        else
        {
            m_selectedId = index;

            int count = m_radios.Count;

            for (int i = 0; i < count; ++i)
            {
                if (m_selectedId == i) continue;
                var r = m_radios[i];
                r.Value = false;
            }
        }
    }

    void Awake()
    {
        Init();
    }

    void OnTransformChildrenChanged()
    {
        Init();
    }
}
