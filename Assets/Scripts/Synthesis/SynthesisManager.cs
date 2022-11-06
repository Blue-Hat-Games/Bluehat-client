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

        public enum PannelStatus
        {
            COLOR_CHANGE,
            ACCESORY,
            FUSION
        }

        /* this function using pannel Switch */
        public class PannelSwitch
        {

            public PannelStatus status;
            public GameObject panel_colorChange;
            public GameObject panel_accessory;
            public GameObject panel_fusion;
            public PannelSwitch(GameObject panel_colorChange, GameObject panel_accessory, GameObject panel_fusion)
            {
                this.panel_colorChange = panel_colorChange;
                this.panel_accessory = panel_accessory;
                this.panel_fusion = panel_fusion;
                this.status = PannelStatus.COLOR_CHANGE;
            }
            public PannelStatus GetStatus()
            {
                return status;
            }

            public bool CheckStatus(PannelStatus status)
            {
                return this.status == status;
            }


            public void ChangeStatus(PannelStatus status)
            {
                this.status = status;
                switch (status)
                {
                    case PannelStatus.COLOR_CHANGE:
                        panel_colorChange.SetActive(true);
                        panel_accessory.SetActive(false);
                        panel_fusion.SetActive(false);
                        break;
                    case PannelStatus.ACCESORY:
                        panel_colorChange.SetActive(false);
                        panel_accessory.SetActive(true);
                        panel_fusion.SetActive(false);
                        break;
                    case PannelStatus.FUSION:
                        panel_colorChange.SetActive(false);
                        panel_accessory.SetActive(false);
                        panel_fusion.SetActive(true);
                        break;
                }
            }
        }

        public ObjectPool contentUiPool;
        public AnimalFactory animalFactory;

        public AnimalAirController animalAirController;

        [Header("Common UI")]
        public GameObject animalListView;
        public Transform animalListContentsView;
        public GameObject animalListContentPrefab;
        [Header("Result Pannel")]
        public GameObject panel_result;
        public Button btn_result;

        public Button btn_goToMain;

        [Header("Color Change UI")]
        public GameObject panel_colorChange;
        public Button btn_colorChange;
        public Button btn_startColorChange;


        [Header("Accessory Change UI")]
        public GameObject panel_accessory;
        public Button btn_accessory;
        public Button btn_startAccessory;

        [Header("Fusion UI")]
        public GameObject panel_fusion;
        public Button btn_fusion;
        public Button btn_exitListView;
        public Button btn_startFusion;
        public Image img_klaytnLogo;

        [Header("AnimalListThumbnail")]
        public Camera thumbnailCamera;
        public RenderTexture renderTexture;
        public Transform thumbnailSpot;
        public Transform thumbnailSpot2;

        private GameObject targetAnimal;
        private AnimalDataFormat selectedAnimalData;
        private AnimalDataFormat fusionSelectedAnimalData_1;
        private AnimalDataFormat fusionSelectedAnimalData_2;

        public ColorChangeManager colorChangeManager;
        public FusionManager fusionManager;
        public AccessoryManager accessoryManager;

        public Camera animalRenderCamera1;
        public Camera animalRenderCamera2;

        private float adjustAnimaionSpeed = 0.2f;

        private Dictionary<string, GameObject> contentUiDictionary;

        public float firstAnimalX;
        public float secondAnimalX;

        public GameObject[] animalObjectArray;

        private int calledCount = 0;
        private bool isCreating = false;

        private int poolSize = 30; // 얼만큼일지 모르지만 이만큼은 만들어놓자

        private PannelSwitch pannelSwitch;
        void Start()
        {
            colorChangeManager.SetSynthesisManager(this);
            fusionManager.SetSynthesisManager(this);
            accessoryManager.SetSynthesisManager(this);

            contentUiDictionary = new Dictionary<string, GameObject>();

            panel_result.SetActive(false);
            btn_startFusion.gameObject.SetActive(false);

            pannelSwitch = new PannelSwitch(panel_colorChange, panel_accessory, panel_fusion);

            btn_goToMain.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneName._03_Main);
                ClearAnimals();
            });

            btn_exitListView.onClick.AddListener(() =>
            {
                animalListView.SetActive(false);
            });

            btn_result.onClick.AddListener(() =>
            {
                panel_result.SetActive(false);
                colorChangeManager.ClearResultAnimal();
                fusionManager.ClearResultAnimal();
            });

            InitContentViewUIObjectPool();
        }

        public void ResetAnimalListView()
        {
            for (int i = 0; i < animalListContentsView.childCount; i++)
            {
                contentUiPool.RetrunPoolObject(animalListContentsView.GetChild(i).gameObject);
            }
        }

        private void InitContentViewUIObjectPool()
        {
            contentUiPool.Init(poolSize, animalListContentPrefab, animalListContentsView);
        }

        // AnimalAirController에서 호출하는 함수
        public void StartMakeThumbnailAnimalList(Dictionary<string, GameObject> animalObjectDictionary, AnimalDataFormat[] animalDataArray)
        {
            if (isCreating == true)
            {
                return;
            }
            isCreating = true;

            calledCount++;


            ResetAnimalListView();
            StartCoroutine(MakeThumbnailAnimalList(animalObjectDictionary, animalDataArray));

        }

        private void SetColorChangeButtonOnClick()
        {
            // color change button in synthesis main menu
            btn_colorChange.onClick.AddListener(() =>
            {
                pannelSwitch.ChangeStatus(PannelStatus.COLOR_CHANGE);
                panel_result.SetActive(false);
                animalListView.SetActive(true);
                img_klaytnLogo.gameObject.SetActive(false);
                btn_exitListView.gameObject.SetActive(false); // 색 변경에서는 exit button 사용 안함
                ClearAnimals();
            });

            // start color change button in color change menu
            btn_startColorChange.onClick.AddListener(() =>
            {
                panel_result.SetActive(true);
                img_klaytnLogo.gameObject.SetActive(false);
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
                pannelSwitch.ChangeStatus(PannelStatus.FUSION);
                panel_result.SetActive(false);
                btn_startFusion.gameObject.SetActive(false);
                animalListView.SetActive(false);
                ClearAnimals();

            });

            btn_startFusion.onClick.AddListener(() =>
            {

                // fusionManager.CreateFusionTexture();
                panel_result.SetActive(true);
                img_klaytnLogo.gameObject.SetActive(true);
                AetherController.instance.SubAetherCount();
                fusionManager.SendFusionAPI();
                ClearAnimals();
            });
        }

        private void SetAccessoryButtonOnClick()
        {
            btn_accessory.onClick.AddListener(() => {
                pannelSwitch.ChangeStatus(PannelStatus.ACCESORY);
                panel_result.SetActive(false);
                btn_startAccessory.gameObject.SetActive(false);
                animalListView.SetActive(true);
                ClearAnimals();

            });

            btn_startAccessory.onClick.AddListener(() => {
                panel_result.SetActive(true);
                AetherController.instance.SubAetherCount();
                accessoryManager.SendRandomHatAPI();
            });
        }

        void ResetAnimalState(GameObject animal)
        {
            animal.GetComponent<Rigidbody>().useGravity = false;
            animal.GetComponent<Rigidbody>().isKinematic = true;
            animal.GetComponent<CapsuleCollider>().enabled = false;

            animal.transform.eulerAngles = new Vector3(0, animal.transform.eulerAngles.y, animal.transform.eulerAngles.z);
        }

        public void TakeScreenshotForMarketPNG()
        {
            StartCoroutine(TakeScreenshot());
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
                GameObject.Destroy(duplicatedAnimal);

            });
        }

        IEnumerator SendPNGToServer(byte[] bytes)
        {
            // Create a Web Form
            WWWForm form = new WWWForm();
            form.AddField("wallet_address", "0x9b09EfC0a10BaCd3f296B069D1C8bD0032570EB8");
            form.AddBinaryData("file", bytes);

            // Upload to a cgi script
            var w = UnityWebRequest.Post(ApiUrl.postFusionResultPNG, form);
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

            w.Dispose();
        }

        void ClearAnimals()
        {
            if (targetAnimal)
            {
                targetAnimal.SetActive(false);
            }
            if (selectedAnimal_1)
            {
                selectedAnimal_1.SetActive(false);
            }
            if (selectedAnimal_2)
            {
                selectedAnimal_2.SetActive(false);
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

            // 기존 꺼가 있으면 그걸 사용하고 없으면 새로 만듦
            GameObject uiSet = null;
            if (contentUiDictionary.ContainsKey(animalData.id))
            {
                uiSet = contentUiDictionary[animalData.id];
            }
            else
            {
                contentUiDictionary.Add(animalData.id, contentUiPool.GetObject());
                uiSet = contentUiDictionary[animalData.id];
            }

            yield return new WaitForEndOfFrame();
            ToTexture2D(renderTexture, (Texture2D resultTex) =>
            {
                uiSet.GetComponent<RawImage>().texture = resultTex;
                uiSet.GetComponent<Button>().onClick.AddListener(() =>
                {
                    animalListView.SetActive(false);
                    if (pannelSwitch.CheckStatus(PannelStatus.COLOR_CHANGE))
                    {
                        OnClickColorChangeThumbnail(animalData, updatedAnimalObject);
                    }
                    else if (pannelSwitch.CheckStatus(PannelStatus.FUSION))
                    {
                        OnClickFusionThumbnail(animalData, updatedAnimalObject);
                    }
                });
            });

            uiSet.GetComponentInChildren<Text>().text = animalData.animalType;
            uiSet.transform.SetParent(animalListContentsView);

            if (pannelSwitch.CheckStatus(PannelStatus.COLOR_CHANGE))
            {
                colorChangeManager.OnRefreshAnimalDataAfterColorChange();
            }
            else
            {
                fusionManager.OnRefreshAnimalDataAfterFusion(updatedAnimalObject);
            }
        }

        private void OnClickColorChangeThumbnail(AnimalDataFormat animalData, GameObject animalObject)
        {
            if (targetAnimal)
            {
                targetAnimal.SetActive(false);
            }

            selectedAnimalData = animalData;

            targetAnimal = animalObject;
            targetAnimal.SetActive(true);
            targetAnimal.GetComponentInChildren<Animator>().speed = adjustAnimaionSpeed;
            // targetAnimal.transform.position = new Vector3(-4, -0.5f, targetAnimal.transform.position.z);
            targetAnimal.transform.position = thumbnailSpot.transform.position;

            ResetAnimalState(targetAnimal);

            colorChangeManager.SetCurSelectedAnimal(selectedAnimalData, targetAnimal);
        }

        private void OnClickAccessoryThumbnail(AnimalDataFormat animalData, GameObject animalObject)
        {
            if (targetAnimal)
            {
                targetAnimal.SetActive(false);
            }

            selectedAnimalData = animalData;

            targetAnimal = animalObject;
            targetAnimal.SetActive(true);
            targetAnimal.GetComponentInChildren<Animator>().speed = adjustAnimaionSpeed;
            // targetAnimal.transform.position = new Vector3(-4, -0.5f, targetAnimal.transform.position.z);
            targetAnimal.transform.position = thumbnailSpot.transform.position;
            ResetAnimalState(targetAnimal);

            accessoryManager.SetCurSelectedAnimal(selectedAnimalData, targetAnimal);

            btn_startAccessory.gameObject.SetActive(true);
        }

        private void OnClickFusionThumbnail(AnimalDataFormat animalData, GameObject animalObject)
        {
            var selectedAnimalName = animalData.animalType;
            if (focusedButtonIndex == 0)
            {
                fusionSelectedAnimalData_1 = animalData;
                if (selectedAnimal_1)
                {
                    selectedAnimal_1.SetActive(false);
                }

                selectedAnimal_1 = animalObject;
                selectedAnimal_1.SetActive(true);
                selectedAnimal_1.GetComponentInChildren<Animator>().speed = adjustAnimaionSpeed;
                selectedAnimal_1.transform.position = thumbnailSpot.transform.position;
                fusionManager.SetCurSelectedAnimal_1(fusionSelectedAnimalData_1, selectedAnimal_1);

                selectedAnimal_1.transform.LookAt(animalRenderCamera1.transform);
                ResetAnimalState(selectedAnimal_1);
            }
            else if (focusedButtonIndex == 1)
            {
                fusionSelectedAnimalData_2 = animalData;
                if (selectedAnimal_2)
                {
                    selectedAnimal_2.SetActive(false);
                }
                selectedAnimal_2 = animalObject;
                selectedAnimal_2.SetActive(true);
                selectedAnimal_2.GetComponentInChildren<Animator>().speed = adjustAnimaionSpeed;
                selectedAnimal_2.transform.position = thumbnailSpot2.transform.position;
                fusionManager.SetCurSelectedAnimal_2(fusionSelectedAnimalData_2, selectedAnimal_2);

                selectedAnimal_2.transform.LookAt(animalRenderCamera2.transform);
                ResetAnimalState(selectedAnimal_2);
            }

            if (selectedAnimal_1 != null && selectedAnimal_2 != null)
            {
                if (fusionSelectedAnimalData_1.Equals(fusionSelectedAnimalData_2))
                {
                    Debug.Log("동일한 동물은 선택할 수 없습니다.");
                    btn_startFusion.gameObject.SetActive(false);
                }
                else
                {
                    btn_startFusion.gameObject.SetActive(true);
                }
            }
        }

        // dictionary version
        IEnumerator MakeThumbnailAnimalList(Dictionary<string, GameObject> animalObjectDictionary, AnimalDataFormat[] animalDataArray)
        {
            animalListView.SetActive(false);
            animalObjectArray = new GameObject[animalDataArray.Length];

            int index = 0;
            for (int i = 0; i < animalObjectDictionary.Count; i++)
            {
                int curIdx = index;
                GameObject animalObject = animalObjectDictionary.Values.ToList()[curIdx];
                contentUiDictionary.Add(animalDataArray[i].id, contentUiPool.GetObject());

                animalObject.transform.position = thumbnailSpot.position;
                animalObject.transform.rotation = thumbnailSpot.rotation;
                animalObject.transform.LookAt(thumbnailCamera.transform);
                animalObjectArray[curIdx] = animalObject;
                animalObject.name = $"{animalObject.name}_{calledCount}";
                ResetAnimalState(animalObject);

                thumbnailCamera.Render();
         
                var uiSet = contentUiDictionary[animalDataArray[i].id];
                uiSet.name = $"{animalDataArray[curIdx].animalType}_{animalDataArray[curIdx].id}";

                yield return new WaitForEndOfFrame();

                if (animalObjectArray[curIdx])
                {
                    animalObjectArray[curIdx].SetActive(false);
                }

                ToTexture2D(renderTexture, (Texture2D resultTex) =>
                {
                    uiSet.GetComponent<RawImage>().texture = resultTex;
                    uiSet.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        animalListView.SetActive(false);
                        // ------------------------ 색변경 모드이면 ------------------------
                        if (pannelSwitch.CheckStatus(PannelStatus.COLOR_CHANGE))
                        {
                            OnClickColorChangeThumbnail(animalDataArray[curIdx], animalObjectArray[curIdx]);
                        }
                        // ------------------------ 합성 모드이면 ------------------------
                        else if (pannelSwitch.CheckStatus(PannelStatus.FUSION))
                        {
                            OnClickFusionThumbnail(animalDataArray[curIdx], animalObjectArray[curIdx]);
                        }
                        // ------------------------ 꾸미기 모드이면 ------------------------
                        else if (pannelSwitch.CheckStatus(PannelStatus.ACCESORY))
                        {
                            OnClickAccessoryThumbnail(animalDataArray[curIdx], animalObjectArray[curIdx]);
                        }
                    });
                    uiSet.GetComponentInChildren<Text>().text = animalDataArray[curIdx].animalType;
                    uiSet.transform.SetParent(animalListContentsView);
                });
                index++;
            }

            isCreating = false;

            SetColorChangeButtonOnClick();
            SetFusionButtonOnClick();
            SetAccessoryButtonOnClick();

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

        public void SendRequestRefreshAnimalDataOnFusion(string animalId1, string animalId2, string resultAnimalId)
        {
            // 여기에서 animalListView 업데이트해주어야함

            contentUiPool.RetrunPoolObject(contentUiDictionary[animalId1]);
            contentUiPool.RetrunPoolObject(contentUiDictionary[animalId2]);
            contentUiDictionary.Remove(animalId1);
            contentUiDictionary.Remove(animalId2);
            animalAirController.RefreshAnimalDataFusion(animalId1, animalId2, resultAnimalId);
        }

        public GameObject GetAnimalObject(string id)
        {
            GameObject animalObj = animalAirController.GetAnimalObject(id);
            ResetAnimalState(animalObj);

            return animalObj;
        }
    }
}