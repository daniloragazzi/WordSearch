using UnityEngine;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Storage;

namespace RagazziStudios.Core.Application
{
    /// <summary>
    /// Singleton responsável pela música de fundo do jogo.
    /// Persiste entre cenas (DontDestroyOnLoad).
    /// Respeita a preferência MUSIC_ENABLED do storage.
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioClip _ambientLoop;
        [SerializeField] private float _defaultVolume = 0.3f;

        private bool _musicEnabled = true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
                _musicSource.volume = _defaultVolume;
            }

            LoadPreference();
        }

        private void Start()
        {
            if (_musicEnabled && _ambientLoop != null && !_musicSource.isPlaying)
            {
                Play();
            }
        }

        /// <summary>
        /// Inicia a reprodução da música de fundo.
        /// </summary>
        public void Play()
        {
            if (_musicSource == null || _ambientLoop == null) return;

            _musicSource.clip = _ambientLoop;
            _musicSource.volume = _defaultVolume;
            _musicSource.loop = true;
            _musicSource.Play();
        }

        /// <summary>
        /// Para a música de fundo.
        /// </summary>
        public void Stop()
        {
            if (_musicSource != null)
                _musicSource.Stop();
        }

        /// <summary>
        /// Habilita ou desabilita a música e persiste a preferência.
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            _musicEnabled = enabled;

            if (enabled)
            {
                if (!_musicSource.isPlaying && _ambientLoop != null)
                    Play();
            }
            else
            {
                Stop();
            }

            SavePreference();
        }

        /// <summary>
        /// Retorna se a música está habilitada.
        /// </summary>
        public bool IsEnabled() => _musicEnabled;

        /// <summary>
        /// Define o volume da música (0 a 1).
        /// </summary>
        public void SetVolume(float volume)
        {
            _defaultVolume = Mathf.Clamp01(volume);
            if (_musicSource != null)
                _musicSource.volume = _defaultVolume;
        }

        private void LoadPreference()
        {
            if (ServiceLocator.TryGet<IStorageService>(out var storage))
            {
                _musicEnabled = storage.GetBool(StorageKeys.MUSIC_ENABLED, true);
            }
        }

        private void SavePreference()
        {
            if (ServiceLocator.TryGet<IStorageService>(out var storage))
            {
                storage.SetBool(StorageKeys.MUSIC_ENABLED, _musicEnabled);
                storage.Save();
            }
        }
    }
}
