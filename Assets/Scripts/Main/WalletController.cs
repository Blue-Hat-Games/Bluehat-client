using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

namespace BluehatGames
{
    public class WalletController : MonoBehaviour
    {

        private static string SECRET_HIDE = "•••••••••••••••••••••••••••••••••••••••••••••••••";
        public Button btn_wallet;
        public Image img_btn_wallet_alert;
        public GameObject walletPanel;
        public Button btn_wallet_pannel_close;
        public AudioClip upperButtonSound;

        [Header("Wallet SignUp")]
        public GameObject walletSignUpPanel;
        public Button btn_wallet_login;
        public Button btn_wallet_signup;


        [Header("Wallet Info")]
        public GameObject walletInfoPanel;
        public InputField input_wallet_address;
        public InputField input_wallet_private_key;
        public Button btn_wallet_private_key_show;
        public InputField input_klaytn_wallet_key;
        public Button btn_klaytn_wallet_key_show;

        public GameObject Mission;



        void Start()
        {
            walletPanel.SetActive(false);
            btn_wallet.onClick.AddListener(() =>
            {
                SoundManager.instance.PlayEffectSound(upperButtonSound);
                // Local Repository에서 월렛 정보 가져옴.
                Wallet wallet = WalletLocalRepositroy.GetWalletInfo();
                walletPanel.SetActive(true);
                if (wallet == null)
                {
                    walletSignUpPanel.SetActive(true);
                    walletInfoPanel.SetActive(false);
                }
                else
                {
                    SetWalletInfo(wallet);
                    walletSignUpPanel.SetActive(false);
                    walletInfoPanel.SetActive(true);
                }
            });

            btn_wallet_pannel_close.onClick.AddListener(() =>
            {
                input_wallet_private_key.text = SECRET_HIDE;
                input_klaytn_wallet_key.text = SECRET_HIDE;
                walletPanel.SetActive(false);
            });


            btn_wallet_signup.onClick.AddListener(() =>
            {
                StartCoroutine(CreateNewWallet());
            });

            btn_wallet_login.onClick.AddListener(() =>
            {

            });

            btn_wallet_private_key_show.onClick.AddListener(() =>
            {
                if (input_wallet_private_key.text == SECRET_HIDE)
                {
                    input_wallet_private_key.text = WalletLocalRepositroy.GetWallletPrivateKey();
                }
                else
                {
                    input_wallet_private_key.text = SECRET_HIDE;
                }
            });

            btn_klaytn_wallet_key_show.onClick.AddListener(() =>
            {
                if (input_klaytn_wallet_key.text == SECRET_HIDE)
                {
                    input_klaytn_wallet_key.text = WalletLocalRepositroy.GetKlaytnWalletKey();
                }
                else
                {
                    input_klaytn_wallet_key.text = SECRET_HIDE;
                }
            });
        }

        public void SetWalletInfo(Wallet wallet)
        {
            if (wallet != null)
            {
                input_wallet_address.text = wallet.address;
                input_wallet_private_key.text = SECRET_HIDE;
                input_klaytn_wallet_key.text = SECRET_HIDE;
            }
        }



        IEnumerator CreateNewWallet()
        {
            using (UnityWebRequest request = UnityWebRequest.Post(ApiUrl.CreateNewWallet, ""))
            {
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Authorization", AccessToken.GetAccessToken());

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log(request);
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("Response: " + request.downloadHandler.text);
                    Wallet walletInfo = JsonUtility.FromJson<Wallet>(request.downloadHandler.text);
                    WalletLocalRepositroy.setWalletInfo(walletInfo);
                    input_wallet_address.text = walletInfo.address;
                    input_wallet_private_key.text = SECRET_HIDE;
                    input_klaytn_wallet_key.text = SECRET_HIDE;
                    walletSignUpPanel.SetActive(false);
                    walletInfoPanel.SetActive(true);
                    img_btn_wallet_alert.gameObject.SetActive(false);
                    MissionUtils missionResult = Mission.GetComponent<MissionUtils>();
                    missionResult.createWalletEvent();
                }
            }
        }

    }


}
