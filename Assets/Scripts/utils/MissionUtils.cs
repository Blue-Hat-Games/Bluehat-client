using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace BluehatGames
{
    public class MissionUtils : MonoBehaviour
    {
        private string eventName;

        public void sendGetEggEvent(int eggCount)
        {
            eventName = "getEgg" + eggCount;
            Debug.Log("sendGetEggEvent : " + eventName);
            StartCoroutine(MissionEventCoroutine(eventName));
        }

        public void createWalletEvent()
        {
            eventName = "createWallet";
            Debug.Log("createWalletEvent");
            StartCoroutine(MissionEventCoroutine(eventName));
        }

        public void setNameEvent()
        {
            eventName = "addName";
            Debug.Log("setNameEvent");
            StartCoroutine(MissionEventCoroutine(eventName));
        }

        private IEnumerator MissionEventCoroutine(string eventName)
        {
            using (var request = UnityWebRequest.Post(ApiUrl.setCompleteQuest, ""))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Authorization", AccessToken.GetAccessToken());
                request.SetRequestHeader("Content-Type", "application/json");

                var json = "{\"event\":\"" + eventName + "\"}";
                var bodyRaw = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request);
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("Response: " + request.downloadHandler.text);
                }
            }
        }
    }
}