using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Physics Event Hook class lets you easily add remote physics event listener functions to an object.
/// Example usage: PhysicsListener.Get(gameObject, tag).onTriggerEnter += MyTriggerEnterFunction;
/// </summary>
public class PhysicsListener : MonoBehaviour {

	public string listenerTag;

	public Action<GameObject, Collider> onTriggerEnter;
	public Action<GameObject, Collider> onTriggerStay;
	public Action<GameObject, Collider> onTriggerExit;
	public Action<GameObject, Collision> onCollisionEnter;
	public Action<GameObject, Collision> onCollisionStay;
	public Action<GameObject, Collision> onCollisionExit;

	private GameObject mGameObject;

	void Awake()
	{
		mGameObject = gameObject;
	}

	void OnTriggerEnter(Collider other)
	{
		if (onTriggerEnter != null)
		{
			onTriggerEnter(mGameObject, other);
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (onTriggerStay != null)
		{
			onTriggerStay(mGameObject, other);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (onTriggerExit != null)
		{
			onTriggerExit(mGameObject, other);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (onCollisionEnter != null)
		{
			onCollisionEnter(mGameObject, collision);
		}
	}

	void OnCollisionStay(Collision collision)
	{
		if (onCollisionStay != null)
		{
			onCollisionStay(mGameObject, collision);
		}
	}

	void OnCollisionExit(Collision collision)
	{
		if (onCollisionExit != null)
		{
			onCollisionExit(mGameObject, collision);
		}
	}

	private static PhysicsListener FindListener(PhysicsListener[] listeners, string listenerTag)
	{
		foreach (PhysicsListener listener in listeners)
		{
			if (string.Equals(listener.listenerTag, listenerTag))
			{
				return listener;
			}
		}
		return null;
	}

	public static PhysicsListener Get(GameObject go, string listenerTag = null)
	{
		PhysicsListener[] listeners = go.GetComponents<PhysicsListener>();
		PhysicsListener listener = FindListener(listeners, listenerTag);
		if (listener == null)
		{
			listener = go.AddComponent<PhysicsListener>();
			listener.listenerTag = listenerTag;
		}
		return listener;
	}

	public static PhysicsListener Get(Component comp, string listenerTag = null)
	{
		return Get(comp.gameObject, listenerTag);
	}
}
