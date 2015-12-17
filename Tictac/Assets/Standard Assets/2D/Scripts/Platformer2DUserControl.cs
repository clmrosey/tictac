using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Linq;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;
		private bool chara_death = false;
		private bool typeJump = true;


        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }

		IEnumerator wait()
		{
			chara_death = true;
			
			yield return new WaitForSeconds (1.5f);

			chara_death = false;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.tag == "Killzone")
			{
				
				StartCoroutine("wait");	
			}

			if (other.gameObject.tag == "up_down")
			{
				typeJump = false;
			}
		}

        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
			if (chara_death == false) {
				m_Character.Move (h, crouch, m_Jump);
			} else {
				m_Character.Move (0, false, false);
			}
			
			m_Jump = false;
		}
    }
}
