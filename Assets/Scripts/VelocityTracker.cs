using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VelocityTracker : MonoBehaviourExt
{
	
	#region Singleton

	#endregion

	#region Private members

	public Rigidbody Rigidbody 
	{
		get;
		set;
	}

	public Vector3 LastVelocity 
	{
		get;
		set;
	}

	public Vector3 LastAngularVelocity 
	{
		get;
		set;
	}

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
		Rigidbody = GetComponent<Rigidbody> ();
	}

	void Update()
	{
		LastVelocity = Rigidbody.velocity;
		LastAngularVelocity = Rigidbody.angularVelocity;
	}

	#endregion

	#region Private methods

	#endregion

	#region Public methods

	#endregion

}


