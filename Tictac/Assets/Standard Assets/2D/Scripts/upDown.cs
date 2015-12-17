using UnityEngine;
using System.Collections;

public class upDown : MonoBehaviour {

    public float gravity = -5.0f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Vector3.Dot()
            Vector3 theScale = transform.localScale;
            theScale.y *= -1;
            transform.localScale = theScale;
            Debug.Log("entering up/down!");
            Physics2D.gravity = new Vector2(0, gravity);
        }
    }
}
