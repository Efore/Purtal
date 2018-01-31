/**************************************************************************\
Module Name:  MonoBehaviourExt.cs
Project:      Purtal

This class extends the basic MonoBehaviour and adds a reference to the Transform class
of the object from the Awake call of the class.

\***************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourExt : MonoBehaviour {

	protected Transform m_transformCached = null;

	protected virtual void Awake()
	{
		m_transformCached = transform;
	}
		
	public Transform TransformCached
	{
		get {
			return m_transformCached;
		}
	}
}
