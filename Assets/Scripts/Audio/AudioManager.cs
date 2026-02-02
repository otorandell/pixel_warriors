using UnityEngine;

namespace PixelWarriors
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource[] _sourcePool;
        private int _nextSourceIndex;
        private AudioSource _musicSource;

        private void Awake()
        {
            SFXLibrary.Initialize();
            CreateSourcePool();
            CreateMusicSource();
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        // --- Event Subscriptions ---

        private void SubscribeEvents()
        {
            // UI
            GameEvents.OnAbilitySelected += HandleAbilitySelected;
            GameEvents.OnTargetSelected += HandleTargetSelected;
            GameEvents.OnActionConfirmed += HandleActionConfirmed;
            GameEvents.OnActionCancelled += HandleActionCancelled;
            GameEvents.OnCharacterDetailRequested += HandleCharacterPopup;
            GameEvents.OnAbilityDetailRequested += HandleAbilityPopup;
            GameEvents.OnTabClicked += HandleTabClicked;

            // Combat
            GameEvents.OnHitResolved += HandleHitResolved;
            GameEvents.OnHealingReceived += HandleHealing;
            GameEvents.OnCharacterDefeated += HandleDefeated;

            // Battle flow
            GameEvents.OnBattleStarted += HandleBattleStarted;
            GameEvents.OnBattleStateChanged += HandleBattleStateChanged;
            GameEvents.OnTurnStarted += HandleTurnStarted;
        }

        private void UnsubscribeEvents()
        {
            GameEvents.OnAbilitySelected -= HandleAbilitySelected;
            GameEvents.OnTargetSelected -= HandleTargetSelected;
            GameEvents.OnActionConfirmed -= HandleActionConfirmed;
            GameEvents.OnActionCancelled -= HandleActionCancelled;
            GameEvents.OnCharacterDetailRequested -= HandleCharacterPopup;
            GameEvents.OnAbilityDetailRequested -= HandleAbilityPopup;
            GameEvents.OnTabClicked -= HandleTabClicked;

            GameEvents.OnHitResolved -= HandleHitResolved;
            GameEvents.OnHealingReceived -= HandleHealing;
            GameEvents.OnCharacterDefeated -= HandleDefeated;

            GameEvents.OnBattleStarted -= HandleBattleStarted;
            GameEvents.OnBattleStateChanged -= HandleBattleStateChanged;
            GameEvents.OnTurnStarted -= HandleTurnStarted;
        }

        // --- UI Handlers ---

        private void HandleAbilitySelected(AbilityData ability)
        {
            PlaySFX(SFXType.UIClick, AudioConfig.UIClickVolume);
        }

        private void HandleTargetSelected(BattleCharacter target)
        {
            PlaySFX(SFXType.UIClick, AudioConfig.UIClickVolume);
        }

        private void HandleActionConfirmed()
        {
            PlaySFX(SFXType.UIConfirm, AudioConfig.UIConfirmVolume);
        }

        private void HandleActionCancelled()
        {
            PlaySFX(SFXType.UICancel, AudioConfig.UICancelVolume);
        }

        private void HandleCharacterPopup(BattleCharacter character)
        {
            PlaySFX(SFXType.UIPopup, AudioConfig.UIPopupVolume);
        }

        private void HandleAbilityPopup(AbilityData ability)
        {
            PlaySFX(SFXType.UIPopup, AudioConfig.UIPopupVolume);
        }

        private void HandleTabClicked(AbilityTab tab)
        {
            PlaySFX(SFXType.TabClick, AudioConfig.TabClickVolume);
        }

        // --- Combat Handlers ---

        private void HandleHitResolved(BattleCharacter target, HitResult result, DamageType damageType)
        {
            if (result.Missed)
            {
                PlaySFX(SFXType.Miss, AudioConfig.MissVolume);
            }
            else if (result.Dodged)
            {
                PlaySFX(SFXType.Dodge, AudioConfig.DodgeVolume);
            }
            else if (result.IsCrit)
            {
                PlayWithVariation(SFXType.CriticalHit, AudioConfig.CritHitVolume);
            }
            else if (damageType == DamageType.Physical)
            {
                PlayWithVariation(SFXType.PhysicalHit, AudioConfig.PhysicalHitVolume);
            }
            else
            {
                PlayWithVariation(SFXType.MagicalHit, AudioConfig.MagicalHitVolume);
            }
        }

        private void HandleHealing(BattleCharacter target, int amount)
        {
            PlaySFX(SFXType.Heal, AudioConfig.HealVolume);
        }

        private void HandleDefeated(BattleCharacter character)
        {
            PlaySFX(SFXType.Defeated, AudioConfig.DefeatedVolume);
        }

        // --- Battle Flow Handlers ---

        private void HandleBattleStarted()
        {
            PlaySFX(SFXType.BattleStart, AudioConfig.BattleStartVolume);
            PlayMusic(AudioConfig.BattleThemePath);
        }

        private void HandleBattleStateChanged(BattleState state)
        {
            if (state == BattleState.Victory)
            {
                StopMusic();
                PlaySFX(SFXType.Victory, AudioConfig.VictoryVolume);
            }
            else if (state == BattleState.Defeat)
            {
                StopMusic();
                PlaySFX(SFXType.BattleDefeat, AudioConfig.BattleDefeatVolume);
            }
        }

        private void HandleTurnStarted(BattleCharacter character)
        {
            // Only notify for player turns to avoid constant pinging
            if (character.Side == TeamSide.Player)
                PlaySFX(SFXType.TurnNotify, AudioConfig.TurnNotifyVolume);
        }

        // --- Playback ---

        private void PlaySFX(SFXType type, float volume)
        {
            AudioClip clip = SFXLibrary.Get(type);
            if (clip == null) return;

            AudioSource source = GetNextSource();
            source.clip = clip;
            source.volume = volume * AudioConfig.MasterVolume;
            source.pitch = 1f;
            source.Play();
        }

        private void PlayWithVariation(SFXType type, float volume)
        {
            AudioClip clip = SFXLibrary.Get(type);
            if (clip == null) return;

            AudioSource source = GetNextSource();
            source.clip = clip;
            source.volume = volume * AudioConfig.MasterVolume;
            source.pitch = Random.Range(AudioConfig.PitchVariationMin, AudioConfig.PitchVariationMax);
            source.Play();
        }

        private AudioSource GetNextSource()
        {
            AudioSource source = _sourcePool[_nextSourceIndex];
            _nextSourceIndex = (_nextSourceIndex + 1) % _sourcePool.Length;
            return source;
        }

        private void CreateSourcePool()
        {
            _sourcePool = new AudioSource[AudioConfig.AudioSourcePoolSize];
            for (int i = 0; i < _sourcePool.Length; i++)
            {
                _sourcePool[i] = gameObject.AddComponent<AudioSource>();
                _sourcePool[i].playOnAwake = false;
            }
        }

        // --- Music ---

        private void CreateMusicSource()
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
            _musicSource.volume = AudioConfig.MusicVolume * AudioConfig.MasterVolume;
        }

        private void PlayMusic(string resourcePath)
        {
            AudioClip clip = Resources.Load<AudioClip>(resourcePath);
            if (clip == null) return;

            _musicSource.clip = clip;
            _musicSource.Play();
        }

        private void StopMusic()
        {
            _musicSource.Stop();
        }
    }
}
