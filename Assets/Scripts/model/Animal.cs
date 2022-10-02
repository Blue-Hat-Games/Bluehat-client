using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Animal
{

    public Texture2D formatTexture;
    private string animalPrefabPath = "Prefab/Animals";

    public string name;
    public int tier;
    public Color32[] color;
    public string id;
    public string animalType;
    public string headItem;
    public string pattern;

    public Animal(string name, int tier, string id, string animalType, string headItem, string pattern, Texture2D formatTexture)
    {
        this.name = name;
        this.tier = tier;
        this.id = id;
        this.animalType = animalType;
        this.headItem = headItem;
        this.pattern = pattern;
        this.formatTexture = formatTexture;
    }


    // Setting Animal Color from Json Color
    public void setAnimalColor(string jsonColor)
    {
        this.color = this.jsonColor2Color32(jsonColor);
    }

    // Get Aniaml Color Texture
    public Texture2D getAnimalTexture()
    {
        Texture2D texture = color2Texture(this.color, formatTexture.width, formatTexture.height);
        return texture;
    }

    // Get Animal Prefab from Animal Type
    public GameObject getAnimalPrefab()
    {
        string path = $"{animalPrefabPath}/{this.animalType}";
        GameObject prefab = Resources.Load<GameObject>(path);
        return prefab;
    }

    // Get Animal Name
    public string getAnimalName()
    {
        return this.name;
    }

    // Set Animal Type
    public void setAnimalType(string animalType)
    {
        this.animalType = animalType;
    }

    private Color32[] jsonColor2Color32(string jsonColor)
    {
        string colorJsonStr = "{\"data\":" + jsonColor + "}";
        Color32[] pix = formatTexture.GetPixels32();
        int originColor32Length = pix.Length;
        Color32[] colorFromJson = JsonHelper.FromJson<Color32>(colorJsonStr);
        Color32[] restoreTexColors = new Color32[originColor32Length];

        int index = 0;
        for (int i = 0; i < restoreTexColors.Length; i++)
        {
            if (i == 16)
            {
                index = 0;
            }
            if (i == 48)
            {
                index = 8;
            }

            restoreTexColors[i] = colorFromJson[index];

            if (i % 2 != 0)
            {
                index++;
            }
        }
        return restoreTexColors;

    }

    private Texture2D color2Texture(Color32[] color, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels32(color);
        texture.Apply(true);
        return texture;
    }
}
