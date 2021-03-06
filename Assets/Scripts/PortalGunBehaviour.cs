﻿/**************************************************************************\
Module Name:  PortalGunBehaviour.cs
Project:      Purtal

This class manages the behaviour of the portal gun which consists on opening portals
after calculating its correct position.

\***************************************************************************/

using UnityEngine;
using System.Collections;

public class PortalGunBehaviour : MonoBehaviourExt
{
	#region Private members

	[SerializeField]
	private PortalBehaviour m_portalA = null;

	[SerializeField]
	private PortalBehaviour m_portalB = null;

	private Vector3 raycastDownDirection;
	private Vector3 raycastUpDirection;
	private Vector3 raycastLeftDirection;
	private Vector3 raycastRightDirection;

	private Vector3 raycastDownLeftDirection;
	private Vector3 raycastDownRightDirection;
	private Vector3 raycastUpLeftDirection;
	private Vector3 raycastUpRightDirection;

	private bool foundUp = false;
	private bool foundDown = false;
	private bool foundLeft = false;
	private bool foundRight = false;

	private Vector3 raycastOrigin;

	float horizontalPortalLimitSize = 0.0f;
	float verticalPortalLimitSize = 0.0f;
	float diagonalPortalLimitSize = 0.0f;

	#endregion

	#region Public members

	#endregion

	#region Properties

	#endregion

	#region Events

	#endregion

	#region MonoBehaviour calls

	protected override void Awake ()
	{
		base.Awake ();
		horizontalPortalLimitSize = m_portalA.BoxCollider.size.x / 2 * m_portalA.transform.localScale.x;
		verticalPortalLimitSize = m_portalA.BoxCollider.size.y / 2 * m_portalA.transform.localScale.y;
		diagonalPortalLimitSize = Mathf.Sqrt (horizontalPortalLimitSize * horizontalPortalLimitSize + verticalPortalLimitSize * verticalPortalLimitSize);
	}

	void Update()
	{
		//Each mouse's button will shoot a different portal
		if (Input.GetMouseButtonDown (0))
			ShootPortal (m_portalA, m_portalB);
		else if (Input.GetMouseButtonDown (1))
			ShootPortal (m_portalB, m_portalA); 
	}

	#if UNITY_EDITOR

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine (raycastOrigin, raycastOrigin + raycastUpDirection * verticalPortalLimitSize);
		Gizmos.color = Color.grey;
		Gizmos.DrawLine (raycastOrigin, raycastOrigin + raycastDownDirection * verticalPortalLimitSize);
		Gizmos.color = Color.grey;
		Gizmos.DrawLine (raycastOrigin, raycastOrigin + raycastLeftDirection * horizontalPortalLimitSize);
		Gizmos.color = Color.red;
		Gizmos.DrawLine (raycastOrigin, raycastOrigin + raycastRightDirection * horizontalPortalLimitSize);
		Gizmos.color = Color.black;
		Gizmos.DrawLine (raycastOrigin, raycastOrigin + raycastUpRightDirection * diagonalPortalLimitSize);
		Gizmos.color = Color.grey;
		Gizmos.DrawLine (raycastOrigin, raycastOrigin + raycastUpLeftDirection * diagonalPortalLimitSize);
		Gizmos.color = Color.grey;
		Gizmos.DrawLine (raycastOrigin, raycastOrigin + raycastDownLeftDirection * diagonalPortalLimitSize);
		Gizmos.color = Color.grey;
		Gizmos.DrawLine (raycastOrigin, raycastOrigin + raycastDownRightDirection * diagonalPortalLimitSize);
	}

	#endif

	#endregion

	#region Private methods

	private void ShootPortal(PortalBehaviour portal, PortalBehaviour otherPortal)
	{
		RaycastHit hit;

		if (Physics.Raycast (m_transformCached.position, m_transformCached.forward, out hit, 1000.0f, 1 << LayerMask.NameToLayer ("PortalLimit")))
		{
			if(hit.collider.transform.parent != portal.transform)
				return;	
		}

		if (!Physics.Raycast (m_transformCached.position, m_transformCached.forward, out hit, 1000.0f, 1 << LayerMask.NameToLayer ("AllowPortal")))
			return;

		PlacePortal (portal, otherPortal, hit.point, hit.normal);
	}

	private void PlacePortal(PortalBehaviour portalToPlace, PortalBehaviour otherPortal, Vector3 position, Vector3 direction)
	{
		Vector3 futurePos = position;
		//If the portal fits in the destined position or anywhere close, 
		//the portal opens in that position and the virtual level of the othe portal is adjusted according to that
		if (PortalFits (portalToPlace, ref futurePos, direction))
		{
			portalToPlace.ChangePosition (futurePos , direction);
			portalToPlace.AdjustLevelForThisPortal ();
			otherPortal.AdjustLevelForThisPortal ();
		}
	}

	private bool PortalFits(PortalBehaviour portal, ref Vector3 position, Vector3 direction)
	{	
		//We create 8 rays, one in each basic direction.
		if (direction == Vector3.up)
		{
			raycastLeftDirection = Vector3.Cross (Vector3.forward, direction).normalized;		 

		}
		else if (direction == Vector3.down)
		{
			raycastLeftDirection = Vector3.Cross (-Vector3.forward, direction).normalized;	
		}
		else
		{
			raycastLeftDirection = Vector3.Cross (Vector3.down, direction).normalized;	
		}

		raycastRightDirection = -raycastLeftDirection;
		raycastDownDirection = Vector3.Cross (raycastRightDirection, direction).normalized;	
		raycastUpDirection = -raycastDownDirection;

		raycastDownLeftDirection = (raycastDownDirection + raycastLeftDirection).normalized;
		raycastDownRightDirection = (raycastDownDirection + raycastRightDirection).normalized;

		raycastUpLeftDirection = (raycastUpDirection + raycastLeftDirection).normalized;
		raycastUpRightDirection = (raycastUpDirection + raycastRightDirection).normalized;

		raycastOrigin = position + direction * 0.1f;

		//The rays will check if they collide with another portal, or with a wall.
		int checkLayer = 1 << LayerMask.NameToLayer ("PortalLimit") | 1 << LayerMask.NameToLayer ("AllowPortal") | 1 << LayerMask.NameToLayer ("Wall");

		//According to this collisions, the portal may get moved to a new position.
		NewPortalPositionAfterRaycasts (portal, ref position,checkLayer);

		//We check if the new position collides again with other portal and/or wall.
		Collider[] collidersAfterReposition = Physics.OverlapBox (position + direction * 0.1f, new Vector3 (horizontalPortalLimitSize - 0.01f, verticalPortalLimitSize - 0.01f, 0.01f), Quaternion.LookRotation (direction), checkLayer);

		//Checking that the portal's fitting checker is not colliding with itself
		if(collidersAfterReposition.Length > 1 || (collidersAfterReposition.Length == 1 && collidersAfterReposition[0].transform.parent != portal.transform))				
			return false;

		//If the new position does not collides with anything, the portal fits in that position
		return true;
	}

	private void NewPortalPositionAfterRaycasts(PortalBehaviour portal, ref Vector3 position, int checkLayer)
	{		
		//Again, several rays are cast to move the portal to a new  possibly fitting position
		foundUp = false;
		foundDown = false;
		foundLeft = false;
		foundRight = false;

		RaycastHit hit;
		if (Physics.Raycast (raycastOrigin, raycastLeftDirection, out hit, horizontalPortalLimitSize, checkLayer))
		{
			if (hit.collider.transform.parent != portal.transform)
			{
				position -= raycastLeftDirection * (horizontalPortalLimitSize - hit.distance);
				foundLeft = true;
			}
		}
		if (Physics.Raycast (raycastOrigin, raycastRightDirection, out hit, horizontalPortalLimitSize, checkLayer))
		{
			if (hit.collider.transform.parent != portal.transform)
			{
				position -= raycastRightDirection * (horizontalPortalLimitSize - hit.distance);
				foundRight = true;
			}
		}
		if (Physics.Raycast (raycastOrigin, raycastUpDirection, out hit, verticalPortalLimitSize, checkLayer))
		{
			if (hit.collider.transform.parent != portal.transform)
			{
				position -= raycastUpDirection * (verticalPortalLimitSize - hit.distance);
				foundUp = true;
			}
		}
		if (Physics.Raycast (raycastOrigin, raycastDownDirection, out hit, verticalPortalLimitSize, checkLayer))
		{
			if (hit.collider.transform.parent != portal.transform)
			{
				position -= raycastDownDirection * (verticalPortalLimitSize - hit.distance);
				foundDown = true;
			}
		}

		//Diagonal raycast only occur if its respective directions' raycast haven't found anything
		if (!foundDown && !foundLeft && Physics.Raycast (raycastOrigin, raycastDownLeftDirection, out hit, diagonalPortalLimitSize, checkLayer))
		{
			if(hit.collider.transform.parent != portal.transform)
				position -= raycastDownLeftDirection * (diagonalPortalLimitSize - hit.distance);
		}
		if (!foundDown && !foundRight && Physics.Raycast (raycastOrigin, raycastDownRightDirection, out hit, diagonalPortalLimitSize, checkLayer))
		{
			if(hit.collider.transform.parent != portal.transform)
				position -= raycastDownRightDirection * (diagonalPortalLimitSize - hit.distance);
		}
		if (!foundUp && !foundLeft && Physics.Raycast (raycastOrigin, raycastUpLeftDirection, out hit, diagonalPortalLimitSize, checkLayer))
		{
			if(hit.collider.transform.parent != portal.transform)
				position -= raycastUpLeftDirection * (diagonalPortalLimitSize - hit.distance);
		}
		if (!foundUp && !foundRight && Physics.Raycast (raycastOrigin, raycastUpRightDirection, out hit, diagonalPortalLimitSize, checkLayer))
		{
			if(hit.collider.transform.parent != portal.transform)
				position -= raycastUpRightDirection * (diagonalPortalLimitSize - hit.distance);
		}
	}

	#endregion

	#region Public methods

	#endregion

}
