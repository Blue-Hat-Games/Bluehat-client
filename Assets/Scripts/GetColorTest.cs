using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetColorTest : MonoBehaviour
{
    public string URL = "192.168.84.169:3000/animal";
    public Texture2D huskyTex;

    public RawImage resultTex;
    public string fileName = "test";

    public Renderer huskyMesh_j;
    private Texture2D huskyTex2;


    private void Start()
    {
        StartCoroutine(DownLoadGet(URL));
        // ------------------------------------------------------------------
        huskyTex2 = new Texture2D(huskyTex.width, huskyTex.height, TextureFormat.ARGB32, false);
        var sourcePixels = huskyTex.GetPixels();
        for (var h = 0; h < huskyTex.height; h++)
        for (var w = 0; w < huskyTex.width; w++)
        {
            var color = sourcePixels[h * huskyTex.width + w];
            color = new Color(1, color.g * 0.5f, color.b * 0.5f, 1);
            huskyTex2.SetPixel(w, h, color);

            Debug.Log($"Color = {color.r * 255}, ${color.g * 255}, ${color.b * 255}");
            var r = (int)color.r * 255;
            var g = (int)color.g * 255;
            var b = (int)color.b * 255;
            // System.Drawing.Color myColor = System.Drawing.Color.FromArgb(r, g, b);
            // string hex = myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2");
            // Debug.Log($"hex : ${hex}");
        }

        huskyTex2.Apply();
        resultTex.texture = huskyTex2;
        huskyMesh_j.material.SetTexture("_MainTex", huskyTex2);
        // 새로 생성된 텍스처를 로컬에 저장
        //SaveTexture2DToPNGFile(huskyTex2, directoryPath, fileName);
        // 쉐이더 프로퍼티 출력
        //string[] shaderPropertyTypes = new string[] { "Color", "Vector", "Float", "Range", "Texture" };
        //int propertyCount = ShaderUtil.GetPropertyCount(huskyMesh_j.material.shader);

        //for (int index = 0; index < propertyCount; ++index)
        //{
        //    Debug.Log(ShaderUtil.GetPropertyName(huskyMesh_j.material.shader, index) + "      "
        //    + shaderPropertyTypes[(int)ShaderUtil.GetPropertyType(huskyMesh_j.material.shader, index)]);
        //}
    }

    public IEnumerator DownLoadGet(string URL)
    {
        var request = UnityWebRequest.Get(URL);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
            Debug.Log(request.error);
        else
            Debug.Log(request.downloadHandler.data);
    }

    private void SaveTexture2DToPNGFile(Texture2D texture, string directoryPath, string fileName)
    {
        if (false == Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        var texturePNGBytes = texture.EncodeToPNG();

        var filePath = directoryPath + fileName + ".png";
        File.WriteAllBytes(filePath, texturePNGBytes);
    }
}