using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonsterPerception : MonoBehaviour
{
    [Header("Vision")]
    public bool canSee = true;
    public Transform eye;
    [Range(-180f, 180f)]
    public float vieYawOffsetDeg = 0f;
    public Vector3 EyeDirection
    {
        get
        {
            var baseForward = eye ? eye.forward : transform.forward;

            var rotation = Quaternion.AngleAxis(vieYawOffsetDeg, Vector3.up);
            var dir = rotation * baseForward;

            return dir.normalized;
        }
    }
    public float SightRange = 20f;
    [Range(0, 360)] public float fovDeg = 110f;
    public LayerMask losBlockMask;

    // vision bookkeeping
    private readonly HashSet<Transform> previouslyVisible = new();
    private readonly HashSet<Transform> currentlyVisible = new();
    private readonly Dictionary<Transform, Vector3> lastSeenPos = new();
    public float leftLineDuration = 0.25f;

    [Header("Hearing")]
    public bool canHear = true;
    public float hearingRadius = 12f;
    [Range(0.1f, 2f)] public float hearingFalloff = 1f;

    [Header("Proximity")]
    public float proximityRange = 3f;

    // Proximity bookkeeping
    private readonly HashSet<Transform> currentlyInProximity = new();
    private readonly HashSet<Transform> previouslyInProximity = new();

    [Header("Filtering")]
    [SerializeField] LayerMask DetectionMask = ~0;

    [Header("Refs")]
    public AwarenessSystem awareness;
    public Blackboard bb;

    [Header("Debug")]
    public bool debugEnabled = true;

    [System.Flags]
    public enum DebugLayers
    {
        None = 0,
        SightCone = 1 << 0,
        HearingRange = 1 << 1,
        ProximityRange = 1 << 2,
        VisionRays = 1 << 3,
        LastSeen = 1 << 4,
        Targets = 1 << 5,
        All = ~0
    }

    public DebugLayers debugLayers =
        DebugLayers.SightCone | DebugLayers.HearingRange | DebugLayers.ProximityRange |
        DebugLayers.VisionRays | DebugLayers.LastSeen | DebugLayers.Targets;

    bool DebugOn(DebugLayers layer) => debugEnabled && (debugLayers & layer) != 0;

    // Player Only / LOD
    public bool IsWithinPlayerRange = true;
    static readonly Collider[] _hits = new Collider[10];

    void Awake()
    {
        if (!awareness) awareness = GetComponent<AwarenessSystem>();
        if (!bb) bb = GetComponent<Blackboard>();
    }

    void OnEnable() => HearingManager.Instance?.RegisterHearing(this);
    void OnDisable() => HearingManager.Instance?.DeregisterHearing(this);

    void Start()
    {
        HearingManager.Instance?.RegisterHearing(this);
    }

    void Update()
    {
        if (canSee) MonsterVision();
        MonsterProximity();
    }

    void MonsterVision()
    {
        if (!eye || !awareness || !IsWithinPlayerRange) return;

        currentlyVisible.Clear();

        int count = Physics.OverlapSphereNonAlloc(
            eye.position, SightRange, _hits, DetectionMask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < count; i++)
        {
            var hit = _hits[i];
            if (!hit) continue;

            var tr = hit.transform.root;
            if (tr == transform.root) continue;

            Vector3 pos = hit.bounds.center;

            Vector3 to = pos - eye.position;
            if (to.sqrMagnitude > SightRange * SightRange) continue;

            float halfFOV = fovDeg * 0.5f;
            float angle = Vector3.Angle(EyeDirection, to);
            if (angle > halfFOV) continue;

            if (!HasLineOfSight(pos)) continue;

            currentlyVisible.Add(tr);
            lastSeenPos[tr] = pos;

            awareness.ReportVision(tr.gameObject, pos);

            if (DebugOn(DebugLayers.VisionRays))
                Debug.DrawLine(eye.position, pos, Color.green);
        }

        foreach (var t in currentlyVisible)
        {
            if (!previouslyVisible.Contains(t))
                Debug.Log($"[Vision] {t.name} entered vision");
        }

        foreach (var t in previouslyVisible)
        {
            if (!t) continue;
            if (!currentlyVisible.Contains(t))
            {
                Debug.Log($"[Vision] {t.name} left");
                if (DebugOn(DebugLayers.LastSeen) && lastSeenPos.TryGetValue(t, out var p))
                {
                    Debug.DrawLine(eye.position, p, Color.red, leftLineDuration);
                    Debug.DrawRay(p, Vector3.up * 0.2f, Color.red, leftLineDuration);
                }
            }
        }

        previouslyVisible.Clear();
        foreach (var t in currentlyVisible)
            previouslyVisible.Add(t);
    }

    public void MonsterHearing(GameObject source, Vector3 location, float intensity)
    {
        if (!canHear || !awareness || !IsWithinPlayerRange) return;
        if (source == null) return;

        if (Vector3.Distance(location, transform.position) <= hearingRadius * intensity)
        {
            Debug.Log($"[Hearing] heard {source.name} at {location} intensity {intensity}");

            awareness.ReportCanHear(source, location, intensity);

            if (DebugOn(DebugLayers.VisionRays))
            {
                var origin = eye ? eye.position : transform.position;
                Debug.DrawLine(origin, location, Color.yellow, 0.15f);
                Debug.DrawRay(location, Vector3.up * 0.25f, Color.yellow, 0.15f);
            }
        }
    }

    void MonsterProximity()
    {
        if (!awareness || !IsWithinPlayerRange) return;
        currentlyInProximity.Clear();

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            proximityRange,
            _hits,
            DetectionMask,
            QueryTriggerInteraction.Ignore
        );

        float proximityRangeSqr = proximityRange * proximityRange;

        for (int i = 0; i < count; i++)
        {
            var hit = _hits[i];
            if (!hit) continue;

            var tr = hit.transform.root;
            if (tr == transform.root) continue;

            Vector3 to = tr.position - transform.position;
            if (to.sqrMagnitude > proximityRangeSqr)
                continue;

            currentlyInProximity.Add(tr);

            awareness.ReportInProximity(tr.gameObject, tr.position);
        }

        foreach (var t in currentlyInProximity)
        {
            if (!previouslyInProximity.Contains(t))
            {
                Debug.Log($"[Proximity] {t.name} entered proximity");
            }
        }

        foreach (var t in previouslyInProximity)
        {
            if (!t) continue;
            if (!currentlyInProximity.Contains(t))
            {
                Debug.Log($"[Proximity] {t.name} left proximity");
            }
        }

        previouslyInProximity.Clear();
        foreach (var t in currentlyInProximity)
            previouslyInProximity.Add(t);
    }

    bool HasLineOfSight(Vector3 worldPos)
    {
        Vector3 origin = eye.position;
        Vector3 dir = worldPos - origin;
        float dist = dir.magnitude;
        if (dist <= 0.001f) return true;
        dir /= dist;
        return !Physics.Raycast(origin, dir, dist, losBlockMask, QueryTriggerInteraction.Ignore);
    }

    

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!debugEnabled || !eye) return;
        DrawDebugGizmos();
    }

    void OnDrawGizmosSelected()
    {
        if (!debugEnabled || !eye) return;
        DrawDebugGizmos();
    }

    void DrawDebugGizmos()
    {
        if (DebugOn(DebugLayers.SightCone))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(eye.position, SightRange);

            Vector3 fwd = EyeDirection;
            Vector3 leftDir = Quaternion.Euler(0, -fovDeg * 0.5f, 0) * fwd;
            Vector3 rightDir = Quaternion.Euler(0, fovDeg * 0.5f, 0) * fwd;

            Gizmos.color = Color.white;
            Gizmos.DrawLine(eye.position, eye.position + leftDir * SightRange);
            Gizmos.DrawLine(eye.position, eye.position + rightDir * SightRange);

            Handles.color = new Color(1f, 1f, 0f, 0.15f);
            Handles.DrawSolidArc(eye.position, Vector3.up, leftDir, fovDeg, SightRange);
        }

        if (DebugOn(DebugLayers.HearingRange))
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(eye.position, hearingRadius);
        }

        if (DebugOn(DebugLayers.ProximityRange))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(eye.position, proximityRange);
        }

        if (DebugOn(DebugLayers.Targets) && bb && bb.Target)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(eye.position, bb.TargetPos);
            Gizmos.DrawWireSphere(bb.TargetPos, 0.15f);
        }
    }
#endif
}
