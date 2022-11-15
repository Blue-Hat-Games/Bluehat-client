using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

namespace BluehatGames
{
    public class SynthesisCoinController : MonoBehaviour
    {
        public Text coinText;
        void Start()
        {
            Debug.Log("SynthesisCoinController Start");
            coinText.text = UserRepository.GetCoin().ToString();
            Debug.Log("SynthesisCoinController Start coinText.text : " + coinText.text);
        }

        public void SubAetherCount()
        {
            StartCoroutine(UpdateCoinData(ApiUrl.updateCoinAndEgg));
        }

        private IEnumerator UpdateCoinData(string URL)
        {
            using(UnityWebRequest request = UnityWebRequest.Post(URL, ""))
            {
                request.SetRequestHeader(ApiUrl.AuthGetHeader, AccessToken.GetAccessToken());
                Debug.Log($"accessToken = {AccessToken.GetAccessToken()}");
                request.SetRequestHeader("Content-Type", "application/json");

                RequestCoinAndEggFormat requestData = new RequestCoinAndEggFormat();
                requestData.coin = "-1";
                requestData.egg = "0";

                string json = JsonUtility.ToJson(requestData);

                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log(request.downloadHandler.text);
                    string jsonData = request.downloadHandler.text;
                }

                request.Dispose();
            }
        }
    }


}