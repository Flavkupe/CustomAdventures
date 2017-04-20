using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class FloatyText : MonoBehaviour
{
    private float speed;
    public void Init(Vector3 startingPlace, string text, float duration = 0.5f, float speed = 1.0f)
    {
        this.speed = speed;
        Destroy(this.gameObject, duration);
        this.transform.position = startingPlace;
        this.SetText(text);
    }

    public void SetText(string text)
    {
        this.GetComponent<TextMeshPro>().text = text;
    }
         
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = this.transform.position;
        this.transform.position = new Vector3(pos.x, pos.y + (speed * Time.deltaTime));
    }
}
