using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �ش� Ŭ������ AnimalFactory.cs ����� ���� ���� �׽�Ʈ Ŭ����
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
        // TODO: �׽�Ʈ ������ ����̹Ƿ� �����κ��� ���� jsonData�� Ȱ���ϵ��� �ڵ� ���� �ʿ�
        string jsonData = System.IO.File.ReadAllText("/Users/minjujuu/GitHub/Bluehat-project/Assets/Scripts/animalJsonData.txt");
        List<GameObject> animalObjectList = new List<GameObject>();

        // json data�� �ѱ�� �� �����͸� ���� ������ ���� ������Ʈ ����Ʈ�� ��ȯ ���� �� �ִ�
        animalObjectList = animalFactory.ConvertJsonToAnimalObject(jsonData);

        StartCoroutine(TakeScreenshot(animalObjectList));
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

    // UI�� ���� ���
    IEnumerator TakeScreenshot(List<GameObject> animalObjectList)
    {
        foreach(GameObject obj in animalObjectList)
        {
            yield return new WaitForEndOfFrame();
            // ����Ͽ� ������ �������� �ϱ� ���� 
            obj.gameObject.layer = LayerMask.NameToLayer("Animal");
            obj.transform.position = photoZone.position;
            obj.transform.rotation = photoZone.rotation;
            thumbnailCamera.Render();
            // UI set ����
            var uiSet = GameObject.Instantiate(nftMarketThumbnailUiPrefab);
            ToTexture2D(renderTexture, (Texture2D resultTex) =>
            {
                uiSet.GetComponent<RawImage>().texture = resultTex;
            });

            uiSet.transform.SetParent(scrollViewContent);
            obj.gameObject.SetActive(false);
        }
    }



}
