using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spider : MonoBehaviour {
	
	public float AttackInterval = 4f;
	
	private Animation mPathAnim;
	
	private const float ANIM_BLEND_TIME = 0.3f;
	
	void Start () {
	
		mPathAnim = transform.parent.GetComponent<Animation>();
		
		StartCoroutine( Animate() );
	}
	
	IEnumerator Animate() 
	{
		yield return new WaitForSeconds( AttackInterval * Random.Range(0.85f, 1.25f) );
	
		float currPathTime = mPathAnim["Spider"].time;
		mPathAnim.Stop();
		
		List<AnimationClip> clips = new List<AnimationClip>();
		foreach(AnimationState anim in GetComponent<Animation>())
			if( anim.name != "Walk" )
				clips.Add( anim.clip );
		
		AnimationClip clip = clips[ Random.Range(0, clips.Count) ];
		GetComponent<Animation>().CrossFade(clip.name, ANIM_BLEND_TIME);
		yield return new WaitForSeconds( clip.length );
		
		mPathAnim.Play();
		mPathAnim["Spider"].time = currPathTime;
		GetComponent<Animation>().CrossFade("Walk", ANIM_BLEND_TIME);
		
		StartCoroutine( Animate() );
	}
}
