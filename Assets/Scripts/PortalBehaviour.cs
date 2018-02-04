/**************************************************************************\
Module Name:  PortalBehaviour.cs
Project:      Purtal

This class manages the full behaviour of the portal: from the teleportation of entities to the
transforming of the virtual level. 
(Virtual levels are copies of the current level used for simulate Portal's effect).
\***************************************************************************/

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
		//When the player or a pickable object enter the collider of this portal, we manage its teleportation.
		//They are treated differently.
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
		//Once the player enters the portal's trigger, we calculate its future position.
		Vector3 newPosition = m_otherPortalBehaviour.TransformCached.position;
		float yLocal = m_transformCached.InverseTransformPoint (PlayerPOV.Singleton.transform.position).y;
		yLocal = m_otherPortalBehaviour.TransformCached.TransformPoint(new Vector3(0.0f,yLocal, 0.0f)).y - m_otherPortalBehaviour.TransformCached.position.y;
		newPosition += new Vector3 (0.0f, yLocal, 0.0f);

		//We calculate the impulse as the local velocity of the player just as they enter the trigger.
		Vector3 impulse = m_otherPortalBehaviour.TransformCached.TransformVector(PlayerPOV.Singleton.transform.InverseTransformVector(PlayerPOV.Singleton.CharacterController.velocity));

		//Whatever rotation the player had just before entering the trigger, it is calculated in the local space of the 
		//inverse forward of this portal. This rotation is added to the player after changing its position.
		Quaternion newPlayerRotation = Quaternion.Inverse (m_inversePortalTransform.rotation) * PlayerPOV.Singleton.GetPlayerRotation();
		newPlayerRotation = m_otherPortalBehaviour.TransformCached.rotation * newPlayerRotation;

		//Applying calculations
		PlayerPOV.Singleton.transform.position = newPosition;
		PlayerPOV.Singleton.SetPlayerDirection (newPlayerRotation * Vector3.forward);
		PlayerPOV.Singleton.CharacterController.SimpleMove (impulse);

		//If the player was grabbing any object, we switch it for its deactivated clone
		if (PlayerPOV.Singleton.PickObjectBehaviour.HasObjectPicked)
			PlayerPOV.Singleton.PickObjectBehaviour.SwitchObject ();
	}

	private void TeleportThrowedObject(PickableObject pickableObject)
	{			
		if (!pickableObject.CanBeTeleported)
			return;

		//For an increased realism in the perception of the teleportation of a throwed object, we will use its clone.

		//We calculate the current velocity and angular velocity of the entering object in the local space of the inverse 
		//forward of this portal.
		Vector3 impulse = m_otherPortalBehaviour.TransformCached.TransformVector (m_inversePortalTransform.InverseTransformVector (pickableObject.VelocityTracker.LastVelocity));
		Vector3 angularVelocity = m_otherPortalBehaviour.TransformCached.TransformVector (m_inversePortalTransform.InverseTransformVector (pickableObject.VelocityTracker.LastAngularVelocity));

		//The future position for the entering object's clone is calculated
		Vector3 newPosition = m_otherPortalBehaviour.TransformCached.position;
		float yLocal = m_transformCached.InverseTransformPoint (pickableObject.transform.position).y;
		yLocal = m_otherPortalBehaviour.TransformCached.TransformPoint (new Vector3 (0.0f, yLocal, 0.0f)).y - m_otherPortalBehaviour.TransformCached.position.y;
		newPosition += new Vector3 (0.0f, yLocal, 0.0f);

		//Also, the future rotation
		Quaternion newRotation = Quaternion.Inverse (m_inversePortalTransform.rotation) * pickableObject.VelocityTracker.Rigidbody.rotation;
		newRotation = m_otherPortalBehaviour.TransformCached.rotation * newRotation;

		//We move the clone to the calculated position and we modify its rotation
		pickableObject.Clone.VelocityTracker.Rigidbody.isKinematic = true;
		pickableObject.Clone.transform.position = newPosition + m_otherPortalBehaviour.TransformCached.forward;
		pickableObject.Clone.transform.rotation = newRotation;

		//The entering object is deactivated and moved away
		pickableObject.gameObject.SetActive (false);
		pickableObject.TransformCached.position = Vector3.one * -1000000;

		//The clone is activated and the physic calculations are applied
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

		//Several parameters of the clone are modified. The PickableObject class will handle this behaviour by itself
		//according to this settings.
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

	/// <summary>
	/// Changes the position and direction of the portal.
	/// </summary>
	/// <param name="position">New position.</param>
	/// <param name="direction">New direction.</param>
	public void ChangePosition(Vector3 position, Vector3 direction)
	{		
		if (!PortalPlaced)
		{
			PortalPlaced = true;

			//If the other portal has been already placed, this portal shows its virtual level
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

	/// <summary>
	/// Adjusts the position and rotation of the virtual level for this portal.
	/// </summary>
	public void AdjustLevelForThisPortal()
	{	
		Quaternion inverseRotation = Quaternion.Inverse (m_otherPortalBehaviour.TransformCached.rotation) * PlayerPOV.Singleton.RealLevelTransform.rotation;
		Vector3 inversePosition = m_otherPortalBehaviour.TransformCached.InverseTransformPoint (PlayerPOV.Singleton.RealLevelTransform.position);

		m_levelForThisPortal.position = m_inversePortalTransform.TransformPoint (inversePosition);
		m_levelForThisPortal.rotation = m_inversePortalTransform.rotation * inverseRotation;
	}

	#endregion


}
