using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PortalController : MonoBehaviourExt {

	[SerializeField]
	private Transform m_otherPortalTransform = null;
	[SerializeField]
	private Transform m_levelForThisPortal = null;
	[SerializeField]
	private BoxCollider m_portalLimit = null;
	[SerializeField]
	private Transform m_inversePortalTransform = null;

	private bool playerOverlapping = false;

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

	void LateUpdate()
	{
		TeleportBehaviour ();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			playerOverlapping = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			playerOverlapping = false;
		}
	}

	#endregion

	#region Private methods

	private void TeleportBehaviour()
	{
		if (playerOverlapping) {

			// transport him to the equivalent position in the other portal
			var newPosition = m_otherPortalTransform.position;
			Vector3 impulse = m_otherPortalTransform.TransformVector(PlayerPOV.Singleton.transform.InverseTransformVector(PlayerPOV.Singleton.CharacterController.velocity));
			PlayerPOV.Singleton.transform.position = newPosition;

			Quaternion newPlayerRotation = Quaternion.Inverse (m_inversePortalTransform.rotation) * PlayerPOV.Singleton.GetPlayerRotation();
			newPlayerRotation = m_otherPortalTransform.rotation * newPlayerRotation;

			//			Vector3 newPlayerDirection = m_inversePortalTransform.InverseTransformVector (PlayerPOV.Singleton.GetPlayerDirection());
			//			newPlayerDirection = m_otherPortalTransform.TransformVector (newPlayerDirection);

			PlayerPOV.Singleton.SetPlayerDirection (newPlayerRotation * Vector3.forward);
			PlayerPOV.Singleton.CharacterController.SimpleMove (impulse * Time.deltaTime);

			playerOverlapping = false;

		}
	}

	#endregion

	#region Public methods

	public void AdjustLevelForThisPortal()
	{		
		Quaternion inverseRotation = Quaternion.Inverse (m_otherPortalTransform.rotation) * PlayerPOV.Singleton.RealLevelTransform.rotation;
		Vector3 inversePosition = m_otherPortalTransform.InverseTransformPoint (PlayerPOV.Singleton.RealLevelTransform.position);

		m_levelForThisPortal.position = m_inversePortalTransform.TransformPoint (inversePosition);
		m_levelForThisPortal.rotation = m_inversePortalTransform.rotation * inverseRotation;
	}

	#endregion


}
