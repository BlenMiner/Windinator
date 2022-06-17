using UnityEngine;

namespace Riten.Windinator.Audio
{
    [CreateAssetMenu(menuName = "Windinator/Sound Library")]
    public class SoundLibrary : ScriptableObject
    {
        public AudioClip[] Clips;

        [Min(0)]
        public float PitchVariation = 0f;

        [Min(0)]
        public float VolumeVariation = 0f;

        public int Priority = 0;

        public void PlayRandom()
        {
            if (Clips != null && Clips.Length > 0)
            {
                int i = Random.Range(0, Clips.Length);

                float volMin = 1f - VolumeVariation;
                float volMax = 1f + VolumeVariation;

                float pitchMin = 1f - PitchVariation;
                float pitchMax = 1f + PitchVariation;

                ClipPlayer.Play2D(Clips[i],
                    Random.Range(volMin, volMax),
                    Random.Range(pitchMin, pitchMax),
                    Priority
                );
            }
        }
    }
}