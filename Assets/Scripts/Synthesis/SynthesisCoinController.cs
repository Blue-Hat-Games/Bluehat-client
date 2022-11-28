using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BluehatGames
{
    public class SynthesisCoinController : MonoBehaviour
    {
        public Text coinText;

        private void Start()
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
            using (var request = UnityWebRequest.Post(URL, ""))
            {
                request.SetRequestHeader(ApiUrl.AuthGetHeader, AccessToken.GetAccessToken());
                Debug.Log($"accessToken = {AccessToken.GetAccessToken()}");
                request.SetRequestHeader("Content-Type", "application/json");

                var requestData = new RequestCoinAndEggFormat();
                requestData.coin = "-1";
                requestData.egg = "0";

                var json = JsonUtility.ToJson(requestData);

                var bodyRaw = Encoding.UTF8.GetBytes(json);

                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log(request.downloadHandler.text);
                    var jsonData = request.downloadHandler.text;
                }

                request.Dispose();
            }
        }
    }
}