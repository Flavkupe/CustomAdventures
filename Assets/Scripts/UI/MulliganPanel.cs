using UnityEngine;

public class MulliganPanel : MonoBehaviour {

    // Use this for initialization
    private void Start () {
		
	}

    // Update is called once per frame
    private void Update () {
		
	}
    
    public void ActivateUIEvent(UIEvent uiEvent)
    {
        Game.UI.SetCurrentUIEvent(uiEvent);
    }    
}
