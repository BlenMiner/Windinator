using Riten.Windinator;
using Riten.Windinator.Material;
using UnityEngine;

public class SimpleMainMenu : WindinatorBehaviour
{
    // This function is called via UI button event
    public void RaiseSimpleDialog()
    {
        // Push the dialog window to the scene
        var dialog = Windinator.Push<GenericModalDialog>();

        // Set it up with the correct values & you are done!
        dialog.Setup("Hello World", "This is an example of how to pass in information & stuff :)", action1: "ok", action1evt: () => {
            Debug.Log("User pressed the OK button");
        });
    }

    // Used to show scene transition doesn't break the system
    public void ReloadScene()
    {
        /*UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );*/

        var snack = Windinator.Push<SnackBar>();
        snack.Setup("Hah! Tricked you.", "Dismiss");
    }
}
