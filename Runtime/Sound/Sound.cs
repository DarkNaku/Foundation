using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;

namespace DarkNaku.Foundation
{
    public class Sound : SingletonScriptable<Sound>
    {
        private const string SFX_VOLUME = "SFX_VOLUME";
        private const string BGM_VOLUME = "BGM_VOLUME";

        [SerializeField] private List<AudioClip> _clips;

        public static float VolumeBGM
        {
            get
            {
                var volume = PlayerPrefs.GetFloat(BGM_VOLUME, 1f);
                return Mathf.Clamp01(volume);
            }
            set
            {
                PlayerPrefs.SetFloat(BGM_VOLUME, value);
                Instance.UpdateVolumeBGM(value);
            }
        }

        public static float VolumeSFX
        {
            get
            {
                var volume = PlayerPrefs.GetFloat(SFX_VOLUME, 1f);
                return Mathf.Clamp01(volume);
            }
            set
            {
                PlayerPrefs.SetFloat(SFX_VOLUME, value);
                Instance.UpdateVolumeSFX(value);
            }
        }

        private Transform _playerRoot;
        private bool _isRunningDefender;
        private AudioSource _bgmPlayer;
        private HashSet<AudioSource> _sfxPlayers = new();
        private Dictionary<string, AudioClip> _clipTable = new();
        private HashSet<string> _playedClipInThisFrame = new();

        public static void PlayBGM(string clipName)
        {
            Instance._PlayBGM(clipName);
        }

        public static void PlaySFX(string clipName)
        {
            Instance._PlaySFX(clipName);
        }

        public static void StopBGM()
        {
            Instance._StopBGM();
        }

        public static void StopSFX(string clipName)
        {
            Instance._StopSFX(clipName);
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Sound")]
        private static void SelectSound()
        {
            Selection.activeObject = Instance;
        }
#endif

        protected override void OnInstantiate()
        {
            _sfxPlayers.Clear();
            _clipTable.Clear();
            _playedClipInThisFrame.Clear();

            for (int i = 0; i < _clips.Count; i++)
            {
                _clipTable.Add(_clips[i].name, _clips[i]);
            }

            _playerRoot = new GameObject("[SOUND]").transform;
            _bgmPlayer = new GameObject("BGM Player").AddComponent<AudioSource>();
            _bgmPlayer.transform.parent = _playerRoot;

            DontDestroyOnLoad(_playerRoot.gameObject);

            Observable.EveryEndOfFrame().Subscribe(OnEndOfFrame);
        }

        private void _PlaySFX(string clipName)
        {
            if (Mathf.Approximately(VolumeSFX, 0f)) return;
            if (_playedClipInThisFrame.Contains(clipName)) return;

            AudioSource player = GetPlayer();

            var clip = GetClip(clipName);

            if (clip == null) return;

            player.clip = clip;
            player.loop = false;
            player.volume = VolumeSFX;
            player.Play();

            _playedClipInThisFrame.Add(clipName);
        }

        private void _StopSFX(string clipName)
        {
            foreach (var player in _sfxPlayers)
            {
                if (player.isPlaying && player.clip.name.Equals(clipName))
                {
                    player.Stop();
                }
            }
        }

        private void _PlayBGM(string clipName)
        {
            if (Mathf.Approximately(VolumeBGM, 0f)) return;

            var clip = GetClip(clipName);

            if (clip == null) return;

            _bgmPlayer.clip = clip;
            _bgmPlayer.loop = true;
            _bgmPlayer.volume = VolumeBGM;
            _bgmPlayer.Play();
        }

        private void _StopBGM()
        {
            _bgmPlayer.Stop();
        }

        private AudioSource GetPlayer()
        {
            AudioSource player = null;

            foreach (var source in _sfxPlayers)
            {
                if (source.isPlaying) continue;
                player = source;
                break;
            }

            if (player == null)
            {
                player = new GameObject("SFX Player").AddComponent<AudioSource>();
                player.transform.parent = _playerRoot;
                _sfxPlayers.Add(player);
            }

            return player;
        }

        private AudioClip GetClip(string clipName)
        {
            AudioClip clip = null;

            if (_clipTable.ContainsKey(clipName))
            {
                clip = _clipTable[clipName];
            }
            else
            {
                Debug.LogErrorFormat("[Sound] GetClip : Can't found audio clip - {0}", clipName);
            }

            return clip;
        }

        private void UpdateVolumeBGM(float volume)
        {
            _bgmPlayer.volume = volume;
        }

        private void UpdateVolumeSFX(float volume)
        {
            foreach (var player in _sfxPlayers)
            {
                player.volume = volume;
            }
        }

        private void OnEndOfFrame(long l)
        {
            _playedClipInThisFrame.Clear();
        }
    }
}