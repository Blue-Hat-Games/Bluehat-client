namespace BluehatGames
{
    public class ApiUrl
    {
        public enum ApiCategory
        {
            emailLoginVerify,
            userLogin,
            postAnimalNew,
            getAnimalList

        }

        public string liveServer = "https://api.bluehat.games";
        public string testServer = "";

        //Login
        public const string emailLoginVerify = "https://api.bluehat.games/auth";
        public const string login = "https://api.bluehat.games/users";

        //Animal
        public const string postAnimalNew = "https://api.bluehat.games/animal";
        public const string getAnimalList = "https://api.bluehat.games/animal";

        //Get Header
        public const string AuthGetHeader = "Authorization";

        // 
        public const string failAddress = "failedAddress";

        public string GetLiveServerApiUrl()
        {
            string url;

            switch(ApiCategory)
            {
                case ApiCategory.emailLoginVerify:
                    url = $"{liveServer}/{emailLoginVerify}";
                    break;
                case ApiCategory.login:
                    url = $"{liveServer}/{login}";
                    break;
                case ApiCategory.postAnimalNew:
                    url = $"{liveServer}/{postAnimalNew}";
                    break;
                case ApiCategory.getAnimalList:
                    url = $"{liveServer}/{getAnimalList}";
                    break;
                default:
                    
                    break;
            }
            return url;
        }

        public string GetTestServerApiUrl()
        {
            string url;

            switch(ApiCategory)
            {
                case ApiCategory.emailLoginVerify:
                    url = $"{testServer}/{emailLoginVerify}";
                    break;
                case ApiCategory.login:
                    url = $"{testServer}/{login}";
                    break;
                case ApiCategory.postAnimalNew:
                    url = $"{testServer}/{postAnimalNew}";
                    break;
                case ApiCategory.getAnimalList:
                    url = $"{testServer}/{getAnimalList}";
                    break;
                default:
                    
                    break;
            }
            return url;
        }
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
