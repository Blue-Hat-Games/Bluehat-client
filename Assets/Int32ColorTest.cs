using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Int32ColorTest : MonoBehaviour
{
    public Texture2D sourceTex;
    public RawImage restoreTexImage;

    private Texture2D restoreTex;

    private int originColor32Length;

    private string GetColorJsonFromTexture(Texture2D tex)
    {
        Color32[] pix = tex.GetPixels32();
        Debug.Log($"origin tex length => {pix.Length}");
        originColor32Length = pix.Length;

        Color32[] fixedPix = new Color32[16];

        int index = 0;
        for(int i = 16; i < 47; i += 2)
        {
            Debug.Log($"{i} | pix = {pix[i]}");
            fixedPix[index++] = pix[i];
        }

        return JsonHelper.ToJson(fixedPix);
    }

    void Start()
    {
        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        for (int h = 0; h < sourceTex.height; h++)
        {
            for (int w = 0; w < sourceTex.width; w++)
            {              
                Color[] sourcePixels = sourceTex.GetPixels();
                Color color = sourcePixels[h * sourceTex.width + w]; 
                Debug.Log($"{w},{h} | color = {color}");
            }
        }   
        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

        string colorJsonStr = GetColorJsonFromTexture(sourceTex);
        Debug.Log(colorJsonStr);

        Debug.Log("------------------------------");

        // 22.9.18 json string을 다시 텍스처로 변환하는 테스트
        Color32[] colorFromJson = JsonHelper.FromJson<Color32>(colorJsonStr);
        for(int i = 0; i < colorFromJson.Length; i++)
        {
            Debug.Log($"{i} | pix = {colorFromJson[i]}");
        }

        // 다시 복원할 때
        Color32[] restoreTexColors = new Color32[originColor32Length];
        int index = 0;
        for(int i=0; i<restoreTexColors.Length; i++)
        {
            // 두번째 줄은 다시 처음으로 
            if(i == 16)
            {
                index = 0;
            }
            if(i == 48)
            {
                index = 8;
            }

            restoreTexColors[i] = colorFromJson[index];

            // 두칸마다 색 바꿔야 함 
            if(i % 2 != 0)
            {
                index ++;
            }
        }

   
        Texture2D restoreTexture = new Texture2D(sourceTex.width, sourceTex.height);
        Debug.Log($"sourceTex | width = {sourceTex.width}, height = {sourceTex.height}");
        
        Debug.Log($"restoreTexture | width = {restoreTexture.width}, height = {restoreTexture.height}");
        // restoreTexture.color = restoreTexColors;
        // index = 0;
        // for (int h = 0; h < restoreTexture.height; h++)
        // {
        //     for (int w = 0; w < restoreTexture.width; w++)
        //     {              
        //         Color color = restoreTexColors[index++];
        //         restoreTexture.SetPixel(w, h, color);                 
        //     }
        // }   

        restoreTexture.SetPixels32(restoreTexColors);
        restoreTexture.Apply(true);

        restoreTexImage.texture = restoreTexture;
        byte[] bytes = restoreTexture.EncodeToPNG();
        Debug.Log(Application.dataPath);
        var dirPath = Application.dataPath + "/SaveImages/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" + ".png", bytes);

        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        for (int h = 0; h < restoreTexture.height; h++)
        {
            for (int w = 0; w < restoreTexture.width; w++)
            {              
                Color[] sourcePixels = restoreTexture.GetPixels();
                Color color = sourcePixels[h * restoreTexture.width + w]; 
                Debug.Log($"{w},{h} | color = {color}");
            }
        }   
        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

      
    }

    void Update()
    {

    }
}