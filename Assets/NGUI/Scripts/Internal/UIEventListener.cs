//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using System;
using UnityEngine;

/// <summary>
/// Event Hook class lets you easily add remote event listener functions to an object.
/// Example usage: UIEventListener.Get(gameObject).onClick += MyClickFunction;
/// </summary>

[AddComponentMenu("NGUI/Internal/Event Listener")]
public class UIEventListener : MonoBehaviour
{
	public string listenerTag;

	public object parameter;

	public Action<GameObject, bool> onEnabled;
	public Action<GameObject> onSubmit;
	public Action<GameObject> onClick;
	public Action<GameObject> onDoubleClick;
	public Action<GameObject, bool> onHover;
	public Action<GameObject, bool> onPress;
	public Action<GameObject, bool> onSelect;
	public Action<GameObject, float> onScroll;
	public Action<GameObject> onDragStart;
	public Action<GameObject, Vector2> onDrag;
	public Action<GameObject> onDragOver;
	public Action<GameObject> onDragOut;
	public Action<GameObject> onDragEnd;
	public Action<GameObject, GameObject> onDrop;
	public Action<GameObject, KeyCode> onKey;
	public Action<GameObject, bool> onTooltip;

	bool isColliderEnabled
	{
		get
		{
			Collider c = GetComponent<Collider>();
			if (c != null) return c.enabled;
			Collider2D b = GetComponent<Collider2D>();
			return (b != null && b.enabled);
		}
	}
	
	void OnEnable ()				{ if (onEnabled != null) onEnabled(gameObject, true); }
	void OnDisable ()				{ if (onEnabled != null) onEnabled(gameObject, false); }
	void OnSubmit ()				{ if (isColliderEnabled && onSubmit != null) onSubmit(gameObject); }
	void OnClick ()					{ if (isColliderEnabled && onClick != null) onClick(gameObject); }
	void OnDoubleClick ()			{ if (isColliderEnabled && onDoubleClick != null) onDoubleClick(gameObject); }
	void OnHover (bool isOver)		{ if (isColliderEnabled && onHover != null) onHover(gameObject, isOver); }
	void OnPress (bool isPressed)	{ if (isColliderEnabled && onPress != null) onPress(gameObject, isPressed); }
	void OnSelect (bool selected)	{ if (isColliderEnabled && onSelect != null) onSelect(gameObject, selected); }
	void OnScroll (float delta)		{ if (isColliderEnabled && onScroll != null) onScroll(gameObject, delta); }
	void OnDragStart ()				{ if (onDragStart != null) onDragStart(gameObject); }
	void OnDrag (Vector2 delta)		{ if (onDrag != null) onDrag(gameObject, delta); }
	void OnDragOver ()				{ if (isColliderEnabled && onDragOver != null) onDragOver(gameObject); }
	void OnDragOut ()				{ if (isColliderEnabled && onDragOut != null) onDragOut(gameObject); }
	void OnDragEnd ()				{ if (onDragEnd != null) onDragEnd(gameObject); }
	void OnDrop (GameObject go)		{ if (isColliderEnabled && onDrop != null) onDrop(gameObject, go); }
	void OnKey (KeyCode key)		{ if (isColliderEnabled && onKey != null) onKey(gameObject, key); }
	void OnTooltip (bool show)		{ if (isColliderEnabled && onTooltip != null) onTooltip(gameObject, show); }

	public void Clear ()
	{
		onSubmit = null;
		onClick = null;
		onDoubleClick = null;
		onHover = null;
		onPress = null;
		onSelect = null;
		onScroll = null;
		onDragStart = null;
		onDrag = null;
		onDragOver = null;
		onDragOut = null;
		onDragEnd = null;
		onDrop = null;
		onKey = null;
		onTooltip = null;
	}

	/// <summary>
	/// Get or add an event listener to the specified game object.
	/// </summary>

	static public UIEventListener Get (GameObject go)
	{
		UIEventListener listener = go.GetComponent<UIEventListener>();
		if (listener == null) listener = go.AddComponent<UIEventListener>();
		return listener;
	}

	static public UIEventListener Get(GameObject go, string listenerTag)
	{
		UIEventListener[] listeners = go.GetComponents<UIEventListener>();
		UIEventListener listener = FindListener(listeners, listenerTag);
		if (listener == null)
		{
			listener = go.AddComponent<UIEventListener>();
			listener.listenerTag = listenerTag;
		}
		return listener;
	}

	static private UIEventListener FindListener(UIEventListener[] listeners, string listenerTag)
	{
		foreach (UIEventListener listener in listeners)
		{
			if (string.Equals(listener.listenerTag, listenerTag))
			{
				return listener;
			}
		}
		return null;
	}
}
