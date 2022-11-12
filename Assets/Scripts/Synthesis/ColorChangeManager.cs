using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
namespace BluehatGames
{

    public class ColorChangeManager : MonoBehaviour
    {
        private SynthesisManager synthesisManager;

        private GameObject selectedAnimalObject;

        public AnimalDataFormat selectedAnimalData;

        private GameObject resultAnimal;
        public GameObject resultAnimalParticle;
        public Transform thumbnailSpot;

        public void SetSynthesisManager(SynthesisManager instance)
        {
            synthesisManager = instance;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (selectedAnimalObject != null)
                {
                    selectedAnimalObject
                    .transform
                    .Rotate(0f, -Input.GetAxis("Mouse X") * 10, 0f, Space.World);
                }

                if (resultAnimal != null)
                {
                    resultAnimal.transform.Rotate(0f, -Input.GetAxis("Mouse X") * 10, 0f, Space.World);
                }
            }
        }

        public void ClearResultAnimal()
        {
            if (resultAnimal)
            {
                resultAnimal.SetActive(false);
                DestroyParticle();
            }
            if (selectedAnimalObject)
            {
                selectedAnimalObject.SetActive(false);
            }
        }

        public void SetCurSelectedAnimal(AnimalDataFormat animalData, GameObject animalObject)
        {
            selectedAnimalData = animalData;
            selectedAnimalObject = animalObject;

        }

        public void SendColorChangeAPI()
        {
            StartCoroutine(GetColorChangeResultFromServer(ApiUrl.postChangeColor));
        }

        public IEnumerator GetColorChangeResultFromServer(string URL)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Post(URL, ""))
            {
                webRequest.SetRequestHeader(ApiUrl.AuthGetHeader, AccessToken.GetAccessToken());
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
                    string responseText = webRequest.downloadHandler.text;

                    var responseMsg = JsonUtility.FromJson<ResponseResult>(responseText).msg;
                    Debug.Log($"ColorChangeManager | [{URL}] - {responseMsg}");

                    // refresh data
                    synthesisManager.SendRequestRefreshAnimalData(selectedAnimalData.id, true);
                }
            }
        }

        public void OnRefreshAnimalDataAfterColorChange()
        {
            GameObject obj = synthesisManager.GetAnimalObject(selectedAnimalData.id);
            obj.transform.position = this.thumbnailSpot.transform.position;

            obj.SetActive(true);
            resultAnimal = obj;

            // obj.transform.position = new Vector3(-2, -1, obj.transform.position.z);
            CreateResultAnimalParticle(obj.transform);
            obj.transform.LookAt(Camera.main.transform);
            obj.transform.eulerAngles = new Vector3(0, obj.transform.eulerAngles.y, 0);

            synthesisManager.SetResultLoadingPanel(false);

        }

        private GameObject tempParticle;
        private void CreateResultAnimalParticle(Transform particlePoint)
        {
            Vector3 newPos = new Vector3(particlePoint.position.x, particlePoint.position.y + 0.5f, particlePoint.position.z);
            tempParticle = Instantiate(resultAnimalParticle, newPos, Quaternion.identity);
            tempParticle.GetComponent<ParticleSystem>().Play();
        }

        private void DestroyParticle()
        {
            GameObject.Destroy(tempParticle);
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
                    tex.SetPixel(w, h, color);
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
