using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIEventButton : MonoBehaviour
{
    public UIEvent ButtonUIEvent;

    public void OnClick()
    {
        Game.UI.SetCurrentUIEvent(this.ButtonUIEvent);
    }
}
