using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour {
	
	public List<SpawnQueueEntry> spawnQueue = new List<SpawnQueueEntry>();
	public GameObject pathContainer;
	public int zLevel = 0; // To prevent z fighting
	
	void Update() {
		
		List<SpawnQueueEntry> toBeRemoved = new List<SpawnQueueEntry>();
		
		foreach (SpawnQueueEntry entry in spawnQueue) {
			
			entry.time -= Time.deltaTime;
			if (entry.time <= 0) {
				
				Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (0.1f * zLevel));
				GameObject enemy = (GameObject) Instantiate(entry.enemy, position, entry.enemy.transform.rotation);
				GameObject.FindGameObjectWithTag("GameController").GetComponent<Level>().currentEnemies.Add(enemy);
				enemy.GetComponent<Agent>().pathContainer = pathContainer;
				toBeRemoved.Add(entry);
				zLevel--;
			}
		}
		
		foreach (SpawnQueueEntry entry in toBeRemoved)
			spawnQueue.Remove(entry);
	}
}
