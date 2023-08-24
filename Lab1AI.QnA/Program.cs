using Azure;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.TextAnalytics;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lab1AI.QnA
{
    public class Program
    {
        private static string translatorEndpoint = "https://api.cognitive.microsofttranslator.com";
        private static string cogSvcKey = ("12ec3ce1ab024f70ae1553a5b6526bbe");
        static async Task Main(string[] args)
        {
            //Keys and endpoints to be able to use AI-components and QnA
            Uri endpoint = new Uri("https://langtranslatetext.cognitiveservices.azure.com/");
            AzureKeyCredential credential = new AzureKeyCredential("3df57ce9444a4431b7de5f8a4e7f8fbf");
            string proName = "QnA";
            string deployName = "production";
            QuestionAnsweringClient client = new QuestionAnsweringClient(endpoint, credential);
            QuestionAnsweringProject proj = new QuestionAnsweringProject(proName, deployName);

            // Set console encoding to unicode
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            // Create client using endpoint and key
            Uri endPoint = new Uri("https://coglangservwest.cognitiveservices.azure.com/");
            AzureKeyCredential credentials = new AzureKeyCredential("12ec3ce1ab024f70ae1553a5b6526bbe");
            TextAnalyticsClient CogClient = new TextAnalyticsClient(endPoint, credentials);

            //Create an empty variable for the question to use in loop
            var question = "";
            
            Console.WriteLine("What can I help you with?");

            while (true)
            {
                //To tell the user to ask a question and also give the answer back in the question variable
                question = Console.ReadLine();

                // Get language
                DetectedLanguage detectedLanguage = CogClient.DetectLanguage(question);
                string detectedLanguageCode = detectedLanguage.Iso6391Name;
                Console.WriteLine($"\nLanguage: {detectedLanguageCode}");

                string translated = question;

                if (detectedLanguageCode != "en")
                {
                    translated = await Translate(question, detectedLanguageCode);
                    //Console.WriteLine($"{translated}");
                }

                // Receive question and send it to QnA
                Response<AnswersResult> response = client.GetAnswers(translated, proj);

                foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
                {
                   
                    Console.WriteLine($"Q:{question}");
                    Console.WriteLine($"A:{answer.Answer}");
                    
                }
                
                Console.WriteLine();
                Console.WriteLine("If you want to end chat, write 'quit', otherwise keep on chatting :) ");
                Console.WriteLine();

                if (question.ToLower() == "quit")
                {
                    Console.WriteLine("Okey, bye!");
                    break;
                }
            }
        }

        static async Task<string> Translate(string text, string sourceLanguage)
        {
            string translation = "";

            // Use the Translator translate function
            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    // Build the request
                    string path = "/translate?api-version=3.0&from=" + sourceLanguage + "&to=en";
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(translatorEndpoint + path);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", cogSvcKey);
                    request.Headers.Add("Ocp-Apim-Subscription-Region", "westeurope");

                    // Send the request and get response
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    // Read response as a string
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Parse JSON array and get translation
                    JArray jsonResponse = JArray.Parse(responseContent);
                    translation = (string)jsonResponse[0]["translations"][0]["text"];
                }
            }

            // Return the translation
            return translation;

        }
    }
}


