using System.Collections;
using System.Collections.Generic;
using Riten.Windinator.Audio;
using Riten.Windinator.Material;
using UnityEngine;
using UnityEngine.EventSystems;

public class MaterialKeyButton : MonoBehaviour
{
    [SerializeField] SoundLibrary m_keyPressetSound;

    [SerializeField] SoundLibrary m_keyCanceledSound;

    [SerializeField] MaterialButton m_button;

    [SerializeField] KeyCode m_key;

    public KeyCode KeyCode
    {
        get => m_key;
        set
        {
            UpdateKey(value);
        }
    }

    public MaterialButton Button => m_button;

    public bool EditingKey {get; private set;} = false;

    KeyCode[] allKeys;

    EventSystem eventHandler;

    void Awake()
    {
        allKeys = System.Enum.GetValues(typeof(KeyCode)) as KeyCode[];
    }

    void OnEnable()
    {
        Button.SetText(m_key.ToString());
        m_button.onClick.AddListener(ButtonClicked);
    }

    void OnDisable()
    {
        m_button.onClick.RemoveListener(ButtonClicked);
    }
    
    void OnValidate()
    {
        Button.SetText(m_key.ToString());
    }

    void ButtonClicked()
    {
        EditingKey = true;
        eventHandler = EventSystem.current;
        eventHandler.enabled = false;
        Button.SetText("...");
    }

    void Update()
    {
        if (!EditingKey) return;

        if (Input.GetKey(KeyCode.Escape))
        {
            UpdateKey(m_key);
            m_keyCanceledSound?.PlayRandom();
            return;
        }

        foreach(var k in allKeys)
        {
            if (Input.GetKey(k))
            {
                UpdateKey(k);
                m_keyPressetSound?.PlayRandom();
                break;
            }
        }
    }

    void UpdateKey(KeyCode key)
    {
        m_key = key;
        Button.SetText(m_key.ToString());
        if (eventHandler != null)
            eventHandler.enabled = true;
        EditingKey = false;
    }
}
