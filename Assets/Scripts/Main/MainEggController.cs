using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BluehatGames
{
    public class MainEggController : MonoBehaviour
    {
        public Text eggText;
        public Button eggButton;
        public GameObject eggResultPanel;

        public GameObject eggAlertPanel;

        public Camera overUICamera;
        public Transform newAnimalTr;

        public Button resultExitButton;
        public Text resultAnimalText;

        public bool isTestMode;

        private GameObject myNewAnimal;

        private void Start()
        {
            PlayerPrefs.SetInt(PlayerPrefsKey.key_AnimalEgg, 79);
            var myEggCount = PlayerPrefs.GetInt(PlayerPrefsKey.key_AnimalEgg);
            eggText.text = myEggCount.ToString();
            eggAlertPanel.SetActive(false);

            eggButton.onClick.AddListener(() =>
            {
                if (eggAlertPanel.activeSelf) return;
                if (myEggCount <= 0)
                {
                    StartCoroutine(ShowAlertPanel());
                    return;
                }

                StartCoroutine(GetNewAnimalFromServer(ApiUrl.postAnimalNew));
            });


            resultExitButton.onClick.AddListener(() =>
            {
                eggResultPanel.SetActive(false);
                Destroy(myNewAnimal);
            });
        }

        public IEnumerator GetNewAnimalFromServer(string URL)
        {
            using var request = UnityWebRequest.Post(URL, "");
            request.downloadHandler = new DownloadHandlerBuffer();

            // Access Token
            var access_token = PlayerPrefs.GetString(PlayerPrefsKey.key_accessToken);
            if (access_token == null || isTestMode)
            {
                Debug.Log("access_token is null. or test mode. access_token is set \"0000\"");
                access_token = "0000";
            }

            // send access token to server
            request.SetRequestHeader(ApiUrl.AuthGetHeader, access_token);

            yield return request.SendWebRequest();

            // error
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            // success
            else
            {
                var responseText = request.downloadHandler.text;
                var responseType = JsonUtility.FromJson<ResponseAnimalNew>(responseText).type;

                Debug.Log(request.downloadHandler.text);

                var animalName = responseType;
                resultAnimalText.text = $"{animalName}!";

                LoadAnimalPrefab(animalName);
                ShowResultPanel();

                // 알 개수 차감
                var originEggCount = PlayerPrefs.GetInt(PlayerPrefsKey.key_AnimalEgg);
                PlayerPrefs.SetInt(PlayerPrefsKey.key_AnimalEgg, originEggCount - 1);
                eggText.text = (originEggCount - 1).ToString();
            }
        }

        private void LoadAnimalPrefab(string animalName)
        {
            var path = $"Prefab/Animals/{animalName}";
            var obj = Resources.Load(path) as GameObject;
            var animal = Instantiate(obj, newAnimalTr.transform.position, Quaternion.identity);
            var overUILayer = LayerMask.NameToLayer("OverUI");
            animal.GetComponentInChildren<Renderer>().gameObject.layer = overUILayer;
            ResetAnimalState(animal);
            animal.transform.LookAt(overUICamera.transform);

            myNewAnimal = animal;
        }

        private void ResetAnimalState(GameObject animal)
        {
            animal.GetComponent<Rigidbody>().useGravity = false;
            animal.GetComponent<Rigidbody>().isKinematic = true;
            animal.GetComponent<CapsuleCollider>().enabled = false;
        }

        private void ShowResultPanel()
        {
            eggResultPanel.SetActive(true);
        }

        private IEnumerator ShowAlertPanel()
        {
            eggAlertPanel.SetActive(true);
            yield return new WaitForSeconds(3);
            eggAlertPanel.SetActive(false);
        }
    }
}