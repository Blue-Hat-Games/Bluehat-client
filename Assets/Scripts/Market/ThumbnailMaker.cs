using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThumbnailMaker : MonoBehaviour
{
    public AnimalFactory animalFactory;
    public GameObject nftMarketThumbnailUiPrefab;

    public Camera thumbnailCamera;
    public RenderTexture renderTexture;
    public Transform photoZone;

    public Transform scrollViewContent;

    void Start()
    {
        string jsonData = System.IO.File.ReadAllText("/Users/minjujuu/GitHub/Bluehat-project/Assets/Scripts/animalJsonData.txt");
        List<GameObject> animalObjectList = new List<GameObject>();

        // json data를 넘기면 그 데이터를 통해 생성된 동물 오브젝트 리스트를 반환 받을 수 있다
        animalObjectList = animalFactory.ConvertJsonToAnimalObject(jsonData);
        StartCoroutine(TakeScreenshot(animalObjectList));
    }

    IEnumerator TakeScreenshot(List<GameObject> animalObjectList)
    {
        foreach(GameObject obj in animalObjectList)
        {
            yield return new WaitForEndOfFrame();
            // 썸네일에 동물만 찍히도록 하기 위해 
            obj.gameObject.layer = LayerMask.NameToLayer("Animal");
            obj.transform.position = photoZone.position;
            obj.transform.rotation = photoZone.rotation;
            thumbnailCamera.Render();
            // UI set 생성
            var uiSet = GameObject.Instantiate(nftMarketThumbnailUiPrefab);
            ToTexture2D(renderTexture, (Texture2D resultTex) =>
            {
                uiSet.GetComponent<RawImage>().texture = resultTex;
            });
            uiSet.transform.SetParent(scrollViewContent);
            obj.gameObject.SetActive(false);
        }
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
