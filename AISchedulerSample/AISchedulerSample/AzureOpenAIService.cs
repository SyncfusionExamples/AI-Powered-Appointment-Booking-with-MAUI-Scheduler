using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AISchedulerSample
{
    internal class AzureOpenAIService
    {
        internal const string endpoint = "https://mobilemaui.openai.azure.com/";

        internal const string deploymentName = "gpt-4o";

        internal const string key = "6673b6975f334c79bd0db8a1cd70aa49";

        private IChatCompletionService? chatCompletions;

        private Kernel? kernel;

        public ChatHistory? ChatHistory
        {
            get
            {
                return chatHistory;
            }
            set
            {
                chatHistory = value;
            }
        }

        public IChatCompletionService? ChatCompletions
        {
            get
            {
                return chatCompletions;
            }
            set
            {
                chatCompletions = value;
            }
        }

        public Kernel? Kernel
        {
            get
            {
                return kernel;
            }
            set
            {
                kernel = value;
            }
        }

        private ChatHistory? chatHistory;
        internal AzureOpenAIService()
        {
            GetAzureOpenAIKernal();
        }

        private void GetAzureOpenAIKernal()
        {
            chatHistory = new ChatHistory();
            var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(deploymentName, endpoint, key);
            kernel = builder.Build();
            chatCompletions = kernel.GetRequiredService<IChatCompletionService>();
        }

        internal async Task<string> GetResponseFromGPT(string userPrompt)
        {
            if (ChatCompletions != null && ChatHistory != null)
            {
                ChatHistory.Clear();
                ChatHistory.AddSystemMessage("You are a predictive analytics assistant.");
                ChatHistory.AddUserMessage(userPrompt);
                try
                {
                    var response = await ChatCompletions.GetChatMessageContentAsync(chatHistory: ChatHistory, kernel: Kernel);
                    return response.ToString();
                }
                catch
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }
    }
}
