namespace BluehatGames
{
    public class ApiUrl
    {
        //Login
        public const string emailLoginVerify = "http://api.bluehat.games/auth";
        public const string login = "http://api.bluehat.games/users";
        //Animal
        public const string postAnimalNew = "http://api.bluehat.games/animal";
        public const string getAnimalList = "http://api.bluehat.games/animal";

        //Get Header
        public const string AuthGetHeader = "Authorization";
    }
    public class ResponseLogin
    {
        public string msg;
        public string access_token;
    }
    public class ResponseAnimalNew
    {
        public string id;
        public string type;
    }
}
