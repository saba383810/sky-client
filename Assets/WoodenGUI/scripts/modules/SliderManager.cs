using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour {

	Slider slider;

	// Use this for initialization
	void Start () {
		slider = gameObject.GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void AddValue (){
		this.slider.value += 0.1f;
	}

	public void SubtractValue (){
		this.slider.value -= 0.1f;
	}
}
