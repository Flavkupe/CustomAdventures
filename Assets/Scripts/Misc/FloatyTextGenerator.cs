
using System;
using System.Collections.Generic;
using UnityEngine;

public class FloatyTextGenerator : MonoBehaviour
{
    private readonly Queue<FloatyText> _floatyTextQueue = new Queue<FloatyText>();
    private bool _floatyTextShowing = false;
    public void ShowFloatyText(string text, Color? color = null, float? size = null)
    {
        var floatyText = Instantiate(TextManager.Instance.DamageTextTemplate);
        floatyText.Init(transform.position, text, 0.5f, 1.0f, !_floatyTextShowing);
        floatyText.TextFinished += HandleTextFinished;
        if (_floatyTextShowing)
        {
            _floatyTextQueue.Enqueue(floatyText);
        }
        else
        {
            _floatyTextShowing = true;
        }

        if (color != null)
        {
            floatyText.SetColor(color.Value);
        }

        if (size != null)
        {
            floatyText.SetSize(size.Value);
        }
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

