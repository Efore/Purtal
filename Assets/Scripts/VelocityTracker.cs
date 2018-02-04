/**************************************************************************\
Module Name:  VelocityTracker.cs
Project:      Purtal

This class keeps track of the last frame's Linear Velocity and Angular Velocity of a Rigid Body

\***************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VelocityTracker : MonoBehaviourExt
{
	
	#region Singleton

	#endregion

	#region Private members

	[SerializeField]
	private Rigidbody m_rigidBody = null;

	#endregion

	#region Public members

	#endregion

	#region Properties


	public Rigidbody Rigidbody 
	{
		get {
			return m_rigidBody;
		}
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

	#region Events

	#endregion

	#region MonoBehaviour calls

	protected override void Awake ()
	{
		base.Awake ();
		if(m_rigidBody == null)
			m_rigidBody = GetComponent<Rigidbody> ();
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


