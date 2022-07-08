# WindinatorBehaviour.cs

WindinatorBehaviour is the same as a simple MonoBehaviour except that it contains some extra code to work with the Windinator system.

## Properties

| Declaration | Description |
| - | - |
| static readonly int **AnimationActors** | Number of animations being played on this window. |
| static readonly bool **IsAnimating** | Is this window being animated? |
| static readonly bool **AnimatedByDefault** | Is this window being animated by default? This reflects the [configuration settings](windows.md#window-settings). |
| static readonly float **AnimationDuration** | This reflects the [configuration settings](windows.md#window-settings). |
| static readonly float **CullBackgroundWindows** | This reflects the [configuration settings](windows.md#window-settings). |
| static readonly AnimationDelegade **FadeIn** | Access the fade in animation delegate. |
| static readonly GameObject **GeneratedBackground** | If automatic window background was selected, this will have a reference to the created GameObject. |
| static readonly CanvasGroup **GeneratedBackgroundGroup** | If background created, this returns the canvas group on the background. |
| static readonly bool **ShoudBlockGameFlow** | Is this window blocking game flow? |

## Functions

| Declaration | Description |
| - | - |
| public void SetCanExit(bool *canExit*) | If *canExit* is false, the window can only be closed manually with 'ForcePopWindow'. Otherwise it can be closed by multiple things like the escape key, pressing on the background, etc |