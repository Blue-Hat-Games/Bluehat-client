using UnityEngine;

namespace BluehatGames
{
    public class Animal
    {
        private readonly string animalPrefabPath = "Prefab/Animals";
        private readonly int formateTextuerPixelsLength = 64;
        private readonly int formatTextureHeight = 4;

        private readonly int formatTextureWidth = 16;
        public string animalType;
        public Color32[] color;
        public string headItem;
        public string id;

        public string name;
        public string pattern;
        public int tier;

        public Animal(AnimalDataFormat animalData)
        {
            name = animalData.name;
            tier = animalData.tier;
            id = animalData.id;
            animalType = animalData.animalType;
            headItem = animalData.headItem;
            pattern = animalData.pattern;
            setAnimalColor(animalData.color);
        }


        // Setting Animal Color from Json Color
        public void setAnimalColor(string jsonColor)
        {
            color = jsonColor2Color32(jsonColor);
        }

        // Get Aniaml Color Texture
        public Texture2D getAnimalTexture()
        {
            var texture = color2Texture(color, formatTextureWidth, formatTextureHeight);
            return texture;
        }

        // Get Animal Prefab from Animal Type
        public GameObject getAnimalPrefab()
        {
            var path = $"{animalPrefabPath}/{animalType}";
            var prefab = Resources.Load<GameObject>(path);
            return prefab;
        }

        // Get Animal Name
        public string getAnimalName()
        {
            return name;
        }

        // Set Animal Type
        public void setAnimalType(string animalType)
        {
            this.animalType = animalType;
        }

        private Color32[] jsonColor2Color32(string jsonColor)
        {
            var colorJsonStr = "{\"data\":" + jsonColor + "}";
            var originColor32Length = formateTextuerPixelsLength;
            var colorFromJson = JsonHelper.FromJson<Color32>(colorJsonStr);
            var restoreTexColors = new Color32[originColor32Length];

            var index = 0;
            for (var i = 0; i < restoreTexColors.Length; i++)
            {
                index = i switch
                {
                    16 => 0,
                    48 => 8,
                    _ => index
                };

                restoreTexColors[i] = colorFromJson[index];

                if (i % 2 != 0) index++;
            }

            return restoreTexColors;
        }

        private Texture2D color2Texture(Color32[] color, int width, int height)
        {
            var texture = new Texture2D(width, height);
            texture.SetPixels32(color);
            texture.Apply(true);
            return texture;
        }
    }
}