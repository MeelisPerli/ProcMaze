using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
    [CreateAssetMenu(menuName = "AudioClipGroup")]
    public class AudioClipGroup : ScriptableObject {

        [Range(0, 2)]
        public float volumeMin;
        [Range(0, 2)]
        public float volumeMax;
        [Range(0, 2)]
        public float pitchMin;
        [Range(0, 2)]
        public float pitchMax;
        [Range(0, 2)]
        public float cooldown;

        public List<AudioClip> clips;

        private float timestamp;
        private AudioSourcePool pool;

        private void OnEnable() {
            timestamp = Time.time;
            pool = FindObjectOfType<AudioSourcePool>();
        }
        
        

        public void play(AudioSource source) {
            if (Time.time - timestamp > cooldown) {
                source.clip = clips[Random.Range(0, clips.Count)];
                source.volume = Random.Range(volumeMin, volumeMax);
                source.pitch = Random.Range(pitchMin, pitchMax);
                source.Play();
                timestamp = Time.time;
            }
            
        }

        public void play() {
            if (pool == null) {
                pool = FindObjectOfType<AudioSourcePool>();
            }
            play(pool.getSource());
        }

    }
}
