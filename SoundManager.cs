using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    [SerializeField] private AudioMixer mixer;

    private const string SFX_GROUP = "SFX";
    private const string MUSIC_GROUP = "Music";

    private AudioSource musicSource;
    private AudioSource musicSourceCrossfade;
    private bool isUsingSource1 = true;

    private float crossfadeTime = 1f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        // DontDestroyOnLoad(gameObject);  // Uncomment if multi-scene

        // Set up music sources
        musicSource = gameObject.AddComponent<AudioSource>();
        SetupAudioSource(musicSource, MUSIC_GROUP, true);

        musicSourceCrossfade = gameObject.AddComponent<AudioSource>();
        SetupAudioSource(musicSourceCrossfade, MUSIC_GROUP, true);
        musicSourceCrossfade.volume = 0f;
    }

    // Helper to configure an AudioSource
    private void SetupAudioSource(AudioSource source, string groupPath, bool loop)
    {
        source.outputAudioMixerGroup = mixer.FindMatchingGroups(groupPath)[0];
        source.loop = loop;
        source.playOnAwake = false;
    }
    
    public void PlaySFX(AudioClip clip, Vector3 position = default, float volume = 1f, float pitchVariation = 0f)
    {
        if (clip == null)
            return;

        GameObject tempGO = new GameObject("TempSFX");
        tempGO.transform.position = position;

        AudioSource source = tempGO.AddComponent<AudioSource>();
        SetupAudioSource(source, SFX_GROUP, false);

        source.clip = clip;
        source.volume = volume;
        source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        source.spatialBlend = (position != default) ? 1f : 0f;

        source.Play();
        Destroy(tempGO, clip.length + 0.1f);
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource activeSource = isUsingSource1 ? musicSource : musicSourceCrossfade;
        AudioSource inactiveSource = isUsingSource1 ? musicSourceCrossfade : musicSource;

        //Set up new track on inactive source
        inactiveSource.clip = clip;
        inactiveSource.volume = 0f;
        inactiveSource.Play();

        //Crossfade
        StartCoroutine(Crossfade(activeSource, inactiveSource, volume));

        isUsingSource1 = !isUsingSource1;
    }

    private IEnumerator Crossfade(AudioSource fadeOut, AudioSource fadeIn, float targetVolume)
    {
        float time = 0f;
        float startOut = fadeOut.volume;
        float startIn = fadeIn.volume;

        while (time < crossfadeTime)
        {
            time += Time.deltaTime;
            fadeOut.volume = Mathf.Lerp(startOut, 0f, time / crossfadeTime);
            fadeIn.volume = Mathf.Lerp(startIn, targetVolume, time / crossfadeTime);
            yield return null;
        }

        fadeOut.Stop();
        fadeOut.volume = 0f;
        fadeIn.volume = targetVolume;
    }

    //Stop music
    public void StopMusic()
    {
        musicSource.Stop();
        musicSourceCrossfade.Stop();
    }
}