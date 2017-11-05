﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerPOV : MonoBehaviour
{
	#region Singleton

	private static PlayerPOV m_instance = null;

	public static PlayerPOV Singleton
	{
		get { return m_instance; }
	}

	#endregion

	#region Private members
	[SerializeField]
	private Transform m_realLevelTransform = null;

	private FirstPersonController m_fpsController = null;
	private CharacterController m_characterController = null;

	#endregion

	#region Public members

	#endregion

	#region Properties

	public Camera PlayerCamera
	{
		get { return m_fpsController.Camera; }
	}

	public Transform RealLevelTransform
	{
		get { return m_realLevelTransform; }
	}

	public CharacterController CharacterController
	{
		get { return m_characterController; }
	}



	#endregion

	#region Events

	#endregion

	#region MonoBehaviour calls

	void Awake ()
	{
		m_instance = this;
		m_fpsController = GetComponent<FirstPersonController> ();
		m_characterController = GetComponent<CharacterController> ();
	}

	#endregion

	#region Private methods

	#endregion

	#region Public methods

	public Quaternion GetPlayerRotation()
	{
		return m_fpsController.ControllerRotation ();
	}

	public Vector3 GetPlayerDirection()
	{
		return m_fpsController.ControllerDirection();
	}

	public void SetPlayerDirection(Vector3 direction)
	{
		m_fpsController.RotateController (direction);
	}

	#endregion

}

