using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PortalBehaviour : MonoBehaviourExt {

	[SerializeField]
	private Transform m_otherPortalTransform = null;
	[SerializeField]
	private Transform m_levelForThisPortal = null;
	[SerializeField]
	private BoxCollider m_portalLimit = null;
	[SerializeField]
	private Transform m_inversePortalTransform = null;
	[SerializeField]
	private Animator m_animator = null;

	public BoxCollider BoxCollider
	{
		get { return m_portalLimit; }
	}

	public Transform LevelForThisPortal
	{
		get {
			return m_levelForThisPortal; 
		}
	}

	#region Monobehaviour calls

	void OnTriggerEnter(Collider other)
	{
		if (!PortalsManager.Singleton.ElementsToTeleport.ContainsKey (other.transform.GetComponent<Rigidbody> ())) {
			PortalsManager.Singleton.ElementsToTeleport.Add (other.transform.GetComponent<Rigidbody> (), this);	
			if (other.tag == "Player")
				TeleportPlayer ();
			else
				TeleportObject (other.transform.GetComponent<Rigidbody> ());	
		} 
	}

	void OnTriggerExit(Collider other)
	{
		if (PortalsManager.Singleton.ElementsToTeleport.ContainsKey(other.transform.GetComponent<Rigidbody>()))
			PortalsManager.Singleton.ElementsToTeleport.Remove (other.transform.GetComponent<Rigidbody>());		
	}



	#endregion

	#region Private methods

	private void TeleportPlayer()
	{		
		// transport him to the equivalent position in the other portal
		Vector3 newPosition = m_otherPortalTransform.position;

		float yLocal = m_transformCached.InverseTransformPoint (PlayerPOV.Singleton.transform.position).y;
		yLocal = m_otherPortalTransform.TransformPoint(new Vector3(0.0f,yLocal, 0.0f)).y - m_otherPortalTransform.position.y;
		newPosition += new Vector3 (0.0f, yLocal, 0.0f);

		Vector3 impulse = m_otherPortalTransform.TransformVector(PlayerPOV.Singleton.transform.InverseTransformVector(PlayerPOV.Singleton.CharacterController.velocity));
		PlayerPOV.Singleton.transform.position = newPosition;

		Quaternion newPlayerRotation = Quaternion.Inverse (m_inversePortalTransform.rotation) * PlayerPOV.Singleton.GetPlayerRotation();
		newPlayerRotation = m_otherPortalTransform.rotation * newPlayerRotation;

		PlayerPOV.Singleton.SetPlayerDirection (newPlayerRotation * Vector3.forward);
		PlayerPOV.Singleton.CharacterController.SimpleMove (impulse);
	}

	private void TeleportObject(Rigidbody objectRigidBody)
	{		
		Debug.Log (objectRigidBody.velocity);
		Vector3 impulse = m_otherPortalTransform.TransformVector(m_inversePortalTransform.
			InverseTransformVector(objectRigidBody.velocity));
		Debug.Log (impulse);

		objectRigidBody.isKinematic = true;
		Vector3 newPosition = m_otherPortalTransform.position;

		float yLocal = m_transformCached.InverseTransformPoint (objectRigidBody.transform.position).y;
		yLocal = m_otherPortalTransform.TransformPoint(new Vector3(0.0f,yLocal, 0.0f)).y - m_otherPortalTransform.position.y;
		newPosition += new Vector3 (0.0f, yLocal, 0.0f);

		Quaternion newRotation = Quaternion.Inverse (m_inversePortalTransform.rotation) * objectRigidBody.rotation;
		newRotation = m_otherPortalTransform.rotation * newRotation;

		objectRigidBody.transform.position = newPosition + m_otherPortalTransform.forward;
		objectRigidBody.transform.rotation = newRotation;

		objectRigidBody.isKinematic = false;
		objectRigidBody.velocity = impulse;
	}

	#endregion

	#region Public methods

	public void ChangePosition(Vector3 position, Vector3 direction)
	{
		m_transformCached.position = position;
		m_transformCached.forward = direction;
		m_animator.Rebind ();
	}

	public void AdjustLevelForThisPortal()
	{	
		Quaternion inverseRotation = Quaternion.Inverse (m_otherPortalTransform.rotation) * PlayerPOV.Singleton.RealLevelTransform.rotation;
		Vector3 inversePosition = m_otherPortalTransform.InverseTransformPoint (PlayerPOV.Singleton.RealLevelTransform.position);

		m_levelForThisPortal.position = m_inversePortalTransform.TransformPoint (inversePosition);
		m_levelForThisPortal.rotation = m_inversePortalTransform.rotation * inverseRotation;
	}

	#endregion


}
