using UnityEngine;
using System.Collections.Generic;
using System.Linq;
//using Ultrahaptics;


/// <summary>
/// Keeps track of collision data to be used for haptics.
/// This class should be attached to GameObjects that will receive haptics when touched,
/// these GameObjects must also have a collider and a Rigidbody.
/// </summary>
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class HapticReceiver : MonoBehaviour
{
    // Dictionary of colliders that are currently touching the haptic receiver,
    // and the corresponding point of contact

    private Rigidbody hapticRigidbody;

    private CoordinateSpaceConverter _coordinateSpaceConverter;

    private const float skinThickness = 0.001f;

    

    private void Awake()
    {

        hapticRigidbody = GetComponent<Rigidbody>();
        _coordinateSpaceConverter = FindObjectOfType<CoordinateSpaceConverter>();
    }

    public RaycastHit[] GetCurrentContactPoint()
    {
        RaycastHit[] downHit = hapticRigidbody.SweepTestAll(-transform.up, skinThickness);
        //RaycastHit[] upHit = hapticRigidbody.SweepTestAll(Vector3.up, 0.01f);

        return downHit.ToArray();
    }
}
