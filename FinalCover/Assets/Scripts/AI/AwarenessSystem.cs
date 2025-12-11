using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

class TrackedTarget
{
    public GameObject Target;
    public Vector3 RawPosition;

    public float LastSensedTime = -1f;
    public float Awareness; // 0 = Unaware, 0-1= rough idea(no set location) 1 = Suspicious(location), 2 = full awareness(see target)
    public bool UpdateAwareness(GameObject target, Vector3 position, float awareness, float minAwareness)
    {
        if (Target != target)
        {
            Target = target;
            RawPosition = position;
        }

        LastSensedTime = Time.time;
        var oldAwareness = Awareness;

        Awareness = Mathf.Clamp(Mathf.Max(Awareness, minAwareness) + awareness, 0f, 2f);

        return !Mathf.Approximately(oldAwareness, Awareness);
    }


    public bool DecayAwareness(float decayTime, float amount)
    {
        if ((Time.time - LastSensedTime) < decayTime)
            return false;

        var oldAwareness = Awareness;

        Awareness -= amount;

        if (oldAwareness >= 2f && Awareness < 2f)
            return true;
        if (oldAwareness >= 1f && Awareness < 1f)
            return true;
        return Awareness <= 0f;
    }

}

[RequireComponent(typeof(MonsterPerception), typeof(Blackboard))]
public class AwarenessSystem : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] AnimationCurve visionSensitivityCurve;
    [SerializeField] float visionMinimumAwareness = 1f;
    [SerializeField] float visionAwarenessBuildRate = 10f;

    [Header("Hearing")]
    [SerializeField] float hearingMinimumAwareness = 0f;
    [SerializeField] float hearingAwarenessBuildRate = 0.5f;

    [Header("Proximity")]
    [SerializeField] float proximityMinimumAwareness = 0f;
    [SerializeField] float proximityAwarenessBuildRate = 1f;

    [Header("Decay")]
    [SerializeField] float awarenessDecayDelay = 0.1f;
    [SerializeField] float awarenessDecayRate = 0.1f;

    readonly Dictionary<GameObject, TrackedTarget> _targets = new();

    Blackboard bb;
    MonsterPerception perception;

    void Awake()
    {
        bb = GetComponent<Blackboard>();
        perception = GetComponent<MonsterPerception>();

        if (visionSensitivityCurve == null || visionSensitivityCurve.length == 0)
        {
            var k0 = new Keyframe(0f, 0f, 0f, 4f);
            var k1 = new Keyframe(0.5f, 1f, 0f, 0f);
            var k2 = new Keyframe(1f, 0f, -4f, 0f);
            visionSensitivityCurve = new AnimationCurve(k0, k1, k2);
        }
    }

    void Update()
    {
        DecayAll();
        ProjectBestTargetToBlackboard();
    }

    public void ReportVision(GameObject targetGO, Vector3 position)
    {
        if (targetGO == null || perception == null) return;

        var eyePos = perception.eye ? perception.eye.position : transform.position;
        var toTarget = (position - eyePos);
        if (toTarget.sqrMagnitude < 0.0001f) return;

        var dirToTarget = toTarget.normalized;
        float dot = Vector3.Dot(perception.EyeDirection, dirToTarget);

        float halfFovDeg = perception.fovDeg * 0.5f;
        float cosHalfFov = Mathf.Cos(halfFovDeg * Mathf.Deg2Rad);
        if (dot < cosHalfFov)
            return; 


        float norm = Mathf.InverseLerp(cosHalfFov, 1f, dot); 

        float curveValue = visionSensitivityCurve.Evaluate(norm);

        float awarenessDelta =
            curveValue *
            visionAwarenessBuildRate *
            Time.deltaTime;

        UpdateAwareness(targetGO, position, awarenessDelta, visionMinimumAwareness, isVisible: true);
    }

    public void ReportCanHear(GameObject source, Vector3 location, float intensity)
    {
        if (source == null) return; 

        float awarenessDelta =
            intensity * hearingAwarenessBuildRate * Time.deltaTime;

        UpdateAwareness(source, location, awarenessDelta, hearingMinimumAwareness, isVisible: false);

        var toSound = location - transform.position;
        bb.HasHeardSomething = true;
        bb.LastHeardPos = location;
        bb.LastHeardDir = toSound.sqrMagnitude > 0.0001f ? toSound.normalized : Vector3.zero;
        bb.LastHeardIntensity = intensity;
        bb.LastHeardTime = Time.time;
    }

    public void ReportInProximity(GameObject targetGO, Vector3 position)
    {
        if (targetGO == null) return;

        float awarenessDelta = proximityAwarenessBuildRate * Time.deltaTime;
        UpdateAwareness(targetGO, position, awarenessDelta, proximityMinimumAwareness, isVisible: false);
    }


    void UpdateAwareness(GameObject targetGO, Vector3 position, float awarenessDelta, float minAwareness, bool isVisible)
    {
        if (!_targets.TryGetValue(targetGO, out var tracked))
        {
            tracked = new TrackedTarget();
            _targets[targetGO] = tracked;
        }

        bool changed = tracked.UpdateAwareness(targetGO, position, awarenessDelta, minAwareness);

        bb.UpdateTargetMemory(targetGO, position, tracked.Awareness, isVisible);


    }

    void DecayAll()
    {
        var toCleanup = new List<GameObject>();

        foreach (var kvp in _targets)
        {
            var tracked = kvp.Value;
            if (tracked.DecayAwareness(awarenessDecayDelay, awarenessDecayRate * Time.deltaTime))
            {
                if (tracked.Awareness <= 0f)
                    toCleanup.Add(kvp.Key);

                bb.UpdateTargetMemory(kvp.Key, tracked.RawPosition, tracked.Awareness, isVisible: false);
            }
        }

        foreach (var key in toCleanup)
        {
            _targets.Remove(key);
            bb.RemoveTargetMemory(key);
        }
    }

    void ProjectBestTargetToBlackboard()
    {
        bb.HasSuspicion = false;
        bb.HasConfirmedTarget = false;
        bb.CurrentAwareness = 0f;

        TrackedTarget best = null;
        GameObject bestGO = null;

        foreach (var kvp in _targets)
        {
            var tracked = kvp.Value;
            if (tracked.Awareness <= 0f)
                continue;

            if (best == null || tracked.Awareness > best.Awareness)
            {
                best = tracked;
                bestGO = kvp.Key;
            }
        }

        if (best == null)
        {
            bb.ClearTarget();
            bb.ClearSuspicion();
            return;
        }

        bb.CurrentAwareness = best.Awareness;

     
        if (best.Awareness < 1f)
        {
            bb.ClearTarget(); 

            if (best.Awareness > 0f)
            {
                bb.HasSuspicion = true;
                bb.SuspicionPos = best.RawPosition;  
            }

            return; 
        }

        if (bestGO != null)
        {
            var tr = bestGO.transform;

            bb.HasConfirmedTarget = true;
            bb.HasSuspicion = false;

            bb.Target = tr;
            bb.TargetPos = best.RawPosition;
            bb.DistToTarget = Vector3.Distance(transform.position, best.RawPosition);

            bb.LogPlayerSight(tr, best.RawPosition, confidence: 1f);
        }
    }

}

