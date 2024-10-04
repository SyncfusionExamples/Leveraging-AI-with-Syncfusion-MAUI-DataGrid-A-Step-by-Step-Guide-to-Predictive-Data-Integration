
using Azure.AI.OpenAI;
using Azure;


namespace AnamolyPrediction
{
    public class OpenAi
    {
        private readonly HttpClient _httpClientFactory;
        const string endpoint = "https://mobilemaui.openai.azure.com/";
        const string deploymentName = "GPT-4O";  // Or "GPT-4O"

        public OpenAi()
        {

        }
        public async Task<string> GetResponseFromOpenAI(string prompt)
        {
            try
            {
                var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential("6673b6975f334c79bd0db8a1cd70aa49"));
                var chatCompletionsOptions = new ChatCompletionsOptions
                {

                    DeploymentName = deploymentName,
                    Temperature = (float)0.5,
                    MaxTokens = 800,
                    NucleusSamplingFactor = (float)0.95,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0,

                };

                // Add the system message and user message to the options
                chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage("You are a predictive analytics assistant"));
                chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(prompt));

                var response = await client.GetChatCompletionsAsync(chatCompletionsOptions);
                var message = response.Value.Choices[0].Message.Content;
                return message;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
