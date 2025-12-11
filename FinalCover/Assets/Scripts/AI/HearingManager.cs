using System.Collections.Generic;
using UnityEngine;

public class HearingManager : MonoBehaviour
{
    public static HearingManager Instance { get; private set; }

    public readonly HashSet<MonsterPerception> hearingMonsters = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterHearing(MonsterPerception target)
    {
        if (target)
        {
            hearingMonsters.Add(target);
            Debug.Log("Registered monster for hearing: " + target.name);
        }
    }

    public void DeregisterHearing(MonsterPerception target)
    {
        if (target)
        {
            hearingMonsters.Remove(target);
            Debug.Log("Deregistered monster from hearing: " + target.name);
        }
    }

    public void BroadcastSoundEvent(GameObject source, Vector3 soundPos, float soundRadius, float soundIntensity)
    {
        List<MonsterPerception> toRemove = null;

        foreach (var monster in hearingMonsters)
        {
            if (!monster)
            {
                (toRemove ??= new List<MonsterPerception>()).Add(monster);
                continue;
            }

            float maxR = Mathf.Max(soundRadius, monster.hearingRadius);
            Vector3 d = monster.transform.position - soundPos;
            if (d.sqrMagnitude > maxR * maxR)
                continue;

            monster.MonsterHearing(source, soundPos, soundIntensity);
        }

        if (toRemove != null)
        {
            foreach (var dead in toRemove)
                hearingMonsters.Remove(dead);
        }
    }

    public void BroadcastSoundEvent(Vector3 soundPos, float soundRadius, float soundIntensity)
    {
        BroadcastSoundEvent(null, soundPos, soundRadius, soundIntensity);
    }
}
