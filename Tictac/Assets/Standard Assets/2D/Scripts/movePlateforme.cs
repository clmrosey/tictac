using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class movePlateforme : MonoBehaviour {

    public Vector2 speed = new Vector2(0, 10);
    public Vector2 direction = new Vector2(0, 1);
	public int tps = 2;

	private Animator m_Anim;
    private Vector2 mouvement;

    IEnumerator routineL()
    {
        //Deplacement vers gauche
        //mouvement = new Vector2(speed.x * direction.x, speed.y * direction.y);
		//m_Anim.SetFloat("Speed", Mathf.Abs(speed.y));

        //Temps de déplacement
        yield return new WaitForSeconds(tps);
        direction = new Vector2(0, -1);
        StartCoroutine("routineD");
    }

    IEnumerator routineD()
    {
        //Deplacement vers droite
        //mouvement = new Vector2(speed.x * direction.x, speed.y * direction.y);
		//m_Anim.SetFloat("Speed", Mathf.Abs(speed.y));

        //Temps de déplacement
        yield return new WaitForSeconds(tps);
        direction = new Vector2(0, 1);
        StartCoroutine("routineL");
    }

	// Use this for initialization
	void Start () {
			StartCoroutine ("routineL");
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
	
}
