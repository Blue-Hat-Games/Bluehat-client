using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BluehatGames {
public class SynthesisManager : MonoBehaviour
{
    private AnimalDataFormat[] animalDataArray;

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
    private string selectedAnimal;

    public ColorChangeManager colorChangeManager;
    public FusionManager fusionManager;

    private int currentMode;
    private int SELECT_MENU_MODE = 0;
    private int COLOR_CHANGE_MODE = 1;
    private int FUSION_MODE = 2;

    private float adjustAnimaionSpeed = 0.2f;

    private GameObject[] contentUis;

    public float firstAnimalX;
    public float secondAnimalX;
    
    // AnimalAirController에서 호출하는 함수
    public void StartMakeThumbnailAnimalList(AnimalDataFormat[] animalDataArray)
    {
        contentUis = new GameObject[animalDataArray.Length];
        StartCoroutine(MakeThumbnailAnimalList(animalDataArray));

        SetColorChangeButtonOnClick();
        SetFusionButtonOnClick();
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

            for(int i=0; i< contentUis.Length; i++)
            {
                contentUis[i].GetComponent<RawImage>().color = new Color(1, 1, 1);
            }
            
            ClearAnimals();
        });
        
        // start color change button in color change menu
        btn_startColorChange.onClick.AddListener(() =>
        {
            
            panel_result.SetActive(true);
            for(int i=0; i<text_NFTs.Length; i++)
            {
                text_NFTs[i].SetActive(false);
            }

            colorChangeManager.ChangeTextureColor();
            animalListView.SetActive(false);    

            // sub aether count
            AetherController.instance.SubAetherCount();
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
            for (int i = 0; i < contentUis.Length; i++)
            {
                contentUis[i].GetComponent<RawImage>().color = new Color(1, 1, 1);
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

    void Start()
    {
        panel_result.SetActive(false);
        panel_fusion.SetActive(false);
        animalListView.SetActive(false);
        panel_colorChange.SetActive(false);

        btn_goToMain.onClick.AddListener(() =>
        {
            if(currentMode == SELECT_MENU_MODE) 
            {
                SceneManager.LoadScene(SceneName._03_Main);
            }
            else if(currentMode == COLOR_CHANGE_MODE) 
            {
                currentMode = SELECT_MENU_MODE;
                panel_colorChange.SetActive(false);
            } 
            else if(currentMode == FUSION_MODE) 
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
    }

    void ResetAnimalState(GameObject animal)
    {
        animal.GetComponent<Rigidbody>().useGravity = false;
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

    IEnumerator SendPNGToServer(byte[] bytes) {
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
        if(buttonIndex == 0)
        {
            focusedButtonIndex = 0;
            Debug.Log($"1_ 선택된 동물은 {selectedAnimal}");
        } else
        {
            focusedButtonIndex = 1;
            Debug.Log($"2_ 선택된 동물은 {selectedAnimal}");
        }
        animalListView.SetActive(true);
        btn_exitListView.gameObject.SetActive(true);
    }

    // 선택한 동물의 id를 서버로 보내서 합성을 진행해야 하므로 각 UI가 해당 동물의 정보를 가지고 있어야 함
    // 데이터만 저장할 수 있는 클래스를 만들어서 추가해주자

    IEnumerator MakeThumbnailAnimalList(AnimalDataFormat[] animalDataArray)
    {
        animalListView.SetActive(true);
        for (int i = 0; i < animalDataArray.Length; i++)
        {
            int index = i;

            var animalPrefab = LoadAnimalPrefab(animalDataArray[i].animalType, thumbnailSpot.position, thumbnailCamera.gameObject);

            ResetAnimalState(animalPrefab);

            yield return new WaitForEndOfFrame();
            thumbnailCamera.Render();

            var uiSet = GameObject.Instantiate(animalListContentPrefab);
            contentUis[index] = uiSet;
          
            ToTexture2D(renderTexture, (Texture2D resultTex) =>
            {
                uiSet.GetComponent<RawImage>().texture = resultTex;
            });

            uiSet.GetComponent<Button>().onClick.AddListener(() => {
                animalListView.SetActive(false);
                uiSet.GetComponent<RawImage>().color = new Color(0.4f, 0.4f, 0.4f);
                // ------------------------ 색변경 모드이면 ------------------------ 
                if (currentMode == COLOR_CHANGE_MODE)
                {
                    if (targetAnimal)
                    {
                        GameObject.Destroy(targetAnimal);
                    }

                    
                    selectedAnimal = animalDataArray[index].animalType;
                    targetAnimal = LoadAnimalPrefab(selectedAnimal, Vector3.zero, Camera.main.gameObject);
                    targetAnimal.GetComponentInChildren<Animator>().speed = adjustAnimaionSpeed;
                    targetAnimal.transform.position = new Vector3(-4, -0.5f, targetAnimal.transform.position.z);

                    ResetAnimalState(targetAnimal);
                    colorChangeManager.SetTargetAnimal(targetAnimal);
                } 
                // ------------------------ 합성 모드이면 ------------------------ 
                else if(currentMode == FUSION_MODE)
                {
                    var selectedAnimalName = animalDataArray[index].animalType;
                    if (focusedButtonIndex == 0)
                    {
                        if(selectedAnimal_1)
                        {
                            GameObject.Destroy(selectedAnimal_1);
                        }
                        
                        selectedAnimal_1 = LoadAnimalPrefab(selectedAnimalName, Vector3.zero, Camera.main.gameObject);
                        selectedAnimal_1.GetComponentInChildren<Animator>().speed = adjustAnimaionSpeed;
                        selectedAnimal_1.transform.position = new Vector3(firstAnimalX, selectedAnimal_1.transform.position.y, selectedAnimal_1.transform.position.z);
                        fusionManager.SetTargetAnimal(0, selectedAnimal_1);
                        ResetAnimalState(selectedAnimal_1);
                    }
                    else if(focusedButtonIndex == 1)
                    {
                        if(selectedAnimal_2)
                        {
                            GameObject.Destroy(selectedAnimal_2);
                        }
                        selectedAnimal_2 = LoadAnimalPrefab(selectedAnimalName, Vector3.zero, Camera.main.gameObject);
                        selectedAnimal_2.GetComponentInChildren<Animator>().speed = adjustAnimaionSpeed;
                        selectedAnimal_2.transform.position = new Vector3(secondAnimalX, selectedAnimal_2.transform.position.y, selectedAnimal_2.transform.position.z);
                        fusionManager.SetTargetAnimal(1, selectedAnimal_2);
                        ResetAnimalState(selectedAnimal_2);
                    }

                    if(selectedAnimal_1 != null && selectedAnimal_2 != null)
                    {
                        btn_startFusion.gameObject.SetActive(true);
                    }
                }
            });
            uiSet.GetComponentInChildren<Text>().text = animalDataArray[i].animalType;
            uiSet.transform.SetParent(animalListContentsView);
            Destroy(animalPrefab);
        }
        animalListView.SetActive(false);
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

    private void MakeNFTMargetImage() {

    }
}
}