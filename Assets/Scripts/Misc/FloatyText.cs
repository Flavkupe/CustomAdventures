using JetBrains.Annotations;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class FloatyText : MonoBehaviour
{
    private float _speed;
    public void Init(Vector3 startingPlace, string text, float duration = 0.5f, float speed = 1.0f)
    {
        _speed = speed;
        Destroy(gameObject, duration);
        transform.position = startingPlace;
        SetText(text);
    }

    public void SetText(string text)
    {
        GetComponent<TextMeshPro>().text = text;
    }


    [UsedImplicitly]

    private void Update () {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y + (_speed * Time.deltaTime));
    }
}
