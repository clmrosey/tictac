using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class moveEnemy : MonoBehaviour {

    public Vector2 speed = new Vector2(6, 0);
    public Vector2 direction = new Vector2(-1, 0);
	public int tps = 3;

	private Animator m_Anim;
    private Vector2 mouvement;

	private bool enemy_death = false;

	
	IEnumerator wait()
	{
		enemy_death = true;
		
		yield return new WaitForSeconds (1.5f);
		Application.LoadLevel (Application.loadedLevelName);
		
		enemy_death = false;
	}

    IEnumerator routineL()
    {
        //Deplacement vers gauche
        mouvement = new Vector2(speed.x * direction.x, speed.y * direction.y);
		m_Anim.SetFloat("Speed", Mathf.Abs(speed.x));
        
        //rotation sprite
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

        //Temps de déplacement
        yield return new WaitForSeconds(tps);
        direction = new Vector2(1, 0);
        StartCoroutine("routineD");
    }

    IEnumerator routineD()
    {
        //Deplacement vers droite
        mouvement = new Vector2(speed.x * direction.x, speed.y * direction.y);
		m_Anim.SetFloat("Speed", Mathf.Abs(speed.x));

        //rotation sprite
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

        //Temps de déplacement
        yield return new WaitForSeconds(tps);
        direction = new Vector2(-1, 0);
        StartCoroutine("routineL");
    }

	// Use this for initialization
	void Start () {
		if (direction.x < 0) {
			StartCoroutine ("routineL");
		} else {
			StartCoroutine ("routineD");
		}
	}


	private void Awake()
	{
		// Setting up references.
		m_Anim = GetComponent<Animator>();
	}
    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().velocity = mouvement;
    }

	void Update()
	{
		m_Anim.SetBool("death",enemy_death);
	}
	
}
