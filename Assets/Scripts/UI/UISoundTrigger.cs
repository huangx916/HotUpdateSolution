using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Main
{
	public class UISoundTrigger : MonoBehaviour
	{
		public enum TriggerType
		{
			OnClick,
			OnMouseOver,
			OnMouseOut,
			OnPress,
			OnRelease,
			OnDragStart,
			OnDragEnd,
			Custom,
		}

		public string m_ModuleName = "Common";
		public string m_SoundName;
		public TriggerType m_Trigger = TriggerType.OnClick;
		[Range(0, 3)]
		public float m_Volume = 1;

		private bool canPlay
		{
			get
			{
				if (enabled)
				{
					Collider myCollider = GetComponent<Collider>();
					return myCollider != null && myCollider.enabled;
				}
				return false;
			}
		}

		private void OnHover(bool isOver)
		{
			if (!canPlay)
			{
				return;
			}
			if ((isOver && m_Trigger == TriggerType.OnMouseOver) ||
				(!isOver && m_Trigger == TriggerType.OnMouseOut))
			{
				Play();
			}
		}

		private void OnPress(bool isPressed)
		{
			if (!canPlay)
			{
				return;
			}
			if ((isPressed && m_Trigger == TriggerType.OnPress) ||
				(!isPressed && m_Trigger == TriggerType.OnRelease))
			{
				Play();
			}
		}

		private void OnClick()
		{
			if (!canPlay)
			{
				return;
			}
			if (m_Trigger == TriggerType.OnClick)
			{
				Play();
			}
		}

		private void OnSelect(bool isSelected)
		{
			if (!canPlay)
			{
				return;
			}
			if (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller)
			{
				OnHover(isSelected);
			}
		}

		private void OnDragStart()
		{
			if (!canPlay)
			{
				return;
			}
			if (m_Trigger == TriggerType.OnDragStart)
			{
				Play();
			}
		}

		private void OnDragEnd()
		{
			if (!canPlay)
			{
				return;
			}
			if (m_Trigger == TriggerType.OnDragEnd)
			{
				Play();
			}
		}

		public void Play()
		{
			LuaTable audioManager = LuaMain.LuaEnv.Global.GetInPath<LuaTable>("LuaClass.AudioManager.Instance");
			if (audioManager != null)
			{
				LuaTable moduleTypeClass = LuaMain.LuaEnv.Global.GetInPath<LuaTable>("LuaClass.ModuleType");
				LuaMain.Instance.FuncInvoke(audioManager, "PlayModuleUIOneShot", audioManager, moduleTypeClass.Get<object>(m_ModuleName), m_SoundName, m_Volume);
			}
		}
	}
}