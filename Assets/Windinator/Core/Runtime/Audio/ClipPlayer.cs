using Riten.Windinator.Audio.Internal;
using UnityEngine;

namespace Riten.Windinator.Audio
{
    public static class ClipPlayer
    {
        static GameObject Prefab;

        static GameObjectPool<AudioSourceHelper> AudioSources;

        static void InitPrefab()
        {
            Prefab = new GameObject("[Windinator] Audio Source", typeof(AudioSource), typeof(AudioSourceHelper));
            Prefab.hideFlags = HideFlags.HideInHierarchy;
            GameObject.DontDestroyOnLoad(Prefab);

            AudioSources?.DestroyAll();
            AudioSources = new GameObjectPool<AudioSourceHelper>(Prefab);
        }

        public static AudioSourceHelper Allocate()
        {
            if (Prefab == null) InitPrefab();

            var instance = AudioSources.Allocate(null);
            instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
            return instance;
        }

        public static void Free(AudioSourceHelper source)
        {
            AudioSources.Free(source);
        }

        public static AudioSource Play2D(AudioClip audio, float volume = 1f, float pitch = 1f, int priority = 0)
        {
            var sourceHelper = Allocate();
            var source = sourceHelper.Source;

            source.clip = audio;
            source.volume = volume;
            source.priority = priority;
            source.pitch = pitch;
            source.spatialBlend = 0f;

            source.Play();
            sourceHelper.OnClipFinished(() => {
                Free(sourceHelper);
            });

            return source;
        }
    }
}
