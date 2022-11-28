namespace BluehatGames
{
    public static class UserRepository
    {
        private static string username;
        private static int coin;
        private static int egg;

        private static bool is_wallet;

        public static void SetUserInfo(string username, int coin, int egg)
        {
            UserRepository.username = username;
            UserRepository.coin = coin;
            UserRepository.egg = egg;
        }

        public static string GetUsername()
        {
            return username;
        }

        public static void SetUsername(string value)
        {
            username = value;
        }

        public static int GetCoin()
        {
            return coin;
        }

        public static void SetCoin(int value)
        {
            coin = value;
        }

        public static int GetEgg()
        {
            return egg;
        }

        public static void SetEgg(int value)
        {
            egg = value;
        }
    }
}