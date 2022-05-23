using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Riten.Windinator.Material
{
    public class ContextMenuItemPreset : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] TMPro.TMP_Text m_label;

        [SerializeField] ContextMenuButton m_button;

        [SerializeField] Image m_arrow;

        [SerializeField] RectTransform m_childList;

        [SerializeField] ContextMenuListPreset m_childListComponent;

        ContextMenuDialog m_controller;

        ContextMenuListPreset m_parent;

        bool m_hasSubmenu = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_parent.UpdateSelected(this);
        }

        public void Setup(ContextMenuListPreset parent, ContextMenuDialog window, ContextMenuItem item)
        {
            m_parent = parent;
            m_parent.onSelectedChanged += OnSelectedChanged;

            m_controller = window;
            m_hasSubmenu = item.Children.Length > 0;

            m_childList.gameObject.SetActive(false);

            m_label.text = item.Value;
            m_button.IsSelected = false;
            m_button.onClick.RemoveAllListeners();

            if (!m_hasSubmenu)
            {
                m_button.onClick.AddListener(() =>
                {
                    item.Action?.Invoke();
                    window.ForcePopWindow();
                });
            }
            else
            {
                // Spawn other submenus
                m_controller.SetupChildren(m_childListComponent, m_childList, item.Children);
            }

            m_arrow.enabled = m_hasSubmenu;
        }

        private void OnEnable()
        {
            if (m_parent != null)
                m_parent.onSelectedChanged += OnSelectedChanged;
        }

        private void OnDisable()
        {
            m_parent.onSelectedChanged -= OnSelectedChanged;
        }

        private void OnSelectedChanged(ContextMenuItemPreset item)
        {
            if (m_hasSubmenu)
            {
                var go = m_childList.gameObject;
                bool enable = item == this;

                if (go.activeSelf != enable)
                    go.SetActive(enable);

                m_button.IsSelected = enable;
            }
            else m_button.IsSelected = false;
        }
    }
}