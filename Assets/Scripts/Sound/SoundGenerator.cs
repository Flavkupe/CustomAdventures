
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class SoundGenerator : MonoBehaviour
{
    public void PlayRandomFrom(IEnumerable<AudioClip> items, AudioClip defaultClip = null)
    {
        var list = items.ToList();
        if (list.Count > 0)
        {
            var clip = list.GetRandom();
            GetComponent<AudioSource>().PlayOneShot(clip);
        }
        else if (defaultClip != null)
        {
            GetComponent<AudioSource>().PlayOneShot(defaultClip);
        }
    }

    public void PlayClip(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
    }
}

