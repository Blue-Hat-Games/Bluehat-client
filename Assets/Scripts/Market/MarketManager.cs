using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace BluehatGames
{

    public class MarketManager : MonoBehaviour
    {
        private bool isLoadig = false;
        public int totalCount = 0;
        private int page = 1;
        public int limit = 10;
        public string order = "Newest";

        public Transform myAnimalContent;

        [Header("Common UI")]
        public GameObject marketItemPrefab;

        public Button myAnimalButton;

        public GameObject myAnimalPanel;
        public Button btnNext;
        public Button btnBefore;


        public Button myAnimalPanelCloseBtn;

        public Button myAnimalSellBtn;

        public GameObject myAnimalDetailPanel;

        public Button bakcToMainBtn;

        public Button myAnimalDetailPanelCloseBtn;

        public Text coinInfoText;
        public Text myAnimalDetailData;

        public Text AnimalDescription;

        public Text AniamlName;

        public Text AnimalPrice;

        public Text AnimalDetailViewCount;

        public Text SellerName;

        public Button animlBuyBtn;

        public User user;

        public GameObject AlertPanel;

        public Button MarketAnimalDoneBtn;

        public GameObject marketMyItemPrefab;



        public Text BuyResultText;
        [Header("My Animal Sell")]
        public InputField inputPrice;

        void Start()
        {
            Debug.Log("MarketManager");
            StartCoroutine(getItemCount());
            StartCoroutine(getItems());
            StartCoroutine(getUserInfo());

            myAnimalPanel.SetActive(false);
            myAnimalDetailPanel.SetActive(false);

            btnNext.onClick.AddListener(() =>
            {
                page = page + 1;
                StartCoroutine(getItems());
            });

            btnBefore.onClick.AddListener(() =>
            {
                page = page - 1;
                StartCoroutine(getItems());
            });

            myAnimalButton.onClick.AddListener(() =>
            {
                myAnimalPanel.SetActive(true);
                StartCoroutine(getUserAnimal());

            });

            myAnimalPanelCloseBtn.onClick.AddListener(() =>
            {
                myAnimalPanel.SetActive(false);
            });

            bakcToMainBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneName._03_Main);
            });

            myAnimalDetailPanelCloseBtn.onClick.AddListener(() =>
            {
                myAnimalDetailPanel.SetActive(false);
            });

            myAnimalSellBtn.onClick.AddListener(() =>
            {
                Debug.Log("myAnimalSellBtn");
                StartCoroutine(sellMyAnimalToMarket());
            });

        }

        private Vector2 setCardPostion(int pageIndex)
        {
            var screenHeight = Screen.height;
            var screenWidth = Screen.width;
            return new Vector2(0, 0);
        }

        private Vector2 setCardSize(int pageCardLimit)
        {
            var screenHeight = Screen.height;
            var screenWidth = Screen.width;
            return new Vector2(100, 100);
        }
        private void OpenAlertPanel(string msg, GameObject prevPanel)
        {
            Debug.Log("Alert Panel ");
            prevPanel.SetActive(false);
            AlertPanel.SetActive(true);
            MarketAnimalDoneBtn.onClick.AddListener(() =>
                {
                    AlertPanel.SetActive(false);
                });
            BuyResultText.text = msg;
        }



        IEnumerator getItemCount()
        {
            string host = "https://api.bluehat.games";
            string localhost = "http://localhost:3000";
            string url = host + "/market/counts";
            Debug.Log($"Request to Get Item -> URL: {url}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError)
                {
                    Debug.Log($"Error: {webRequest.error}");
                }
                else
                {
                    Debug.Log($"Received: {webRequest.downloadHandler.text}");
                }
            }
        }


        IEnumerator getUserInfo()
        {
            string host = "https://api.bluehat.games";
            string localhost = "http://localhost:3000";
            string url = host + "/users";
            Debug.Log($"Request to Get Item -> URL: {url}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.SetRequestHeader("Authorization", "0000");
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError)
                {
                    Debug.Log($"Error: {webRequest.error}");
                }
                else
                {
                    Debug.Log($"Received: {webRequest.downloadHandler.text}");
                    var response = "{\"user\":" + webRequest.downloadHandler.text + "}";
                    Debug.Log($"Received: {response}");
                    var parse_result = JsonUtility.FromJson<UserInfo>(response);
                    Debug.Log($"User Coin: {parse_result.user.coin}");
                    user = parse_result.user;
                    coinInfoText.text = $"{user.coin.ToString()}";
                }
            }
        }

        IEnumerator getItems()
        {
            string host = "https://api.bluehat.games";
            string localhost = "http://localhost:3000";
            string url = host + "/market/list?order=" + order + "&limit=" + limit.ToString() + "&page=" + page.ToString();
            Debug.Log($"Request to Get Item -> URL: {url}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError)
                {
                    Debug.Log($"Error: {webRequest.error}");
                }
                else
                {
                    var response = "{\"items\":" + webRequest.downloadHandler.text + "}";
                    var parse_result = JsonUtility.FromJson<ItemCardList>(response);
                    Debug.Log(parse_result.items);
                    for (int i = 0; i < parse_result.items.Length; i++)
                    {
                        GameObject itemObj = GameObject.Instantiate(marketItemPrefab);
                        itemObj.transform.SetParent(GameObject.Find("MarketMainPanel").transform);
                        itemObj.transform.Find("animal_id").GetComponent<Text>().text = parse_result.items[i].id.ToString();
                        itemObj.transform.Find("animal_name").GetComponent<Text>().text = parse_result.items[i].username;
                        itemObj.transform.Find("price").GetComponent<Text>().text = parse_result.items[i].price.ToString();
                        itemObj.transform.Find("view_count").GetComponent<Text>().text = parse_result.items[i].view_count.ToString();
                        itemObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 + 350 * i, 0);
                        itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                        {
                            Debug.Log("Click");
                            Debug.Log(int.Parse(itemObj.transform.Find("animal_id").GetComponent<Text>().text));
                            StartCoroutine(getAnimalDetail(int.Parse(itemObj.transform.Find("animal_id").GetComponent<Text>().text)));
                        });
                    }
                }
            }
        }



        IEnumerator getUserAnimal()
        {
            string host = "https://api.bluehat.games";
            string localhost = "http://localhost:3000";
            string url = host + "/animal/get-user-animal";
            Debug.Log($"Request to Get Item -> URL: {url}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.SetRequestHeader("Authorization", "0000");
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError)
                {
                    Debug.Log($"Error: {webRequest.error}");
                }
                else
                {
                    Debug.Log($"Received: {webRequest.downloadHandler.text}");
                    var animalInfo = JsonUtility.FromJson<UserAnimalList>(webRequest.downloadHandler.text);
                    for (int i = 0; i < animalInfo.data.Length; i++)
                    {
                        GameObject itemObj = GameObject.Instantiate(marketMyItemPrefab);
                        itemObj.transform.SetParent(myAnimalContent);
                        itemObj.transform.Find("animal_id").GetComponent<Text>().text = animalInfo.data[i].id.ToString();
                        itemObj.transform.Find("animal_name").GetComponent<Text>().text = animalInfo.data[i].name;
                        itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                        {
                            Debug.Log("Click");
                            myAnimalDetailData.text = itemObj.transform.Find("animal_id").GetComponent<Text>().text;
                        });
                    }
                }
            }
        }



        IEnumerator getAnimalDetail(int id)
        {
            string host = "https://api.bluehat.games";
            string localhost = "http://localhost:3000";
            string url = host + "/market/detail?id=" + id.ToString();
            Debug.Log($"Request to Get Item -> URL: {url}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError)
                {
                    Debug.Log($"Error: {webRequest.error}");
                }
                else
                {
                    Debug.Log($"Received: {webRequest.downloadHandler.text}");
                    myAnimalDetailPanel.SetActive(true);
                    var animalInfo = JsonUtility.FromJson<AnimalDetailFromServer>(webRequest.downloadHandler.text).data;
                    AnimalDescription.text = animalInfo.description;
                    AniamlName.text = animalInfo.animal_name;
                    AnimalPrice.text = animalInfo.price.ToString();
                    AnimalDetailViewCount.text = animalInfo.view_count.ToString() + " Views";
                    SellerName.text = animalInfo.username;
                    var buyAnimalId = animalInfo.id;
                    animlBuyBtn.onClick.AddListener(() =>
                    {
                        StartCoroutine(buyAnimal(buyAnimalId));
                    });
                }
            }
        }

        IEnumerator sellMyAnimalToMarket()
        {
            string host = "https://api.bluehat.games";
            string localhost = "http://localhost:3000";
            string url = host + "/market/sell";
            Debug.Log($"Request to Get Item -> URL: {url}");
            using (UnityWebRequest webRequest = UnityWebRequest.Post(url, ""))
            {
                webRequest.SetRequestHeader("Authorization", "0000");
                webRequest.SetRequestHeader("Content-Type", "application/json");
                var price = inputPrice.text;
                var animalId = myAnimalDetailData.text;
                var json = "{\"animal_id\":" + animalId + ", " + "\"price\":" + price + ", " + "\"seller_private_key\":" + "\"0000\"" + "}";
                Debug.Log(json);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log($"Error: {webRequest.error}");
                    OpenAlertPanel("Sell Fail", myAnimalPanel);
                }
                else
                {
                    Debug.Log($"Received: {webRequest.downloadHandler.text}");
                    OpenAlertPanel("Sell Success", myAnimalPanel);
                }
            }
        }


        IEnumerator buyAnimal(int id)
        {
            string host = "https://api.bluehat.games";
            string localhost = "http://localhost:3000";
            string url = host + "/market/buy";
            Debug.Log($"Request to Get Item -> URL: {url}");
            using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
            {
                webRequest.SetRequestHeader("Authorization", "0000");
                webRequest.SetRequestHeader("Content-Type", "application/json");
                string json = "{\"buy_animal_id\":" + id.ToString() + "}";
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                Debug.Log(json);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log($"Error: {webRequest.error}");
                }
                else
                {
                    Debug.Log($"Received: {webRequest.downloadHandler.text}");

                    var result = JsonUtility.FromJson<AnimalBuyResult>(webRequest.downloadHandler.text);
                    if (result.msg == "success")
                    {
                        OpenAlertPanel("Buy Success", myAnimalDetailPanel);
                    }
                    else
                    {
                        OpenAlertPanel("Buy Fail", myAnimalDetailPanel);
                    }
                }
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
        public string animal_color;

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
        public int aniaml_type;
        public string animal_name;
        public string updatedAt;
        public float price;
        public int view_count;
        public string description;

        public ItemCard(int id, string username, int aniaml_type, string animal_name, string updatedAt, float price, int view_count, string description)
        {
            this.id = id;
            this.username = username;
            this.aniaml_type = aniaml_type;
            this.animal_name = animal_name;
            this.updatedAt = updatedAt;
            this.price = price;
            this.view_count = view_count;
            this.description = description;
        }

        public ItemCard getItemCard()
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
        public string antimalType;
        public string headItem;
        public string pattern;
    }

    [Serializable]
    public class UserAnimalList
    {
        public AnimalFormatData[] data;
    }
}
