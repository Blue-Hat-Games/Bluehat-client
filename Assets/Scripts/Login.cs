using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerInfo {
    public string email;
}
public class Login : MonoBehaviour
{
    public Button btn_upload;

    public InputField inputEmail;
    public string URL;
    void Start()
    {
        btn_upload.onClick.AddListener(() =>
        {
            StartCoroutine(Upload(URL, inputEmail.text));
        });
    }

    IEnumerator Upload(string URL, string inputEmail)
    {
        PlayerInfo playerInfo = new PlayerInfo();

        playerInfo.email = inputEmail;
        if (inputEmail == null)
        {
            Debug.LogError("Email is null");
        }

        //Tranform it to Json object
        string jsonData = JsonUtility.ToJson(playerInfo);

        Debug.Log(jsonData);
        
        byte[] byteEmail = Encoding.UTF8.GetBytes(jsonData);
        // 웹서버로 Post 요청을 보냄
        using (UnityWebRequest request = UnityWebRequest.Post(URL, jsonData))
        {
            request.uploadHandler = new UploadHandlerRaw(byteEmail); // 업로드 핸들러
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer(); // 다운로드 핸들러
            // 헤더를 Json으로 설정
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            else
            {
            	// 웹서버로부터 받은 응답 내용 출력
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
}
