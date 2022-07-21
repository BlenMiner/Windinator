using Riten.Windinator;
using Riten.Windinator.Material;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenuExample : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject m_loginForm;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var menu = Windinator.Push<ContextMenuDialog>();

            menu.Setup(
                new ContextMenuItem("Test", () => 
                {
                    Windinator.Push<GenericModalDialog>().Setup(message: "Hello World", title: "Title stuff");
                }),
                new ContextMenuItem("Test 1",
                    new ContextMenuItem("Test Child Of 1", () => Debug.Log("Child 1")),
                    new ContextMenuItem("Test Child Of 2", () => Debug.Log("Child 2")),
                    new ContextMenuItem("Test Child Of 3",
                        new ContextMenuItem("I'm here!", () => Debug.Log("Child 3, even further"))
                    )
                ),
                new ContextMenuItem("Maybe a login window?", () => 
                {
                    Windinator.Push<ModalDialog>().Setup(m_loginForm);
                }), 
                new ContextMenuItem("Test 3", () => 
                {
                    Windinator.Push<GenericModalDialog>().Setup(action1: "Ok", message: "Hello World but this time around with an action!", title: "Cool stuff");
                }),
                new ContextMenuItem("Test 4", () =>
                {
                    Windinator.Push<GenericModalDialog>().Setup(action1: "Ok", action2: "SECOND ACTION ?!!", message: "Hello World but this time around with an action!\nANOTHER ONE??", title: "OMG");
                })
            );
        }
    }
}
