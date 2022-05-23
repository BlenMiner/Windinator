using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Riten.Windinator.Material
{
    public class ContextMenuListPreset : MonoBehaviour
    {
        ContextMenuItemPreset m_selected = null;

        public event Action<ContextMenuItemPreset> onSelectedChanged;

        public void UpdateSelected(ContextMenuItemPreset select)
        {
            if (m_selected != select)
            {
                m_selected = select;
                onSelectedChanged?.Invoke(select);
            }
        }

        private void OnDisable()
        {
            UpdateSelected(null);
        }
    }
}
