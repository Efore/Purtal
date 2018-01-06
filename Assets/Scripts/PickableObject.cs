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

	public bool IsPicked {
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
		if(m_velocityTracker == null)
			m_velocityTracker = GetComponent<VelocityTracker> ();
	}

	#endregion

	#region Private methods

	#endregion

	#region Public methods

	#endregion

}


