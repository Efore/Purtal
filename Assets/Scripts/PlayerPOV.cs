using UnityEngine;
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

	#endregion

	#region Public members

	#endregion

	#region Properties

	public Camera PlayerCamera
	{
		get { return m_fpsController.Camera; }
	}

	public Vector3 PlayerDirection
	{
		get{ return m_fpsController.ControllerDirection(); }
	}

	public Quaternion PlayerRotation
	{
		get { return m_fpsController.ControllerRotation (); }
	}

	public Transform RealLevelTransform
	{
		get { return m_realLevelTransform; }
	}

	#endregion

	#region Events

	#endregion

	#region MonoBehaviour calls

	void Awake ()
	{
		m_instance = this;
		m_fpsController = GetComponent<FirstPersonController> ();
	}

	#endregion

	#region Private methods

	#endregion

	#region Public methods

	public void SetPlayerDirection(Vector3 direction)
	{
		m_fpsController.RotateController (direction);
	}

	#endregion

}

