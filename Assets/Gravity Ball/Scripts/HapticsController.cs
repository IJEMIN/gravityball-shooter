using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ultrahaptics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

/// <summary>
///     Controller class for all the haptics in the scene.
///     This class is responsible for controlling the Ultrahaptics device
///     and telling it when and where to emit control points.
/// </summary>
public class HapticsController : MonoBehaviour
{
    public enum IntensityMode
    {
        Strong,
        Normal,
        Weak
    }

    // The AmplitudeModulationEmitter allows us to control the Ultrahaptics array.
    // Please refer to the SDK documentation for an explanation of the difference between
    // AmplitudeModulationEmitter and TimePointStreamingEmitter.
    private AmplitudeModulationEmitter _amEmitter;

    // The CoordinateSpaceConverter enables conversion between world space and device coordinate space
    private CoordinateSpaceConverter _coordinateSpaceConverter;
    private List<Vector3> _debugPoints = new List<Vector3>();


    // The parent GameObject of the hands (so we can get access to the haptic receivers)
    [SerializeField] private GameObject _handsRoot;

    // A collection of all the haptic receivers in the scene
    private HapticReceiver[] _hapticReceivers;


    // 4 is the maximum number of control points that should be emitted at any time
    // If this number is set higher than 4 then all points will be weaker
    private readonly int _maxControlPoints = 4;
    public float contactPointBackwardAdjust = 0.001f;

    private readonly List<Vector3> contactPoints = new List<Vector3>();


    private int currentlyPayingTick;

    public IntensityMode intensityMode = IntensityMode.Normal;

    public float currentFrequency { get; private set; }

    public float hapticStrength { get; private set; }

    // Use this for initialization
    private void Start()
    {
        _amEmitter = new AmplitudeModulationEmitter();
        _coordinateSpaceConverter = FindObjectOfType<CoordinateSpaceConverter>();
        _hapticReceivers = _handsRoot.GetComponentsInChildren<HapticReceiver>(true);
        currentlyPayingTick = 0;

        if (!_amEmitter.isConnected()) Debug.LogWarning("No Ultrahaptics array connected");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateIntensity();


        // Get the current contact points from all the haptic receivers
        contactPoints.Clear();

        for (var i = 0; i < _hapticReceivers.Length; i++)
            if (_hapticReceivers[i].gameObject.activeInHierarchy)
            {
                var contactHits = _hapticReceivers[i].GetCurrentContactPoint();

                if (contactHits != null)
                    contactPoints.AddRange(contactHits.Select(hit =>
                        hit.point + hit.normal * contactPointBackwardAdjust));
            }

        if (contactPoints.Count <= 0)
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
            var deviceSpacePosition =
                _coordinateSpaceConverter.WorldToDevicePosition(pointToEmit);
            // Construct a control point with the position and intensity of the point
            var amControlPoint =
                new AmplitudeModulationControlPoint(deviceSpacePosition, hapticStrength);

            amControlPoint.setFrequency(currentFrequency * (float) Units.hertz);

            amControlPoints.Add(amControlPoint);
        }

        // Give the list of control points to the emitter
        if (contactPoints.Count > 0) _amEmitter.update(amControlPoints);
    }

    private void UpdateIntensity()
    {
        if (currentlyPayingTick > 0) return;

        if (intensityMode == IntensityMode.Weak)
        {
            currentFrequency = 144f;
            hapticStrength = 0.5f;
        }
        else if (intensityMode == IntensityMode.Normal)
        {
            currentFrequency = 164f;
            hapticStrength = 0.86f;
        }
        else if (intensityMode == IntensityMode.Strong)
        {
            currentFrequency = 196f;
            hapticStrength = 0.96f;
        }
    }

    private void OnDrawGizmos()
    {
        if (_debugPoints == null || _debugPoints.Count == 0) return;

        // Draw a wire sphere at each of the points
        Gizmos.color = Color.red;
        foreach (var point in _debugPoints) Gizmos.DrawWireSphere(point, 0.005f);
    }

    private void OnDisable()
    {
        // Stop the emitter when this GameObject is destroyed
        _amEmitter.stop();
        _amEmitter.Dispose();
    }

    /// <summary>
    ///     Chooses which of the given points should be emitted this frame.
    ///     If there are fewer or equal points in the given list than the maximum then all will be chosen.
    /// </summary>
    /// <param name="contactPoints">The list of possible points that could be emitted this frame.</param>
    /// <returns>A subset of the given list.</returns>
    private List<Vector3>
        ChoosePointsToEmit(List<Vector3> contactPoints)
    {
        var pointsToEmit = new List<Vector3>();

        if (contactPoints.Count <= _maxControlPoints)
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
            for (var i = 0; i < contactPoints.Count; i++) indices[i] = i;

            indices.OrderBy(a => Random.Range(0, int.MaxValue));

            for (var i = 0; i < _maxControlPoints; i++) pointsToEmit.Add(contactPoints[indices[i]]);
        }

        return pointsToEmit;
    }

    public void PlayTick(float time)
    {
        if (currentFrequency > 0) return;

        StartCoroutine(PlayTickRoutine(time));
    }

    private IEnumerator PlayTickRoutine(float time)
    {
        currentlyPayingTick++;

        yield return new WaitForSeconds(time * 0.125f);


        currentFrequency = 200f;
        hapticStrength = 1.0f;

        yield return new WaitForSeconds(time * 0.875f);

        currentlyPayingTick--;
    }
}