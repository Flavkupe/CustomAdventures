using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileEntity : MonoBehaviour, IObjectOnTile
{
    public int XCoord { get; set; }
    public int YCoord { get; set; }

    public void ShowFloatyText(string text)
    {
        FloatyText damageText = Instantiate(TextManager.Instance.DamageTextTemplate);
        damageText.Init(this.transform.position, text);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    protected virtual void OnClicked()
    {
    }

    private void OnMouseDown()
    {
        this.OnClicked();
    }
}
