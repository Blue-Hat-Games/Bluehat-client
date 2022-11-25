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

        private int egg;

        private GameObject myNewAnimal;

        private void Start()
        {
            eggAlertPanel.SetActive(false);

            eggButton.onClick.AddListener(() =>
            {
                if (eggAlertPanel.activeSelf) return;
                if (eggResultPanel.activeSelf)
                {
                    eggResultPanel.SetActive(false);
                    Destroy(myNewAnimal);
                    return;
                }

                if (UserRepository.GetEgg() <= 0)
                {
                    StartCoroutine(ShowAlertPanel());
                    return;
                }

                StartCoroutine(GetNewAnimalFromServer(ApiUrl.postAnimalNew));
                UserRepository.SetEgg(UserRepository.GetEgg() - 1);
                eggText.text = UserRepository.GetEgg().ToString();
            });


            resultExitButton.onClick.AddListener(() =>
            {
                eggResultPanel.SetActive(false);
                Destroy(myNewAnimal);
            });
        }


        public IEnumerator GetNewAnimalFromServer(string URL)
        {
            using (var request = UnityWebRequest.Post(URL, ""))
            {
                request.downloadHandler = new DownloadHandlerBuffer();

                // send access token to server
                request.SetRequestHeader(ApiUrl.AuthGetHeader, AccessToken.GetAccessToken());

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
                }
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