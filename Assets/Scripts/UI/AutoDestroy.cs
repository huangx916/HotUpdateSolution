using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {

	public float duration;

	void Awake()
	{
		Destroy(gameObject, duration);
	}
}
