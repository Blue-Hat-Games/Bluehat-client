using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace BluehatGames
{
    public class AccessoryManager : MonoBehaviour
    {
        public AnimalDataFormat selectedAnimalData;
        public GameObject selectedAnimalObject;

        public GameObject hatParticle;

        private Transform curHatPoint;
        private GameObject resultAnimal;
        private SynthesisManager synthesisManager;

        private GameObject tempParticle;

        private void Start()
        {
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
                if (selectedAnimalObject != null)
                    selectedAnimalObject
                        .transform
                        .Rotate(0f, -Input.GetAxis("Mouse X") * 10, 0f, Space.World);

            if (resultAnimal != null)
                resultAnimal.transform.Rotate(0f, -Input.GetAxis("Mouse X") * 10, 0f, Space.World);
        }

        public void SetSynthesisManager(SynthesisManager instance)
        {
            synthesisManager = instance;
        }

        public void SendRandomHatAPI()
        {
            StartCoroutine(GetRandomHatResultFromServer(ApiUrl.getRandomHat));
        }

        public IEnumerator GetRandomHatResultFromServer(string URL)
        {
            var request = UnityWebRequest.Get(URL);

            using (var webRequest = UnityWebRequest.Post(URL, ""))
            {
                webRequest.SetRequestHeader(ApiUrl.AuthGetHeader, AccessToken.GetAccessToken());
                webRequest.SetRequestHeader("Content-Type", "application/json");

                var requestData = new RequestRandomHatFormat();
                requestData.animalId = selectedAnimalData.id;

                var json = JsonUtility.ToJson(requestData);
                Debug.Log(json);

                var bodyRaw = Encoding.UTF8.GetBytes(json);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log($"Error: {webRequest.error}");
                }
                else
                {
                    var responseText = webRequest.downloadHandler.text;

                    var new_item = JsonUtility.FromJson<ResponseHatResult>(responseText).new_item;
                    Debug.Log($"AccessoryManager | [{URL}] - new_item = {new_item}");
                    LoadHatItemPrefab(new_item);
                    synthesisManager.SetResultLoadingPanel(false);

                    // refresh data
                    synthesisManager.SendRequestRefreshAnimalData(selectedAnimalData.id, false);
                }

                webRequest.Dispose();
            }
        }

        private void LoadHatItemPrefab(string itemName)
        {
            var path = $"Prefab/Hats/{itemName}";
            var obj = Resources.Load(path) as GameObject;
            var hatObj = Instantiate(obj);
            var allChildren = selectedAnimalObject.GetComponentsInChildren<Transform>();
            Transform hatPoint = null;

            foreach (var childTr in allChildren)
                if (childTr.name == "HatPoint")
                    hatPoint = childTr;

            curHatPoint = hatPoint;

            if (hatPoint.childCount > 0) Destroy(hatPoint.GetChild(0).gameObject);
            hatObj.transform.SetParent(hatPoint);
            hatObj.transform.localPosition = Vector3.zero;
            hatObj.transform.localEulerAngles = Vector3.zero;
        }

        public void CreateHatParticle()
        {
            var newPos = new Vector3(-2, curHatPoint.position.y, 0);
            tempParticle = Instantiate(hatParticle, newPos, Quaternion.identity);
            tempParticle.GetComponent<ParticleSystem>().Play();
            Invoke("DestroyParticle", 2.0f);
        }

        private void DestroyParticle()
        {
            Destroy(tempParticle);
        }

        public void SetCurSelectedAnimal(AnimalDataFormat animalData, GameObject animalObject)
        {
            selectedAnimalData = animalData;
            selectedAnimalObject = animalObject;
        }
    }
}