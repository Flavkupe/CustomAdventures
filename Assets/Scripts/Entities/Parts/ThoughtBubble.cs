using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

