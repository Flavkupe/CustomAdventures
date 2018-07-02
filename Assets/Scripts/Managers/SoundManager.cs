using JetBrains.Annotations;
using UnityEngine;

public class SoundManager : SingletonObject<SoundManager>
{
    public AudioClip[] DefaultItemPickupSounds;
    public AudioClip[] DefaultItemDropSounds;

    [UsedImplicitly]
    private void Awake()
    {
        Instance = this;
    }
    public void PlayFromClips(AudioClip[] clips, AudioClip defaultClip = null)
    {
        Game.Player.PlaySounds(clips, defaultClip);
    }

    public void PlayClip(AudioClip clip)
    {
        Game.Player.PlayClip(clip);
    }
}
