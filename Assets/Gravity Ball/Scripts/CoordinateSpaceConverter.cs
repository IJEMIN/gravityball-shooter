using UnityEngine;
using Vector3 = Ultrahaptics.Vector3;

/// <summary>
///     Helper class to handle conversion between world space and Ultrahaptics device space.
/// </summary>
public class CoordinateSpaceConverter : MonoBehaviour
{
    // Expose this field to the Unity inspector so it can be set to
    // the transform that corresponds to the origin of the array (top-centre)
    [SerializeField] private Transform _arrayOrigin;

	/// <summary>
	///     Converts the given position vector from world space to device space.
	/// </summary>
	/// <param name="vector">The position vector in world space.</param>
	/// <returns>The equivalent position vector in device space.</returns>
	public Vector3 WorldToDevicePosition(UnityEngine.Vector3 vector)
    {
        // Transform the world position to the local space of the array
        var localPosition = _arrayOrigin.InverseTransformPoint(vector);
        // Construct an Ultrahaptics Vector3
        // Note that the y and z coordinates are swapped as Ultrahaptics uses a coordinate system where the positive Y-axis is up
        var ultrahapticsPosition = new Vector3(localPosition.x, localPosition.z, localPosition.y);
        return ultrahapticsPosition;
    }

	/// <summary>
	///     Converts the given direction vector from world space to device space.
	/// </summary>
	/// <param name="vector">The direction vector in world space.</param>
	/// <returns>The equivalent direction vector in device space.</returns>
	public Vector3 WorldToDeviceDirection(UnityEngine.Vector3 vector)
    {
        // Transform the world direction to the local space of the array
        var localDirection = _arrayOrigin.InverseTransformDirection(vector);
        // Construct an Ultrahaptics Vector3
        // Note that the y and z coordinates are swapped as Ultrahaptics uses a coordinate system where the positive Y-axis is up
        var ultrahapticsDirection = new Vector3(localDirection.x, localDirection.z, localDirection.y);
        return ultrahapticsDirection;
    }
}