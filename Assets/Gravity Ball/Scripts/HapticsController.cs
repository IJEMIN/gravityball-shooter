using UnityEngine;
using Ultrahaptics;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Controller class for all the haptics in the scene.
/// This class is responsible for controlling the Ultrahaptics device
/// and telling it when and where to emit control points.
/// </summary>
public class HapticsController : MonoBehaviour
{
    public float contactPointBackwardAdjust = 0.001f;

    [Range(100f,200f)]
    public float currentFrequency = 200f;

    [Range(0f,1.0f)]
    public float hapticStrength = 0.5f;

    // The parent GameObject of the hands (so we can get access to the haptic receivers)
    [SerializeField]
    private GameObject _handsRoot;

    public int currentControlPoints
    {
        get
        {
            if(hapticStrength < 0.95f)
            {
                return (int)(_maxControlPoints * 1.5f);
            }
            else
            {
                return _maxControlPoints;
            }
        }
    }

    // 4 is the maximum number of control points that should be emitted at any time
    // If this number is set higher than 4 then all points will be weaker
    private int _maxControlPoints = 4;

    // The AmplitudeModulationEmitter allows us to control the Ultrahaptics array.
    // Please refer to the SDK documentation for an explanation of the difference between
    // AmplitudeModulationEmitter and TimePointStreamingEmitter.
    private AmplitudeModulationEmitter _amEmitter;
    List<UnityEngine.Vector3> _debugPoints = new List<UnityEngine.Vector3>();

    // The CoordinateSpaceConverter enables conversion between world space and device coordinate space
    private CoordinateSpaceConverter _coordinateSpaceConverter;
    
    // A collection of all the haptic receivers in the scene
    private HapticReceiver[] _hapticReceivers;

    // Use this for initialization
    void Start()
    {
        _amEmitter = new AmplitudeModulationEmitter();
        _coordinateSpaceConverter = FindObjectOfType<CoordinateSpaceConverter>();
        _hapticReceivers = _handsRoot.GetComponentsInChildren<HapticReceiver>(true);

        if (!_amEmitter.isConnected())
        {
            Debug.LogWarning("No Ultrahaptics array connected");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get the current contact points from all the haptic receivers
        var contactPoints = new List<UnityEngine.Vector3>();

        for(int i = 0; i < _hapticReceivers.Length; i++)        {
            if (_hapticReceivers[i].gameObject.activeInHierarchy)
            {
                var contactHits = _hapticReceivers[i].GetCurrentContactPoint();

                contactPoints.AddRange(contactHits.Select((hit) => hit.point + hit.normal * contactPointBackwardAdjust));
            }
        }

        if(contactPoints.Count <= 0)
        {
            _amEmitter.stop();
            return;
        }

        // Choose which points to emit if there are too many
        var pointsToEmit = ChoosePointsToEmit(contactPoints);

        // Store a reference to these points so they can be rendered for debugging
        _debugPoints = pointsToEmit;

        // Create a list to hold all the control points we want to emit this frame
        var amControlPoints = new List<AmplitudeModulationControlPoint>();

        // Construct control points for each of the points we want to emit
        foreach (var pointToEmit in pointsToEmit)
        {
            // The positions are in world space so convert them to device space
            var deviceSpacePosition = _coordinateSpaceConverter.WorldToDevicePosition(pointToEmit);
            // Construct a control point with the position and intensity of the point
            var amControlPoint = new AmplitudeModulationControlPoint(deviceSpacePosition, hapticStrength);

            amControlPoint.setFrequency(currentFrequency * (float)Units.hertz);

            amControlPoints.Add(amControlPoint);
        }

        // Give the list of control points to the emitter
        if (contactPoints.Count > 0)
        {
            _amEmitter.update(amControlPoints);
        }
    }

    void OnDrawGizmos()
    {
        if (_debugPoints == null || _debugPoints.Count == 0)
        {
            // Nothing to draw
            return;
        }

        // Draw a wire sphere at each of the points
        Gizmos.color = Color.red;
        foreach (var point in _debugPoints)
        {
            Gizmos.DrawWireSphere(point, 0.005f);
        }
    }

    void OnDisable()
    {
        // Stop the emitter when this GameObject is destroyed
        _amEmitter.stop();
        _amEmitter.Dispose();
    }

    /// <summary>
    /// Chooses which of the given points should be emitted this frame.
    /// If there are fewer or equal points in the given list than the maximum then all will be chosen.
    /// </summary>
    /// <param name="contactPoints">The list of possible points that could be emitted this frame.</param>
    /// <returns>A subset of the given list.</returns>
    List<UnityEngine.Vector3> ChoosePointsToEmit(List<UnityEngine.Vector3> contactPoints)
    {
        var pointsToEmit = new List<UnityEngine.Vector3>();

        if (contactPoints.Count <= currentControlPoints)
        {
            // We can emit all the points
            pointsToEmit.AddRange(contactPoints);
        }
        else
        {
            // There are more points of contact than haptic points we can emit this frame,
            // so we must choose which ones to emit.
            // This implementation chooses them randomly, but more sophisticated algorithms could be used

                
            
            var indices = new int[contactPoints.Count];
            for (var i = 0; i < contactPoints.Count; i++)
            {
                indices[i] = i;
            }
            indices.OrderBy(a => Random.Range(0, int.MaxValue));

            for (var i = 0; i < currentControlPoints; i++)
            {
                pointsToEmit.Add(contactPoints[indices[i]]);
            }
            
        }

        return pointsToEmit;
    }
}
