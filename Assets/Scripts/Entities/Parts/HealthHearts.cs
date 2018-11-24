using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHearts : MonoBehaviour {

    public GameObject HeartSprite;

    public int Columns = 5;
    public Vector3 HeartScale = new Vector3(1.0f, 1.0f, 1.0f);
    public float HeartDimX = 0.11f;
    public float HeartDimY = 0.09f;

    private List<GameObject> _heartObjects = new List<GameObject>();

    public void UpdateHearts(int health)
    {
        foreach (var heartObj in _heartObjects)
        {
            Destroy(heartObj);
        }

        _heartObjects.Clear();
        var x = 0.0f;
        var y = 0.0f;
        for (var i = 1; i <= health; ++i)
        {
            var heart = Instantiate(HeartSprite);
            heart.transform.SetParent(transform);
            heart.transform.localScale = HeartScale;
            heart.transform.localPosition = new Vector3(x, y, 1.0f);
            x += HeartDimX;
            if (i % Columns == 0)
            {
                x = 0.0f;
                y -= HeartDimY;
            }

            _heartObjects.Add(heart);
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
