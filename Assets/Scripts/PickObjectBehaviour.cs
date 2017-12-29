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
	private float m_lerpValue = 0.5f;

	private GameObject m_objectPicked = null;
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
			if (m_objectPicked != null) {
				m_objectPicked.GetComponent<Rigidbody> ().useGravity = true;
				m_objectPicked.GetComponent<Rigidbody> ().isKinematic = false;
				m_objectPicked = null;
			} else
				TryGetObject ();
		}
	}

	void LateUpdate()
	{		
		if (m_objectPicked != null) {
			m_objectPicked.transform.rotation = Quaternion.Lerp (m_objectPicked.transform.rotation, m_pickedObjectTransform.rotation,m_lerpValue);
			m_objectPicked.transform.position = Vector3.Lerp (m_objectPicked.transform.position, m_pickedObjectTransform.position,m_lerpValue);
		}
	}

	#endregion

	#region Private methods

	private void TryGetObject()
	{
		RaycastHit hit;
		if (Physics.Raycast (m_transformCached.position, m_transformCached.forward, out hit, m_maxDistanceForPicking,1 << LayerMask.NameToLayer ("Object"))) {			
			//hit.collider.attachedRigidbody.isKinematic = true;
			hit.collider.attachedRigidbody.useGravity = false;
			m_objectPicked = hit.collider.gameObject;
		}
	}

	#endregion

	#region Public methods

	#endregion
}
