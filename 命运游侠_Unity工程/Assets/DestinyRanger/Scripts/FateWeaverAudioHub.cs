using UnityEngine;

namespace DestinyRanger
{
    public sealed class FateWeaverAudioHub : MonoBehaviour
    {
        public AudioEventCatalog catalog;
        public AudioSource sfxSource;
        public AudioSource uiSource;
        public AudioSource ambientSource;

        public void PlayButtonClick()
        {
            PlayOneShot(uiSource, catalog ? catalog.buttonClick : null);
        }

        public void PlaySlotStop()
        {
            PlayOneShot(sfxSource, catalog ? catalog.slotStopClick : null);
        }

        public void PlayPerfectLine()
        {
            PlayOneShot(sfxSource, catalog ? catalog.slotPerfectLine : null);
        }

        public void PlayAmbient(AudioClip clip)
        {
            if (!ambientSource || !clip)
                return;

            ambientSource.clip = clip;
            ambientSource.loop = true;
            ambientSource.Play();
        }

        private static void PlayOneShot(AudioSource source, AudioClip clip)
        {
            if (source && clip)
                source.PlayOneShot(clip);
        }
    }
}
