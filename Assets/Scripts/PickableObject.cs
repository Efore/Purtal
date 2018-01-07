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

	#endregion

}


