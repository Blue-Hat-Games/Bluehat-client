using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
namespace BluehatGames {

    public class ColorChangeManager : MonoBehaviour
    {
        public bool isTest = false;

        private SynthesisManager synthesisManager;

        private GameObject targetAnimal;
        
        public Color tempColor1;

        public Color tempColor2;

        public Color tempColor3;

        public Color tempColor4;

        public AnimalDataFormat selectedAnimalData;

        public void SetSynthesisManager(SynthesisManager instance)
        {
            synthesisManager  = instance;
        }

        public void SetTargetAnimal(GameObject animal)
        {
            targetAnimal = animal;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (targetAnimal == null)
                {
                    return;
                }
                targetAnimal
                    .transform
                    .Rotate(0f, -Input.GetAxis("Mouse X") * 10, 0f, Space.World);
            }
        }

        public void SetCurSelectedAnimal(AnimalDataFormat animalData)
        {
            selectedAnimalData = animalData;
        }

        public void SendColorChangeAPI()
        {
            Debug.Log($"selected animal id = {selectedAnimalData.id}, selected animal Type = {selectedAnimalData.animalType}");
            StartCoroutine(GetColorChangeResultFromServer(ApiUrl.postChangeColor));
        }

        public IEnumerator GetColorChangeResultFromServer(string URL)
        {
            // Access Token
            string access_token = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
            Debug.Log($"access_token = {access_token}");

            // TODO: 테스트이면 0000 으로 
            if(isTest) access_token = "0000";

            using (UnityWebRequest webRequest = UnityWebRequest.Post(URL, ""))
            {
                webRequest.SetRequestHeader(ApiUrl.AuthGetHeader, access_token);
                webRequest.SetRequestHeader("Content-Type", "application/json");
                
                RequestColorChangeAnimalFormat requestData = new RequestColorChangeAnimalFormat();
                requestData.animalId = selectedAnimalData.id;

                string json = JsonUtility.ToJson(requestData);
                Debug.Log(json);

                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log($"Error: {webRequest.error}");
                }
                else
                {
                    Debug.Log($"Received: {webRequest.downloadHandler.text}");
                    string responseText = webRequest.downloadHandler.text;

                    var responseMsg = JsonUtility.FromJson<ResponseResult>(responseText).msg;
                    Debug.Log($"ColorChangeManager | [{URL}] - {responseMsg}");
                }
            }
        }

        public void ChangeTextureColor()
        {
            Renderer animalMesh = targetAnimal.GetComponentInChildren<Renderer>();
            Texture2D originalTex =
                animalMesh.material.GetTexture("_MainTex") as Texture2D;
            Debug.Log("ChangeTextureColor");
            Texture2D resultTex =
                new Texture2D(originalTex.width,
                    originalTex.height,
                    TextureFormat.RGB24,
                    false);
            UnityEngine.Color[] sourcePixels = originalTex.GetPixels();
            for (int h = 0; h < originalTex.height; h++)
            {
                for (int w = 0; w < originalTex.width; w++)
                {
                    UnityEngine.Color color =
                        sourcePixels[h * originalTex.width + w];

                    // 1. 이번 픽셀을 변경될 것인가에 대한 랜덤값
                    var random = UnityEngine.Random.Range(0, 10);
                    bool isChangeThisPixel = random > 4 ? false : true;

                    UnityEngine.Color randomColor = color;
                    if (isChangeThisPixel)
                    {
                        // 2. 1번이 true일 경우 변경할 색에 대한 랜덤값
                        var randomColorValue = UnityEngine.Random.Range(0.0f, 1.0f);
                        Debug.Log($"random Color Value = {randomColorValue}");

                        // 3. r,g,b, 중 어느 값을 바꿀 것인거에 대한 랜덤값
                        random = UnityEngine.Random.Range(0, 10);

                        Debug.Log($"Before => {randomColor}");
                        switch (random)
                        {
                            case 0:
                                randomColor = tempColor1;
                                break;
                            case 1:
                                randomColor = tempColor2;
                                break;
                            case 2:
                                randomColor = tempColor3;
                                break;
                            default:
                                randomColor = tempColor4;
                                break;
                        }
                        Debug.Log($"After => {randomColor}");
                    }

                    resultTex.SetPixel (w, h, randomColor);
                }
            }

            resultTex.Apply();
            
            animalMesh.material.SetTexture("_MainTex", resultTex);
            targetAnimal.transform.position =
                new Vector3(-2, -1, targetAnimal.transform.position.z);
        }

        void SetResultAnimalTexture()
        {
            // id를 가지고 결과 데이터를 다시 불러와서 동물의 텍스처를 적용해주어야 함 
            GameObject obj = synthesisManager.SendRequestRefreshAnimalData();
            
        }

        // EncodeToPNG 함수를 사용하기 위해 원래 에셋의 텍스처 포맷을 변경해야 함
        // 그리고 byte를 생성해서 string값을 서버와 주고 받기 위한 함수

        void ChangeTextureFormat(Texture2D testTexture)
        {
            Texture2D tex =
                new Texture2D(testTexture.width,
                    testTexture.height,
                    TextureFormat.RGB24,
                    false);
            Color[] sourcePixels = testTexture.GetPixels();

            for (int h = 0; h < testTexture.height; h++)
            {
                for (int w = 0; w < testTexture.width; w++)
                {
                    Color color = sourcePixels[h * testTexture.width + w];
                    tex.SetPixel (w, h, color);
                }
            }
            tex.Apply();

            byte[] myTextureBytes = tex.EncodeToPNG();

            string myTextureBytesEncodedAsBase64 =
                System.Convert.ToBase64String(myTextureBytes);
            Debug
                .Log("myTextureBytesEncodedAsBase64: " +
                myTextureBytesEncodedAsBase64);

            // 만들어진 string을 통해 텍스처를 로드해서 다시 적용
            Texture2D loadImg =
                new Texture2D(testTexture.width, testTexture.height);
            loadImg
                .LoadImage(System
                    .Convert
                    .FromBase64String(myTextureBytesEncodedAsBase64));

            //testAnimal.material.SetTexture("_MainTex", loadImg);
        }
    }
}
