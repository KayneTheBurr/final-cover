using System;
using UnityEngine;

public enum  PlayerActionType {Idle, Walk, Run, Heal, Roll, Parry, Attack, Jump, Buff, Default}
public enum  AIActionType {Idle, Patrol, Investigate, Search, Chase, Attack, Flee, Eat, Default}

public struct PlayerActionEvent
{
    public PlayerActionType Type;
    public Transform Actor;
    public Vector3 WorldPos;
    public float Timestamp;
}

public struct AIActionEvent
{
    public AIActionType Type;
    public Transform Actor;
    public Vector3 WorldPos;
    public float Timestamp;
}

public static class AIEventBus
{
    public static event Action<PlayerActionEvent> OnPlayerAction;
    public static event Action<AIActionEvent> OnAIAction;
    // Notify all subscribers about a player action
    public static void NotifyPlayerAction(in PlayerActionEvent e)
    {
        var handlers = OnPlayerAction;
        if (handlers == null) return;

        foreach (var d in handlers.GetInvocationList())
        {
            try
            {
                ((Action<PlayerActionEvent>)d).Invoke(e);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error invoking PlayerAction handler: {ex}");
            }
        }
    }

    // Notify all subscribers about an AI action
    public static void NotifyAIAction(in AIActionEvent e)
    {
        var handlers = OnAIAction;
        if (handlers == null) return;
        foreach (var d in handlers.GetInvocationList())
        {
            try
            {
                ((Action<AIActionEvent>)d).Invoke(e);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error invoking AIAction handler: {ex}");
            }
        }
    }

    //Player Action Notifiers
    #region Player Action Notifiers

    public static void NotifyIdle(Transform player, Vector3 pos) =>
       NotifyPlayerAction(new PlayerActionEvent
       {
           Type = PlayerActionType.Idle,
           Actor = player,
           WorldPos = pos,
           Timestamp = Time.time
       });
    public static void NotifyWalk(Transform player, Vector3 pos) =>
        NotifyPlayerAction(new PlayerActionEvent
        {
            Type = PlayerActionType.Walk,
            Actor = player,
            WorldPos = pos,
            Timestamp = Time.time
        });

    public static void NotifyRun(Transform player, Vector3 pos) =>
        NotifyPlayerAction(new PlayerActionEvent
        {
            Type = PlayerActionType.Run,
            Actor = player,
            WorldPos = pos,
            Timestamp = Time.time
        });

    public static void NotifyHeal(Transform player, Vector3 pos) =>
        NotifyPlayerAction(new PlayerActionEvent
        {
            Type = PlayerActionType.Heal,
            Actor = player,
            WorldPos = pos,
            Timestamp = Time.time
        });

    public static void NotifyRoll(Transform player, Vector3 pos) =>
    NotifyPlayerAction(new PlayerActionEvent
    {
        Type = PlayerActionType.Roll,
        Actor = player,
        WorldPos = pos,
        Timestamp = Time.time
    });

    public static void NotifyParry(Transform player, Vector3 pos) =>
    NotifyPlayerAction(new PlayerActionEvent
    {
        Type = PlayerActionType.Parry,
        Actor = player,
        WorldPos = pos,
        Timestamp = Time.time
    });

    public static void NotifyAttack(Transform player, Vector3 pos) =>
       NotifyPlayerAction(new PlayerActionEvent
       {
           Type = PlayerActionType.Attack,
           Actor = player,
           WorldPos = pos,
           Timestamp = Time.time
       });

    public static void NotifyJump(Transform player, Vector3 pos) =>
       NotifyPlayerAction(new PlayerActionEvent
       {
           Type = PlayerActionType.Jump,
           Actor = player,
           WorldPos = pos,
           Timestamp = Time.time
       });

    public static void NotifyBuff(Transform player, Vector3 pos) =>
       NotifyPlayerAction(new PlayerActionEvent
       {
           Type = PlayerActionType.Buff,
           Actor = player,
           WorldPos = pos,
           Timestamp = Time.time
       });

    #endregion

    // AI Action Notifiers
    #region AI Action Notifiers

    //potentially consider merging with idle if not needed separately
    public static void NotifyAIIdle(Transform ai, Vector3 pos) =>
        NotifyAIAction(new AIActionEvent
        {
            Type = AIActionType.Idle,
            Actor = ai,
            WorldPos = pos,
            Timestamp = Time.time
        });
    public static void NotifyAIPatrol(Transform ai, Vector3 pos) =>
        NotifyAIAction(new AIActionEvent
        {
            Type = AIActionType.Patrol,
            Actor = ai,
            WorldPos = pos,
            Timestamp = Time.time
        });
    public static void NotifyAIInvestigate(Transform ai, Vector3 pos) =>
        NotifyAIAction(new AIActionEvent
        {
            Type = AIActionType.Investigate,
            Actor = ai,
            WorldPos = pos,
            Timestamp = Time.time
        });

    public static void NotifyAISearch(Transform ai, Vector3 pos) =>
        NotifyAIAction(new AIActionEvent
        {
            Type = AIActionType.Search,
            Actor = ai,
            WorldPos = pos,
            Timestamp = Time.time
        });

    public static void NotifyAIChase(Transform ai, Vector3 pos) =>
        NotifyAIAction(new AIActionEvent
        {
            Type = AIActionType.Chase,
            Actor = ai,
            WorldPos = pos,
            Timestamp = Time.time
        });

    public static void NotifyAIAttack(Transform ai, Vector3 pos) =>
        NotifyAIAction(new AIActionEvent
        {
            Type = AIActionType.Attack,
            Actor = ai,
            WorldPos = pos,
            Timestamp = Time.time
        });

    public static void NotifyAIFlee(Transform ai, Vector3 pos) =>
        NotifyAIAction(new AIActionEvent
        {
            Type = AIActionType.Flee,
            Actor = ai,
            WorldPos = pos,
            Timestamp = Time.time
        });

    public static void NotifyAIEat(Transform ai, Vector3 pos) =>
        NotifyAIAction(new AIActionEvent
        {
            Type = AIActionType.Eat,
            Actor = ai,
            WorldPos = pos,
            Timestamp = Time.time
        });

    public static void NotifyAIDefault(Transform ai, Vector3 pos) =>
        NotifyAIAction(new AIActionEvent
        {
            Type = AIActionType.Default,
            Actor = ai,
            WorldPos = pos,
            Timestamp = Time.time
        });

    #endregion
}
