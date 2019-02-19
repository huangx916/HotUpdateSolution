using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
	public class CoroutineManager : MonoBehaviour
	{
		public static CoroutineManager Instance
		{
			get
			{
				return GameMain.Instance.ManagerCollection.GetManager<CoroutineManager>();
			}
		}

		private Queue<Action> m_MainThreadActionQueue = new Queue<Action>();

		void Update()
		{
			while (m_MainThreadActionQueue.Count > 0)
			{
				Action action = m_MainThreadActionQueue.Dequeue();
				if (action != null)
				{
					action();
				}
			}
		}

		public static Coroutine Start(IEnumerator ie, MonoBehaviour behaviour = null)
		{
			return (behaviour ?? Instance).StartCoroutine(ie);
		}

		public static void Stop(IEnumerator ie, MonoBehaviour behaviour = null)
		{
			if (behaviour || Instance)
			{
				(behaviour ?? Instance).StopCoroutine(ie);
			}
		}

		public static void StopCo(Coroutine co, MonoBehaviour behaviour = null)
		{
			if (behaviour || Instance)
			{
				(behaviour ?? Instance).StopCoroutine(co);
			}
		}

		public static void MoveNext(IEnumerator ie)
		{
			if (ie != null)
			{
				ie.MoveNext();
			}
		}

		public static void Flush(IEnumerator ie, int maxSteps = int.MaxValue)
		{
			if (ie != null)
			{
				Stop(ie);
				bool hasNext = true;
				int steps = 0;
				while (hasNext && (steps < maxSteps))
				{
					hasNext = ie.MoveNext();
					steps++;
				}
				if (steps >= maxSteps)
				{
					Debugger.LogError("MoveNext " + maxSteps + " times!");
				}
			}
		}

		#region delay

		public static void MainThread(Action operation)
		{
			WaitUpdate(operation);
		}

		public static void WaitUpdate(Action operation)
		{
			if (operation != null)
			{
				Instance.m_MainThreadActionQueue.Enqueue(operation);
			}
		}

		public static Coroutine EndOfFrame(Action operation, MonoBehaviour behaviour = null)
		{
			return Delay(new WaitForEndOfFrame(), operation, behaviour);
		}

		public static Coroutine WaitWww(WWW www, Action operation, MonoBehaviour behaviour = null)
		{
			return Start(DoDelay(www, operation), behaviour);
		}

		public static Coroutine Delay(float delay, Action operation, MonoBehaviour behaviour = null)
		{
			return Delay(new WaitForSeconds(delay), operation, behaviour);
		}

		public static Coroutine Delay(YieldInstruction delay, Action operation, MonoBehaviour behaviour = null)
		{
			return Start(DoDelay(delay, operation), behaviour);
		}

		private static IEnumerator DoDelay(object delay, Action operation)
		{
			if (operation != null)
			{
				yield return delay;
				operation();
			}
		}

		#endregion

		#region loop

		public static Coroutine Loop(System.Func<float, bool> whileFunc, Action operation, MonoBehaviour behaviour = null)
		{
			return Loop(whileFunc, null, operation, behaviour);
		}

		public static Coroutine Loop(System.Func<float, bool> whileFunc, float interval, Action operation, MonoBehaviour behaviour = null)
		{
			return Loop(whileFunc, new WaitForSeconds(interval), operation, behaviour);
		}

		public static Coroutine Loop(System.Func<float, bool> whileFunc, YieldInstruction interval, Action operation, MonoBehaviour behaviour = null)
		{
			return Start(DoLoop(whileFunc, interval, operation), behaviour);
		}

		private static IEnumerator DoLoop(System.Func<float, bool> whileFunc, YieldInstruction interval, Action operation)
		{
			float startTime = Time.time;
			while (whileFunc == null || whileFunc(Time.time - startTime))
			{
				operation();
				yield return interval;
			}
		}

		public static Coroutine UnscaledLoop(Func<float, bool> whileFunc, Action operation, MonoBehaviour behaviour = null)
		{
			return UnscaledLoop(whileFunc, 0, operation, behaviour);
		}

		public static Coroutine UnscaledLoop(Func<float, bool> whileFunc, float interval, Action operation, MonoBehaviour behaviour = null)
		{
			return Start(DoUnscaledLoop(whileFunc, interval, operation), behaviour);
		}

		private static IEnumerator DoUnscaledLoop(Func<float, bool> whileFunc, float interval, Action operation)
		{
			float startTime = Time.unscaledTime;
			while (whileFunc == null || whileFunc(Time.unscaledTime - startTime))
			{
				operation();
				yield return new WaitForSeconds(interval * Time.timeScale);
			}
		}

		#endregion

		#region wait: if null or not waiting, do immediately

		public static Coroutine Wait(Func<bool> waitUntil, Action operation, MonoBehaviour behaviour = null)
		{
#if UNITY_5_3_OR_NEWER
			return Wait(new WaitUntil(waitUntil), operation, behaviour);
#else
			return Start(DoWait(waitUntil, operation), behaviour);
#endif
		}

		private static IEnumerator DoWait(Func<bool> waitUntil, Action operation = null)
		{
			if (operation != null)
			{
				if (waitUntil != null)
				{
					while (!waitUntil())
					{
						yield return null;
					}
				}
				operation();
			}
		}

#if UNITY_5_3_OR_NEWER
		public static Coroutine Wait(WaitUntil waitUntil, Action operation, MonoBehaviour behaviour = null)
		{
			return Start(DoWait(waitUntil, operation), behaviour);
		}

		private static IEnumerator DoWait(WaitUntil waitUntil, Action operation = null)
		{
			if (operation != null)
			{
				if (waitUntil != null && waitUntil.keepWaiting)
				{
					yield return waitUntil;
				}
				operation();
			}
		}

		public static Coroutine Wait(ICollection<WaitUntil> waitUntils, Action operation, MonoBehaviour behaviour = null)
		{
			return Start(DoWait(waitUntils, operation), behaviour);
		}

		private static IEnumerator DoWait(ICollection<WaitUntil> waitUntils, Action operation = null)
		{
			if (operation != null)
			{
				foreach (WaitUntil waitUntil in waitUntils)
				{
					if (waitUntil != null && waitUntil.keepWaiting)
					{
						yield return waitUntil;
					}
				}
				operation();
			}
		}
#endif

		#endregion

		#region lag

		public static Coroutine EndOfLag(Action operation, MonoBehaviour behaviour = null)
		{
			return Start(DoEndOfLag(operation, behaviour), behaviour);
		}

		private static IEnumerator DoEndOfLag(Action operation, MonoBehaviour behaviour = null)
		{
			if (operation != null)
			{
				yield return WaitForEndOfLag(behaviour);
				operation();
			}
		}

		public static Coroutine WaitForEndOfLag(MonoBehaviour behaviour = null)
		{
			return Start(DoWaitForEndOfLag(), behaviour);
		}

		private const int LAG_FRAME_COUNT_MAX = 20;
		private const int LAG_CHECK_FRAME_COUNT = 3;
		private const float LAG_CHECK_THRESHOLD = 0.001F;

		private static IEnumerator DoWaitForEndOfLag()
		{
			float maxFrame = Time.frameCount + LAG_FRAME_COUNT_MAX;

			List<float> deltaTimeList = new List<float>();
			deltaTimeList.Add(Time.unscaledDeltaTime);
			yield return null;
			deltaTimeList.Add(Time.unscaledDeltaTime);
			while (Time.frameCount < maxFrame)
			{
				yield return null;
				deltaTimeList.Add(Time.unscaledDeltaTime);
				if (deltaTimeList.Count > LAG_CHECK_FRAME_COUNT)
				{
					deltaTimeList.RemoveAt(0);
				}
				float variance = GetVariance(deltaTimeList);
				if (variance < LAG_CHECK_THRESHOLD)
				{
					yield return null;
					break;
				}
			}
			deltaTimeList.Clear();
		}

		private static float GetVariance(ICollection<float> nums)
		{
			int count = nums.Count;
			float average = System.Linq.Enumerable.Average(nums);
			float varianceSum = 0;
			foreach (float num in nums)
			{
				varianceSum += Mathf.Pow(num - average, 2);
			}
			return varianceSum / count;
		}

#endregion
	}
}