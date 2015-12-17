using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce;  
		[SerializeField] private float m_JumpForceTrue;  // Amount of force added when the player jumps.
		[SerializeField] private float m_UpsideJumpForce;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
		private bool chara_death = false;
		//### Gestion de la vie
		public Slider healthBarSlider;
		public Image Fill;  // assign in the editor the "Fill"
		public Color MaxHealthColor = Color.green;
		public Color MinHealthColor = Color.red;
		public Color fillColor = Color.green;
		private bool invincible  = false;
		//### Gestion de la vie
		private bool sensJump = true;
		private bool upPlateForme = true;



		IEnumerator wait()
		{
			chara_death = true;
			
			yield return new WaitForSeconds (1.5f);
			Application.LoadLevel (Application.loadedLevelName);
			
			chara_death = false;
		}

		IEnumerator waitPlateforme()
		{
			yield return new WaitForSeconds (1.5f);
		}

		IEnumerator Invincible() {
			Debug.Log("Invincible pour 2s");
			yield return new WaitForSeconds(2);
			Debug.Log("Invincible Over");
			invincible = false;
		}

		
		
        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }


		private void Update()
		{
			//
			if (upPlateForme == true) {
				SliderJoint2D slider = GameObject.FindGameObjectWithTag("tomb").GetComponent<SliderJoint2D>();
				slider.enabled = true;
			}

			//### Gestion health over time ###
			if(fillColor == Color.green) // Damage overtime Mode Normal
			{
				if(healthBarSlider.value == 0 && fillColor == Color.green) // Passage en mode Enfer si plus de vie
				{
					Debug.Log   ("Nightmare Mode");
					Fill.color = Color.red;
					healthBarSlider.value = 1f;
					fillColor = Color.red;
				}
				healthBarSlider.value -=.0003f;  //Damage lié au temps en mode normal
			}
			if(fillColor == Color.red) // Damage en mode nightmare
			{
				if(healthBarSlider.value == 0)
				{
					Debug.Log   ("Vous etes mort");
					StartCoroutine("wait");
				}
				healthBarSlider.value -=.0009f;  //Damage lié au temps
			}
			
			//### Gestion health over time ###
			m_Anim.SetBool("death",chara_death);

			bool shoot = Input.GetButtonDown("Fire1");
			shoot |= Input.GetButtonDown("Fire2");
			// Astuce pour ceux sous Mac car Ctrl + flèches est utilisé par le système
			
			if (shoot)
			{
				WeaponScript weapon = GetComponent<WeaponScript>();
				if (weapon != null)
				{
					// false : le joueur n'est pas un ennemi
					weapon.Attack(false);
				}
			}
		}


        private void FixedUpdate()
        {
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        }


        public void Move(float move, bool crouch, bool jump)
        {
            // If crouching, check to see if the character can stand up
            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            // Set whether or not the character is crouching in the animator
            m_Anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move*m_CrouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...
            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
				/*
				if (sensJump == true)
				{
					Debug.Log(sensJump);
					m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
					//sensJump = false;
				}
				if(sensJump == false)
				{
					Debug.Log(sensJump);
					m_Rigidbody2D.AddForce(new Vector2(0f, m_UpsideJumpForce));
					//sensJump = true;
				}
				*/
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				//Debug.Log(sensJump);
				StartCoroutine("waitPlateforme");
				upPlateForme = true;
            }
        }

		private void OnTriggerEnter2D(Collider2D other)
		{	
			if (other.tag == "CDE")
			{
				
				m_JumpForce = m_JumpForceTrue;
				
			}
			if (other.tag == "Killzone")
			{
				
				StartCoroutine("wait");
				
			}
			if (other.gameObject.tag == "up_down")
			{

				sensJump = false;
				m_JumpForce = m_UpsideJumpForce;
				Vector3 theScale = transform.localScale;
				theScale.y *= -1;
				transform.localScale = theScale;
				Debug.Log("je me tourne!");
			}
		
		}
		//#####################  Gestion de la barre de vie ######################
		// Cas des dommages sur les collisions
		private void OnCollisionEnter2D(Collision2D other)
		{	
			if (other.gameObject.tag == "OneShot") {  // Cas du OneShot
				StartCoroutine("wait");
			}

			if(other.gameObject.tag == "Enemy")  // Cas du tag enemy
			{
				if(fillColor == Color.red) // Nightmare Mode, Gestion vie
				{
					if(healthBarSlider.value == 0)
					{
						Debug.Log   ("Vous etes mort");
					}
					if(healthBarSlider.value + 0.3 > 1)
					{
						Debug.Log ("Passage en mode normal");
						Fill.color = Color.green;
						fillColor  = Color.green;
						healthBarSlider.value = .15f;
						
					}
					if(healthBarSlider.value + 0.3 < 1)
					{
						Debug.Log   ("Healing Damage Nightmare || Health : "+healthBarSlider.value);
						healthBarSlider.value +=.3f;
					}
					
				}
				if(fillColor == Color.green)
				{
					Debug.Log   ("Taking Damage || Health : "+healthBarSlider.value);
					healthBarSlider.value -=.1f;  //reduce health
					if(healthBarSlider.value == 0 && fillColor == Color.green) // Passage en mode Enfer
					{
						Debug.Log   ("Nightmare Mode");
						Fill.color = Color.red;
						healthBarSlider.value = 1f;
						fillColor = Color.red;
					}
				}
				
			}
			if (other.gameObject.tag == "soins") // Gestion des soins
			{
				if(fillColor == Color.red) // Nightmare Mode, Gestion vie
				{
					healthBarSlider.value -=.1f;  //Damage en mode cauchemar
					if(healthBarSlider.value == 0)
					{
						Debug.Log   ("Vous etes mort");
					}
				}
				Debug.Log   ("Healing Damage || Health : "+healthBarSlider.value);
				healthBarSlider.value +=.1f;  //healing
			}

			if (other.gameObject.tag == "tomb") {
				SliderJoint2D slider = GameObject.FindGameObjectWithTag("tomb").GetComponent<SliderJoint2D>();
				slider.enabled = false;
				upPlateForme = false;
			}

		}
		//Cas des dommages sur les contacts (Feu,Piques...)
		private void OnTriggerStay2D(Collider2D other)
		{
			if (!invincible)
			{
				if(other.gameObject.tag == "Enemy")  // Cas du tag enemy
				{
					if(fillColor == Color.red) // Nightmare Mode, Gestion vie
					{
						if(healthBarSlider.value == 0)
						{
							Debug.Log   ("Vous etes mort");
						}
						if(healthBarSlider.value + 0.3 > 1)
						{
							Debug.Log ("Passage en mode normal");
							Fill.color = Color.green;
							fillColor  = Color.green;
							invincible = true;
							StartCoroutine("Invincible");
							healthBarSlider.value = .15f;
						}
						if(healthBarSlider.value + 0.3 < 1)
						{
							Debug.Log   ("Healing Damage Nightmare || Health : "+healthBarSlider.value);
							healthBarSlider.value +=.3f;
							invincible = true;
							StartCoroutine("Invincible");
						}
						
					}
					if(fillColor == Color.green)
					{
						Debug.Log   ("Taking Damage || Health : "+healthBarSlider.value);
						healthBarSlider.value -=.1f;  //reduce health
						invincible = true;
						StartCoroutine("Invincible");
						if(healthBarSlider.value == 0 && fillColor == Color.green) // Passage en mode Enfer
						{
							Debug.Log   ("Nightmare Mode");
							Fill.color = Color.red;
							healthBarSlider.value = 1f;
							fillColor = Color.red;
							invincible = true;
							StartCoroutine("Invincible");
						}
					}
				}
				if (other.gameObject.tag == "soins") // Gestion des soins
				{
					if(fillColor == Color.red) // Nightmare Mode, Gestion vie
					{
						healthBarSlider.value -=.1f;  //Damage en mode cauchemar
						if(healthBarSlider.value == 0)
						{
							Debug.Log   ("Vous etes mort");
						}
						
					}
					Debug.Log   ("Healing Damage || Health : "+healthBarSlider.value);
					healthBarSlider.value +=.1f;  //healing
				}
			}
		}
		
		//#####################  Gestion de la barre de vie ######################

		private void Flip()
		{
			// Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}
