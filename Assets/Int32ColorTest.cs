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

        // pix�� �ִ� ������ �� 64���ε� �� ĭ�� 4���� ���� ���� �׷� ������ 16ĭ�� ���� 16���� �ʿ���
        // �ؽ�ó���� �� ��°�ٺ��� �� ��°�ٱ����� ������ ��������
        // �׸��� 2���� ������ ���� �����;� �� 
        // �� �� ���� 16���ϱ� 16�� �پ���� �ͺ��� �����ؾ� ��

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
