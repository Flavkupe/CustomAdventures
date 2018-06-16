
using System;
using System.Collections.Generic;
using UnityEngine;

public class FloatyTextOptions
{
    public string Text;
    public Color? Color;
    public FloatyTextSize? Size;
    public bool OnlyShowOnEmptyQueue;
}

public class FloatyTextGenerator : MonoBehaviour
{
    private readonly Queue<FloatyText> _floatyTextQueue = new Queue<FloatyText>();
    private bool _floatyTextShowing = false;

    public void ShowFloatyText(FloatyTextOptions options)
    {
        if (options.OnlyShowOnEmptyQueue && (_floatyTextShowing || _floatyTextQueue.Count > 0))
        {
            // With option OnlyShowOnEmptyQueue, only show if no other text
            return;
        }

        var floatyText = Instantiate(TextManager.Instance.DamageTextTemplate);
        floatyText.Init(transform.position, options.Text, 0.5f, 1.0f, !_floatyTextShowing);
        floatyText.TextFinished += HandleTextFinished;
        if (_floatyTextShowing)
        {
            _floatyTextQueue.Enqueue(floatyText);
        }
        else
        {
            _floatyTextShowing = true;
        }

        if (options.Color != null)
        {
            floatyText.SetColor(options.Color.Value);
        }

        if (options.Size != null)
        {
            floatyText.SetSize(options.Size.Value);
        }
    }

    public void ShowFloatyText(string text, Color? color = null, FloatyTextSize? size = null)
    {
        ShowFloatyText(new FloatyTextOptions()
        {
            Text = text,
            Color = color,
            Size = size,
        });
    }

    private void HandleTextFinished(object sender, EventArgs e)
    {
        if (_floatyTextQueue.Count == 0)
        {
            _floatyTextShowing = false;
        }
        else
        {
            var text = _floatyTextQueue.Dequeue();
            _floatyTextShowing = true;
            text.TextFinished += HandleTextFinished;
            text.transform.position = this.transform.position;
            text.Activate();
        }
    }

}

