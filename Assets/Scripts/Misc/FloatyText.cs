using JetBrains.Annotations;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class FloatyText : MonoBehaviour
{
    private TextMeshPro _tmp;
    private float _speed;
    public void Init(Vector3 startingPlace, string text, float duration = 0.5f, float speed = 1.0f)
    {
        _tmp = GetComponent<TextMeshPro>();
        _speed = speed;
        Destroy(gameObject, duration);
        transform.position = startingPlace;
        SetText(text);
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
