using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using static OpenAI_API.Chat.ChatRequest;

namespace GenXdev.Helpers
{
    public class OpenAiExampleQuestion
    {
        public string ExampleQuestion { get; set; }
        public string ExampleAnswer { get; set; }
    }

    public class OpenAi : IHttpClientFactory
    {
        public static string InvokeChatRequest(string systemInstructions, string userRequest, List<OpenAiExampleQuestion> Examples = null, string ApiUrlFormat = null, double Temperature = 0)
        {
            var task = InvokeChatRequestAsync(systemInstructions, userRequest, Examples, ApiUrlFormat, Temperature);

            task.Wait();

            return task.Result;
        }

        HttpClient IHttpClientFactory.CreateClient(string name)
        {
            var client = new HttpClient();

            client.Timeout = TimeSpan.FromDays(1);

            return client;
        }

        public static Conversation CreateChatRequest(string systemInstructions, List<OpenAiExampleQuestion> Examples = null, string ApiUrlFormat = null, double Temperature = 0)
        {
            // Initialize the OpenAI API with your key
            OpenAIAPI api = new OpenAIAPI("not-needed");

            // Set the custom endpoint
            if (!String.IsNullOrWhiteSpace(ApiUrlFormat))
            {
                api.ApiUrlFormat = ApiUrlFormat; // "http://host:1234/{0}/{1}";
            }
            api.HttpClientFactory = new OpenAi();

            var chat = api.Chat.CreateConversation();

            if (!String.IsNullOrWhiteSpace(ApiUrlFormat) && !ApiUrlFormat.Contains("openai"))
            {
                chat.Model = Model.DefaultTTSModel;
            }
            else
            {
                chat.Model = Model.GPT4_Turbo;
                api.Auth = APIAuthentication.LoadFromEnv();
            }

            chat.RequestParameters.Temperature = Temperature;
            chat.RequestParameters.ResponseFormat = ResponseFormats.JsonObject;

            /// give instruction as System
            chat.AppendSystemMessage(systemInstructions);

            if (Examples != null)
            {

                foreach (var example in Examples)
                {
                    chat.AppendUserInput(example.ExampleQuestion);
                    chat.AppendExampleChatbotOutput(example.ExampleAnswer);
                }
            }

            // and get the response
            return chat;
        }

        public async static Task<string> InvokeChatRequestAsync(string systemInstructions, string userRequest, List<OpenAiExampleQuestion> Examples = null, string ApiUrlFormat = null, double Temperature = 0)
        {
            var chat = CreateChatRequest(systemInstructions, Examples, ApiUrlFormat, Temperature);

            // and get the response
            return await InvokeNextChatRequestAsync(chat, userRequest);
        }


        public async static Task<string> InvokeNextChatRequestAsync(Conversation chat, string userRequest)
        {
            chat.AppendUserInput(userRequest);

            // and get the response
            return await chat.GetResponseFromChatbotAsync();
        }

        public static string InvokeNextChatRequest(Conversation chat, string userRequest)
        {
            chat.AppendUserInput(userRequest);

            // and get the response
            var task = chat.GetResponseFromChatbotAsync();

            task.Wait();

            return task.Result;
        }
    }
}
