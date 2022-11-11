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
        public int totalCount;
        public int limit = 10;
        public string order = "Newest";

        public User user;

        [Header("Market Main Panel")] public Button backToMainBtn;

        public Button beforeBtn;
        public Button nextBtn;
        public Text coinInfoText;
        public Button myAnimalBtn;
        public GameObject marketItemPrefab;

        [Header("MyAnimal Panel")] public GameObject myAnimalPanel;

        public Button myAnimalCloseBtn;
        public Button myAnimalSellBtn;
        public InputField myAnimalInputPrice;
        public Transform myAnimalContent;
        public GameObject myAnimalItemPrefab;

        [Header("Animal Detail Panel")] public GameObject animalDetailPanel;

        public Text animalDetailName;
        public Text animalDetailSellerName;
        public Text animalDetailPrice;
        public Text animalDetailViewCount;
        public Text animalDetailDescription;
        public RawImage animalDetailImg;
        public Button animalDetailBuyBtn;
        public Button animalDetailCloseBtn;

        [Header("MyAnimalDetail UI")] public Text myAnimalIdHidedData;

        public RawImage myAnimalDetailRawImage;
        public Text myAnimalDetailName;
        public Text myAnimalDetailType;
        public Text myAnimalHatItem;

        [Header("Alert Panel")] public GameObject alertPanel;

        public Button alertPanelDoneBtn;
        public Text alertPanelMsg;

        public AnimalFactory animalFactory;

        public Transform thumbnailSpot;
        public Camera thumbnailCamera;
        public RenderTexture renderTexture;
        private readonly string _host = "https://api.bluehat.games";

        private string _authToken;

        private bool _hasItemObjects;
        private int _page = 1;

        private GameObject[] _pageItemObjects;


        private void Start()
        {
            myAnimalPanel.SetActive(false);
            animalDetailPanel.SetActive(false);

            _authToken = "0000";
            Debug.Log("authToken: " + _authToken);
            _pageItemObjects = new GameObject[5];


            StartCoroutine(GetItemCount());
            StartCoroutine(GetItems());
            StartCoroutine(GetUserInfo());

            nextBtn.onClick.AddListener(() =>
            {
                _page = _page + 1;
                StartCoroutine(GetItems());
            });

            beforeBtn.onClick.AddListener(() =>
            {
                _page = _page - 1;
                StartCoroutine(GetItems());
            });

            myAnimalBtn.onClick.AddListener(() =>
            {
                myAnimalPanel.SetActive(true);
                if (_isBuyResult == false) return;
                StartCoroutine(GetUserAnimal());
            });

            myAnimalCloseBtn.onClick.AddListener(() => { myAnimalPanel.SetActive(false); });

            backToMainBtn.onClick.AddListener(() => { SceneManager.LoadScene(SceneName._03_Main); });

            animalDetailCloseBtn.onClick.AddListener(() => { animalDetailPanel.SetActive(false); });

            myAnimalSellBtn.onClick.AddListener(() => { StartCoroutine(SellMyAnimalToMarket()); });
        }

        /*
         * This Method call for open new panel to send Information
         */
        private void OpenAlertPanel(string msg, GameObject prevPanel)
        {
            prevPanel.SetActive(false);
            alertPanel.SetActive(true);
            alertPanelDoneBtn.onClick.AddListener(() => { alertPanel.SetActive(false); });
            alertPanelMsg.text = msg;
        }


        private IEnumerator GetItemCount()
        {
            var url = _host + "/market/counts";
            using var webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                Debug.Log($"Error: {webRequest.error}");
            else
                Debug.Log($"Received: {webRequest.downloadHandler.text}");
        }

        private IEnumerator GetUserInfo()
        {
            var url = _host + "/user";
            using var webRequest = UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("Authorization", _authToken);
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
            var url = _host + "/market/list?order=" + order + "&limit=" + limit + "&page=" + _page;
            using var webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Error: {webRequest.error}");
            }
            else
            {
                var response = "{\"items\":" + webRequest.downloadHandler.text + "}";
                var parseResult = JsonUtility.FromJson<ItemCardList>(response).items;
                if (_hasItemObjects)
                {
                    _hasItemObjects = false;
                    for (var i = 0; i < _pageItemObjects.Length; i++)
                        if (_pageItemObjects[i] != null)
                            Destroy(_pageItemObjects[i]);
                }

                StartCoroutine(MakeAnimalThumbnail(parseResult));
            }
        }


        private void ResetAnimalState(GameObject animal)
        {
            animal.GetComponent<Rigidbody>().useGravity = false;
            animal.GetComponent<Rigidbody>().isKinematic = true;
            animal.GetComponent<CapsuleCollider>().enabled = false;
        }

        private IEnumerator MakeAnimalThumbnail(ItemCard[] itemCardArray)
        {
            _hasItemObjects = true;

            for (var i = 0; i < itemCardArray.Length; i++)
            {
                var item = itemCardArray[i];
                var itemObj = Instantiate(marketItemPrefab);
                _pageItemObjects[i] = itemObj;

                itemObj.transform.SetParent(GameObject.Find("MarketMainPanel").transform);
                itemObj.transform.Find("animal_id").GetComponent<Text>().text = item.id.ToString();
                itemObj.transform.Find("animal_name").GetComponent<Text>().text = item.username;
                itemObj.transform.Find("price").GetComponent<Text>().text = item.price.ToString();
                itemObj.transform.Find("view_count").GetComponent<Text>().text = item.view_count.ToString();

                var rawImage = itemObj.transform.Find("animal_img").GetComponent<RawImage>();

                var updatedAnimalData = new AnimalDataFormat();
                updatedAnimalData.name = item.animal_name;
                updatedAnimalData.tier = 0;
                updatedAnimalData.id = item.id.ToString();
                updatedAnimalData.animalType = item.animal_type;
                updatedAnimalData.headItem = item.head_item;
                updatedAnimalData.pattern = "";
                updatedAnimalData.color = item.color;

                Debug.Log(
                    $"MarketManager | id = {item.id.ToString()}, animal_type = {item.animal_type}, color = {item.color}");

                var animal = new Animal(updatedAnimalData);
                var animalObject = animalFactory.GetAnimalGameObject(animal);
                ResetAnimalState(animalObject);

                animalObject.transform.position = thumbnailSpot.position;
                animalObject.transform.rotation = thumbnailSpot.rotation;

                thumbnailCamera.Render();

                yield return new WaitForEndOfFrame();

                animalObject.SetActive(false);

                ToTexture2D(renderTexture, resultTex => { rawImage.texture = resultTex; });

                itemObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(340 + 430 * i, 0);
                itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    Debug.Log("Click");
                    Debug.Log(int.Parse(itemObj.transform.Find("animal_id").GetComponent<Text>().text));
                    StartCoroutine(GetAnimalDetail(
                        int.Parse(itemObj.transform.Find("animal_id").GetComponent<Text>().text), rawImage));
                });
            }
        }

        private void ToTexture2D(RenderTexture rTex, Action<Texture2D> action)
        {
            var tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
            // ReadPixels looks at the active RenderTexture.
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            action.Invoke(tex);
        }

        // My Animal List 

        private IEnumerator GetUserAnimal()
        {
            var url = _host + "/animal/get-user-animal";
            using var webRequest = UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("Authorization", _authToken);
            yield return webRequest.SendWebRequest();
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Error: {webRequest.error}");
            }
            else
            {
                Debug.Log($"Received: {webRequest.downloadHandler.text}");
                var animalInfo = JsonUtility.FromJson<UserAnimalList>(webRequest.downloadHandler.text);
                StartCoroutine(MakeMyAnimalThumbnail(animalInfo.data));
                _isBuyResult = false;
            }
        }

        // my room thumbnail
        private IEnumerator MakeMyAnimalThumbnail(AnimalDataFormat[] animalDataArray)
        {
            for (var i = 0; i < animalDataArray.Length; i++)
            {
                var animalData = animalDataArray[i];

                var itemObj = Instantiate(myAnimalItemPrefab, myAnimalContent, true);
                itemObj.transform.Find("animal_id").GetComponent<Text>().text = animalData.id;
                itemObj.transform.Find("animal_name").GetComponent<Text>().text = animalData.name;

                var rawImage = itemObj.transform.Find("animal_img").GetComponent<RawImage>();

                itemObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    Debug.Log("Click");
                    myAnimalDetailName.text = animalData.name;
                    myAnimalDetailRawImage.texture = rawImage.texture;
                    myAnimalDetailType.text = animalData.animalType;
                    var hatItemStr = animalData.headItem;
                    if (animalData.headItem == "" || animalData.headItem == "None") hatItemStr = "-";
                    myAnimalHatItem.text = hatItemStr;
                    myAnimalIdHidedData.text = animalData.id;
                });

                var animal = new Animal(animalData);
                var animalObject = animalFactory.GetAnimalGameObject(animal);
                ResetAnimalState(animalObject);

                animalObject.transform.position = thumbnailSpot.position;
                animalObject.transform.rotation = thumbnailSpot.rotation;

                thumbnailCamera.Render();

                yield return new WaitForEndOfFrame();

                animalObject.SetActive(false);

                ToTexture2D(renderTexture, resultTex => { rawImage.texture = resultTex; });

                if (i == 0)
                {
                    myAnimalDetailName.text = animalData.name;
                    myAnimalDetailRawImage.texture = rawImage.texture;
                    myAnimalDetailType.text = animalData.animalType;
                    myAnimalHatItem.text = animalData.headItem;
                    myAnimalIdHidedData.text = animalData.id;
                }
            }
        } // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator GetAnimalDetail(int id, RawImage rawImg)
        {
            var url = _host + "/market/detail?id=" + id;
            using var webRequest = UnityWebRequest.Get(url);
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
                animalDetailBuyBtn.onClick.AddListener(() => { StartCoroutine(BuyAnimal(buyAnimalId)); });

                animalDetailImg.texture = rawImg.texture;
            }
        }

        public string sellFailString;
        public string sellSuccessString;

        private IEnumerator SellMyAnimalToMarket()
        {
            var url = _host + "/market/sell";
            using var webRequest = UnityWebRequest.Post(url, "");
            webRequest.SetRequestHeader("Authorization", _authToken);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            var price = myAnimalInputPrice.text;
            var animalId = myAnimalIdHidedData.text;
            var json = "{\"animal_id\":" + animalId + ", " + "\"price\":" + price + ", " + "\"seller_private_key\":" +
                       "\"0000\"" + "}";
            var bodyRaw = Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Error: {webRequest.error}");
                OpenAlertPanel(sellFailString, myAnimalPanel);
            }
            else
            {
                OpenAlertPanel(sellSuccessString, myAnimalPanel);
            }
        }

        public string paySuccessMessage;
        public string payFailMessage;

        private bool _isBuyResult = true;

        private IEnumerator BuyAnimal(int id)
        {
            var url = _host + "/market/buy";
            using var webRequest = new UnityWebRequest(url, "POST");
            webRequest.SetRequestHeader("Authorization", _authToken);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            var json = "{\"buy_animal_id\":" + id + "}";
            var bodyRaw = Encoding.UTF8.GetBytes(json);
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
                OpenAlertPanel(result.msg == "success" ? paySuccessMessage : payFailMessage, animalDetailPanel);
                if (result.msg == "success") _isBuyResult = true;
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
        public int animal_type;
        public string animal_name;
        public string color;
    }

    public class AnimalDetailFromServer
    {
        public AnimalDetail data;
        public string status;
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
    }

    [Serializable]
    public class ItemCardList
    {
        public ItemCard[] items;
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
    public class UserAnimalList
    {
        public AnimalDataFormat[] data;
    }
}