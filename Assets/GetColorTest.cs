using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Drawing;
using UnityEditor;
using UnityEngine.Networking;

public class GetColorTest : MonoBehaviour
{
    public string URL = "192.168.84.169:3000/animal";
    public Texture2D huskyTex;
    private Texture2D huskyTex2;

    public RawImage resultTex;
    private string directoryPath = "Assets/Resources/NewTextures/";
    public string fileName = "test";

    public Renderer huskyMesh_j;

    public IEnumerator DownLoadGet(string URL)
    {
        UnityWebRequest request = UnityWebRequest.Get(URL);

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            } else {
                Debug.Log(request.downloadHandler.data);
            }
    }

    
    void Start()
    {
        StartCoroutine(DownLoadGet(URL));
        // ------------------------------------------------------------------
        huskyTex2 = new Texture2D(huskyTex.width, huskyTex.height, TextureFormat.ARGB32, false);
        UnityEngine.Color[] sourcePixels = huskyTex.GetPixels();
        for (int h = 0; h < huskyTex.height; h++)
        {
            for (int w = 0; w < huskyTex.width; w++)
            {
                UnityEngine.Color color = sourcePixels[h * huskyTex.width + w];
                color = new UnityEngine.Color(1, color.g * 0.5f, color.b * 0.5f, 1);
                huskyTex2.SetPixel(w, h, color);    
              
                Debug.Log($"Color = {color.r*255}, ${color.g*255}, ${color.b*255}");
                int r = (int)color.r*255;
                int g = (int)color.g*255;
                int b = (int)color.b*255;
                
                // System.Drawing.Color myColor = System.Drawing.Color.FromArgb(r, g, b);
                // string hex = myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2");
                // Debug.Log($"hex : ${hex}");
            }
        }
        huskyTex2.Apply();
        resultTex.texture = huskyTex2;
        huskyMesh_j.material.SetTexture("_MainTex", huskyTex2);
        // 새로 생성된 텍스처를 로컬에 저장
        //SaveTexture2DToPNGFile(huskyTex2, directoryPath, fileName);
        // 쉐이더 프로퍼티 출력
        string[] shaderPropertyTypes = new string[] { "Color", "Vector", "Float", "Range", "Texture" };
        int propertyCount = ShaderUtil.GetPropertyCount(huskyMesh_j.material.shader);

        for (int index = 0; index < propertyCount; ++index)
        {
            Debug.Log(ShaderUtil.GetPropertyName(huskyMesh_j.material.shader, index) + "      "
            + shaderPropertyTypes[(int)ShaderUtil.GetPropertyType(huskyMesh_j.material.shader, index)]);
        }

    }
     private void SaveTexture2DToPNGFile(Texture2D texture, string directoryPath, string fileName)
    {
        
        if(false == Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        byte[] texturePNGBytes = texture.EncodeToPNG();

        string filePath = directoryPath + fileName + ".png";
        File.WriteAllBytes(filePath, texturePNGBytes);
 
    }
}
