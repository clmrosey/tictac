using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class ExitPlateforme : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Killzone")
		{
			
			Destroy(gameObject);
		}
	}
	
}
