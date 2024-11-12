using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDamage : MonoBehaviour

{
	public int damage;
	public PlayerHealth playerHealth;
	public Haroldthe4thScript rb;


	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			rb.KBCounter = rb.KBTotalTime;
			if (collision.transform.position.x <= transform.position.x)
			{
				rb.KnockFromRight = true;
			}
			if (collision.transform.position.x > transform.position.x)
			{
				rb.KnockFromRight = false;
			}
			playerHealth.TakeDamage(damage);
		}

	}


}

