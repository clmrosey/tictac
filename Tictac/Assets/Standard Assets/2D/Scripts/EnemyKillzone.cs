using UnityEngine;
using System.Collections;

public class EnemyKillzone : MonoBehaviour {
	public GameObject DeadStar;
	public GameObject Player;

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.tag == "enemy")
		{
			Debug.Log ("entering killzone!");
			Destroy (other.gameObject);
			Instantiate (DeadStar, transform.position, Quaternion.identity);
			this.gameObject.SetActive (false);
		}
	}
}
