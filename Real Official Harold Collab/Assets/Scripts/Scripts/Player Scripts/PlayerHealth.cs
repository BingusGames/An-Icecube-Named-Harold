using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	public GameObject player;
	public Transform respawnPoint;
	public int maxHealth = 10;
	public int health;

	void start()
	{
		health = maxHealth;
	}

	public void TakeDamage(int damage)
	{
		health -= damage;
		if (health <= 0)
		{
			player.transform.position = respawnPoint.position;
			health = maxHealth;
		}

	}
}

