using UnityEngine;
using System.Collections;

public class TowerShooting : MonoBehaviour {
	
	public float minRange = 0;
	public float maxRange;
	public float rangeStep = 0.01f;
	public GameObject target;
	public GameObject projectile;
	public GameObject spawn;
	public float projectileSpeed;
	public float cooldown;
	public float tolerance = 0.05f;
	public float timeStep = 0.001f;
	
	float time;
	
	void Start() {
		
		GameObject control = GameObject.FindGameObjectWithTag("GameController");
		if (control != null) {
			
			Level list = control.GetComponent<Level>();
			if (list != null)
				target = list.avaliableEnemies[0];
		}
		
		//transform.LookAt(target.prefab.transform);
		/*transform.eulerAngles.x = 0;
		transform.eulerAngles.y = 0;*/
	}
	
	void Update() {
		
		basicAiming();
		//advanceAiming();
	}
	
	void basicAiming() { // Can't be trusted
		
		if (spawn != null && target != null && projectileSpeed > 0) {
			
			Vector3 point = this.target.transform.position;
			point.z = gameObject.transform.position.z;
			
			if (Vector3.Distance(point, gameObject.transform.position) > maxRange ||
				Vector3.Distance(point, gameObject.transform.position) < minRange)
				return;
			
			transform.rotation = Quaternion.LookRotation(point - gameObject.transform.position, transform.up);
			
			if (this.time <= 0) {
			
				GameObject shot = (GameObject) Instantiate(projectile, spawn.transform.position, projectile.transform.rotation);
				shot.GetComponent<Projectile>().initialise(target.transform.position, projectileSpeed);
				this.time = cooldown;
			}
			else {
				
				this.time -= Time.deltaTime;
			}
		}
	}
	
	void advanceAiming() { // Can't be trusted
		
		if (spawn != null && target != null && projectileSpeed > 0 && tolerance >= timeStep) {
			
			Agent enemy = this.target.GetComponent<Agent>();
			if (enemy == null) return;
			
			// Circle's centre
			Vector3 centre = gameObject.transform.position;
			centre.z = 0;
			
			// For each point along path: is point within max range and outwith min range.
			for (float time = 0; true; time += timeStep) {
				
				if (!enemy.pathBoundsTest(time)) break;
				Vector3 point = enemy.positionAt(time);
				
				// Check if point is within max range and outwith min range
				if (Vector2.Distance(centre, point) <= maxRange &&
					Vector2.Distance(centre, point) >= minRange) {
					
					// Time for projectile to reach point on curve
					float projectileTime = Vector3.Distance(spawn.transform.position, point) / projectileSpeed;
					
					// Check if projectile can reach point in around the same time as  enemy
					if (projectileTime >= time - (time * tolerance) &&
						projectileTime <= time + (time * tolerance)) {
						
						point.z = centre.z;
						transform.rotation = Quaternion.LookRotation(point - centre, transform.up);
						
						if (this.time <= 0) {
						
							GameObject shot = (GameObject) Instantiate(projectile, spawn.transform.position, projectile.transform.rotation);
							shot.GetComponent<Projectile>().initialise(point, projectileSpeed);
							this.time = cooldown;
						}
						else {
							
							this.time -= Time.deltaTime;
						}
						return;
					}
				}
			}
		}
	}
	
	/*void Update() {
		
		if (projectileSpeed > 0 && tolerance >= timeStep && target != null) {
			
			Vector3 p = gameObject.transform.position;
			Vector3 a = target.transform.position;
			
			float dpa = Vector3.Distance(p, a);
			
			float t = dpa / projectileSpeed;
			
		repeat:
			if (!target.GetComponent<Agent>().canHit(t)) goto end;
			Vector3 b = target.GetComponent<Agent>().positionAt(t);
			
			float dpb = Vector3.Distance(p, b);
			
			float t2 = dpb / projectileSpeed;
			
			if (t2 > t + t * tolerance) {
				
				t += timeStep;
				goto repeat;
			}
			else if (t2 < t - t * tolerance) {
				
				t -= timeStep;
				goto repeat;
			}
			
			b.z = p.z;
			
			transform.rotation = Quaternion.LookRotation(b - p, transform.up);
			
			if (time <= 0) {
			
				GameObject shot = (GameObject) Instantiate(projectile, transform.position, projectile.transform.rotation);
				shot.GetComponent<Projectile>().initialise(b, projectileSpeed);
				time = cooldown;
			}
			else {
				
				time -= Time.deltaTime;
			}
			
		end:
			int i = 0;
		}
	}*/
}
