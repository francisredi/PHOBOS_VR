using UnityEngine;

/*
 * Draw the IR Camera view Cone with a line renderer
 * By Peter Koch
 * Keys: R = Reset Orientation
 *       I = Hide/Show view cone
 * Note: Requires OVRUtilities to access the tracker pose and frustrum:
 *       Go to https://developer.oculus.com/downloads and import "Oculus Utilities for Unity" 
 */
public class VRViewCone : MonoBehaviour 
{
	public bool Show = false;
	public Transform VRCamera;
	private Vector3 TrackerResetPosition;
    public GameObject irCameraPosition;

	void Update () 
	{
		HandleInput();

		if ((OVRManager.tracker.isPresent && OVRManager.tracker.isEnabled))
		{
			OVRPose ss = OVRManager.tracker.GetPose(0);
            irCameraPosition.transform.localPosition = ss.position;

            // Handle when the IR camera reports a spatial change
            if (ss.position != TrackerResetPosition)
			{
				TrackerResetPosition = ss.position;
				OVRTracker.Frustum ff = OVRManager.tracker.GetFrustum(0);
				UpdateViewCone(TrackerResetPosition, ss.orientation, ff.fov, ff.nearZ, ff.farZ);
			}

			ShowViewCone(Show);
		}
	}

	void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			UnityEngine.VR.InputTracking.Recenter();
		}
		if (Input.GetKeyDown(KeyCode.I))
		{
			Show = !Show;
		}
	}

	// Show or Hide our view cone 
	void ShowViewCone(bool show)
	{
		LineRenderer lr = gameObject.GetComponent<LineRenderer>();
		if (lr != null)
		{
			lr.enabled = show;
		}
	}

	// Update (create if necessary) our view cone 
	void UpdateViewCone(Vector3 trackerPosition, Quaternion trackerOrientation, Vector2 fov, float nearZ, float farZ)
	{
		LineRenderer lr = gameObject.GetComponent<LineRenderer>();
		if (lr == null)
		{
			lr = gameObject.AddComponent<LineRenderer>();
			lr.useWorldSpace = false;
			lr.SetWidth(0.001f, 0.001f);
			lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			lr.receiveShadows = false;
			lr.material = new Material(Shader.Find("UI/Default"));
			lr.material.color = Color.red;
		}
		
		float z = farZ - nearZ;
		Vector3 x = trackerOrientation * Vector3.left * (z * Mathf.Sin(Mathf.Deg2Rad * fov.x/2));
		Vector3 y = trackerOrientation * Vector3.up * (z * Mathf.Sin(Mathf.Deg2Rad * fov.y/2));
		Vector3 farCenter = trackerOrientation * Vector3.forward * z;

		// Normalized View Cone co-ordinates
		Vector3[] points = new Vector3[] { 
			new Vector3(0,0,0), 	// Apex
			new Vector3(-1,1,-1), 	// Top-Left to
			new Vector3(1,1,-1), 	// Top-Right
			new Vector3(0,0,0), 	// Apex
			new Vector3(-1,-1,-1), 	// Bottom-Left to
			new Vector3(1,-1,-1),	// Bottom-Right
			new Vector3(0,0,0), 	// Apex
			new Vector3(-1,1,-1), 	// Top-Left to
			new Vector3(-1,-1,-1), 	// Bottom-Left
			new Vector3(1,-1,-1), 	// Bottom-Right to
			new Vector3(1,1,-1)		// Top-Right
		};
		
		// Scale and position the View Cone into world space
		lr.SetVertexCount(points.Length);
		int n = 0;
		foreach (Vector3 point in points)
		{
			lr.SetPosition(n++, trackerPosition + farCenter * -point.z + x * point.x + y * point.y);
		}
	}
}
