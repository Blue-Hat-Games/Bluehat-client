/* Scripted by Omabu - omabuarts@gmail.com */
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour {

	private Animator [] animator;

	[Space (10)]
	public Transform animal;
	public Dropdown dropdown;

	void Start () {

		int count = 0;

		for (int i = 0; i < animal.childCount; i++)
			if (animal.GetChild (i).GetComponent <Animator> () != null)
				count++;

		animator = new Animator [count];

		for (int i = 0; i < animal.childCount; i++)
			if (animal.GetChild (i).GetComponent <Animator> () != null)
				animator [i] = animal.GetChild (i).GetComponent <Animator> ();
	}

	void Update () {

		if (Input.GetKeyDown ("right")) { NextAnim (); }
		else if (Input.GetKeyDown ("left")) { PrevAnim (); }
	}

	public void NextAnim () {

		if (dropdown.value >= dropdown.options.Count - 1)
			dropdown.value = 0;
		else
			dropdown.value++;

		PlayAnim ();
	}

	public void PrevAnim () {

		if (dropdown.value <= 0)
			dropdown.value = dropdown.options.Count - 1;
		else
			dropdown.value--;
		
		PlayAnim ();
	}

	public void PlayAnim () {

		for (int i = 0; i < animator.Length; i++)
		{
			animator [i].Play (dropdown.options [dropdown.value].text);
		}
	}

	public void GoToWebsite (string URL) {

		Application.OpenURL (URL);
	}
}