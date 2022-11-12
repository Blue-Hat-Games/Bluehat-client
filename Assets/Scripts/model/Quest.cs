using System;

namespace BluehatGames
{
    [Serializable]
    public class Quest
    {
        public string type;
        public string title;
        public string description;
        public string action;
        public int reward_coin;
        public int reward_egg;
        public bool status;
        public bool get_reward;
        public string createdAt;
    }
}