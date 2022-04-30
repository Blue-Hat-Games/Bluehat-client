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
        // ���õ� ������ ������
        // �� ������ renderer�� �����ͼ� material�� texture�� ������ �ؽ�ó�� ������
        // ������ ����ī�޶� �Ĵٺ��� �� 

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
                // 1. �̹� �ȼ��� ����� ���ΰ��� ���� ������
                var random = UnityEngine.Random.Range(0, 2);
                bool isChangeThisPixel = random == 0? false : true;

                UnityEngine.Color randomColor = color;
                if (isChangeThisPixel)
                {
                    // 2. 1���� true�� ��� ������ ���� ���� ������
                    var randomColorValue = UnityEngine.Random.Range(0.0f, 1.0f);

                    // 3. r,g,b, �� ��� ���� �ٲ� ���ΰſ� ���� ������
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

    // EncodeToPNG �Լ��� ����ϱ� ���� ���� ������ �ؽ�ó ������ �����ؾ� ��
    // �׸��� byte�� �����ؼ� string���� ������ �ְ� �ޱ� ���� �Լ� 
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

        // ������� string�� ���� �ؽ�ó�� �ε��ؼ� �ٽ� ����
        Texture2D loadImg = new Texture2D(testTexture.width, testTexture.height);
        loadImg.LoadImage(System.Convert.FromBase64String(myTextureBytesEncodedAsBase64));

        //testAnimal.material.SetTexture("_MainTex", loadImg);
    }




  
}
