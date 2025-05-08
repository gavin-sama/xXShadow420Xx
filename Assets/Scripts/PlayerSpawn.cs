using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSpawn : MonoBehaviour
{
	[SerializeField] private Transform spawnPoint;//will prob be assigned in Inspector
	[SerializeField] private float respawnDelay = 2.0f; //Time before player respawns (after death most likely)


    // Ref to PlayerStats to check health--triggers the spawn
    private PlayerStats playerStats;
	private Animator anim;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		playerStats = GetComponent<PlayerStats>();
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
    {
        //Checks if player helath is at zero
		if (playerStats != null && playerStats.GetCurrentHealth() <=0) //Need to have this in PlayerStats
		{
			//Only trigger if player is dead dead
			if (anim != null && anim.GetBool("isDead"))
			{
				StartCoroutine(RespawnAfterDelay());//Coroutine works similarly to await/async---pauses execution
			}
		}
	}

	//courotine to handle respawn after delay
	private IEnumerator RespawnAfterDelay()
	{
		// Wait for the respawn delay
		yield return new WaitForSecondsRealtime(respawnDelay);

		// Reset death animation
		if (anim != null)
		{
			anim.SetBool("isDead", false);
		}

		// Call respawn
		Respawn();

		// Resume game time
		Time.timeScale = 1.0f;
	}

	// This will activate when the player dies
	public void Respawn()
	{
		// Reset player position to spawn point
		transform.position = spawnPoint.position;
		transform.rotation = spawnPoint.rotation;
	}

}
