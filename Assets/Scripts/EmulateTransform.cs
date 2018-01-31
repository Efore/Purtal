/**************************************************************************\
Module Name:  EmulateTransform.cs
Project:      Purtal

This class takes an external transform and update its own one at each LateUpdate
with the local/global position/rotation/scale of the former.

\***************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmulateTransform : MonoBehaviourExt {

	[SerializeField]
	private Transform m_transformToEmulate = null;
	[SerializeField]
	private bool m_emulateLocal = false;
	[SerializeField]
	private bool m_emulatePosition = false;
	[SerializeField]
	private bool m_emulateRotation = false;
	[SerializeField]
	private bool m_emulateScale = false;


	void LateUpdate()
	{
		if (m_transformToEmulate != null) 
		{
			if (m_emulateLocal) {
				if (m_emulatePosition)
					m_transformCached.localPosition = m_transformToEmulate.localPosition;
				if (m_emulateRotation)
					m_transformCached.localRotation = m_transformToEmulate.localRotation;
				if (m_emulateScale)
					m_transformCached.localScale = m_transformToEmulate.localScale;
			} else {
				if (m_emulatePosition)
					m_transformCached.position = m_transformToEmulate.position;
				if (m_emulateRotation)
					m_transformCached.rotation = m_transformToEmulate.rotation;
			}
		}
	}

}
