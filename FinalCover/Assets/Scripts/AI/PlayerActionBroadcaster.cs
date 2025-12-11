using UnityEngine;
using UnityEngine.UIElements;

public class PlayerActionBroadcaster : MonoBehaviour
{
    [Header("Sounds")]
    public bool broadcastSounds = true;
    public float SoundRadius = 8f;
    public float SoundIntensity = 1f;

    Transform _tr;

    private void Awake()
    {
        _tr = transform;
    }

    public void OnIdle()
    {
        Vector3 pos = _tr.position;
        AIEventBus.NotifyIdle(_tr, pos);
        if (broadcastSounds)
            HearingManager.Instance?.BroadcastSoundEvent(pos, SoundRadius, 0f);
    }
    public void OnWalk()
    {    Vector3 pos = _tr.position;
        AIEventBus.NotifyWalk(_tr, pos);
        if (broadcastSounds)
            HearingManager.Instance?.BroadcastSoundEvent(pos, SoundRadius, 0.3f);
    }

    public void OnRun()
    {
        Vector3 pos = _tr.position;
        AIEventBus.NotifyRun(_tr, pos);
        if (broadcastSounds)
            HearingManager.Instance?.BroadcastSoundEvent(pos, SoundRadius, 0.6f);
    }

    public void OnAttack()
    {
        Vector3 pos = _tr.position;
        AIEventBus.NotifyAttack(_tr, pos);
        if (broadcastSounds)
            HearingManager.Instance?.BroadcastSoundEvent(pos, SoundRadius, 1.5f);
    }

    public void OnRoll()
    { 
        Vector3 pos = _tr.position;
        AIEventBus.NotifyRoll(_tr, pos);
        if (broadcastSounds)
            HearingManager.Instance?.BroadcastSoundEvent(pos, SoundRadius, 0.5f);
    }

    public void OnParry()
    {
        Vector3 pos = _tr.position;
        AIEventBus.NotifyParry(_tr, pos);
        if (broadcastSounds)
            HearingManager.Instance?.BroadcastSoundEvent(pos, SoundRadius, 1f);
    }
    public void OnHeal()
    {
        Vector3 pos = _tr.position;
        AIEventBus.NotifyHeal(_tr, pos);
        if (broadcastSounds)
            HearingManager.Instance?.BroadcastSoundEvent(pos, SoundRadius, 0.4f);

    }

    public void OnJump()
    {
        Vector3 pos = _tr.position;
        AIEventBus.NotifyJump(_tr, pos);
        if (broadcastSounds)
            HearingManager.Instance?.BroadcastSoundEvent(pos, SoundRadius, 0.7f);
    }

    public void OnBuff()
    {
        Vector3 pos = _tr.position;
        AIEventBus.NotifyBuff(_tr, pos);
        if (broadcastSounds)
            HearingManager.Instance?.BroadcastSoundEvent(pos, SoundRadius, 0.6f);
    }
}
