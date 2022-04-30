using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ColorChangeManager : MonoBehaviour
{

    private GameObject targetAnimal;
    
    public void SetTargetAnimal(GameObject animal)
    {
        targetAnimal = animal;
    }

    private void Update()
    {

        if (Input.GetMouseButton(0))
        {
            if(targetAnimal == null)
            {
                return;
            }
            targetAnimal.transform.Rotate(0f, -Input.GetAxis("Mouse X") * 10, 0f, Space.World);
        }
    }


    void ShowColorChangingResult()
    {
        // 선택된 동물을 생성함
        // 그 동물의 renderer를 가져와서 material의 texture를 생성된 텍스처로 변경함
        // 동물은 메인카메라를 쳐다보게 함 

    }

    public void ChangeTextureColor()
    {
        Renderer animalMesh = targetAnimal.GetComponentInChildren<Renderer>();
        Texture2D originalTex = animalMesh.material.GetTexture("_MainTex") as Texture2D;
        Debug.Log("ChangeTextureColor");
        Texture2D resultTex = new Texture2D(originalTex.width, originalTex.height, TextureFormat.RGB24, false);
        UnityEngine.Color[] sourcePixels = originalTex.GetPixels();
        for (int h = 0; h < originalTex.height; h++)
        {
            for (int w = 0; w < originalTex.width; w++)
            {
                UnityEngine.Color color = sourcePixels[h * originalTex.width + w];
                // 1. 이번 픽셀을 변경될 것인가에 대한 랜덤값
                var random = UnityEngine.Random.Range(0, 2);
                bool isChangeThisPixel = random == 0? false : true;

                UnityEngine.Color randomColor = color;
                if (isChangeThisPixel)
                {
                    // 2. 1번이 true일 경우 변경할 색에 대한 랜덤값
                    var randomColorValue = UnityEngine.Random.Range(0.0f, 1.0f);

                    // 3. r,g,b, 중 어느 값을 바꿀 것인거에 대한 랜덤값
                    random = UnityEngine.Random.Range(0, 5);

                    switch (random)
                    {
                        case 0:
                            randomColor = new UnityEngine.Color(randomColorValue, color.g, color.b);
                            break;
                        case 1:
                            randomColor = new UnityEngine.Color(color.r, randomColorValue, color.b);
                            break;
                        case 2:
                            randomColor = new UnityEngine.Color(color.r, color.g, randomColorValue);
                            break;
                        default:
                            randomColor = new UnityEngine.Color(randomColorValue, randomColorValue, randomColorValue);
                            break;
                    }

                }
                resultTex.SetPixel(w, h, randomColor);     
            }
        }

        resultTex.Apply();
        animalMesh.material.SetTexture("_MainTex", resultTex);
        targetAnimal.transform.position = new Vector3(-2, -1, targetAnimal.transform.position.z); 
    }

    // EncodeToPNG 함수를 사용하기 위해 원래 에셋의 텍스처 포맷을 변경해야 함
    // 그리고 byte를 생성해서 string값을 서버와 주고 받기 위한 함수 
    void ChangeTextureFormat(Texture2D testTexture)
    {
        Texture2D tex = new Texture2D(testTexture.width, testTexture.height, TextureFormat.RGB24, false);
        Color[] sourcePixels = testTexture.GetPixels();

        for (int h = 0; h < testTexture.height; h++)
        {
            for (int w = 0; w < testTexture.width; w++)
            {
                Color color = sourcePixels[h * testTexture.width + w];
                tex.SetPixel(w, h, color);
            }
        }
        tex.Apply();

        byte[] myTextureBytes = tex.EncodeToPNG();


        string myTextureBytesEncodedAsBase64 = System.Convert.ToBase64String(myTextureBytes);
        Debug.Log("myTextureBytesEncodedAsBase64: " + myTextureBytesEncodedAsBase64);

        // 만들어진 string을 통해 텍스처를 로드해서 다시 적용
        Texture2D loadImg = new Texture2D(testTexture.width, testTexture.height);
        loadImg.LoadImage(System.Convert.FromBase64String(myTextureBytesEncodedAsBase64));

        //testAnimal.material.SetTexture("_MainTex", loadImg);
    }




  
}
