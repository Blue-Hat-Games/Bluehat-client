using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FusionManager : MonoBehaviour
{

    public GameObject listView;

    private GameObject targetAnimal_1;
    private GameObject targetAnimal_2;

    private Texture2D[] referenceTextures;
    private GameObject resultAnimal;
    private void Start()
    {
        referenceTextures = new Texture2D[2];
    }
    private void Update()
    {

        if (Input.GetMouseButton(0))
        {
            if (resultAnimal == null)
            {
                return;
            }
            resultAnimal.transform.Rotate(0f, -Input.GetAxis("Mouse X") * 10, 0f, Space.World);
        }
    }

    public void SetTargetAnimal(int index, GameObject animal)
    {
        if (index == 0)
        {
            Debug.Log($"1 SetTargetAnimal => {index}, animal => {animal.name}");
            targetAnimal_1 = animal;

            referenceTextures[0] = targetAnimal_1.GetComponentInChildren<Renderer>().material.GetTexture("_MainTex") as Texture2D;

        }
        else
        {
            Debug.Log($"2 SetTargetAnimal => {index}, animal => {animal.name}");
            targetAnimal_2 = animal;
            referenceTextures[1] = targetAnimal_2.GetComponentInChildren<Renderer>().material.GetTexture("_MainTex") as Texture2D;
        }
    }

    public void CreateFusionTexture()
    {
        int randomAnimal = Random.Range(0, 2);

        Texture2D formatTexture = referenceTextures[0];
        Texture2D resultTex = new Texture2D(formatTexture.width, formatTexture.height, TextureFormat.RGB24, false);

        for (int h = 0; h < formatTexture.height; h++)
        {
            for (int w = 0; w < formatTexture.width; w++)
            {
                int randomIdx = UnityEngine.Random.Range(0, referenceTextures.Length);
                Texture2D randomTexture = referenceTextures[randomIdx];
                Debug.Log($"selected randomTexture = {randomTexture.name}");
                Color[] sourcePixels = randomTexture.GetPixels();
                Color color = sourcePixels[h * formatTexture.width + w];
                resultTex.SetPixel(w, h, color);
            }
        }
        resultTex.Apply();

        if (randomAnimal == 0)
        {
            var obj = GameObject.Instantiate(targetAnimal_1);
            obj.name = "resultAnimal";

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.SetTexture("_MainTex", resultTex);
            }
            obj.transform.position = new Vector3(-2, -0.5f, obj.transform.position.z);
            obj.GetComponentInChildren<Animator>().speed = 0.3f;
            resultAnimal = obj;
        }
        else
        {
            var obj = GameObject.Instantiate(targetAnimal_2);
            obj.name = "resultAnimal";

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.SetTexture("_MainTex", resultTex);
            }
            obj.transform.position = new Vector3(-2, -0.5f, obj.transform.position.z);
            obj.GetComponentInChildren<Animator>().speed = 0.3f;
            resultAnimal = obj;
        }
    }

    public GameObject GetResultAnimal()
    {
        return resultAnimal;
    }
    public void ClearAnimals()
    {
        Destroy(resultAnimal);
    }
}
