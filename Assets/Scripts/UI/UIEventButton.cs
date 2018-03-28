using UnityEngine;

public class UIEventButton : MonoBehaviour
{
    public UIEvent ButtonUIEvent;

    public void OnClick()
    {
        Game.UI.SetCurrentUIEvent(ButtonUIEvent);
    }
}
