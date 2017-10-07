using UnityEngine;
using System.Collections;

public class PortalGun : MonoBehaviourExt
{
	#region Private members
	[SerializeField]
	private PortalController m_portalA = null;
	[SerializeField]
	private PortalController m_portalB = null;


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
		if (Input.GetMouseButtonDown (0))
			ShootPortal (m_portalA);
		else if (Input.GetMouseButtonDown (1))
			ShootPortal (m_portalB); 
	}


	#endregion

	#region Private methods

	private void ShootPortal(PortalController portal)
	{
		RaycastHit hit;

		if (Physics.Raycast (m_transformCached.position, m_transformCached.forward, out hit, 1000.0f, 1 << LayerMask.NameToLayer ("PortalLimit")))
			return;	

		if (!Physics.Raycast (m_transformCached.position, m_transformCached.forward, out hit, 1000.0f, 1 << LayerMask.NameToLayer ("AllowPortal")))
			return;

		PlacePortal (portal, hit.point, hit.normal);
	}

	private void PlacePortal(PortalController portal, Vector3 position, Vector3 direction)
	{
		Vector3 futurePos = position;
		if (PortalFits (portal, ref futurePos, direction))
		{
			portal.transform.position = futurePos + direction * 0.01f;
			portal.transform.forward = direction;
			portal.OtherPortalCamera.ChangePosition ();
		}
	}

	private bool PortalFits(PortalController portal, ref Vector3 position, Vector3 direction)
	{		
		Vector3 checkRayOrigin = position + direction * 0.1f;
		float horizontalPortalLimitSize = portal.BoxCollider.size.x / 2 * portal.transform.localScale.x;
		float verticalPortalLimitSize = portal.BoxCollider.size.y / 2 * portal.transform.localScale.y;

		int checkLayer = 1 << LayerMask.NameToLayer ("PortalLimit") | 1 << LayerMask.NameToLayer ("AllowPortal");


		return true;
	}

	#endregion

	#region Public methods

	#endregion

}
