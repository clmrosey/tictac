using System;
using UnityEngine;
using System.Collections;
using System.Linq;

/*private IEnumerator wait(){
    yield return new WaitForSeconds(0.8s);
}

StartCoroutine("wait");*/

namespace UnityStandardAssets._2D
{
    
    public class Restarter : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
		
				Application.LoadLevel (Application.loadedLevelName);
               
            }
        }
	 }
}
