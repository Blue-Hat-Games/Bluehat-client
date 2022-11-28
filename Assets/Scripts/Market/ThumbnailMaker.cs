using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace BluehatGames
{
// 해당 클래스는 AnimalFactory.cs 사용방법 데모를 위한 테스트 클래스
    public class ThumbnailMaker : MonoBehaviour
    {
        public AnimalFactory animalFactory;
        public GameObject nftMarketThumbnailUiPrefab;

        public Camera thumbnailCamera;
        public RenderTexture renderTexture;
        public Transform photoZone;

        public Transform scrollViewContent;

        private void Start()
        {
            // TODO: 테스트 파일의 경로이므로 서버로부터 받은 jsonData를 활용하도록 코드 수정 필요
            var jsonData = File.ReadAllText("/Users/minjujuu/GitHub/Bluehat-project/Assets/Scripts/animalJsonData.txt");
            var animalObjectList = new Dictionary<string, GameObject>();

            // json data를 넘기면 그 데이터를 통해 생성된 동물 오브젝트 리스트를 반환 받을 수 있다
            animalObjectList = animalFactory.ConvertJsonToAnimalObject(jsonData);

            StartCoroutine(TakeScreenshot(animalObjectList));
        }

        private void ToTexture2D(RenderTexture rTex, Action<Texture2D> action)
        {
            var tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
            // ReadPixels looks at the active RenderTexture.
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            action.Invoke(tex);
        }

        // UI가 있을 경우
        private IEnumerator TakeScreenshot(Dictionary<string, GameObject> animalObjectDictionary)
        {
            foreach (var pair in animalObjectDictionary)
            {
                var obj = pair.Value;
                yield return new WaitForEndOfFrame();
                // 썸네일에 동물만 찍히도록 하기 위해 
                obj.gameObject.layer = LayerMask.NameToLayer("Animal");
                obj.transform.position = photoZone.position;
                obj.transform.rotation = photoZone.rotation;
                thumbnailCamera.Render();
                // UI set 생성
                var uiSet = Instantiate(nftMarketThumbnailUiPrefab);
                ToTexture2D(renderTexture, resultTex => { uiSet.GetComponent<RawImage>().texture = resultTex; });

                uiSet.transform.SetParent(scrollViewContent);
                obj.gameObject.SetActive(false);
            }
        }
    }
}