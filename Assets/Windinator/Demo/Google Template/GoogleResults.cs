using UnityEngine;
using Riten.Windinator;

public class GoogleResults : WindinatorBehaviour
{
    public void Setup(string searchStr)
    {
        SetOpenCloseAnimation(
            WindinatorAnimations.SlideFromRight,
            WindinatorAnimations.SlideToLeft
        );
    }
}
