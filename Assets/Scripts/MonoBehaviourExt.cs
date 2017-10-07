using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourExt : MonoBehaviour {

	protected Transform m_transformCached = null;

	protected virtual void Awake()
	{
		m_transformCached = transform;
	}
		
}
