using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
namespace BluehatGames
{
    public class SynthesisManager : MonoBehaviour
    {
        public ObjectPool contentUiPool;
        public AnimalFactory animalFactory;
        public Texture2D formatTexture;

        public AnimalAirController animalAirController;

        [Header("Common UI")]
        public GameObject animalListView;
        public Transform animalListContentsView;
        public GameObject animalListContentPrefab;
        public GameObject panel_result;
        public Button btn_goToMain;

        [Header("Color Change UI")]
        public GameObject panel_colorChange;
        public Button btn_colorChange;
        public Button btn_startColorChange;

        [Header("Fusion UI")]
        public GameObject panel_fusion;
        public Button btn_fusion;
        public Button btn_exitListView;
        public Button btn_startFusion;
        public GameObject[] text_NFTs;

        [Header("AnimalListThumbnail")]
        public Camera thumbnailCamera;
        public RenderTexture renderTexture;
        public Transform thumbnailSpot;

        private GameObject targetAnimal;
        private AnimalDataFormat selectedAnimalData;
        private AnimalDataFormat fusionSelectedAnimalData_1;
        private AnimalDataFormat fusionSelectedAnimalData_2;

        public ColorChangeManager colorChangeManager;
        public FusionManager fusionManager;

        private int currentMode;
        private int SELECT_MENU_MODE = 0;
        private int COLOR_CHANGE_MODE = 1;
        private int FUSION_MODE = 2;

        private float adjustAnimaionSpeed = 0.2f;

        private GameObject[] contentUis;
        private Dictionary<string, GameObject> contentUiDictionary;

        public float firstAnimalX;
        public float secondAnimalX;

        public GameObject[] animalObjectArray;

        private int calledCount = 0;
        private bool isCreating = false;

        private int poolSize = 30; // 얼만큼일지 모르지만 이만큼은 만들어놓자

        void Start()
        {
            colorChangeManager.SetSynthesisManager(this);
            contentUiDictionary = new Dictionary<string, GameObject>();

            panel_result.SetActive(false);
            panel_fusion.SetActive(false);
            animalListView.SetActive(false);
            panel_colorChange.SetActive(false);

            btn_goToMain.onClick.AddListener(() =>
            {
                if (currentMode == SELECT_MENU_MODE)
                {
                    SceneManager.LoadScene(SceneName._03_Main);
                }
                else if (currentMode == COLOR_CHANGE_MODE)
                {
                    currentMode = SELECT_MENU_MODE;
                    panel_colorChange.SetActive(false);
                    colorChangeManager.ClearResultAnimal();
                }
                else if (currentMode == FUSION_MODE)
                {
                    currentMode = SELECT_MENU_MODE;
                    panel_fusion.SetActive(false);
                }

                animalListView.SetActive(false);
                panel_result.SetActive(false);
                ClearAnimals();
                fusionManager.ClearAnimals();
            });

            btn_exitListView.onClick.AddListener(() =>
            {
                animalListView.SetActive(false);
            });

            InitContentViewUIObjectPool();
        }

        public void ResetAnimalListView()
        {
            for(int i = 0; i < animalListContentsView.childCount; i++)
            {
                contentUiPool.RetrunPoolObject(animalListContentsView.GetChild(i).gameObject);
            }
        }

        private void InitContentViewUIObjectPool()
        {
            contentUiPool.Init(poolSize, animalListContentPrefab, animalListContentsView);
        }
        
        // AnimalAirController에서 호출하는 함수
        public void StartMakeThumbnailAnimalList(Dictionary<string, GameObject> animalObjectDictionary, AnimalDataFormat[] animalDataArray, bool isRefresh)
        {
            if(isCreating == true)
            {
                return;
            }
            isCreating = true;

            calledCount++;
            Debug.Log($"called Count = {calledCount}");
            // contentUis = new GameObject[animalDataArray.Length];
            
            Debug.Log($"animalDataArray.Length = {animalDataArray.Length}");
            // for(int i = 0; i < animalDataArray.Length; i++)
            // {
            //     contentUis[i] = contentUiPool.GetObject();
            // }

            ResetAnimalListView();
            StartCoroutine(MakeThumbnailAnimalList(animalObjectDictionary, animalDataArray, isRefresh));

        }

        private void SetColorChangeButtonOnClick()
        {
            // color change button in synthesis main menu
            btn_colorChange.onClick.AddListener(() =>
            {
                currentMode = COLOR_CHANGE_MODE;
                animalListView.SetActive(true);
                panel_colorChange.SetActive(true);
                btn_exitListView.gameObject.SetActive(false); // 색 변경에서는 exit button 사용 안함

                for (int i = 0; i < contentUiDictionary.Count; i++)
                {
                    GameObject contentUi = contentUiDictionary.Values.ToList()[i];
                    contentUi.GetComponent<RawImage>().color = new Color(1, 1, 1);
                }

                ClearAnimals();
            });

            // start color change button in color change menu
            btn_startColorChange.onClick.AddListener(() =>
            {

                panel_result.SetActive(true);
                for (int i = 0; i < text_NFTs.Length; i++)
                {
                    text_NFTs[i].SetActive(false);
                }

                // colorChangeManager.ChangeTextureColor();
                animalListView.SetActive(false);

                // sub aether count
                AetherController.instance.SubAetherCount();

                colorChangeManager.SendColorChangeAPI();
            });
        }

        private void SetFusionButtonOnClick()
        {
            btn_fusion.onClick.AddListener(() =>
            {

                currentMode = FUSION_MODE;
                panel_fusion.SetActive(true);
                btn_startFusion.gameObject.SetActive(false);

                animalListView.SetActive(false);
                for (int i = 0; i < contentUiDictionary.Count; i++)
                {
                    GameObject contentUi = contentUiDictionary.Values.ToList()[i];
                    contentUi.GetComponent<RawImage>().color = new Color(1, 1, 1);
                }
                
            });

            btn_startFusion.onClick.AddListener(() =>
            {

                fusionManager.CreateFusionTexture();
                panel_result.SetActive(true);
                StartCoroutine(TakeScreenshot());
                for (int i = 0; i < text_NFTs.Length; i++)
                {
                    text_NFTs[i].SetActive(true);
                }
                ClearAnimals();
                AetherController.instance.SubAetherCount();
            });
        }


        void ResetAnimalState(GameObject animal)
        {
            animal.GetComponent<Rigidbody>().useGravity = false;
            animal.GetComponent<Rigidbody>().isKinematic = true;
            animal.GetComponent<CapsuleCollider>().enabled = false;
        }

        IEnumerator TakeScreenshot()
        {
            yield return new WaitForEndOfFrame();

            GameObject resultAnimal = fusionManager.GetResultAnimal();
            // 사진 찍기용
            GameObject duplicatedAnimal = GameObject.Instantiate(resultAnimal);
            ResetAnimalState(resultAnimal);
            ResetAnimalState(duplicatedAnimal);

            duplicatedAnimal.transform.position = thumbnailSpot.position;
            duplicatedAnimal.transform.eulerAngles = new Vector3(-5, -144, 0);
            thumbnailCamera.Render();

            ToTexture2D(renderTexture, (Texture2D resultTex) =>
            {
                Texture2D texture = resultTex;
                byte[] bytes = texture.EncodeToPNG();
                StartCoroutine(this.SendPNGToServer(bytes));

            });
        }

        IEnumerator SendPNGToServer(byte[] bytes)
        {
            // Create a Web Form
            WWWForm form = new WWWForm();
            form.AddField("wallet_address", "0x9b09EfC0a10BaCd3f296B069D1C8bD0032570EB8");
            form.AddBinaryData("file", bytes);

            // Upload to a cgi script
            var w = UnityWebRequest.Post("https://api.bluehat.games/nft/test-nft", form);
            yield return w.SendWebRequest();

            if (w.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(w.error);
            }
            else
            {
                Debug.Log(w.result);
                Debug.Log("Finished Uploading Screenshot");
            }
        }

        void ClearAnimals()
        {
            if (targetAnimal)
            {
                Destroy(targetAnimal);
            }
            if (selectedAnimal_1)
            {
                Destroy(selectedAnimal_1);
            }
            if (selectedAnimal_2)
            {
                Destroy(selectedAnimal_2);
            }
        }

        private GameObject selectedAnimal_1;
        private GameObject selectedAnimal_2;

        private int focusedButtonIndex;

        public void OnClickSelectAnimalButton(int buttonIndex)
        {
            if (buttonIndex == 0)
            {
                focusedButtonIndex = 0;
                Debug.Log($"1_ 선택된 동물은 {selectedAnimalData}");
            }
            else
            {
                focusedButtonIndex = 1;
                Debug.Log($"2_ 선택된 동물은 {selectedAnimalData}");
            }
            animalListView.SetActive(true);
            btn_exitListView.gameObject.SetActive(true);
        }

        public void RefreshAnimalThumbnail(GameObject updatedAnimalObject, AnimalDataFormat animalData)
        {
            StartCoroutine(UpdateThumbnail(updatedAnimalObject, animalData));
        }

        IEnumerator UpdateThumbnail(GameObject updatedAnimalObject, AnimalDataFormat animalData)
        {
            updatedAnimalObject.transform.position = thumbnailSpot.position;
            updatedAnimalObject.transform.rotation = thumbnailSpot.rotation;
            updatedAnimalObject.transform.LookAt(thumbnailCamera.transform);

            ResetAnimalState(updatedAnimalObject);
            thumbnailCamera.Render();

            var uiSet = contentUiDictionary[animalData.id];

            yield return new WaitForEndOfFrame();
            ToTexture2D(renderTexture, (Texture2D resultTex) =>
            {
                uiSet.GetComponent<RawImage>().texture = resultTex;
                uiSet.GetComponent<Button>().onClick.AddListener(() =>
                {
                    animalListView.SetActive(false);
                    uiSet.GetComponent<RawImage>().color = new Color(0.4f, 0.4f, 0.4f);
                    if (targetAnimal)
                    {
                        targetAnimal.SetActive(false);
                    }

                    selectedAnimalData = animalData;

                    targetAnimal = updatedAnimalObject;
                    targetAnimal.SetActive(true);
                    targetAnimal.GetComponentInChildren<Animator>().speed = adjustAnimaionSpeed;
                    targetAnimal.transform.position = new Vector3(-4, -0.5f, targetAnimal.transform.position.z);

                    ResetAnimalState(targetAnimal);

                    colorChangeManager.SetCurSelectedAnimal(selectedAnimalData, targetAnimal);        
                });
            });

            colorChangeManager.OnRefreshAnimalDataAfterColorChange();
        }

        // dictionary version
        IEnumerator MakeThumbnailAnimalList(Dictionary<string, GameObject> animalObjectDictionary, AnimalDataFormat[] animalDataArray, bool isRefresh)
        {
            animalListView.SetActive(true);
            animalObjectArray = new GameObject[animalDataArray.Length];


            int index = 0;
            for (int i = 0; i < animalObjectDictionary.Count; i++)
            {
                int curIdx = index;
                GameObject animalObject = animalObjectDictionary.Values.ToList()[curIdx];
                Debug.Log($"Dictionary Count = {animalObjectDictionary.Count}, this animal = {animalObject.name}");
                contentUiDictionary.Add(animalDataArray[i].id, contentUiPool.GetObject());

                // if(pair.Value == null)
                // {
                //     Debug.Log($"pair.Key = {pair.Key} value is null");
                // }
                animalObject.transform.position = thumbnailSpot.position;
                animalObject.transform.rotation = thumbnailSpot.rotation;
                animalObject.transform.LookAt(thumbnailCamera.transform);
                animalObjectArray[curIdx] = animalObject;
                animalObject.name = animalObject.name + calledCount;
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
                    uiSet.GetComponent<RawImage>().texture = resultTex;
                    uiSet.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        animalListView.SetActive(false);
                        uiSet.GetComponent<RawImage>().color = new Color(0.4f, 0.4f, 0.4f);
                        // ------------------------ 색변경 모드이면 ------------------------
                        if (currentMode == COLOR_CHANGE_MODE)
                        {
                            if (targetAnimal)
                            {
                                targetAnimal.SetActive(false);
                            }

                            Debug.Log($"index = {curIdx}, animalDataArray.Length = {animalDataArray.Length}");
                            selectedAnimalData = animalDataArray[curIdx];


                            // LoadAnimalPrefab 대신에 AnimalFactory에서 오브젝트 가져와야 할 듯
                            Debug.Log($"animalObjectArray.length => {animalObjectArray.Length}, index => {curIdx}");
                            targetAnimal = animalObjectArray[curIdx];
                            targetAnimal.SetActive(true);
                            // targetAnimal = LoadAnimalPrefab(selectedAnimalData.animalType, Vector3.zero, Camera.main.gameObject);
                            targetAnimal.GetComponentInChildren<Animator>().speed = adjustAnimaionSpeed;
                            targetAnimal.transform.position = new Vector3(-4, -0.5f, targetAnimal.transform.position.z);

                            ResetAnimalState(targetAnimal);

                            colorChangeManager.SetCurSelectedAnimal(selectedAnimalData, targetAnimal);
                        }
                        // ------------------------ 합성 모드이면 ------------------------
                        else if (currentMode == FUSION_MODE)
                        {
                            var selectedAnimalName = animalDataArray[curIdx].animalType;

                            if (focusedButtonIndex == 0)
                            {
                                fusionSelectedAnimalData_1 = animalDataArray[curIdx];
                                if (selectedAnimal_1)
                                {
                                    selectedAnimal_1.SetActive(false);
                                }

                                selectedAnimal_1 = animalObjectArray[curIdx];
                                selectedAnimal_1.SetActive(true);
                                selectedAnimal_1.GetComponentInChildren<Animator>().speed = adjustAnimaionSpeed;
                                selectedAnimal_1.transform.position = new Vector3(firstAnimalX, selectedAnimal_1.transform.position.y, selectedAnimal_1.transform.position.z);
                                fusionManager.SetTargetAnimal(0, selectedAnimal_1);
                                ResetAnimalState(selectedAnimal_1);
                            }
                            else if (focusedButtonIndex == 1)
                            {
                                fusionSelectedAnimalData_2 = animalDataArray[curIdx];
                                if (selectedAnimal_2)
                                {
                                    selectedAnimal_2.SetActive(false);
                                }
                                selectedAnimal_2 = animalObjectArray[curIdx];
                                selectedAnimal_2.SetActive(true);
                                selectedAnimal_2.GetComponentInChildren<Animator>().speed = adjustAnimaionSpeed;
                                selectedAnimal_2.transform.position = new Vector3(secondAnimalX, selectedAnimal_2.transform.position.y, selectedAnimal_2.transform.position.z);
                                fusionManager.SetTargetAnimal(1, selectedAnimal_2);
                                ResetAnimalState(selectedAnimal_2);
                            }

                            if (selectedAnimal_1 != null && selectedAnimal_2 != null)
                            {
                                btn_startFusion.gameObject.SetActive(true);
                            }
                        }
                    });
                    uiSet.GetComponentInChildren<Text>().text = animalDataArray[curIdx].animalType;
                    uiSet.transform.SetParent(animalListContentsView);


                });

                
                
                index++;

            }

            animalListView.SetActive(false);

            if(isRefresh)
            {
                colorChangeManager.OnRefreshAnimalDataAfterColorChange();
            }

            isCreating = false;

            SetColorChangeButtonOnClick();
            SetFusionButtonOnClick();

        }

        private GameObject LoadAnimalPrefab(string animalName, Vector3 position, GameObject lookAtTarget)
        {
            var path = $"Prefab/Animals/{animalName}";
            GameObject obj = Resources.Load(path) as GameObject;
            GameObject animal = Instantiate(obj, position, Quaternion.identity);
            ResetAnimalState(animal);
            animal.transform.LookAt(lookAtTarget.transform);

            Debug.Log($"Creating Animal is Success! => {animalName}");
            return animal;
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

        public void SendRequestRefreshAnimalDataOnColorChange(string animalId)
        {
            animalAirController.RefreshAnimalDataColorChange(animalId);
        }

        public GameObject GetAnimalObject(string id)
        {
            GameObject animalObj = animalAirController.GetAnimalObject(id);
            ResetAnimalState(animalObj);

            return animalObj;
        }
    }
}