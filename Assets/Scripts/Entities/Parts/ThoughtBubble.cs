using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ThoughtBubble : MonoBehaviourEx
{
    [TemplatePrefab]
    public static ThoughtBubble Template;

    public TextMeshPro Text;

    private void Awake()
    {
        Text = GetComponentInChildren<TextMeshPro>();
    }

    private void Update()
    {

    }

    public void SetText(string text)
    {
        Text.text = text;
    }
}

