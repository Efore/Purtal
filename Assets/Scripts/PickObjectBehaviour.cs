﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickObjectBehaviour : MonoBehaviourExt {

	#region Private members

	[SerializeField]
	private Transform m_pickedObjectTransform = null;
	[SerializeField]
	private float m_maxDistanceForPicking = 5.0f;
	[SerializeField]
	[Range(0.0f,1.0f)]
	private float m_lerpValue = 0.5f;
	[SerializeField]
	[Range(1.0f,10.0f)]
	private float m_multiplicatorForThrowing = 1.0f;

	private Transform m_pickedObjectParent = null;
	private Rigidbody m_rigidBodyPicked = null;
	#endregion

	#region Public members

	#endregion

	#region Properties

	#endregion

	#region Events

	#endregion

	#region MonoBehaviour calls
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.E)) {
			if (m_rigidBodyPicked != null) {
				m_rigidBodyPicked.useGravity = true;
				m_rigidBodyPicked.isKinematic = false;
				m_rigidBodyPicked.transform.SetParent (m_pickedObjectParent);
				Vector3 impulse = PlayerPOV.Singleton.transform.InverseTransformVector (PlayerPOV.Singleton.CharacterController.velocity);
				impulse = m_transformCached.TransformVector (impulse);

				if(Vector3.Dot(PlayerPOV.Singleton.CharacterController.velocity,m_transformCached.forward) > 0.5f)
					m_rigidBodyPicked.AddForce (impulse,ForceMode.Impulse);
				
				m_rigidBodyPicked = null;
			} else
				TryGetObject ();
		}

	}

	void FixedUpdate()
	{		
		if (m_rigidBodyPicked != null) {
			m_rigidBodyPicked.MoveRotation(Quaternion.Lerp (m_rigidBodyPicked.transform.rotation, m_pickedObjectTransform.rotation,m_lerpValue));
			m_rigidBodyPicked.MovePosition(Vector3.Lerp (m_rigidBodyPicked.transform.position, m_pickedObjectTransform.position,m_lerpValue));
		}
	}

	#endregion

	#region Private methods

	private void TryGetObject()
	{
		RaycastHit hit;
		if (Physics.Raycast (m_transformCached.position, m_transformCached.forward, out hit, m_maxDistanceForPicking,1 << LayerMask.NameToLayer ("Object"))) {	
			m_rigidBodyPicked = hit.collider.attachedRigidbody;
			m_rigidBodyPicked.useGravity = false;
			m_rigidBodyPicked.isKinematic = true;
			m_pickedObjectParent = m_rigidBodyPicked.transform.parent;
			m_rigidBodyPicked.transform.SetParent (m_pickedObjectTransform);
		}
	}

	#endregion

	#region Public methods

	#endregion
}
