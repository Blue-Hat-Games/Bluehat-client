using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BluehatGames {
public class MyAnimalListController : MonoBehaviour
{
    public string[] testAnimalList = { "Zebra", "Flamingo", "Cheetah" };
    [Header("Common UI")]
    public GameObject animalListView;
    public Transform animalListContentsView;
    public GameObject animalListContentPrefab;
    [Header("AnimalListThumbnail")]
    public Camera thumbnailCamera;
    public RenderTexture renderTexture;
    public Transform thumbnailSpot;
     private GameObject[] contentUis;
    // Start is called before the first frame update
    void Start()
    {
        contentUis = new GameObject[testAnimalList.Length];
        StartCoroutine(MakeThumbnailAnimalList());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator MakeThumbnailAnimalList()
    {
        animalListView.SetActive(true);
        for (int i = 0; i < testAnimalList.Length; i++)
        {
            int index = i;
            string animalName = testAnimalList[i];
            var animalPrefab = LoadAnimalPrefab(testAnimalList[i], thumbnailSpot.position, thumbnailCamera.gameObject);

            yield return new WaitForEndOfFrame();
            thumbnailCamera.Render();

            var uiSet = GameObject.Instantiate(animalListContentPrefab);
            contentUis[index] = uiSet;
            //penguinUiSetList[i] = uiSet;
            ToTexture2D(renderTexture, (Texture2D resultTex) =>
            {
                uiSet.GetComponent<RawImage>().texture = resultTex;
            });
            uiSet.GetComponent<Button>().onClick.AddListener(() => {
                PlayerPrefs.SetString(PlayerPrefsKey.key_multiplayAnimal, animalName);
            });
            uiSet.GetComponentInChildren<Text>().text = testAnimalList[i];
            uiSet.transform.SetParent(animalListContentsView);
            Destroy(animalPrefab);
        }
    }

    private GameObject LoadAnimalPrefab(string animalName, Vector3 position, GameObject lookAtTarget)
    {
        var path = $"Prefab/Animals/{animalName}";
        GameObject obj = Resources.Load(path) as GameObject;
        GameObject animal = Instantiate(obj, position, Quaternion.identity);
       
        animal.transform.LookAt(lookAtTarget.transform);

        Debug.Log($"Creating Animal is Success! => {animalName}");
        return animal;
    }

    void ToTexture2D(RenderTexture rTex, Action<Texture2D> action)
    {
        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        action.Invoke(tex);
    }

}
}