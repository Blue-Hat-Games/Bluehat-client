using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace BluehatGames
{
    public class MyAnimalListController : MonoBehaviour
    {
        public ObjectPool contentUiPool;
        public AnimalFactory animalFactory;

        public AnimalAirController animalAirController;
        [Header("Common UI")]
        public GameObject animalListView;
        public Transform animalListContentsView;
        public GameObject LoadingPanel;
        public GameObject animalListContentPrefab;
        private Dictionary<string, GameObject> contentUiDictionary;
        public RawImage selectedAnimalImage;

        [Header("AnimalListThumbnail")]
        public Camera thumbnailCamera;
        public RenderTexture renderTexture;
        public Transform thumbnailSpot;
        public GameObject[] animalObjectArray;
        private GameObject activeAnimalObject;

        private int poolSize = 30; // 얼만큼일지 모르지만 이만큼은 만들어놓자

        void Start()
        {
            selectedAnimalImage.gameObject.SetActive(false);
            InitContentViewUIObjectPool();
            contentUiDictionary = new Dictionary<string, GameObject>();
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (activeAnimalObject != null)
                {

                    activeAnimalObject
                    .transform
                    .Rotate(0f, -Input.GetAxis("Mouse X") * 10, 0f, Space.World);
                }
            }
                
        }

        private void InitContentViewUIObjectPool()
        {
            contentUiPool.Init(poolSize, animalListContentPrefab, animalListContentsView);
        }


        // AnimalAirController에서 호출하는 함수
        public void StartMakeThumbnailAnimalList(Dictionary<string, GameObject> animalObjectDictionary, AnimalDataFormat[] animalDataArray)
        {
            StartCoroutine(MakeThumbnailAnimalList(animalObjectDictionary, animalDataArray));
        }


        IEnumerator MakeThumbnailAnimalList(Dictionary<string, GameObject> animalObjectDictionary, AnimalDataFormat[] animalDataArray)
        {
            animalListView.SetActive(true);
            animalObjectArray = new GameObject[animalDataArray.Length];

            int index = 0;
            for (int i = 0; i < animalObjectDictionary.Count; i++)
            {
                int curIdx = index;
                GameObject animalObject = animalObjectDictionary.Values.ToList()[curIdx];
                Debug.Log($"animalDataArray.length = {animalDataArray.Length}, contentUiPool.size = {contentUiPool.GetPoolSize()}");
                contentUiDictionary.Add(animalDataArray[i].id, contentUiPool.GetObject());

                animalObject.transform.position = thumbnailSpot.position;
                animalObject.transform.rotation = thumbnailSpot.rotation;

                animalObjectArray[curIdx] = animalObject;
                animalObject.name = $"{animalObject.name}";
                ResetAnimalState(animalObject);

                thumbnailCamera.Render();

                var uiSet = contentUiDictionary[animalDataArray[i].id];
                uiSet.name = $"{animalDataArray[curIdx].animalType}_{animalDataArray[curIdx].id}";

                yield return new WaitForEndOfFrame();

                if(animalObjectArray[curIdx]) {
                    animalObjectArray[curIdx].SetActive(false);
                }

                ToTexture2D(renderTexture, (Texture2D resultTex) =>
                {
                    uiSet.GetComponentInChildren<RawImage>().texture = resultTex;
                    uiSet.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if(activeAnimalObject)
                        {
                            activeAnimalObject.SetActive(false);
                        }
                        // 선택한 동물 활성화
                        animalObject.SetActive(true);
                        activeAnimalObject = animalObject;
                        GameObject.FindObjectOfType<PhotonManager>().SetconnectButtonActive(true);

                        GameObject.FindObjectOfType<SelectedAnimalDataCupid>().SetSelectedAnimalData(animalDataArray[curIdx]);
                        // id를 저장
                        PlayerPrefs.SetString(PlayerPrefsKey.key_multiplayAnimal, animalDataArray[curIdx].id);
                        Debug.Log($"PlayerPrefsKey.key_multiplayAnimal => {animalDataArray[curIdx].id}");
                    });
                
                    uiSet.GetComponentInChildren<Text>().text = animalDataArray[curIdx].animalType;
                    uiSet.transform.SetParent(animalListContentsView, false);
                });
                index++;
            }

            LoadingPanel.SetActive(false);
            selectedAnimalImage.gameObject.SetActive(true);

        }

        void ResetAnimalState(GameObject animal)
        {
            animal.GetComponent<Rigidbody>().useGravity = false;
            animal.GetComponent<Rigidbody>().isKinematic = true;
            animal.GetComponent<CapsuleCollider>().enabled = false;
        }

        void ToTexture2D(RenderTexture rTex, Action<Texture2D> action)
        {
            Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
            // ReadPixels looks at the active RenderTexture.
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            action.Invoke(tex);
        }

        // ----------------------------------

        // IEnumerator MakeThumbnailAnimalList()
        // {
        //     animalListView.SetActive(true);
        //     for (int i = 0; i < testAnimalList.Length; i++)
        //     {
        //         int index = i;
        //         string animalName = testAnimalList[i];
        //         var animalPrefab = LoadAnimalPrefab(testAnimalList[i], thumbnailSpot.position, thumbnailCamera.gameObject);

        //         yield return new WaitForEndOfFrame();
        //         thumbnailCamera.Render();

        //         var uiSet = GameObject.Instantiate(animalListContentPrefab);
        //         contentUis[index] = uiSet;
        //         //penguinUiSetList[i] = uiSet;
        //         ToTexture2D(renderTexture, (Texture2D resultTex) =>
        //         {
        //             uiSet.GetComponent<RawImage>().texture = resultTex;
        //         });
        //         uiSet.GetComponent<Button>().onClick.AddListener(() =>
        //         {
        //             PlayerPrefs.SetString(PlayerPrefsKey.key_multiplayAnimal, animalName);
        //         });
        //         uiSet.GetComponentInChildren<Text>().text = testAnimalList[i];
        //         uiSet.transform.SetParent(animalListContentsView);
        //         Destroy(animalPrefab);
        //     }
        // }

        // private GameObject LoadAnimalPrefab(string animalName, Vector3 position, GameObject lookAtTarget)
        // {
        //     var path = $"Prefab/Animals/{animalName}";
        //     GameObject obj = Resources.Load(path) as GameObject;
        //     GameObject animal = Instantiate(obj, position, Quaternion.identity);

        //     animal.transform.LookAt(lookAtTarget.transform);

        //     Debug.Log($"Creating Animal is Success! => {animalName}");
        //     return animal;
        // }



    }
}