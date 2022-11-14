using UnityEngine;
using UnityEngine.UI;

namespace BluehatGames
{
    public class SynthesisCoinController : MonoBehaviour
    {
        public Text coinText;
        void Start()
        {
            Debug.Log("SynthesisCoinController Start");
            coinText.text = UserRepository.GetCoin().ToString();
            Debug.Log("SynthesisCoinController Start coinText.text : " + coinText.text);
        }
    }
}