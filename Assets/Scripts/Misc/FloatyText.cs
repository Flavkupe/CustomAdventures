using JetBrains.Annotations;
using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class FloatyText : MonoBehaviour
{
    public event EventHandler TextFinished;

    private TextMeshPro _tmp;
    private float _speed;
    private float _duration;
    public void Init(Vector3 startingPlace, string text, float duration = 0.5f, float speed = 1.0f, bool startActive = true)
    {
        _tmp = GetComponent<TextMeshPro>();
        _speed = speed;
        _duration = duration;
        transform.position = startingPlace;
        SetText(text);

        if (startActive)
        {
            Activate();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        Invoke("Finished", _duration);
    }

    private void Finished()
    {
        if (TextFinished != null)
        {
            TextFinished(this, new EventArgs());
        }

        Destroy(gameObject);
    }

    public void SetText(string text)
    {
        _tmp.text = text;
    }

    public void SetColor(Color color)
    {
        _tmp.color = color;
    }

    public void SetSize(float size)
    {
        _tmp.fontSize = size;
    }

    [UsedImplicitly]
    private void Update () {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y + (_speed * Time.deltaTime));
    }
}
