using Azure;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Text;


namespace Lab1AI.QnA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Keys and endpoints to be able to use AI-components and QnA
            Uri endpoint = new Uri("https://langservwesteu.cognitiveservices.azure.com/");
            AzureKeyCredential credential = new AzureKeyCredential("ef4e6380edeb4cf08a9510d564871198");
            string proName = "QnA";
            string deployName = "production";
            QuestionAnsweringClient client = new QuestionAnsweringClient(endpoint, credential);
            QuestionAnsweringProject proj = new QuestionAnsweringProject(proName, deployName);

            // Get config settings from AppSettings
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            string cogSvcEndpoint = configuration["CognitiveServicesEndpoint"];
            string cogSvcKey = configuration["CognitiveServiceKey"];

            // Set console encoding to unicode
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            // Create client using endpoint and key
            AzureKeyCredential credentials = new AzureKeyCredential(cogSvcKey);
            Uri endPoint = new Uri(cogSvcEndpoint);
            TextAnalyticsClient CogClient = new TextAnalyticsClient(endPoint, credentials);

            //Create an empty variable for the question to use in loop
            var question = "";

            while (true)
            {
                //To tell the user to ask a question and also give the answer back in the question variable
                //Kanske ta denna ett steg upp och ändra denna för att bara få ut "quit" under konversationens gång
                Console.WriteLine("What can I help you with?");
                question = Console.ReadLine();

                if (question.ToLower() == "quite") {
                    Console.WriteLine("Okey, bye!");
                    break;
                }

                // Get language
                DetectedLanguage detectedLanguage = CogClient.DetectLanguage(question);
                string detectedLanguageCode = detectedLanguage.Iso6391Name;
                Console.WriteLine($"\nLanguage: {detectedLanguageCode}");

                // Recive question and send it to QnA
                Response<AnswersResult> response = client.GetAnswers(question, proj);

                foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
                {
                    Console.WriteLine($"Q:{question}");
                    Console.WriteLine($"A:{answer.Answer}");
                }
            }
        }
    }
}

//using Azure;
//using Azure.AI.Language.QuestionAnswering;
//using Azure.AI.TextAnalytics;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Text;

//namespace Lab1AI.QnA
//{
//    internal class Program
//    {
//        static void Main(string[] args)
//        {
//            // Keys and endpoints for AI components and QnA
//            Uri endpoint = new Uri("https://langservwesteu.cognitiveservices.azure.com/");
//            AzureKeyCredential credential = new AzureKeyCredential("ef4e6380edeb4cf08a9510d564871198");
//            string proName = "QnA";
//            string deployName = "production";
//            QuestionAnsweringClient qaClient = new QuestionAnsweringClient(endpoint, credential);
//            QuestionAnsweringProject qaProject = new QuestionAnsweringProject(proName, deployName);

//            // Get config settings from AppSettings
//            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
//            IConfigurationRoot configuration = builder.Build();
//            string cogSvcEndpoint = configuration["CognitiveServicesEndpoint"];
//            string cogSvcKey = configuration["CognitiveServiceKey"];

//            // Set console encoding to Unicode
//            Console.InputEncoding = Encoding.Unicode;
//            Console.OutputEncoding = Encoding.Unicode;

//            // Create clients for Text Analytics
//            AzureKeyCredential textAnalyticsCredentials = new AzureKeyCredential(cogSvcKey);
//            Uri textAnalyticsEndPoint = new Uri(cogSvcEndpoint);
//            TextAnalyticsClient textAnalyticsClient = new TextAnalyticsClient(textAnalyticsEndPoint, textAnalyticsCredentials);

//            string question = "";

//            while (question.ToLower() != "quit")
//            {
//                Console.WriteLine("What can I help you with?");
//                question = Console.ReadLine();

//                // Detect language of the question
//                DetectedLanguage detectedLanguage = textAnalyticsClient.DetectLanguage(question);

//                // Receive question and send it to QnA
//                Response<AnswersResult> response = qaClient.GetAnswers(question, qaProject);

//                foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
//                {
//                    // Translate the answer to the original language if needed
//                    string translatedAnswer = answer.Answer;
//                    if (detectedLanguage.Iso6391Name != "en")
//                    {
//                        TranslationResult backTranslationResult = textAnalyticsClient.TranslateText(new[] { answer.Answer }, detectedLanguage.Iso6391Name);
//                        translatedAnswer = backTranslationResult.Translations[0].Text;
//                    }

//                    Console.WriteLine($"Q: {question}");
//                    Console.WriteLine($"A: {translatedAnswer}");
//                }
//            }
//        }
//    }
//}


