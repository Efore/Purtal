using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PortalBehaviour : MonoBehaviourExt {

	[SerializeField]
	private PortalBehaviour m_otherPortalBehaviour = null;
	[SerializeField]
	private Transform m_levelForThisPortal = null;
	[SerializeField]
	private BoxCollider m_portalLimit = null;
	[SerializeField]
	private Transform m_inversePortalTransform = null;
	[SerializeField]
	private Animator m_animator = null;
	[SerializeField]
	private ParticleSystem m_interiorParticleSystem = null;
	[SerializeField]
	private GameObject m_portalMeshContainer = null;


	public bool PortalPlaced
	{
		get;
		private set;
	}

	public BoxCollider BoxCollider
	{
		get { return m_portalLimit; }
	}

	public Transform LevelForThisPortal
	{
		get {
			return m_levelForThisPortal; 
		}
	}

	public PortalBehaviour OtherPortalBehaviour
	{
		get {
			return m_otherPortalBehaviour;
		}
	}

	public Transform InverseTransform
	{
		get{
			return m_inversePortalTransform;
		}
	}

	public ParticleSystem InteriorParticleSystem
	{
		get {
			return m_interiorParticleSystem;
		}
	}
		
	public GameObject PortalMeshContainer
	{
		get { 
			return m_portalMeshContainer;
		}
	}

	#region Monobehaviour calls

	void OnTriggerEnter(Collider other)
	{
		if (!PortalsManager.Singleton.ElementsToTeleport.ContainsKey (other.transform.GetComponent<Rigidbody> ())) 
		{
			PortalsManager.Singleton.ElementsToTeleport.Add (other.transform.GetComponent<Rigidbody> (), this);	
			if (other.tag == "Player")
				TeleportPlayer ();
			else 
			{
				if (other.transform.GetComponent<PickableObject> ().IsPicked)
					TeleportPickedObject(other.transform.GetComponent<PickableObject> (),true);
				else
					TeleportThrowedObject (other.transform.GetComponent<PickableObject> ());	
			}
		} 
	}

	void OnTriggerExit(Collider other)
	{
		if (PortalsManager.Singleton.ElementsToTeleport.ContainsKey(other.transform.GetComponent<Rigidbody>()))
			PortalsManager.Singleton.ElementsToTeleport.Remove (other.transform.GetComponent<Rigidbody>());	

		PickableObject otherPickableObject = other.transform.GetComponent<PickableObject> ();
		if (otherPickableObject != null && otherPickableObject.IsPicked)
			TeleportPickedObject (otherPickableObject, false);		
	}

	#endregion

	#region Private methods

	private void TeleportPlayer()
	{		
		Vector3 newPosition = m_otherPortalBehaviour.TransformCached.position;

		float yLocal = m_transformCached.InverseTransformPoint (PlayerPOV.Singleton.transform.position).y;
		yLocal = m_otherPortalBehaviour.TransformCached.TransformPoint(new Vector3(0.0f,yLocal, 0.0f)).y - m_otherPortalBehaviour.TransformCached.position.y;
		newPosition += new Vector3 (0.0f, yLocal, 0.0f);

		Vector3 impulse = m_otherPortalBehaviour.TransformCached.TransformVector(PlayerPOV.Singleton.transform.InverseTransformVector(PlayerPOV.Singleton.CharacterController.velocity));
		PlayerPOV.Singleton.transform.position = newPosition;

		Quaternion newPlayerRotation = Quaternion.Inverse (m_inversePortalTransform.rotation) * PlayerPOV.Singleton.GetPlayerRotation();
		newPlayerRotation = m_otherPortalBehaviour.TransformCached.rotation * newPlayerRotation;

		PlayerPOV.Singleton.SetPlayerDirection (newPlayerRotation * Vector3.forward);
		PlayerPOV.Singleton.CharacterController.SimpleMove (impulse);

		if (PlayerPOV.Singleton.PickObjectBehaviour.HasObjectPicked)
			PlayerPOV.Singleton.PickObjectBehaviour.SwitchObject ();
	}

	private void TeleportThrowedObject(PickableObject pickableObject)
	{			
		if (!pickableObject.CanBeTeleported)
			return;
		
		pickableObject.gameObject.SetActive (false);

		Vector3 impulse = m_otherPortalBehaviour.TransformCached.TransformVector (m_inversePortalTransform.InverseTransformVector (pickableObject.VelocityTracker.LastVelocity));
		Vector3 angularVelocity = m_otherPortalBehaviour.TransformCached.TransformVector (m_inversePortalTransform.InverseTransformVector (pickableObject.VelocityTracker.LastAngularVelocity));

		pickableObject.Clone.VelocityTracker.Rigidbody.isKinematic = true;
		Vector3 newPosition = m_otherPortalBehaviour.TransformCached.position;

		float yLocal = m_transformCached.InverseTransformPoint (pickableObject.transform.position).y;
		yLocal = m_otherPortalBehaviour.TransformCached.TransformPoint (new Vector3 (0.0f, yLocal, 0.0f)).y - m_otherPortalBehaviour.TransformCached.position.y;
		newPosition += new Vector3 (0.0f, yLocal, 0.0f);

		Quaternion newRotation = Quaternion.Inverse (m_inversePortalTransform.rotation) * pickableObject.VelocityTracker.Rigidbody.rotation;
		newRotation = m_otherPortalBehaviour.TransformCached.rotation * newRotation;

		pickableObject.TransformCached.position = Vector3.one * -1000000;

		pickableObject.Clone.transform.position = newPosition + m_otherPortalBehaviour.TransformCached.forward;
		pickableObject.Clone.transform.rotation = newRotation;

		pickableObject.Clone.gameObject.SetActive (true);

		pickableObject.Clone.VelocityTracker.Rigidbody.isKinematic = false;
		pickableObject.Clone.VelocityTracker.Rigidbody.velocity = impulse;
		pickableObject.Clone.VelocityTracker.Rigidbody.angularVelocity = angularVelocity;

	}

	private void TeleportPickedObject(PickableObject pickableObject, bool enter)
	{
		if (!pickableObject.CanBeTeleported)
			return;

		pickableObject.InPortalTrigger = enter;

		if (enter) 
		{
			pickableObject.Clone.gameObject.SetActive (true);
			pickableObject.Clone.VelocityTracker.Rigidbody.isKinematic = true;
			pickableObject.Clone.CanBeTeleported = false;
			pickableObject.Clone.PortalBehaviourForLocalPositioning = m_otherPortalBehaviour.GetComponent<PortalBehaviour> ();
		} 
		else 
		{
			pickableObject.Clone.gameObject.SetActive (false);
			pickableObject.Clone.VelocityTracker.Rigidbody.isKinematic = false;
			pickableObject.Clone.TransformCached.position = Vector3.one * -1000000;
			pickableObject.Clone.CanBeTeleported = true;
			pickableObject.Clone.PortalBehaviourForLocalPositioning = null;
		}
	}

	#endregion

	#region Public methods

	public void ChangePosition(Vector3 position, Vector3 direction)
	{
		if (!PortalPlaced)
		{
			PortalPlaced = true;
		
			if (m_otherPortalBehaviour.PortalPlaced)
			{
				m_otherPortalBehaviour.InteriorParticleSystem.gameObject.SetActive (false);
				m_otherPortalBehaviour.PortalMeshContainer.SetActive (true);
				InteriorParticleSystem.gameObject.SetActive (false);
				PortalMeshContainer.SetActive (true);
			}
		}
				
		m_transformCached.position = position;
		m_transformCached.forward = direction;
		m_animator.Rebind ();
	}

	public void AdjustLevelForThisPortal()
	{	
		Quaternion inverseRotation = Quaternion.Inverse (m_otherPortalBehaviour.TransformCached.rotation) * PlayerPOV.Singleton.RealLevelTransform.rotation;
		Vector3 inversePosition = m_otherPortalBehaviour.TransformCached.InverseTransformPoint (PlayerPOV.Singleton.RealLevelTransform.position);

		m_levelForThisPortal.position = m_inversePortalTransform.TransformPoint (inversePosition);
		m_levelForThisPortal.rotation = m_inversePortalTransform.rotation * inverseRotation;
	}

	#endregion


}
