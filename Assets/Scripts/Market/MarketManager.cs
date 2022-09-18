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
        public GameObject marketItemPrefab;

        public Button myAnimalButton;

        public GameObject myAnimalPanel;
        public Button btnNext;
        public Button btnBefore;


        public Button myAnimalPanelCloseBtn;

        public Image myAnimalSellBtn;

        public GameObject myAnimalDetailPanel;

        public Button bakcToMainBtn;

        public Button myAnimalDetailPanelCloseBtn;


        public Text myAnimalDetailData;

        public Text AnimalDetailData;

        void Start()
        {
            Debug.Log("MarketManager");
            StartCoroutine(getItemCount());
            StartCoroutine(getItems());

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



        IEnumerator getItemCount()
        {
            string host = "https://api.bluehat.games";
            string localhost = "http://localhost:3000";
            string url = localhost + "/market/counts";
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


        IEnumerator getItems()
        {
            string host = "https://api.bluehat.games";
            string localhost = "http://localhost:3000";
            Debug.Log(order);
            Debug.Log(limit);
            string url = localhost + "/market/list?order=" + order + "&limit=" + limit.ToString() + "&page=" + page.ToString();
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
            string url = localhost + "/animal/get-user-animal";
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
                    string[] nameList = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
                    Debug.Log($"Received: {webRequest.downloadHandler.text}");
                    myAnimalDetailData.text = webRequest.downloadHandler.text;
                }
            }
        }

        IEnumerator getAnimalDetail(int id)
        {
            string host = "https://api.bluehat.games";
            string localhost = "http://localhost:3000";
            string url = localhost + "/market?id=" + id.ToString();
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
                    AnimalDetailData.text = webRequest.downloadHandler.text;
                }
            }
        }
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

}
