# Windinator.cs

## Properties

| Declaration | Description |
| - | - |
| static readonly int **WindowCount**  | Gets the number of pushed windows. |
| static readonly bool **IsEmpty**     | Is the window stack empty. Same thing as checking if **WindowCount** is 0. |
| static readonly Canvas **Canvas**    | Get root canvas, this canvas holds all other windows. If no window was ever pushed, this value can be null |
| static readonly bool **ShoudBlockGameFlow**      | Use this to block input for your player controllers when true. |

## Stack Management Functions

| Declaration | Description |
| - | - |
| static **Push**<*T*>(AnimationDelegade?)    | Pushes a window of type T to the stack. This will instantiate or pool the window and animate it if specified.|
| static **PushPrefab**(WindinatorBehaviour, AnimationDelegade?) | Same as Push, except it accepts a reference to a WindinatorBehaviour prefab (this reference has to be from your projects folder, not from your scene). |
| static **Pop**(bool? *force*, AnimationDelegade?) | This will Pop (remove) the top most window, which is usually the active window. If the window can't be closed it won't do anything unless *force* is true. |
| static **MoveToTop**(WindinatorBehaviour) | This will move the window reference to the top of the stack. This affects rendering order and input priotity. |
| static **Replace**<*T*>(WindinatorBehaviour) | Same as Push but it will automatically remove the top most window. |

## Manual Animation Management

| Declaration | Description |
| - | - |
| static **Animate**(WindinatorBehaviour, AnimationDelegade, Action? *onDone*) | Plays animation for window instance based on its [config](windows.md#window-settings) settings. Once animation finishes, *onDone* callback will be invoked. |
| static **ClearAnimations**(WindinatorBehaviour) | If there are any animations being played for the specified window, stop them and don't invoke *onDone* callback. |

## Helpful Functions

| Declaration | Description |
| - | - |
| static bool **Warmup**() | Warmup the Windinator system. This will create the root Canvas, create all the window pools and get the reference to the WindinatorConfig resource file.  |
| static **RunNextFrame**(Action?) | Simply queue an Action that will be executed one time next frame and then discarded. This will warmup the system if it wasn't already. |
| static Color **GetColor**(Colors) | Convert Material Color to Unity3D Color, a preffered way of doing it would be: Colors.Background.ToColor() which has the same effect. |
| static GameObject **GetElementPrefab**<*T*>() | Accepts a LayoutBaker type and returns its corresponding Prefab. Use it to Instantiate other copies. |
| static **SetupCanvas**(Canvas, CanvasScaler) | Configure the canvas and canvas scaler based on the WindinatorConfig resource file settings. |
| static **UpdateVisibility**() | Force Windindator to recheck which windows are visible or not. A window is not visible if another window that culls background windows is in front of it. |
