using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerActionObservation
{
    public float time;
    public Vector3 pos;
    public Transform actor;
    public float connfidence01;
    public PlayerActionType type;
}

[Serializable]
public struct AIActionObservation
{
    public float time;
    public Vector3 pos;
    public Transform actor;
    public float connfidence01;
    public AIActionType type;
}

[Serializable]
public class RecentPlayerActions
{
    public readonly Dictionary<PlayerActionType, PlayerActionObservation> last = new();
    const int MaxHistory = 8;
    public readonly Queue<PlayerActionObservation> history = new();

    public void Mark(PlayerActionType t, in PlayerActionObservation obs)
    {
        last[t] = obs;
        history.Enqueue(obs);
        while (history.Count > MaxHistory) history.Dequeue();
    }

    public bool IsRecent(PlayerActionType t, float windowSec) =>
        last.TryGetValue(t, out var obs) && (Time.time - obs.time) <= windowSec;

    public Vector3? LastPos(PlayerActionType t) =>
        last.TryGetValue(t, out var obs) ? obs.pos : (Vector3?)null;
}

[Serializable]
public class RecentAIActions
{
    public readonly Dictionary<AIActionType, AIActionObservation> last = new();
    const int MaxHistory = 8;
    public readonly Queue<AIActionObservation> history = new();

    public void Mark(AIActionType t, in AIActionObservation obs)
    {
        last[t] = obs;
        history.Enqueue(obs);
        while (history.Count > MaxHistory) history.Dequeue();
    }

    public bool IsRecent(AIActionType t, float windowSec) =>
        last.TryGetValue(t, out var obs) && (Time.time - obs.time) <= windowSec;

    public Vector3? LastPos(AIActionType t) =>
        last.TryGetValue(t, out var obs) ? obs.pos : (Vector3?)null;
}
public enum TargetKind
{
    Unknown,
    Player,
    AI
}

public enum CreatureFaction
{
    Unknown,
    Predator,  
    Neutral,
    Prey
}

[Serializable]
public class TargetMemory
{
    public GameObject Target;
    public Vector3 LastKnownPos;
    public float LastKnownTime;
    public float LastKnownAwareness;
    public bool IsVisible;

    public TargetKind Kind = TargetKind.Unknown;
    public CreatureFaction Faction = CreatureFaction.Unknown;

    public bool IsFullyTracked;
}

#region Target Classifier
public static class TargetClassifier
{
    public static void ClassifyTarget(GameObject target, out TargetKind kind, out CreatureFaction faction)
    {
        kind = TargetKind.Unknown;
        faction = CreatureFaction.Unknown;
        if (!target) return;

        // Identify Player
        if (target.CompareTag("Player"))
        {
            kind = TargetKind.Player;
            faction = CreatureFaction.Unknown;
            return;
        }

        // Identify AI
        var ai = target.GetComponent<MonsterPerception>();
        if (ai != null)
        {
            kind = TargetKind.AI;

            // Later distigush faction based on AI identity
            // var identity = target.GetComponent<AIIdentity>();
            // if (identity != null)
            //     faction = identity.faction;

            return;
        }

        kind = TargetKind.Unknown;
        faction = CreatureFaction.Unknown;
    }
}
#endregion
public class Blackboard : MonoBehaviour
{
    [Header("Primary Target (summary, from AwarenessSystem)")]
    public Transform Target;
    public Vector3 TargetPos;
    public float DistToTarget, AngleToTarget;
    public float MYHP01, MyRange01, MyStamina01;
    public bool LineOfSight;

    [Header("Recent Actions")]
    public RecentPlayerActions RecentPlayers;
    public RecentAIActions RecentAIs;

    [Header("Perception")]
    public float LastSeenPlayerTime;
    public Vector3 LastSeenPlayerPos;
    public readonly Queue<PlayerActionObservation> SightHistory = new();

    [Header("Identification")]
    public float identifyAwarenessThreshold = 1.0f;
    public float trackAwarenessThreshold = 2.0f;

    [Header("Awareness Summary (best target)")]
    [Tooltip("Managed in AwarenessSystem")]
    public float CurrentAwareness;
    public bool HasSuspicion;
    public Vector3 SuspicionPos;
    public bool HasConfirmedTarget;

    [Header("Hearing (Suspicious)")]
    public bool HasHeardSomething;
    public Vector3 LastHeardPos;
    public Vector3 LastHeardDir;
    public float LastHeardIntensity;
    public float LastHeardTime;

    [Header("Known Targets (from AwarenessSystem)")]
    public readonly Dictionary<GameObject, TargetMemory> KnownTargets = new();

    [Header("Player Target (identified)")]
    public TargetMemory PlayerTargetMemory;
    public bool HasPlayerTarget;

    MonsterPerception _perception;
    AwarenessSystem _awareness;

    void Awake()
    {
        RecentPlayers ??= new RecentPlayerActions();
        RecentAIs ??= new RecentAIActions();
        _perception = GetComponent<MonsterPerception>();
        _awareness = GetComponent<AwarenessSystem>();
    }

    void OnEnable()
    {
        AIEventBus.OnPlayerAction += OnPlayerAction;
        AIEventBus.OnAIAction += OnAIAction;
    }

    void OnDisable()
    {
        AIEventBus.OnPlayerAction -= OnPlayerAction;
        AIEventBus.OnAIAction -= OnAIAction;
    }

    public float GetAwarenessFor(Transform actor)
    {
        if (actor == null) return 0f;
        if (KnownTargets.TryGetValue(actor.gameObject, out var mem))
        {
            return mem.LastKnownAwareness;
        }
        return 0f;
    }

    public void UpdateTargetMemory(GameObject target, Vector3 pos, float awareness, bool isVisible)
    {
        if (!target) return;

        // 0 ~ < identifyAwarenessThreshold:
        // Knows where the sound came from, but not who made it
        if (awareness < identifyAwarenessThreshold)
        {
            // position update only
            HasSuspicion = true;
            SuspicionPos = pos;

            return;
        }

        // awareness >= identifyAwarenessThreshold 
        // Guessing the target's identity and tracking

        if (!KnownTargets.TryGetValue(target, out var mem))
        {
            mem = new TargetMemory();
            mem.Target = target;
            KnownTargets[target] = mem;
        }

        mem.Target = target;
        mem.LastKnownPos = pos;
        mem.LastKnownTime = Time.time;
        mem.LastKnownAwareness = awareness;
        mem.IsVisible = isVisible;

        // if unknown, classify target
        if (mem.Kind == TargetKind.Unknown)
        {
            TargetClassifier.ClassifyTarget(target, out var kind, out var faction);
            mem.Kind = kind;
            mem.Faction = faction;
#if UNITY_EDITOR
            Debug.Log($"[Blackboard] {name} identified {target.name} as {mem.Kind} / {mem.Faction} (A={awareness:0.00})");
#endif
        }

        // (Awareness >= trackAwarenessThreshold)
        bool wasFullyTracked = mem.IsFullyTracked;
        bool nowFullyTracked = awareness >= trackAwarenessThreshold;
        mem.IsFullyTracked = nowFullyTracked;

        // Player target memory update
        if (mem.Kind == TargetKind.Player && nowFullyTracked)
        {
            PlayerTargetMemory.Target = mem.Target;
            PlayerTargetMemory.LastKnownPos = mem.LastKnownPos;
            PlayerTargetMemory.LastKnownTime = mem.LastKnownTime;
            PlayerTargetMemory.LastKnownAwareness = mem.LastKnownAwareness;
            PlayerTargetMemory.IsVisible = mem.IsVisible;
            PlayerTargetMemory.Kind = mem.Kind;
            PlayerTargetMemory.Faction = mem.Faction;
            HasPlayerTarget = true;

            LastSeenPlayerPos = mem.LastKnownPos;
            LastSeenPlayerTime = mem.LastKnownTime;
        }
        else if (mem.Kind == TargetKind.Player && wasFullyTracked && !nowFullyTracked)
        {
            // When tracking is lost
            if (HasPlayerTarget && PlayerTargetMemory.Target == target)
            {
                HasPlayerTarget = false;
                PlayerTargetMemory = new TargetMemory();
            }
        }
    }

    public void RemoveTargetMemory(GameObject target)
    {
        if (!target) return;

        if (KnownTargets.TryGetValue(target, out var mem))
        {
            if (HasPlayerTarget && PlayerTargetMemory.Target == target)
            {
                HasPlayerTarget = false;
                PlayerTargetMemory = new TargetMemory();
            }
        }

        KnownTargets.Remove(target);
    }

    void OnPlayerAction(PlayerActionEvent e)
    {
        if (e.Actor == null) return;

        float maxDist = 30f;
        if (_perception != null)
        {
            maxDist = Mathf.Max(_perception.SightRange, _perception.hearingRadius) * 1.2f;
        }

        Vector3 to = e.WorldPos - transform.position;
        if (to.sqrMagnitude > maxDist * maxDist)
            return;

        if(_awareness != null)
        {
            _awareness.ReportInProximity(e.Actor.gameObject, e.WorldPos);
        }

        float awareness = GetAwarenessFor(e.Actor);
        float confidence = awareness;

        var obs = new PlayerActionObservation
        {
            time = e.Timestamp,
            pos = e.WorldPos,
            actor = e.Actor,
            connfidence01 = confidence,
            type = e.Type
        };

        RecentPlayers.Mark(e.Type, obs);


    }

    void OnAIAction(AIActionEvent e)
    {
        if (e.Actor == null) return;

        if (e.Actor == transform || e.Actor.root == transform.root)
            return;

        float maxDist = 30f;
        if (_perception != null)
        {
            maxDist = Mathf.Max(_perception.SightRange, _perception.hearingRadius) * 1.2f;
        }

        Vector3 to = e.WorldPos - transform.position;
        if (to.sqrMagnitude > maxDist * maxDist)
            return;

        float awareness = GetAwarenessFor(e.Actor);
        float confidence = awareness;

        var obs = new AIActionObservation
        {
            time = e.Timestamp,
            pos = e.WorldPos,
            actor = e.Actor,
            connfidence01 = confidence,
            type = e.Type
        };

        RecentAIs.Mark(e.Type, obs);

#if UNITY_EDITOR
        Debug.Log($"[Blackboard] {name} observed AI action {e.Type} by {e.Actor.name} at {e.WorldPos} (conf={confidence:0.00})");
#endif
    }


    public void LogPlayerSight(Transform actor, Vector3 pos, float confidence)
    {
        LastSeenPlayerTime = Time.time;
        LastSeenPlayerPos = pos;

        DistToTarget = Vector3.Distance(transform.position, pos);
        AngleToTarget = Vector3.Angle(
            (GetComponent<MonsterPerception>()?.EyeDirection ?? transform.forward),
            (pos - transform.position)
        );
        LineOfSight = confidence >= 1f;

        var obs = new PlayerActionObservation
        {
            time = Time.time,
            pos = pos,
            actor = actor,
            connfidence01 = confidence,
            type = PlayerActionType.Default // sight dummy
        };
        SightHistory.Enqueue(obs);
        const int MaxSightHistory = 16;
        while (SightHistory.Count > MaxSightHistory) SightHistory.Dequeue();
    }

    public void ClearTarget()
    {
        Target = null;
        HasConfirmedTarget = false;
        LineOfSight = false;
        DistToTarget = 0f;
        AngleToTarget = 0f;
    }

    public void ClearSuspicion()
    {
        HasSuspicion = false;
        SuspicionPos = Vector3.zero;
        CurrentAwareness = 0f;
    }
}
