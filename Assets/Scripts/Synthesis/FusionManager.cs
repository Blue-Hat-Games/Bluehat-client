using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace BluehatGames
{
    public class FusionManager : MonoBehaviour
    {
        public bool isTest;
        public string tempAccessToken;

        public Transform thumbnailSpot;
        public GameObject thumbnailCamera;

        public GameObject resultAnimalParticle;

        private Texture2D[] referenceTextures;

        private GameObject resultAnimal;

        private AnimalDataFormat selectedAnimalData_1;

        private AnimalDataFormat selectedAnimalData_2;

        private SynthesisManager synthesisManager;

        private GameObject targetAnimal_1;

        private GameObject targetAnimal_2;

        private GameObject tempParticle;

        private void Start()
        {
            referenceTextures = new Texture2D[2];
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (resultAnimal == null) return;
                resultAnimal
                    .transform
                    .Rotate(0f, -Input.GetAxis("Mouse X") * 10, 0f, Space.World);
            }
        }

        public void SetSynthesisManager(SynthesisManager instance)
        {
            synthesisManager = instance;
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
            using (var webRequest = UnityWebRequest.Post(URL, ""))
            {
                webRequest.SetRequestHeader(ApiUrl.AuthGetHeader, AccessToken.GetAccessToken());
                webRequest.SetRequestHeader("Content-Type", "application/json");

                var requestData = new RequestFusionAnimalFormat();
                requestData.animalId1 = selectedAnimalData_1.id;
                requestData.animalId2 = selectedAnimalData_2.id;

                var json = JsonUtility.ToJson(requestData);
                Debug.Log(json);

                var bodyRaw = Encoding.UTF8.GetBytes(json);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                yield return webRequest.SendWebRequest();

                var responseCode = webRequest.responseCode;
                var responseText = webRequest.downloadHandler.text;

                switch (responseCode)
                {
                    case 400:
                    case 419:
                        var responseMsg = JsonUtility.FromJson<ResponseResult>(responseText).msg;
                        Debug.Log($"responseCode 400 | msg = {responseMsg}");
                        break;
                    default:
                        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                            webRequest.result == UnityWebRequest.Result.ProtocolError)
                        {
                            Debug.Log($"Error: {webRequest.error}");
                        }
                        else
                        {
                            var responseData = JsonUtility.FromJson<AnimalDataFormat>(responseText);
                            Debug.Log($"FusionManager | [{URL}] - {responseData}");

                            var resultAnimalId = responseData.id;
                            // refresh data
                            synthesisManager.SendRequestRefreshAnimalDataOnFusion(selectedAnimalData_1.id,
                                selectedAnimalData_2.id, resultAnimalId);
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
            resultAnimal.transform.rotation = thumbnailSpot.transform.rotation;

            // resultAnimal.transform.LookAt(thumbnailCamera.transform);
            resultAnimal.transform.eulerAngles = new Vector3(0, resultAnimal.transform.eulerAngles.y, 0);

            synthesisManager.SetResultLoadingPanel(false);

            synthesisManager.TakeScreenshotForMarketPNG(resultAnimalId);
        }

        public void CreateResultAnimalParticle()
        {
            var newPos = new Vector3(-2, 0, 0);
            tempParticle = Instantiate(resultAnimalParticle, newPos, Quaternion.identity);
            tempParticle.GetComponent<ParticleSystem>().Play();
        }

        private void DestroyParticle()
        {
            Destroy(tempParticle);
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