using UnityEngine;

namespace BluehatGames
{
    public class SelectedAnimalDataCupid : MonoBehaviour
    {
        private AnimalDataFormat selectedAnimalData;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void SetSelectedAnimalData(AnimalDataFormat animalData)
        {
            selectedAnimalData = animalData;
        }

        public string GetSelectedAnimalType()
        {
            return selectedAnimalData.animalType;
        }

        public void SetAnimalTexture(GameObject animalObject)
        {
            // 업데이트 할 동물의 오브젝트를 딕셔너리에서 가져옴
            var animal = new Animal(selectedAnimalData);
            // animal의 텍스처 변경
            var meshTex = animal.getAnimalTexture();
            animalObject.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", meshTex);
        }
    }
}