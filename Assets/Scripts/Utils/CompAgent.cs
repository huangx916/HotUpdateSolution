using UnityEngine;

namespace Main
{
	public static class CompAgent
	{
		public static void SetChildName(Component parentComp, string path, object name)
		{
			SetChildName(parentComp, path, name == null ? null : name.ToString());
		}

		public static void SetChildName(GameObject parentGo, string path, object name)
		{
			SetChildName(parentGo, path, name == null ? null : name.ToString());
		}

		public static void SetName(Component comp, object name)
		{
			SetName(comp, name == null ? null : name.ToString());
		}

		public static void SetName(GameObject go, object name)
		{
			SetName(go, name == null ? null : name.ToString());
		}

		public static void SetChildName(Component parentComp, string path, string name)
		{
			if (IsObjectNotNull(parentComp))
			{
				Transform trans = FindChild(parentComp, path);
				SetName(trans, name);
			}
		}

		public static void SetChildName(GameObject parentGo, string path, string name)
		{
			if (IsObjectNotNull(parentGo))
			{
				SetChildName(parentGo.transform, path, name);
			}
		}

		public static void SetName(Component comp, string name)
		{
			if (IsObjectNotNull(comp))
			{
				SetName(comp.gameObject, name);
			}
		}

		public static void SetName(GameObject go, string name)
		{
			if (IsObjectNotNull(go) && !string.IsNullOrEmpty(name))
			{
				go.name = name;
			}
		}

		public static void SetChildMaterial(Component parentComp, string path, Material mat)
		{
			if (IsObjectNotNull(parentComp))
			{
				Transform trans = FindChild(parentComp, path);
				SetMaterial(trans, mat);
			}
		}

		public static void SetChildMaterial(GameObject parentGo, string path, Material mat)
		{
			if (IsObjectNotNull(parentGo))
			{
				SetChildMaterial(parentGo.transform, path, mat);
			}
		}

		public static void SetMaterial(Component comp, Material mat)
		{
			if (IsObjectNotNull(comp))
			{
				Renderer renderer = comp.GetComp<Renderer>();
				renderer.material = mat;
			}
		}

		public static void SetMaterial(GameObject go, Material mat)
		{
			if (IsObjectNotNull(go))
			{
				SetMaterial(go.GetComponent<Renderer>(), mat);
			}
		}

		public static void SetChildSharedMaterial(Component parentComp, string path, Material mat)
		{
			if (IsObjectNotNull(parentComp))
			{
				Transform trans = FindChild(parentComp, path);
				SetSharedMaterial(trans, mat);
			}
		}

		public static void SetChildSharedMaterial(GameObject parentGo, string path, Material mat)
		{
			if (IsObjectNotNull(parentGo))
			{
				SetChildSharedMaterial(parentGo.transform, path, mat);
			}
		}

		public static void SetSharedMaterial(Component comp, Material mat)
		{
			if (IsObjectNotNull(comp))
			{
				Renderer renderer = comp.GetComp<Renderer>();
				renderer.sharedMaterial = mat;
			}
		}

		public static void SetSharedMaterial(GameObject go, Material mat)
		{
			if (IsObjectNotNull(go))
			{
				SetSharedMaterial(go.GetComponent<Renderer>(), mat);
			}
		}

		public static void SetChildActive(Component parentComp, string path, int active)
		{
			SetChildActive(parentComp, path, active > 0);
		}

		public static void SetChildActive(GameObject parentGo, string path, int active)
		{
			SetChildActive(parentGo, path, active > 0);
		}

		public static void SetActive(Component comp, int active)
		{
			SetActive(comp, active > 0);
		}

		public static void SetActive(GameObject go, int active)
		{
			SetActive(go, active > 0);
		}

		public static void SetChildActive(Component parentComp, string path, bool active)
		{
			if (IsObjectNotNull(parentComp))
			{
				Transform trans = FindChild(parentComp, path);
				SetActive(trans, active);
			}
		}

		public static void SetChildActive(GameObject parentGo, string path, bool active)
		{
			if (IsObjectNotNull(parentGo))
			{
				SetChildActive(parentGo.transform, path, active);
			}
		}

		public static void SetActive(Component comp, bool active)
		{
			if (IsObjectNotNull(comp))
			{
				SetActive(comp.gameObject, active);
			}
		}

		public static void SetActive(GameObject go, bool active)
		{
			if (IsObjectNotNull(go))
			{
				go.SetActive(active);
			}
		}

		public static void PlayChildParticles(Component parentComp, string path, bool restart = true)
		{
			if (IsObjectNotNull(parentComp))
			{
				Transform trans = FindChild(parentComp, path);
				PlayParticles(trans, restart);
			}
		}

		public static void PlayChildParticles(GameObject parentGo, string path, bool restart = true)
		{
			if (IsObjectNotNull(parentGo))
			{
				PlayChildParticles(parentGo.transform, path, restart);
			}
		}

		public static void PlayParticles(Component comp, bool restart = true)
		{
			if (IsObjectNotNull(comp))
			{
				PlayParticles(comp.gameObject, restart);
			}
		}

		public static void PlayParticles(GameObject go, bool restart = true)
		{
			if (IsObjectNotNull(go))
			{
				ParticleSystem[] particles = go.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particle in particles)
				{
					if (restart)
					{
						particle.Clear();
					}
					particle.Play();
				}
			}
		}

		public static void StopChildParticles(Component parentComp, string path, bool clear = true)
		{
			if (IsObjectNotNull(parentComp))
			{
				Transform trans = FindChild(parentComp, path);
				StopParticles(trans, clear);
			}
		}

		public static void StopChildParticles(GameObject parentGo, string path, bool clear = true)
		{
			if (IsObjectNotNull(parentGo))
			{
				StopChildParticles(parentGo.transform, path, clear);
			}
		}

		public static void StopParticles(Component comp, bool clear = true)
		{
			if (IsObjectNotNull(comp))
			{
				StopParticles(comp.gameObject, clear);
			}
		}

		public static void StopParticles(GameObject go, bool clear = true)
		{
			if (IsObjectNotNull(go))
			{
				ParticleSystem[] particles = go.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particle in particles)
				{
					if (clear)
					{
						particle.Clear();
					}
					else
					{
						particle.Stop();
					}
				}
			}
		}

		public static void SetChildEnabled<T>(Component parentComp, string path, int enabled) where T : Component
		{
			SetChildEnabled<T>(parentComp, path, enabled > 0);
		}

		public static void SetChildEnabled<T>(GameObject parentGo, string path, int enabled) where T : Component
		{
			SetChildEnabled<T>(parentGo, path, enabled > 0);
		}

		public static void SetEnabled<T>(Component comp, int enabled) where T : Component
		{
			SetEnabled<T>(comp, enabled > 0);
		}

		public static void SetEnabled<T>(GameObject go, int enabled) where T : Component
		{
			SetEnabled<T>(go, enabled > 0);
		}

		public static void SetEnabled(Component comp, int enabled)
		{
			SetEnabled(comp, enabled > 0);
		}

		public static void SetChildEnabled<T>(Component parentComp, string path, bool enabled) where T : Component
		{
			if (IsObjectNotNull(parentComp))
			{
				Transform trans = FindChild(parentComp, path);
				SetEnabled<T>(trans, enabled);
			}
		}

		public static void SetChildEnabled<T>(GameObject parentGo, string path, bool enabled) where T : Component
		{
			if (IsObjectNotNull(parentGo))
			{
				SetChildEnabled<T>(parentGo.transform, path, enabled);
			}
		}

		public static void SetEnabled<T>(Component comp, bool enabled) where T : Component
		{
			if (IsObjectNotNull(comp))
			{
				if (comp is T)
				{
					SetEnabled(comp, enabled);
				}
				else
				{
					SetEnabled<T>(comp.gameObject, enabled);
				}
			}
		}

		public static void SetEnabled<T>(GameObject go, bool enabled) where T : Component
		{
			if (IsObjectNotNull(go))
			{
				T[] ts = go.GetComponents<T>();
				for (int index = 0; index < ts.Length; index++)
				{
					SetEnabled(ts[index], enabled);
				}
			}
		}

		public static void SetEnabled(Component comp, bool enabled)
		{
			if (IsObjectNotNull(comp))
			{
				if (comp is Behaviour)
				{
					Behaviour behaviour = comp as Behaviour;
					if (IsObjectNotNull(behaviour))
					{
						behaviour.enabled = enabled;
					}
				}
				else if (comp is Collider)
				{
					Collider collider = comp as Collider;
					if (IsObjectNotNull(collider))
					{
						collider.enabled = enabled;
					}
				}
				else if (comp is Renderer)
				{
					Renderer renderer = comp as Renderer;
					if (IsObjectNotNull(renderer))
					{
						renderer.enabled = enabled;
					}
				}
			}
		}

		public static void SetChildLocalPosition(Component parentComp, string path, Component targetComp)
		{
			if (IsObjectNotNull(targetComp))
			{
				SetChildLocalPosition(parentComp, path, targetComp.GetTrans().localPosition);
			}
		}

		public static void SetChildLocalPosition(GameObject parentGo, string path, Component targetComp)
		{
			if (IsObjectNotNull(targetComp))
			{
				SetChildLocalPosition(parentGo, path, targetComp.GetTrans().localPosition);
			}
		}

		public static void SetLocalPosition(Component comp, Component targetComp)
		{
			if (IsObjectNotNull(targetComp))
			{
				SetLocalPosition(comp, targetComp.GetTrans().localPosition);
			}
		}

		public static void SetLocalPosition(GameObject go, Component targetComp)
		{
			if (IsObjectNotNull(targetComp))
			{
				SetLocalPosition(go, targetComp.GetTrans().localPosition);
			}
		}

		public static void SetChildLocalPosition(Component parentComp, string path, GameObject targetGo)
		{
			if (IsObjectNotNull(targetGo))
			{
				SetChildLocalPosition(parentComp, path, targetGo.transform);
			}
		}

		public static void SetChildLocalPosition(GameObject parentGo, string path, GameObject targetGo)
		{
			if (IsObjectNotNull(targetGo))
			{
				SetChildLocalPosition(parentGo, path, targetGo.transform);
			}
		}

		public static void SetLocalPosition(Component comp, GameObject targetGo)
		{
			if (IsObjectNotNull(targetGo))
			{
				SetLocalPosition(comp, targetGo.transform);
			}
		}

		public static void SetLocalPosition(GameObject go, GameObject targetGo)
		{
			if (IsObjectNotNull(targetGo))
			{
				SetLocalPosition(go, targetGo.transform);
			}
		}

		public static void SetChildLocalPosition(Component parentComp, string path, Vector3 position)
		{
			if (IsObjectNotNull(parentComp))
			{
				Transform trans = FindChild(parentComp, path);
				SetLocalPosition(trans, position);
			}
		}

		public static void SetChildLocalPosition(GameObject parentGo, string path, Vector3 position)
		{
			if (IsObjectNotNull(parentGo))
			{
				SetChildLocalPosition(parentGo.transform, path, position);
			}
		}

		public static void SetLocalPosition(Component comp, Vector3 position)
		{
			if (IsObjectNotNull(comp))
			{
				Transform trans = comp.GetTrans();
				if (IsObjectNotNull(trans))
				{
					trans.localPosition = position;
				}
			}
		}

		public static void SetLocalPosition(GameObject go, Vector3 position)
		{
			if (IsObjectNotNull(go))
			{
				SetLocalPosition(go.transform, position);
			}
		}

		public static void SetChildPosition(Component parentComp, string path, Component targetComp)
		{
			if (IsObjectNotNull(targetComp))
			{
				SetChildPosition(parentComp, path, targetComp.GetTrans().position);
			}
		}

		public static void SetChildPosition(GameObject parentGo, string path, Component targetComp)
		{
			if (IsObjectNotNull(targetComp))
			{
				SetChildPosition(parentGo, path, targetComp.GetTrans().position);
			}
		}

		public static void SetPosition(Component comp, Component targetComp)
		{
			if (IsObjectNotNull(targetComp))
			{
				SetPosition(comp, targetComp.GetTrans().position);
			}
		}

		public static void SetPosition(GameObject go, Component targetComp)
		{
			if (IsObjectNotNull(targetComp))
			{
				SetPosition(go, targetComp.GetTrans().position);
			}
		}

		public static void SetChildPosition(Component parentComp, string path, GameObject targetGo)
		{
			if (IsObjectNotNull(targetGo))
			{
				SetChildPosition(parentComp, path, targetGo.transform);
			}
		}

		public static void SetChildPosition(GameObject parentGo, string path, GameObject targetGo)
		{
			if (IsObjectNotNull(targetGo))
			{
				SetChildPosition(parentGo, path, targetGo.transform);
			}
		}

		public static void SetPosition(Component comp, GameObject targetGo)
		{
			if (IsObjectNotNull(targetGo))
			{
				SetPosition(comp, targetGo.transform);
			}
		}

		public static void SetPosition(GameObject go, GameObject targetGo)
		{
			if (IsObjectNotNull(targetGo))
			{
				SetPosition(go, targetGo.transform);
			}
		}

		public static void SetChildPosition(Component parentComp, string path, Vector3 position)
		{
			if (IsObjectNotNull(parentComp))
			{
				Transform trans = FindChild(parentComp, path);
				SetPosition(trans, position);
			}
		}

		public static void SetChildPosition(GameObject parentGo, string path, Vector3 position)
		{
			if (IsObjectNotNull(parentGo))
			{
				SetChildPosition(parentGo.transform, path, position);
			}
		}

		public static void SetPosition(Component comp, Vector3 position)
		{
			if (IsObjectNotNull(comp))
			{
				Transform trans = comp.GetTrans();
				if (IsObjectNotNull(trans))
				{
					trans.position = position;
				}
			}
		}

		public static void SetPosition(GameObject go, Vector3 position)
		{
			if (IsObjectNotNull(go))
			{
				SetPosition(go.transform, position);
			}
		}

		public static void SetChildParent(Component oldParentComp, string path, GameObject newParentGo, bool resetTrans = false)
		{
			if (IsObjectNotNull(newParentGo))
			{
				SetParent(oldParentComp, newParentGo.transform, resetTrans);
			}
		}

		public static void SetChildParent(GameObject oldParentGo, string path, GameObject newParentGo, bool resetTrans = false)
		{
			if (IsObjectNotNull(newParentGo))
			{
				SetParent(oldParentGo, newParentGo.transform, resetTrans);
			}
		}

		public static void SetParent(Component comp, GameObject parentGo, bool resetTrans = false)
		{
			if (IsObjectNotNull(parentGo))
			{
				SetParent(comp, parentGo.transform, resetTrans);
			}
		}

		public static void SetParent(GameObject go, GameObject parentGo, bool resetTrans = false)
		{
			if (IsObjectNotNull(parentGo))
			{
				SetParent(go, parentGo.transform, resetTrans);
			}
		}

		public static void SetChildParent(Component oldParentComp, string path, Component newParentComp, bool resetTrans = false)
		{
			if (IsObjectNotNull(oldParentComp))
			{
				Transform trans = FindChild(oldParentComp, path);
				SetParent(trans, newParentComp, resetTrans);
			}
		}

		public static void SetChildParent(GameObject oldParentGo, string path, Component newParentComp, bool resetTrans = false)
		{
			if (IsObjectNotNull(oldParentGo))
			{
				SetChildParent(oldParentGo.transform, path, newParentComp, resetTrans);
			}
		}

		public static void SetParent(GameObject go, Component parentComp, bool resetTrans = false)
		{
			if (IsObjectNotNull(go))
			{
				SetParent(go.transform, parentComp, resetTrans);
			}
		}

		public static void SetParent(Component comp, Component parentComp, bool resetTrans = false)
		{
			if (IsObjectNotNull(comp))
			{
				Transform trans = comp.GetTrans();
				trans.parent = parentComp ? parentComp.GetTrans() : null;
				if (resetTrans)
				{
					trans.localPosition = Vector3.zero;
					trans.localRotation = Quaternion.identity;
					trans.localScale = Vector3.one;
				}
			}
		}

		public static T AddChild<T>(Component parentComp) where T : Component
		{
			return AddChild<T>(parentComp, typeof(T).Name);
		}

		public static T AddChild<T>(GameObject parentGo) where T : Component
		{
			return AddChild<T>(parentGo, typeof(T).Name);
		}

		public static T AddChild<T>(Component parentComp, string name) where T : Component
		{
			if (IsObjectNotNull(parentComp))
			{
				GameObject go = AddChild(parentComp, name);
				if (typeof(T) == typeof(Transform))
				{
					return go.transform as T;
				}
				else
				{
					return go.AddComponent<T>();
				}
			}
			return null;
		}

		public static T AddChild<T>(GameObject parentGo, string name = null) where T : Component
		{
			if (IsObjectNotNull(parentGo))
			{
				return AddChild<T>(parentGo.transform, name);
			}
			return null;
		}

		public static GameObject AddChild(Component parentComp, string name = null)
		{
			if (IsObjectNotNull(parentComp))
			{
				GameObject go = new GameObject(name);
				SetParent(go, parentComp.GetTrans(), true);
				return go;
			}
			return null;
		}

		public static GameObject AddChild(GameObject parentGo, string name = null)
		{
			if (IsObjectNotNull(parentGo))
			{
				return AddChild(parentGo.transform, name);
			}
			return null;
		}

		public static T AddChild<T>(Component parentComp, T prefabT, string name = null) where T : Component
		{
			if (IsObjectNotNull(parentComp))
			{
				T t = Object.Instantiate(prefabT, parentComp.GetTrans());
				if (name != null)
				{
					t.name = name;
				}
				return t;
			}
			return null;
		}

		public static T AddChild<T>(GameObject parentGo, T prefabT, string name = null) where T : Component
		{
			if (IsObjectNotNull(parentGo))
			{
				return AddChild(parentGo.transform, prefabT, name);
			}
			return null;
		}

		public static GameObject AddChild(Component parentComp, GameObject prefabGo, string name = null)
		{
			if (IsObjectNotNull(parentComp))
			{
				GameObject go = Object.Instantiate(prefabGo, parentComp.GetTrans());
				if (name != null)
				{
					go.name = name;
				}
				return go;
			}
			return null;
		}

		public static GameObject AddChild(GameObject parentGo, GameObject prefabGo, string name = null)
		{
			if (IsObjectNotNull(parentGo))
			{
				return AddChild(parentGo.transform, prefabGo, name);
			}
			return null;
		}

		public static void ClearChildren(Component parentComp)
		{
			if (IsObjectNotNull(parentComp))
			{
				Transform parentTrans = parentComp.GetTrans();
				for (int index = parentTrans.childCount - 1; index >= 0; index--)
				{
					GameObject childGo = parentTrans.GetChild(index).gameObject;
					SetActive(childGo, false);
					Object.Destroy(childGo);
				}
			}
		}

		public static void ClearChildren(GameObject parentGo)
		{
			if (IsObjectNotNull(parentGo))
			{
				ClearChildren(parentGo.transform);
			}
		}

		public static void ClearCompnents<T>(Component comp) where T : Component
		{
			if (IsObjectNotNull(comp))
			{
				ClearCompnents<T>(comp.gameObject);
			}
		}

		public static void ClearCompnents<T>(GameObject go) where T : Component
		{
			if (IsObjectNotNull(go))
			{
				foreach (T t in go.GetComponents<T>())
				{
					Object.Destroy(t);
				}
			}
		}

		public static T[] FindChildComps<T>(GameObject parentGo, string path) where T : Component
		{
			if (IsObjectNotNull(parentGo))
			{
				return FindChildComps<T>(parentGo.transform, path);
			}
			return new T[0];
		}

		public static T[] FindChildComps<T>(Component parentComp, string path) where T : Component
		{
			if (IsObjectNotNull(parentComp))
			{
				Transform child = FindChild(parentComp, path);
				if (child)
				{
					T[] ts = child.GetComponents<T>();
					return ts;
				}
			}
			return new T[0];
		}

		public static T FindChild<T>(Component parentComp) where T : Component
		{
			return FindChild<T>(parentComp, typeof(T).Name);
		}

		public static T FindChild<T>(GameObject parentGo) where T : Component
		{
			return FindChild<T>(parentGo, typeof(T).Name);
		}

		public static T FindChild<T>(GameObject parentGo, string path) where T : Component
		{
			if (IsObjectNotNull(parentGo))
			{
				return FindChild<T>(parentGo.transform, path);
			}
			return null;
		}

		public static T FindChild<T>(Component parentComp, string path) where T : Component
		{
			if (IsObjectNotNull(parentComp))
			{
				Transform child = FindChild(parentComp, path);
				if (child)
				{
					T t = child.GetComp<T>();
					if (t)
					{
						return t;
					}
					Debugger.LogError(typeof(T).Name + " with path [" + path + "] is not exist!", parentComp);
				}
			}
			return null;
		}

		public static Transform FindChild(Component parentComp, string path)
		{
			Transform child = parentComp.GetTrans().Find(path);
			if (child)
			{
				return child;
			}

			Debugger.LogError("Child with path [" + path + "] is not exist!", parentComp);
			return null;
		}

		public static void SetLayer(Component comp, string layerName, int deep = 0)
		{
			SetLayer(comp, LayerMask.NameToLayer(layerName), deep);
		}

		public static void SetLayer(GameObject go, string layerName, int deep = 0)
		{
			SetLayer(go, LayerMask.NameToLayer(layerName), deep);
		}

		public static void SetLayer(Component comp, int layer, int deep = 0)
		{
			if (IsObjectNotNull(comp))
			{
				comp.gameObject.layer = layer;
				SetChildrenLayer(comp, layer, deep);
			}
		}

		public static void SetLayer(GameObject go, int layer, int deep = 0)
		{
			if (IsObjectNotNull(go))
			{
				SetLayer(go.transform, layer, deep);
			}
		}

		public static void SetChildrenLayer(Component comp, string layerName, int deep = 1)
		{
			SetChildrenLayer(comp, LayerMask.NameToLayer(layerName), deep);
		}

		public static void SetChildrenLayer(GameObject go, string layerName, int deep = 1)
		{
			SetChildrenLayer(go, LayerMask.NameToLayer(layerName), deep);
		}

		public static void SetChildrenLayer(Component comp, int layer, int deep = 1)
		{
			bool willDeep = deep == -1 || deep > 0;
			if (willDeep && IsObjectNotNull(comp))
			{
				deep = deep == -1 ? -1 : deep - 1;
				Transform trans = comp.GetTrans();
				foreach (Transform child in trans)
				{
					SetLayer(child, layer, deep);
				}
			}
		}

		public static void SetChildrenLayer(GameObject go, int layer, int deep = 1)
		{
			if (IsObjectNotNull(go))
			{
				SetChildrenLayer(go.transform, layer, deep);
			}
		}

		private static bool IsObjectNull(Object obj, string message = null)
		{
			return PointerCheck.IsNull(obj, message ?? "Object is null!");
		}

		private static bool IsObjectNotNull(Object obj, string message = null)
		{
			return !IsObjectNull(obj, message);
		}
	}
}