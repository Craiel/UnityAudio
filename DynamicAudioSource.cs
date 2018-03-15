using IPoolable = Craiel.UnityEssentials.Contracts.IPoolable;
using ResourceProvider = Craiel.UnityEssentials.Resource.ResourceProvider;

namespace Assets.Scripts.Craiel.Audio
{
    using Craiel.GameData;
    using Data;
    using Enums;
    using States;
    using UnityEngine;
    using UnityEngine.Audio;

    public class DynamicAudioSource : MonoBehaviour, IPoolable
    {
        private readonly EnumStateMachine<DynamicAudioSource, DynamicAudioSourceStateBase, DynamicAudioSourceState> state;

        private Transform anchorTransform;
        private bool followAnchor;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DynamicAudioSource()
        {
            this.state = new EnumStateMachine<DynamicAudioSource, DynamicAudioSourceStateBase, DynamicAudioSourceState>(this, DynamicAudioSourceStateInactive.Instance);
            this.state.SetState(DynamicAudioSourceState.Inactive, DynamicAudioSourceStateInactive.Instance);
            this.state.SetState(DynamicAudioSourceState.FadeIn, DynamicAudioSourceStateFadeIn.Instance);
            this.state.SetState(DynamicAudioSourceState.FadeOut, DynamicAudioSourceStateFadeOut.Instance);
            this.state.SetState(DynamicAudioSourceState.Playing, DynamicAudioSourceStatePlaying.Instance);
            this.state.SetState(DynamicAudioSourceState.Finished, DynamicAudioSourceStateFinished.Instance);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public AudioSource Source;

        // Fields for performance
        [HideInInspector]
        public AudioPlayParameters Parameters;

        [HideInInspector]
        public float CurrentFadeTime;

        public GameDataId ActiveId { get; private set; }

        public AudioTicket Ticket { get; private set; }

        public bool IsActive
        {
            get { return this.state.CurrentState != DynamicAudioSourceStateFinished.Instance; }
        }

        public void Awake()
        {
            this.gameObject.SetActive(false);
        }

        public void Reset()
        {
            this.Source.spatialBlend = 1f;
            this.Source.loop = false;
            this.Source.outputAudioMixerGroup = null;
            this.Source.clip = null;

            this.Parameters = default(AudioPlayParameters);

            this.ActiveId = GameDataId.Invalid;

            this.state.SwitchState(DynamicAudioSourceState.Inactive);

            this.gameObject.SetActive(false);
        }

        public void Update()
        {
            if (this.followAnchor)
            {
                this.transform.position = this.anchorTransform.position;
                this.transform.rotation = this.anchorTransform.rotation;
            }
            
            this.state.Update();
        }

        public void SetPosition(Vector3 position)
        {
            this.gameObject.transform.position = position;
        }

        public void Play(AudioTicket ticket, RuntimeAudioData entry, bool is3D, AudioMixerGroup mixerGroup, AudioPlayParameters parameters)
        {
            this.Source.clip = parameters.UseRandomClip
                ? GetClip(entry, (ushort)Random.Range(0, entry.ClipKeys.Count))
                : GetClip(entry, parameters.ClipIndex);
            
            this.Source.spatialBlend = is3D ? 1f : 0f;

            switch (entry.PlayBehavior)
            {
                case AudioPlayBehavior.Loop:
                    {
                        this.Source.loop = true;
                        break;
                    }
            }

            this.Parameters = parameters;
            this.Source.outputAudioMixerGroup = mixerGroup;

            this.ActiveId = entry.Id;
            this.Ticket = ticket;

            this.gameObject.SetActive(true);

            this.state.SwitchState(DynamicAudioSourceState.FadeIn);
        }

        public void SetAnchor(Transform anchor)
        {
            this.anchorTransform = anchor;
            this.followAnchor = this.anchorTransform != null;
        }
        
        public void Stop()
        {
            this.state.SwitchState(DynamicAudioSourceState.FadeOut);
        }
        
        // -------------------------------------------------------------------
        // Internal
        // -------------------------------------------------------------------
        internal void SwitchState(DynamicAudioSourceState newState)
        {
            this.state.SwitchState(newState);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private AudioClip GetClip(RuntimeAudioData data, int index)
        {
            using (var resource = ResourceProvider.Instance.AcquireOrLoadResource<AudioClip>(data.ClipKeys[index]))
            {
                if (resource != null && resource.Data != null)
                {
                    return resource.Data;
                }
            }

            return null;
        }
    }
}
