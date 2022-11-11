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

        //Login
        public const string emailLoginVerify = "https://api.bluehat.games/auth";
        public const string login = "https://api.bluehat.games/user";

        //Animal
        public const string postAnimalNew = "https://api.bluehat.games/animal/make-animal";
        public const string getAnimalList = "https://api.bluehat.games/animal";
        public const string getUserAnimal = "https://api.bluehat.games/animal/get-user-animal";

        //Synthesis
        public const string postChangeColor = "https://api.bluehat.games/animal/change-color";
        public const string postFusion = "https://api.bluehat.games/animal/merge";
        public const string postFusionResultPNG = "https://api.bluehat.games/nft/mint";
        public const string getRandomHat = "https://api.bluehat.games/hat/new-hat";


        // Quest
        public const string getQuestList = "https://api.bluehat.games/quest";
        public const string getQuestCount = "https://api.bluehat.games/quest/count";


        // wallet
        public const string CreateNewWallet = "https://api.bluehat.games" + "/wallet/create-new-wallet";

        //Get Header Authorization
        public const string AuthGetHeader = "Authorization";

        // 해당되는 값이 없을 때 리턴할까 싶어서 만들어 본 변수인데 또 굳이..? 싶기도? 
        public const string failAddress = "failedAddress";
        public static string prodSever = "https://api.bluehat.games";

        public static string devServer = "http://localhost:3000";

        public string liveServer = "https://api.bluehat.games";

        public string testServer = "";

        public string GetLiveServerApiUrl(ApiCategory apiCategory)
        {
            var url = apiCategory switch
            {
                ApiCategory.emailLoginVerify => $"{liveServer}/{emailLoginVerify}",
                ApiCategory.userLogin => $"{liveServer}/{login}",
                ApiCategory.postAnimalNew => $"{liveServer}/{postAnimalNew}",
                ApiCategory.getAnimalList => $"{liveServer}/{getAnimalList}",
                _ => ""
            };

            return url;
        }

        public string GetTestServerApiUrl(ApiCategory apiCategory)
        {
            var url = apiCategory switch
            {
                ApiCategory.emailLoginVerify => $"{testServer}/{emailLoginVerify}",
                ApiCategory.userLogin => $"{testServer}/{login}",
                ApiCategory.postAnimalNew => $"{testServer}/{postAnimalNew}",
                ApiCategory.getAnimalList => $"{testServer}/{getAnimalList}",
                _ => ""
            };

            return url;
        }
    }

    public class ResponseLogin
    {
        public string access_token;
        public string msg;
    }

    public class ResponseAnimalNew
    {
        public string id;
        public string type;
    }

    public class ResponseResult
    {
        public string msg;
    }

    public class ResponseHatResult
    {
        public string new_item;
    }

    public class RequestRandomHatFormat
    {
        public string animalId;
    }

    public class RequestColorChangeAnimalFormat
    {
        public string animalId;
    }

    public class RequestFusionAnimalFormat
    {
        public string animalId1;
        public string animalId2;
    }
}