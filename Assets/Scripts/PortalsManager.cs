﻿/**************************************************************************\
Module Name:  PortalsManager.cs
Project:      Purtal

Simple singleton to keep the score of entities passing through the portals

\***************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalsManager : MonoBehaviour
{
	#region Singleton

	private static PortalsManager m_instance = null;

	public static PortalsManager Singleton
	{
		get { return m_instance; }
	}

	#endregion

	#region Private members
	#endregion

	#region Public members

	#endregion

	#region Properties

	public Dictionary<Rigidbody,PortalBehaviour> ElementsToTeleport
	{
		get;
		set;
	}

	#endregion

	#region Events

	#endregion

	#region MonoBehaviour calls

	void Awake ()
	{
		m_instance = this;
		ElementsToTeleport = new Dictionary<Rigidbody,PortalBehaviour> ();
	}

	#endregion

	#region Private methods

	#endregion

	#region Public methods

	#endregion

}


