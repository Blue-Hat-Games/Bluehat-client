/* Scripted by Omabu - omabuarts@gmail.com */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Demo : MonoBehaviour {

	private GameObject[] animal;
	private int animalIndex;
	private List<string> animationList = new List<string> {
															  "Attack",
															  "Bounce",
															  "Clicked",
															  "Death",
															  "Eat",
															  "Fear",
															  "Fly",
															  "Hit",
															  "Idle_A", "Idle_B", "Idle_C",
															  "Jump",
															  "Roll",
															  "Run",
															  "Sit",
															  "Spin/Splash",
															  "Swim",
															  "Walk"
															};
	private List<string> facialExpList = new List<string> {
															  "Eyes_Annoyed",
															  "Eyes_Blink",
															  "Eyes_Cry",
															  "Eyes_Dead",
															  "Eyes_Excited",
															  "Eyes_Happy",
															  "Eyes_LookDown",
															  "Eyes_LookIn",
															  "Eyes_LookOut",
															  "Eyes_LookUp",
															  "Eyes_Rabid",
															  "Eyes_Sad",
															  "Eyes_Shrink",
															  "Eyes_Sleep",
															  "Eyes_Spin",
															  "Eyes_Squint",
															  "Eyes_Trauma",
															  "Sweat_L",
															  "Sweat_R",
															  "Teardrop_L",
															  "Teardrop_R"
															};

	[Space(10)]
	[Tooltip("Assign: the game object where the animal are parented to")]
	public Transform animal_parent;
	public Dropdown dropdownAnimal;
	public Dropdown dropdownAnimation;
	public Dropdown dropdownFacialExp;

	void Start() {

		int count = animal_parent.childCount;
		animal = new GameObject[count];
		List<string> animalList = new List<string>();

		for(int i = 0; i< count; i++)
		{
			animal[i] = animal_parent.GetChild(i).gameObject;
			string n = animal_parent.GetChild(i).name;
			animalList.Add(n);
			// animalList.Add(n.Substring(0, n.IndexOf("_")));

			if(i==0) animal[i].SetActive(true);
			else animal[i].SetActive(false);
		}
		dropdownAnimal.AddOptions(animalList);
		dropdownAnimation.AddOptions(animationList);
		dropdownFacialExp.AddOptions(facialExpList);
		dropdownFacialExp.value = 1;
		ChangeExpression();

		Bounds b = animal[0].transform.GetChild(0).GetChild(0).GetComponent<Renderer>().bounds;
	}

	void Update() {

		if(Input.GetKeyDown("up")) { PrevAnimal(); }
		else if(Input.GetKeyDown("down")) { NextAnimal(); }
		else if(Input.GetKeyDown("right")) { NextAnimation(); }
		else if(Input.GetKeyDown("left")) { PrevAnimation(); }
	}


	public void NextAnimal() {

		if(dropdownAnimal.value >= dropdownAnimal.options.Count - 1)
			dropdownAnimal.value = 0;
		else
			dropdownAnimal.value++;

		ChangeAnimal();
	}

	public void PrevAnimal() {

		if(dropdownAnimal.value<= 0)
			dropdownAnimal.value = dropdownAnimal.options.Count - 1;
		else
			dropdownAnimal.value--;
		
		ChangeAnimal();
	}

	public void ChangeAnimal() {

		animal[animalIndex].SetActive(false);
		animal[dropdownAnimal.value].SetActive(true);
		animalIndex = dropdownAnimal.value;

		ChangeAnimation();
		ChangeExpression();
	}

	public void NextAnimation() {

		if(dropdownAnimation.value >= dropdownAnimation.options.Count - 1)
			dropdownAnimation.value = 0;
		else
			dropdownAnimation.value++;

		ChangeAnimation();
	}


	public void PrevAnimation() {

		if(dropdownAnimation.value<= 0)
			dropdownAnimation.value = dropdownAnimation.options.Count - 1;
		else
			dropdownAnimation.value--;
		
		ChangeAnimation();
	}

	public void ChangeAnimation() {

		GameObject a = animal[dropdownAnimal.value];

		int count = a.transform.childCount;
		for(int i = 0; i< count; i++)
		{
			if(a.GetComponent<Animator>() != null)
			{
				a.GetComponent<Animator>().Play(dropdownAnimation.options[dropdownAnimation.value].text);
			}
			else if(a.transform.GetChild(i).GetComponent<Animator>() != null)
			{
				a.transform.GetChild(i).GetComponent<Animator>().Play(dropdownAnimation.options[dropdownAnimation.value].text);
			}
		}
	}

	public void NextExpression() {

		if(dropdownFacialExp.value >= dropdownFacialExp.options.Count - 1)
			dropdownFacialExp.value = 0;
		else
			dropdownFacialExp.value++;

		ChangeExpression();
	}

	public void PrevExpression() {

		if(dropdownFacialExp.value<= 0)
			dropdownFacialExp.value = dropdownFacialExp.options.Count - 1;
		else
			dropdownFacialExp.value--;
		
		ChangeExpression();
	}

	public void ChangeExpression() {

		GameObject a = animal[dropdownAnimal.value];

		int count = a.transform.childCount;
		for(int i = 0; i< count; i++)
		{
			if(a.GetComponent<Animator>() != null)
			{
				a.GetComponent<Animator>().Play(dropdownFacialExp.options[dropdownFacialExp.value].text);
			}
			else if(a.transform.GetChild(i).GetComponent<Animator>() != null)
			{
				a.transform.GetChild(i).GetComponent<Animator>().Play(dropdownFacialExp.options[dropdownFacialExp.value].text);
			}
		}
	}

	public void GoToWebsite(string URL) {

		Application.OpenURL(URL);
	}
}