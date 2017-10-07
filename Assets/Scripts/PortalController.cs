using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PortalController : MonoBehaviour {

	[SerializeField]
	private PortalCameraBehaviour m_otherPortalCamera = null;
	[SerializeField]
	private BoxCollider m_portalLimit = null;

	private bool playerOverlapping = false;

	public BoxCollider BoxCollider
	{
		get { return m_portalLimit; }
	}

	public PortalCameraBehaviour OtherPortalCamera
	{
		get {
			return m_otherPortalCamera; 
		}
	}

	#region Monobehaviour calls

	void Start () {
	}

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

	#region Private members

	private void TeleportBehaviour()
	{
		if (playerOverlapping) {

			// transport him to the equivalent position in the other portal
			var newPosition = m_otherPortalCamera.ThisPortal.position;
			PlayerPOV.Singleton.transform.position = newPosition;
			PlayerPOV.Singleton.SetPlayerDirection (m_otherPortalCamera.transform.forward);
			playerOverlapping = false;

		}
	}

	#endregion
}
