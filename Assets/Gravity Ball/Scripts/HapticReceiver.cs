using System.Collections.Generic;
using UnityEngine;

//using Ultrahaptics;


/// <summary>
///     Keeps track of collision data to be used for haptics.
///     This class should be attached to GameObjects that will receive haptics when touched,
///     these GameObjects must also have a collider and a Rigidbody.
/// </summary>
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class HapticReceiver : MonoBehaviour
{
    private const float skinThickness = 0.001f;

    private CoordinateSpaceConverter _coordinateSpaceConverter;
    // Dictionary of colliders that are currently touching the haptic receiver,
    // and the corresponding point of contact

    private Rigidbody hapticRigidbody;

    private readonly List<RaycastHit> hits = new List<RaycastHit>();


    private void Awake()
    {
        hapticRigidbody = GetComponent<Rigidbody>();
        _coordinateSpaceConverter = FindObjectOfType<CoordinateSpaceConverter>();
    }

    public RaycastHit[] GetCurrentContactPoint()
    {
        hits.Clear();
        hits.AddRange(hapticRigidbody.SweepTestAll(-transform.up, skinThickness));
        hits.AddRange(hapticRigidbody.SweepTestAll(Vector3.down, skinThickness));

        return hits.ToArray();
    }
}