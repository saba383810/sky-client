using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour {

	public ParticleSystem particles;


	// Use this for initialization
	void Start () {
		//particles = this.GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Burst()
	{
		particles.Play ();
	}
}
