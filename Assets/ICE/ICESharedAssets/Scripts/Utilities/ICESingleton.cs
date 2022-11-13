// ##############################################################################
//
// ICESingleton.cs
// Version 1.1.15
//
// © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.ice-technologies.com
// mailto:support@ice-technologies.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// ##############################################################################


using UnityEngine;
using System.Collections;

public class ICESingleton<_instance> : MonoBehaviour where _instance : ICESingleton<_instance>
{

	private static _instance m_Instance;
	public static _instance Instance{
		get{ return m_Instance; }
	}
	public bool isPersistant = true;

	public virtual void Awake() {

			if(!m_Instance) {
				m_Instance = this as _instance;
			}
			else {
				DestroyObject( gameObject );
			}
			DontDestroyOnLoad(gameObject);

		if(isPersistant) {
		}
		else {
			//m_Instance = this as _instance;
		}
	}
}
