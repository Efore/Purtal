using System.Collections;
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

	private Transform m_pickedObjectParent = null;
	private PickableObject m_pickablePicked = null;
	#endregion

	#region Public members

	#endregion

	#region Properties

	public bool HasObjectPicked
	{
		get { return m_pickablePicked != null; }
	}

	#endregion

	#region Events

	#endregion

	#region MonoBehaviour calls

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.E)) 
		{
			if (m_pickablePicked != null) 
			{					
				m_pickablePicked.VelocityTracker.Rigidbody.useGravity = true;
				m_pickablePicked.VelocityTracker.Rigidbody.isKinematic = false;
				//m_pickablePicked.transform.SetParent (m_pickedObjectParent);

				m_pickablePicked.IsPicked = false;

				Vector3 impulse = PlayerPOV.Singleton.transform.InverseTransformVector (PlayerPOV.Singleton.CharacterController.velocity);
				impulse = m_transformCached.TransformVector (impulse);

				if(Vector3.Dot(PlayerPOV.Singleton.CharacterController.velocity,m_transformCached.forward) > 0.5f)
					m_pickablePicked.VelocityTracker.Rigidbody.AddForce (impulse,ForceMode.Impulse);

				if (m_pickablePicked.InPortalTrigger)
					m_pickablePicked.DroppedObjectInPortal ();

				m_pickablePicked = null;
			} 
			else
				TryGetObject ();
		}
	}

	void FixedUpdate()
	{		
		if (m_pickablePicked != null) 
		{
			m_pickablePicked.transform.rotation = Quaternion.Lerp (m_pickablePicked.transform.rotation, m_pickedObjectTransform.rotation,m_lerpValue);
			m_pickablePicked.transform.position = Vector3.Lerp (m_pickablePicked.transform.position, m_pickedObjectTransform.position,m_lerpValue);
		}
	}

	#endregion

	#region Private methods

	private void TryGetObject()
	{
		RaycastHit hit;
		if (Physics.Raycast (m_transformCached.position, m_transformCached.forward, out hit, m_maxDistanceForPicking,1 << LayerMask.NameToLayer ("Object"))) 
		{	
			m_pickablePicked = hit.collider.GetComponent<PickableObject>();
			m_pickablePicked.VelocityTracker.Rigidbody.useGravity = false;
			m_pickablePicked.VelocityTracker.Rigidbody.isKinematic = true;
			m_pickablePicked.IsPicked = true;
			//m_pickedObjectParent = m_pickablePicked.transform.parent;
			//m_pickablePicked.transform.SetParent (m_pickedObjectTransform);
		}
	}

	#endregion

	#region Public methods

	public void SwitchObject()
	{
		PickableObject clone = m_pickablePicked.Clone;
		clone.VelocityTracker.Rigidbody.useGravity = false;
		clone.VelocityTracker.Rigidbody.isKinematic = true;
		clone.IsPicked = true;

		m_pickablePicked.gameObject.SetActive (false);
		m_pickablePicked.VelocityTracker.Rigidbody.isKinematic = false;
		m_pickablePicked.TransformCached.position = Vector3.one * -1000000;
		m_pickablePicked.CanBeTeleported = true;

		m_pickablePicked = clone;
		m_pickablePicked.transform.rotation = m_pickedObjectTransform.rotation;
		m_pickablePicked.transform.position = m_pickablePicked.transform.position;
		m_pickablePicked.PortalBehaviourForLocalPositioning = null;
		m_pickablePicked.gameObject.SetActive (true);

	}

	#endregion
}
