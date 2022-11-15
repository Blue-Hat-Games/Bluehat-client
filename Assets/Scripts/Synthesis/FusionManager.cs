using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace BluehatGames
{

    public class FusionManager : MonoBehaviour
    {
        public bool isTest;
        public string tempAccessToken;

        private SynthesisManager synthesisManager;

        private GameObject targetAnimal_1;

        private GameObject targetAnimal_2;

        private AnimalDataFormat selectedAnimalData_1;

        private AnimalDataFormat selectedAnimalData_2;

        private Texture2D[] referenceTextures;

        private GameObject resultAnimal;

        public Transform thumbnailSpot;

        public GameObject resultAnimalParticle;
        public void SetSynthesisManager(SynthesisManager instance)
        {
            synthesisManager = instance;
        }

        private void Start()
        {
            referenceTextures = new Texture2D[2];
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (resultAnimal == null)
                {
                    return;
                }
                resultAnimal
                    .transform
                    .Rotate(0f, -Input.GetAxis("Mouse X") * 10, 0f, Space.World);
            }
        }

        public void SetCurSelectedAnimal_1(AnimalDataFormat animalData, GameObject animalObject)
        {
            selectedAnimalData_1 = animalData;
            targetAnimal_1 = animalObject;
        }

        public void SetCurSelectedAnimal_2(AnimalDataFormat animalData, GameObject animalObject)
        {
            selectedAnimalData_2 = animalData;
            targetAnimal_2 = animalObject;
        }

        public void SendFusionAPI()
        {
            StartCoroutine(GetFusionResultFromServer(ApiUrl.postFusion));
        }

        public IEnumerator GetFusionResultFromServer(string URL)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Post(URL, ""))
            {
                webRequest.SetRequestHeader(ApiUrl.AuthGetHeader, AccessToken.GetAccessToken());
                webRequest.SetRequestHeader("Content-Type", "application/json");

                RequestFusionAnimalFormat requestData = new RequestFusionAnimalFormat();
                requestData.animalId1 = selectedAnimalData_1.id;
                requestData.animalId2 = selectedAnimalData_2.id;

                string json = JsonUtility.ToJson(requestData);
                Debug.Log(json);

                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

                yield return webRequest.SendWebRequest();

                long responseCode = webRequest.responseCode;
                string responseText = webRequest.downloadHandler.text;

                switch (responseCode)
                {
                    case 400:
                    case 419:
                        var responseMsg = JsonUtility.FromJson<ResponseResult>(responseText).msg;
                        Debug.Log($"responseCode 400 | msg = {responseMsg}");
                        break;
                    default:
                        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                        {
                            Debug.Log($"Error: {webRequest.error}");
                        }
                        else
                        {
                            var responseData = JsonUtility.FromJson<AnimalDataFormat>(responseText);
                            Debug.Log($"FusionManager | [{URL}] - {responseData}");

                            string resultAnimalId = responseData.id;
                            // refresh data
                            synthesisManager.SendRequestRefreshAnimalDataOnFusion(selectedAnimalData_1.id, selectedAnimalData_2.id, resultAnimalId);
                        }
                        break;
                }

            }
        }

        public void OnRefreshAnimalDataAfterFusion(GameObject resultAnimalObject, string resultAnimalId)
        {
            resultAnimalObject.transform.position = new Vector3(-2, -0.5f, resultAnimalObject.transform.position.z);
            resultAnimalObject.GetComponentInChildren<Animator>().speed = 0.3f;
            resultAnimal = resultAnimalObject;
            resultAnimal.transform.position = thumbnailSpot.transform.position;
            resultAnimal.transform.LookAt(Camera.main.transform);
            resultAnimal.transform.eulerAngles = new Vector3(0, resultAnimal.transform.eulerAngles.y, 0);

            synthesisManager.SetResultLoadingPanel(false);

            synthesisManager.TakeScreenshotForMarketPNG(resultAnimalId);
        }

        private GameObject tempParticle;
        public void CreateResultAnimalParticle()
        {
            Vector3 newPos = new Vector3(-2, 0, 0);
            tempParticle = Instantiate(resultAnimalParticle, newPos, Quaternion.identity);
            tempParticle.GetComponent<ParticleSystem>().Play();
        }

        private void DestroyParticle()
        {
            GameObject.Destroy(tempParticle);
        }

        public GameObject GetResultAnimal()
        {
            return resultAnimal;
        }

        public void ClearResultAnimal()
        {
            if (resultAnimal)
            {
                resultAnimal.SetActive(false);
                DestroyParticle();
            }
        }
    }
}
