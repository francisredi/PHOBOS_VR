using UnityEngine;
using System.Collections;

namespace CrowdSim
{
	public class BreadCrumbs : MonoBehaviour {
		
		private ArrayList breadcrumbs;
		private Vector3 lastPos;
		private Color color;
		
		void RemoveBreadCrumb()
		{
		    Destroy((Object)breadcrumbs[0]);
		    breadcrumbs.RemoveAt(0);
		}

		// Use this for initialization
		void Start () {
			breadcrumbs = new ArrayList();
			color = new Color(Random.value,Random.value,Random.value);
			InvokeRepeating("Timed",0.0f,0.2f);
		}
		
		// Update is called once per frame
		private void Timed() {
			if(CitySimulator.breadCrumbs) // show bread crumbs
		    {
		        if(lastPos != this.transform.position)
		        {
		            GameObject bc = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					Vector3 pos = new Vector3();
					pos.x = transform.position.x;
					pos.y = transform.position.y;
					pos.z = transform.position.z;
		            bc.transform.position = pos;
		            Destroy(bc.GetComponent<Collider>());
		            bc.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
		            bc.GetComponent<Renderer>().material.color = color;
		            breadcrumbs.Add(bc);
		        }
		        lastPos = this.transform.position;
		
		        if(breadcrumbs.Count > 30)
		        {
		            RemoveBreadCrumb();
		        }
		    }
		    else{ // do not show breadcrumbs
		        while(breadcrumbs.Count > 0){
		            RemoveBreadCrumb();
		        }
		    }
		}
	}
}
