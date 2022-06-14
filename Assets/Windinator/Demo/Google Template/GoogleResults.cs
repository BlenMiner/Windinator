using UnityEngine;
using Riten.Windinator;

public class GoogleResults : WindinatorBehaviour
{
    [SerializeField] GoogleResultsContent m_content;

    private void Awake()
    {
        SetOpenCloseAnimation(
            WindinatorAnimations.SlideFromRight,
            WindinatorAnimations.SlideToLeft
        );
    }

    public void Setup(string searchStr)
    {
        m_content.Setup(searchStr);
    }
}
