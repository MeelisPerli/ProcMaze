using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
    public class AudioSourcePool : MonoBehaviour {

        private List<AudioSource> audios;

        private void Awake() {
            audios = new List<AudioSource>();
        }

        public AudioSource getSource() {
            foreach (var source in audios) {
                if (!source.isPlaying) {
                    return source;
                }
            }

            var audioSource = FindObjectOfType<AudioSourcePool>().gameObject.AddComponent<AudioSource>();
            audios.Add(audioSource);
            return audioSource;
        }

    }
}
