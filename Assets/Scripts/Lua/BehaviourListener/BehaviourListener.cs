using System;
using UnityEngine;

namespace Main
{
	public abstract class BehaviourListener : MonoBehaviour, IDisposable
	{
		public abstract void Dispose();
	}
}