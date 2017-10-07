using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class PortalCameraBehaviour : MonoBehaviourExt
{
	#region Private members

	[SerializeField]
	private Transform m_thisPortal = null;
	[SerializeField]
	private Transform m_otherPortal = null;

	private Quaternion m_difRotation;
	private Camera m_camera = null;
	private float m_initialLocalY = 0;
	private float m_initialFoV = 60;
	#endregion

	#region Public members

	#endregion

	#region Properties

	public Quaternion DifRotation
	{
		get { return m_difRotation; }
	}

	public Transform ThisPortal
	{
		get {
			return m_thisPortal; 
		}
	}

	#endregion

	#region Events

	#endregion

	#region MonoBehaviour calls

	protected override void Awake()
	{
		m_camera = GetComponent<Camera> ();
		m_initialLocalY = transform.localPosition.y;
	}

	void Start()
	{
		ChangePosition ();
	}

	void LateUpdate () {
		ManageCamera ();
	}

	#endregion

	#region Private methods

	private void ManageCamera()
	{		
		//transform.localPosition = PlayerPOV.Singleton.PlayerCamera.transform.localPosition + new Vector3(0.0f,m_initialLocalY,0.0f);
		//m_camera.fieldOfView = m_initialFoV + (Vector3.Distance (PlayerPOV.Singleton.transform.position, m_otherPortal.position) * 3);
		float fovDelta = m_camera.fieldOfView / m_initialFoV;
		transform.forward = (m_otherPortal.rotation * (PlayerPOV.Singleton.PlayerDirection/fovDelta));

		transform.eulerAngles = transform.eulerAngles - (m_difRotation.eulerAngles);
	}


	#endregion

	#region Public methods


	public void ChangePosition()
	{
		m_difRotation = Quaternion.FromToRotation (m_thisPortal.forward, PlayerPOV.Singleton.PlayerDirection);
	}


	#endregion

}
