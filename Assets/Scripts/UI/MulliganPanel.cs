using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MulliganPanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void ActivateUIEvent(UIEvent uiEvent)
    {
        Game.UI.SetCurrentUIEvent(uiEvent);
    }    
}
