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

	void Update()
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
			PlayerPOV.Singleton.transform.position = newPosition;
			PlayerPOV.Singleton.SetPlayerDirection (m_otherPortalTransform.forward);
			playerOverlapping = false;

		}
	}

	#endregion

	#region Public methods

	public void AdjustLevelForThisPortal()
	{
		Vector3 inverseDirection = m_transformCached.InverseTransformDirection (PlayerPOV.Singleton.RealLevelTransform.forward);
		Vector3 inversePosition = m_transformCached.InverseTransformPoint (PlayerPOV.Singleton.RealLevelTransform.position);



	}

	#endregion


}
