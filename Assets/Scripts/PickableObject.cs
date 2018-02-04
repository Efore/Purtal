/**************************************************************************\
Module Name:  PicklableObject.cs
Project:      Purtal

This class defines the concept of a PickableObject (like companion cubes) and 
manages its behaviour.

\***************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickableObject : MonoBehaviourExt
{
	#region Singleton

	#endregion

	#region Private members
	[SerializeField]
	private PickableObject m_clone = null;
	[SerializeField]
	private VelocityTracker m_velocityTracker = null;

	#endregion

	#region Public members

	#endregion

	#region Properties

	public PickableObject Clone {
		get { return m_clone; }
	}

	public VelocityTracker VelocityTracker {
		get { return m_velocityTracker; }
	}

	public bool CanBeTeleported {
		get;
		set;
	}

	public bool IsPicked {
		get;
		set;
	}

	public bool InPortalTrigger
	{
		get;
		set;
	}

	public PortalBehaviour PortalBehaviourForLocalPositioning {
		get;
		set;
	}

	#endregion

	#region Events

	#endregion

	#region MonoBehaviour calls

	protected override void Awake ()
	{
		base.Awake ();
		CanBeTeleported = true;
		if(m_velocityTracker == null)
			m_velocityTracker = GetComponent<VelocityTracker> ();
	}

	void Update()
	{
		//For simulating the movement of a picked Pickable Object through portals, we use two instances of that Pickable Object, but only one of them is shown at the same time.
		//The one that is invisible (and unpicked), will be visible and will be moved when the picked one pass through the portal without being thrown.
		//If a portal is assigned, we use the position and direction of the picked Pickable Object in the local space of the portal that is passing through, and pour it on this one.
		if (PortalBehaviourForLocalPositioning != null) 
		{
			Vector3 localPositionOfClone = PortalBehaviourForLocalPositioning.OtherPortalBehaviour.InverseTransform.InverseTransformPoint (Clone.TransformCached.position);
			Vector3 localDirectionOfClone = PortalBehaviourForLocalPositioning.OtherPortalBehaviour.InverseTransform.InverseTransformDirection (Clone.TransformCached.forward);

			Vector3 newPosition = PortalBehaviourForLocalPositioning.TransformCached.TransformPoint (localPositionOfClone);
			Vector3 newDirection =  PortalBehaviourForLocalPositioning.TransformCached.TransformDirection (localDirectionOfClone);

			TransformCached.position = newPosition;
			TransformCached.forward = newDirection;
		}
	}

	#endregion

	#region Private methods

	#endregion

	#region Public methods

	/// <summary>
	/// Settings to apply when the picked Pickable Object passing through the portal is dropped
	/// </summary>
	public void DroppedObjectInPortal()
	{
		InPortalTrigger = false;
		Clone.PortalBehaviourForLocalPositioning = null;

		TransformCached.position = Vector3.one * -1000000;

		Clone.gameObject.SetActive (true);
		Clone.VelocityTracker.Rigidbody.isKinematic = false;
		Clone.CanBeTeleported = true;
	}

	#endregion

}


