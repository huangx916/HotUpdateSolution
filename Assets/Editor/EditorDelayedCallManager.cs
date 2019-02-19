using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public sealed class EditorWaitForSeconds
{
	private double m_Seconds;
	private double m_DoneTime;

	public EditorWaitForSeconds(double seconds)
	{
		m_Seconds = seconds;
	}

	public void ResetDoneTime()
	{
		m_DoneTime = EditorApplication.timeSinceStartup + m_Seconds;
	}

	public bool isDone
	{
		get
		{
			double time = EditorApplication.timeSinceStartup;
			return time >= m_DoneTime;
		}
	}
}

public sealed class EditorCoroutine
{
	private IEnumerator m_Ie;
	private object m_Owner;

	public EditorCoroutine(IEnumerator ie, object owner)
	{
		m_Ie = ie;
		m_Owner = owner;
	}

	public IEnumerator ie
	{
		get
		{
			return m_Ie;
		}
	}

	public object owner
	{
		get
		{
			return m_Owner;
		}
	}
}

[InitializeOnLoad]
public class EditorDelayedCallManager
{
	private static EditorDelayedCallManager s_Instance = new EditorDelayedCallManager();

	public static EditorDelayedCallManager Instance
	{
		get
		{
			return s_Instance;
		}
	}

	private Dictionary<Type, List<EditorCoroutine>> m_CoroutineListDict = new Dictionary<Type, List<EditorCoroutine>>();

	private EditorDelayedCallManager()
	{
		InitCoroutineList(typeof(object));
		InitCoroutineList(typeof(EditorWaitForSeconds));
		InitCoroutineList(typeof(WWW));
		InitCoroutineList(typeof(EditorCoroutine));

		EditorApplication.update += DelayedCallUpdate;
	}

	~EditorDelayedCallManager()
	{
		EditorApplication.update -= DelayedCallUpdate;
	}

	private void InitCoroutineList(Type key)
	{
		if (!m_CoroutineListDict.ContainsKey(key))
		{
			m_CoroutineListDict.Add(key, new List<EditorCoroutine>());
		}
	}

	private void DelayedCallUpdate()
	{
		InvokeOtherCoroutine();
		InvokeSecondsCoroutine();
		InvokeWWWCoroutine();
		InvokeStartCoroutineCoroutine();
	}

	/// <summary>
	/// In order to prevent the coroutine can not be stopped, add a parameter named owner.
	/// While owner is equals null, coroutine will be stopped.
	/// </summary>
	public EditorCoroutine StartCoroutine(IEnumerator ie, object owner)
	{
		EditorCoroutine coroutine = new EditorCoroutine(ie, owner);
		if (coroutine.owner != null)
		{
			if (coroutine.ie.MoveNext())
			{
				UpdateCoroutine(coroutine);
				AddCoroutine(coroutine);
			}
		}
		return coroutine;
	}

	/// <summary>
	/// <seealso cref="StartCoroutine"/>
	/// </summary>
	public EditorCoroutine StartCoroutine(IEnumerator ie)
	{
		return StartCoroutine(ie, "");
	}

	public void StopCoroutine(IEnumerator ie)
	{
		foreach (List<EditorCoroutine> list in m_CoroutineListDict.Values)
		{
			for (int index = list.Count - 1; index >= 0; --index)
			{
				if (list[index].ie == ie)
				{
					list.RemoveAt(index);
				}
			}
		}
	}

	private void UpdateCoroutine(EditorCoroutine coroutine)
	{
		object current = coroutine.ie.Current;
		if (current is EditorWaitForSeconds)
		{
			(current as EditorWaitForSeconds).ResetDoneTime();
		}
	}

	private void AddCoroutine(EditorCoroutine coroutine)
	{
		object current = coroutine.ie.Current;
		if (current != null)
		{
			Type type = current.GetType();
			if (m_CoroutineListDict.ContainsKey(type))
			{
				m_CoroutineListDict[type].Add(coroutine);
				return;
			}
		}
		m_CoroutineListDict[typeof(object)].Add(coroutine);
	}

	private bool IsCoroutineDone(EditorCoroutine coroutine)
	{
		foreach (List<EditorCoroutine> list in m_CoroutineListDict.Values)
		{
			if (list.Contains(coroutine))
			{
				return false;
			}
		}
		return true;
	}

	private void InvokeOtherCoroutine()
	{
		List<EditorCoroutine> list = m_CoroutineListDict[typeof(object)];
		m_CoroutineListDict[typeof(object)] = new List<EditorCoroutine>();
		foreach (EditorCoroutine coroutine in list)
		{
			if (coroutine.owner != null)
			{
				if (coroutine.ie.MoveNext())
				{
					UpdateCoroutine(coroutine);
					AddCoroutine(coroutine);
				}
			}
		}
	}

	private void InvokeSecondsCoroutine()
	{
		List<EditorCoroutine> list = m_CoroutineListDict[typeof(EditorWaitForSeconds)];
		m_CoroutineListDict[typeof(EditorWaitForSeconds)] = new List<EditorCoroutine>();
		foreach (EditorCoroutine coroutine in list)
		{
			if (coroutine.owner != null)
			{
				if ((coroutine.ie.Current as EditorWaitForSeconds).isDone)
				{
					if (coroutine.ie.MoveNext())
					{
						UpdateCoroutine(coroutine);
						AddCoroutine(coroutine);
					}
				}
				else
				{
					AddCoroutine(coroutine);
				}
			}
		}
	}

	private void InvokeWWWCoroutine()
	{
		List<EditorCoroutine> list = m_CoroutineListDict[typeof(WWW)];
		m_CoroutineListDict[typeof(WWW)] = new List<EditorCoroutine>();
		foreach (EditorCoroutine coroutine in list)
		{
			if (coroutine.owner != null)
			{
				if ((coroutine.ie.Current as WWW).isDone)
				{
					if (coroutine.ie.MoveNext())
					{
						UpdateCoroutine(coroutine);
						AddCoroutine(coroutine);
					}
				}
				else
				{
					AddCoroutine(coroutine);
				}
			}
		}
	}

	private void InvokeStartCoroutineCoroutine()
	{
		List<EditorCoroutine> list = m_CoroutineListDict[typeof(EditorCoroutine)];
		m_CoroutineListDict[typeof(EditorCoroutine)] = new List<EditorCoroutine>();
		foreach (EditorCoroutine coroutine in list)
		{
			if (coroutine.owner != null)
			{
				if (IsCoroutineDone(coroutine.ie.Current as EditorCoroutine))
				{
					if (coroutine.ie.MoveNext())
					{
						UpdateCoroutine(coroutine);
						AddCoroutine(coroutine);
					}
				}
				else
				{
					AddCoroutine(coroutine);
				}
			}
		}
	}
}
