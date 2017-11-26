﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PortalBehaviour : MonoBehaviourExt {

	[SerializeField]
	private Transform m_otherPortalTransform = null;
	[SerializeField]
	private Transform m_levelForThisPortal = null;
	[SerializeField]
	private BoxCollider m_portalLimit = null;
	[SerializeField]
	private Transform m_inversePortalTransform = null;
	[SerializeField]
	private Animator m_animator = null;

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

	#region Monobehaviour calls

	void OnTriggerEnter(Collider other)
	{
		if (!PortalsManager.Singleton.ElementsToTeleport.ContainsKey(other.transform.GetComponent<Rigidbody>()))
		{
			PortalsManager.Singleton.ElementsToTeleport.Add (other.transform.GetComponent<Rigidbody>(), this);
			if(other.tag == "Player")
				TeleportPlayer ();
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (PortalsManager.Singleton.ElementsToTeleport.ContainsKey(other.transform.GetComponent<Rigidbody>()) && PortalsManager.Singleton.ElementsToTeleport [other.transform.GetComponent<Rigidbody>()] != this)
		{
			PortalsManager.Singleton.ElementsToTeleport.Remove (other.transform.GetComponent<Rigidbody>());
		}
	}

	#endregion

	#region Private methods

	private void TeleportPlayer()
	{		
		// transport him to the equivalent position in the other portal
		var newPosition = m_otherPortalTransform.position;
		Vector3 impulse = m_otherPortalTransform.TransformVector(PlayerPOV.Singleton.transform.InverseTransformVector(PlayerPOV.Singleton.CharacterController.velocity));
		PlayerPOV.Singleton.transform.position = newPosition;

		Quaternion newPlayerRotation = Quaternion.Inverse (m_inversePortalTransform.rotation) * PlayerPOV.Singleton.GetPlayerRotation();
		newPlayerRotation = m_otherPortalTransform.rotation * newPlayerRotation;

		PlayerPOV.Singleton.SetPlayerDirection (newPlayerRotation * Vector3.forward);
		//PlayerPOV.Singleton.CharacterController.SimpleMove (impulse * Time.deltaTime);
	}

	#endregion

	#region Public methods

	public void ChangePosition(Vector3 position, Vector3 direction)
	{
		m_transformCached.position = position;
		m_transformCached.forward = direction;
		m_animator.Rebind ();
	}

	public void AdjustLevelForThisPortal()
	{	
		Quaternion inverseRotation = Quaternion.Inverse (m_otherPortalTransform.rotation) * PlayerPOV.Singleton.RealLevelTransform.rotation;
		Vector3 inversePosition = m_otherPortalTransform.InverseTransformPoint (PlayerPOV.Singleton.RealLevelTransform.position);

		m_levelForThisPortal.position = m_inversePortalTransform.TransformPoint (inversePosition);
		m_levelForThisPortal.rotation = m_inversePortalTransform.rotation * inverseRotation;
	}

	#endregion


}
