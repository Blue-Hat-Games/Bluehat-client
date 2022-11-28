using UnityEngine;

/** 멀티플레이에서 플레이어의 상태
- 에테르 에너지
- 에테르 획득 개수
*/
namespace BluehatGames
{
    public class PlayerStatusController : MonoBehaviour
    {
        public static PlayerStatusController instance;

        public int energyToExchangeAether = 50;
        public float addedAetherEnergyValue = 5;

        public float gameTime;

        public AudioClip addAetherCoinEffectSound;

        public Animator addAetherPlusUIAnim;

        private int aetherCount;
        private float aetherEnergy;


        private int eggCount;

        private bool isGameOver;
        private bool isStartTimeAttack;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != null)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            MultiplayUIController.instance.SetCurrentAetherCoinCount(aetherCount);
        }

        private void Update()
        {
            if (false == isStartTimeAttack)
                return;

            gameTime -= Time.deltaTime;

            if (gameTime < 0)
                GameOver();
            else
                MultiplayUIController.instance.UpdateGameTimeText(gameTime);
        }

        public void AddMultiplayEggCount()
        {
            eggCount++;
            MultiplayUIController.instance.SetCurrentEggCount(eggCount);
        }

        // 멀티플레이 도중에 얻는 egg Count에 대한 처리
        public int GetMultiplyEggCount()
        {
            return eggCount;
        }

        public bool IsGameOver()
        {
            return isGameOver;
        }

        public void SetStartTimeAttack()
        {
            isStartTimeAttack = true;
        }

        private void GameOver()
        {
            // Update에서 여러 번 호출될 수 있는 경우를 방지하기 위함
            if (isGameOver) return;

            MultiplayUIController.instance.ResetGameTimeText();

            isGameOver = true;
            // 이번 판에서 획득한 코인
            var myCoin = GetMultiplyAetherCount();
            // AetherController를 통해 획득 정보 저장
            AetherController.instance.AddAetherCount(myCoin);

            // 총 얻은 코인 
            var allMyCoin = AetherController.instance.GetAetherCount();

            // 알
            var originEggCount = PlayerPrefs.GetInt(PlayerPrefsKey.key_AnimalEgg);
            var allEggCount = originEggCount + GetMultiplyAetherCount();
            PlayerPrefs.SetInt(PlayerPrefsKey.key_AnimalEgg, allEggCount);

            // Joystick 비활성화
            MultiplayUIController.instance.SetJoystickCanvasActive(false);
            // UI에 반영
            MultiplayUIController.instance.SetMultiplayResultPanel(GetMultiplyAetherCount(), allMyCoin,
                GetMultiplyEggCount(), allEggCount);
        }

        // 멀티플레이 도중에 얻는 Aether Count에 대한 처리
        public int GetMultiplyAetherCount()
        {
            return aetherCount;
        }

        private void AddMultiplayAetherCount()
        {
            aetherCount++;
            SoundManager.instance.PlayEffectSound(addAetherCoinEffectSound);
            MultiplayUIController.instance.SetCurrentAetherCoinCount(aetherCount);
            addAetherPlusUIAnim.SetTrigger("PlayTrigger");
        }

        // Aether Energy는 멀티플레이에서 플레이어 상태 정보이므로 여기에서 관리함
        public float GetAetherEnergy()
        {
            return aetherEnergy;
        }

        public void AddAetherEnergy()
        {
            aetherEnergy += addedAetherEnergyValue;
            var adjustedEnergyValue = aetherEnergy / energyToExchangeAether;

            // 교환 가능한 만큼 에너지를 다 모았으면 에너지 초기화, 에테르 획득
            if (aetherEnergy >= energyToExchangeAether)
            {
                aetherEnergy = 0;
                // UI 설정
                MultiplayUIController.instance.ResetAetherProgressBar();
                AddMultiplayAetherCount();
            }
            else
            {
                // UI 설정
                MultiplayUIController.instance.SetAetherProgressBar(adjustedEnergyValue);
            }
        }

        public void SubAetherEnergy(float value)
        {
            aetherEnergy -= value;
            if (aetherEnergy < 0)
                // 에너지양은 0보다 작아질 수는 없도록 한다.
                aetherEnergy = 0;
        }
    }
}