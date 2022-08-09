using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Int32ColorTest : MonoBehaviour
{
    public Texture2D sourceTex;

    void Start()
    {
        Color32[] pix = sourceTex.GetPixels32();
        
        Color32[] fixedPix = new Color32[16];

        // pix에 있는 색상은 총 64개인데 한 칸당 4개의 값이 들어가서 그런 것으로 16칸의 값인 16개만 필요함
        // 텍스처에서 두 번째줄부터 세 번째줄까지만 색상을 가져오자
        // 그리고 2개씩 띄엄띄엄 값을 가져와야 함 
        // 맨 윗 줄은 16개니까 16개 뛰어넘은 것부터 시작해야 함

        int index = 0;
        for(int i = 16; i < 47; i += 2)
        {
            Debug.Log($"{i} | pix = {pix[i]}");
            fixedPix[index++] = pix[i];
        }
        Debug.Log(JsonHelper.ToJson(fixedPix));
        
    }

    void Update()
    {
        
    }
}
