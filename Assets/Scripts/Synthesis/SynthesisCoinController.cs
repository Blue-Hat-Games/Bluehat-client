using UnityEngine;
using TMPro;

namespace BluehatGames
{
    public class SynthesisCoinController : MonoBehaviour
    {
        public TextMeshProUGUI coinText;
        void Start()
        {
            coinText.text = UserRepository.GetCoin().ToString();
        }
    }
}