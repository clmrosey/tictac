using UnityEngine;

/// <summary>
/// Contrôleur du joueur
/// </summary>
public class PlayerScript : MonoBehaviour
{
	/// <summary>
	/// 1 - La vitesse de déplacement
	/// </summary>
	public Vector2 speed = new Vector2(10, 10);
	
	// 2 - Stockage du mouvement
	private Vector2 movement;

	public bool Xctrl = false;

	void Update()
	{
		float inputX = Input.GetAxis ("Horizontal");
		float inputY = Input.GetAxis ("Vertical");

		if (Xctrl) {
			// controls normaux
			inputX = Input.GetAxis ("Horizontal");
			inputY = Input.GetAxis ("Vertical");
		} else {
			// 3 - Récupérer les informations du clavier/manette inversés
			inputX = -1*Input.GetAxis ("Horizontal");
			inputY = -1*Input.GetAxis ("Vertical");
		}
		
		// 4 - Calcul du mouvement
		movement = new Vector2(
			speed.x * inputX,
			speed.y * inputY);

		// 5 - Tir
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

		// 6 - Déplacement limité au cadre de la caméra
		var dist = (transform.position - Camera.main.transform.position).z;
		
		var leftBorder = Camera.main.ViewportToWorldPoint(
			new Vector3(0, 0, dist)
			).x;
		
		var rightBorder = Camera.main.ViewportToWorldPoint(
			new Vector3(1, 0, dist)
			).x;
		
		var topBorder = Camera.main.ViewportToWorldPoint(
			new Vector3(0, 0, dist)
			).y;
		
		var bottomBorder = Camera.main.ViewportToWorldPoint(
			new Vector3(0, 1, dist)
			).y;
		
		transform.position = new Vector3(
			Mathf.Clamp(transform.position.x, leftBorder, rightBorder),
			Mathf.Clamp(transform.position.y, topBorder, bottomBorder),
			transform.position.z
			);
		
	}
	
	void FixedUpdate()
	{
		// 5 - Déplacement
		GetComponent<Rigidbody2D>().velocity = movement;
	}
}