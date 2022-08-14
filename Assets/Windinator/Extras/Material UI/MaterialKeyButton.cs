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

#if ENABLE_LEGACY_INPUT_MANAGER
    [SerializeField] KeyCode m_key;

    public KeyCode KeyCode
    {
        get => m_key;
        set
        {
            UpdateKey(value);
        }
    }
#else
    [SerializeField] UnityEngine.InputSystem.Key m_key;

    public UnityEngine.InputSystem.Key KeyCode
    {
        get => m_key;
        set
        {
            UpdateKey(value);
        }
    }
#endif

    public MaterialButton Button => m_button;

    public bool EditingKey {get; private set;} = false;

#if ENABLE_LEGACY_INPUT_MANAGER
    KeyCode[] allKeys;
#endif

    EventSystem eventHandler;

    void Awake()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        allKeys = System.Enum.GetValues(typeof(KeyCode)) as KeyCode[];
#endif
    }

    void OnEnable()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        Button.SetText(m_key.ToString());
#endif
        m_button.onClick.AddListener(ButtonClicked);
    }

    void OnDisable()
    {
        m_button.onClick.RemoveListener(ButtonClicked);
    }
    
    void OnValidate()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        Button.SetText(m_key.ToString());
#endif
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

        bool escapePressing = false;

#if ENABLE_LEGACY_INPUT_MANAGER
        escapePressing = Input.GetKey(KeyCode.Escape);
#elif ENABLE_INPUT_SYSTEM
        var currentKeyboard = UnityEngine.InputSystem.Keyboard.current;
        if (currentKeyboard != null)
            escapePressing = currentKeyboard.escapeKey.isPressed;
#endif

        if (escapePressing)
        {
            UpdateKey(m_key);
            m_keyCanceledSound?.PlayRandom();
            return;
        }

#if ENABLE_LEGACY_INPUT_MANAGER
        foreach(var k in allKeys)
        {
            if (Input.GetKey(k))
            {
                UpdateKey(k);
                m_keyPressetSound?.PlayRandom();
                break;
            }
        }

#else
        var keyboard = UnityEngine.InputSystem.Keyboard.current;

        if (keyboard != null)
        {
            foreach (var k in keyboard.allKeys)
            {
                if (k.wasPressedThisFrame)
                {
                    UpdateKey(k.keyCode);
                    m_keyPressetSound?.PlayRandom();
                    break;
                }
            }
        }
#endif
    }

#if ENABLE_LEGACY_INPUT_MANAGER
    void UpdateKey(KeyCode key)
    {
        m_key = key;
        Button.SetText(m_key.ToString());
        if (eventHandler != null)
            eventHandler.enabled = true;
        EditingKey = false;
    }
#else
    void UpdateKey(UnityEngine.InputSystem.Key key)
    {
        m_key = key;
        Button.SetText(m_key.ToString());
        if (eventHandler != null)
            eventHandler.enabled = true;
        EditingKey = false;
    }
#endif
}
