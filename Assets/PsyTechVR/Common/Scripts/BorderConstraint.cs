using UnityEngine;
using System.Collections;

public class BorderConstraint : MonoBehaviour {

	public GameObject Target;
	public int PushSide = -1;
	public bool SmoothTransition = false;
	public bool TranslateOnlyWhenNoParent = false;

	private const float SMOOTHNESS = 0.015f;
	
	private float mCilinderRadius;
	private float mBorderHalfHeight;
	private float mBorderHalfWidth;

	void Start () {
	
		mCilinderRadius = Target.GetComponent<Renderer>().bounds.size.x /2f;
		mBorderHalfHeight = (transform.TransformPoint(new Vector3(0, GetComponent<BoxCollider>().size.y *0.5f, 0)) - transform.position).magnitude;
		mBorderHalfWidth = (transform.TransformPoint(new Vector3(GetComponent<BoxCollider>().size.x *0.5f, 0, 0)) - transform.position).magnitude;
	}

	void Update () { // we want this to by in sync with physics sim
	
		if(TranslateOnlyWhenNoParent && Target.transform.parent.parent != null)
			return; // Hack to avoid the player to be pulled out from the wagons when he is already in but near this colliders
	
		float distY = Mathf.Abs (Vector3.Dot (transform.up, (Target.transform.position - transform.position)));
		float distX = Mathf.Abs (Vector3.Dot (transform.right, (Target.transform.position - transform.position)));
		if(distY < mCilinderRadius + mBorderHalfHeight && distX < mCilinderRadius + mBorderHalfWidth)
		{
			float multiplier = PushSide;
			if(SmoothTransition)
			{
				multiplier *= SMOOTHNESS;
			}
			
			float translateMagnitude = (mCilinderRadius + mBorderHalfHeight - distY);
			Target.transform.parent.Translate(multiplier * transform.up * translateMagnitude, Space.World);
			
			Target.transform.parent.SendMessage("SetControllable", Mathf.Abs(translateMagnitude) < 0.1);
		}
	}


}
