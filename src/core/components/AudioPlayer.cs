using Microsoft.Xna.Framework.Audio;

namespace Flatlanders.Core.Components;

public class AudioPlayer(Entity entity) : Component(entity)
{
    private bool _loop;

    private float _volume = 1f;
    private float _pitch;
    private float _pan;

    public SoundEffect PlayingAudio { get; private set; }

    public bool Loop
    {
        get => _loop;

        set
        {
            _loop = value;

            if (SoundEffectInstance != null)
            {
                SoundEffectInstance.IsLooped = value;
            }
        }
    }

    public float Volume
    {
        get => _volume;

        set
        {
            _volume = value;

            if (SoundEffectInstance != null)
            {
                SoundEffectInstance.Volume = value;
            }
        }
    }
    public float Pitch
    {
        get => _pitch;

        set
        {
            _pitch = value;

            if (SoundEffectInstance != null)
            {
                SoundEffectInstance.Pitch = value;
            }
        }
    }
    public float Pan
    {
        get => _pan;

        set
        {
            _pan = value;

            if (SoundEffectInstance != null)
            {
                SoundEffectInstance.Pan = value;
            }
        }
    }

    private SoundEffectInstance SoundEffectInstance { get; set; }

    public void Play(SoundEffect audio)
    {
        PlayingAudio = audio;

        SoundEffectInstance instance = audio.CreateInstance();
        
        instance.IsLooped = Loop;
        instance.Volume = Volume;
        instance.Pitch = Pitch;
        instance.Pan = Pan;
        
        instance.Play();
    }
}