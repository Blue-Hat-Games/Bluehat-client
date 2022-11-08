using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace BluehatGames
{

    public class MarketManager : MonoBehaviour
    {
        private string host = "https://api.bluehat.games";
        public int totalCount = 0;
        private int page = 1;
        public int limit = 10;
        public string order = "Newest";

        public User user;

        [Header("Market Main Panel")]
        public Button backToMainBtn;
        public Button beforeBtn;
        public Button nextBtn;
        public Text coinInfoText;
        public Button myAnimalBtn;
        public GameObject marketItemPrefab;

        [Header("MyAnimal Panel")]
        public GameObject myAnimalPanel;
        public Button myAnimalCloseBtn;
        public Button myAnimalSellBtn;
        public InputField myAnimalInputPrice;
        public Transform myAnimalContent;
        public GameObject myAnimalItemPrefab;

        [Header("Animal Detail Panel")]
        public GameObject animalDetailPanel;
        public Text animalDetailName;
        public Text animalDetailSellerName;
        public Text animalDetailPrice;
        public Text animalDetailViewCount;
        public Text animalDetailDescription;
        public Button animalDetailBuyBtn;
        public Button animalDetailCloseBtn;

        [Header("Common UI")]
        public Text myAnimalDetailData;

        [Header("Alert Panel")]
        public GameObject alertPanel;
        public Button alertPanelDoneBtn;
        public Text alertPanelMsg;

        public AnimalFactory animalFactory;

        private String authToken;

        void Start()
        {
            myAnimalPanel.SetActive(false);
            animalDetailPanel.SetActive(false);

            authToken = "0000";
            Debug.Log("authToken: " + authToken);

            StartCoroutine(GetItemCount());
            StartCoroutine(GetItems());
            StartCoroutine(GetUserInfo());

            nextBtn.onClick.AddListener(() =>
            {
                page = page + 1;
                StartCoroutine(GetItems());
            });

            beforeBtn.onClick.AddListener(() =>
            {
                page = page - 1;
                StartCoroutine(GetItems());
            });

            myAnimalBtn.onClick.AddListener(() =>
            {
                myAnimalPanel.SetActive(true);
                StartCoroutine(GetUserAnimal());

            });

            myAnimalCloseBtn.onClick.AddListener(() =>
            {
                myAnimalPanel.SetActive(false);
            });

            backToMainBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneName._03_Main);
            });

            animalDetailCloseBtn.onClick.AddListener(() =>
            {
                animalDetailPanel.SetActive(false);
            });

            myAnimalSellBtn.onClick.AddListener(() =>
            {
                StartCoroutine(SellMyAnimalToMarket());
            });

        }

        /*
         * This Method call for open new panel to send Information
         */
        private void OpenAlertPanel(string msg, GameObject prevPanel)
        {
            prevPanel.SetActive(false);
            alertPanel.SetActive(true);
            alertPanelDoneBtn.onClick.AddListener(() =>
                {
                    alertPanel.SetActive(false);
                });
            alertPanelMsg.text = msg;
        }


        IEnumerator GetItemCount()
        {
            string url = host + "/market/counts";
            using UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Error: {webRequest.error}");
            }
            else
            {
                Debug.Log($"Received: {webRequest.downloadHandler.text}");
            }
        }

        private IEnumerator GetUserInfo()
        {
            string url = host + "/user";
            using UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("Authorization", authToken);
            yield return webRequest.SendWebRequest();
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Error: {webRequest.error}");
            }
            else
            {
                var response = "{\"user\":" + webRequest.downloadHandler.text + "}";
                var parseResult = JsonUtility.FromJson<UserInfo>(response);
                user = parseResult.user;
                coinInfoText.text = $"{user.coin.ToString()}";
            }
        }

        private IEnumerator GetItems()
        {
            string url = host + "/market/list?order=" + order + "&limit=" + limit.ToString() + "&page=" + page.ToString();
            using UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Error: {webRequest.error}");
            }
            else
            {
                var response = "{\"items\":" + webRequest.downloadHandler.text + "}";
                ItemCard[] parseResult = JsonUtility.FromJson<ItemCardList>(response).items;
                StartCoroutine(MakeAnimalThumbnail(parseResult));
                
            }
        }


        void ResetAnimalState(GameObject animal)
        {
            animal.GetComponent<Rigidbody>().useGravity = false;
            animal.GetComponent<Rigidbody>().isKinematic = true;
            animal.GetComponent<CapsuleCollider>().enabled = false;

        }

        public Transform thumbnailSpot;
        public Camera thumbnailCamera;
        public RenderTexture renderTexture;

        IEnumerator MakeAnimalThumbnail(ItemCard[] itemCardArray)
        {
            for (int i = 0; i < itemCardArray.Length; i++)
                {                    
                    ItemCard item = itemCardArray[i];
                    GameObject itemObj = GameObject.Instantiate(marketItemPrefab);
                    itemObj.transform.SetParent(GameObject.Find("MarketMainPanel").transform);
                    itemObj.transform.Find("animal_id").GetComponent<Text>().text = item.id.ToString();
                    itemObj.transform.Find("animal_name").GetComponent<Text>().text = item.username;
                    itemObj.transform.Find("price").GetComponent<Text>().text = item.price.ToString();
                    itemObj.transform.Find("view_count").GetComponent<Text>().text = item.view_count.ToString();
                    
                    RawImage rawImage = itemObj.transform.Find("animal_img").GetComponent<RawImage>();

                    AnimalDataFormat updatedAnimalData = new AnimalDataFormat();
                    updatedAnimalData.name = item.animal_name;
                    updatedAnimalData.tier = 0;
                    updatedAnimalData.id = item.id.ToString();
                    updatedAnimalData.animalType = item.animal_type;
                    updatedAnimalData.headItem = item.head_item;
                    updatedAnimalData.pattern = "";
                    updatedAnimalData.color = item.color;
                    
                    Debug.Log($"MarketManager | id = {item.id.ToString()}, animal_type = {item.animal_type}, color = {item.color}");

                    Animal animal = new Animal(updatedAnimalData);
                    GameObject animalObject = animalFactory.GetAnimalGameObject(animal);
                    ResetAnimalState(animalObject);

                    animalObject.transform.position = thumbnailSpot.position;
                    animalObject.transform.rotation = thumbnailSpot.rotation;

                    thumbnailCamera.Render();
                    
                    yield return new WaitForEndOfFrame();

                    animalObject.SetActive(false);
                    ToTexture2D(renderTexture, (Texture2D resultTex) =>
                    {
                        rawImage.texture = resultTex;

                    });

                    itemObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 + 350 * i, 0);
                    itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        Debug.Log("Click");
                        Debug.Log(int.Parse(itemObj.transform.Find("animal_id").GetComponent<Text>().text));
                        StartCoroutine(GetAnimalDetail(int.Parse(itemObj.transform.Find("animal_id").GetComponent<Text>().text)));
                    });
                }

             
            
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

        private IEnumerator GetUserAnimal()
        {
            string url = host + "/animal/get-user-animal";
            using UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("Authorization", authToken);
            yield return webRequest.SendWebRequest();
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Error: {webRequest.error}");
            }
            else
            {
                Debug.Log($"Received: {webRequest.downloadHandler.text}");
                var animalInfo = JsonUtility.FromJson<UserAnimalList>(webRequest.downloadHandler.text);
                for (int i = animalInfo.data.Length - 1; i >= 0; i--)
                {
                    GameObject itemObj = GameObject.Instantiate(myAnimalItemPrefab, myAnimalContent, true);
                    itemObj.transform.Find("animal_id").GetComponent<Text>().text = animalInfo.data[i].id;
                    itemObj.transform.Find("animal_name").GetComponent<Text>().text = animalInfo.data[i].name;
                    itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        Debug.Log("Click");
                        myAnimalDetailData.text = itemObj.transform.Find("animal_id").GetComponent<Text>().text;
                    });
                }
            }
        }


        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator GetAnimalDetail(int id)
        {
            string url = host + "/market/detail?id=" + id;
            using UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Error: {webRequest.error}");
            }
            else
            {
                animalDetailPanel.SetActive(true);
                var animalInfo = JsonUtility.FromJson<AnimalDetailFromServer>(webRequest.downloadHandler.text).data;
                animalDetailDescription.text = animalInfo.description;
                animalDetailName.text = animalInfo.animal_name;
                animalDetailPrice.text = animalInfo.price.ToString();
                animalDetailViewCount.text = animalInfo.view_count + " Views";
                animalDetailSellerName.text = animalInfo.username;
                var buyAnimalId = animalInfo.id;
                animalDetailBuyBtn.onClick.AddListener(() =>
                {
                    StartCoroutine(BuyAnimal(buyAnimalId));
                });
            }
        }

        private IEnumerator SellMyAnimalToMarket()
        {
            string url = host + "/market/sell";
            using UnityWebRequest webRequest = UnityWebRequest.Post(url, "");
            webRequest.SetRequestHeader("Authorization", authToken);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            var price = myAnimalInputPrice.text;
            var animalId = myAnimalDetailData.text;
            var json = "{\"animal_id\":" + animalId + ", " + "\"price\":" + price + ", " + "\"seller_private_key\":" + "\"0000\"" + "}";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Error: {webRequest.error}");
                OpenAlertPanel("Sell Fail", myAnimalPanel);
            }
            else
            {
                OpenAlertPanel("Sell Success", myAnimalPanel);
            }
        }


        private IEnumerator BuyAnimal(int id)
        {
            string url = host + "/market/buy";
            using UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
            webRequest.SetRequestHeader("Authorization", authToken);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            string json = "{\"buy_animal_id\":" + id.ToString() + "}";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Error: {webRequest.error}");
            }
            else
            {
                var result = JsonUtility.FromJson<AnimalBuyResult>(webRequest.downloadHandler.text);
                OpenAlertPanel(result.msg == "success" ? "Buy Success" : "Buy Fail", animalDetailPanel);
            }
        }
    }



    [Serializable]
    public class AnimalDetail
    {
        public int id;
        public int price;
        public string description;
        public string updatedAt;
        public int view_count;

        public string username;
        public int aniaml_type;
        public string animal_name;
        public string color;

    }

    public class AnimalDetailFromServer
    {
        public string status;
        public AnimalDetail data;
    }


    [Serializable]
    public class ItemCard
    {
        public int id;
        public string username;
        public string animal_type;
        public string animal_name;
        public string color;
        public string updatedAt;
        public float price;
        public int view_count;
        public string head_item;
        public string description;

        public ItemCard(int id, string username, string animal_type, string color, string head_item, string animal_name, string updatedAt, float price, int view_count, string description)
        {
            this.id = id;
            this.username = username;
            this.animal_type = animal_type;
            this.color = color;
            this.head_item = head_item;
            this.animal_name = animal_name;
            this.updatedAt = updatedAt;
            this.price = price;
            this.view_count = view_count;
            this.description = description;
        }

        public ItemCard GetItemCard()
        {
            return this;
        }
    }

    [Serializable]
    public class ItemCardList
    {
        public ItemCard[] items;
    }

    [Serializable]
    public class User
    {
        public string username;
        public int coin;
        public string wallet_address;
        public string email;
        public string createdAt;
    }

    [Serializable]
    public class UserInfo
    {
        public User user;
    }
    [Serializable]
    public class AnimalBuyResult
    {
        public string status;
        public string msg;
    }

    [Serializable]
    public class AnimalFormatData
    {
        public string name;
        public int tier;
        public string color;
        public string id;
        public string animalType;
        public string headItem;
        public string pattern;
    }

    [Serializable]
    public class UserAnimalList
    {
        public AnimalFormatData[] data;
    }
}
