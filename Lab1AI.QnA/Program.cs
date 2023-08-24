using Azure;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Text;
using DotNetEnv;


namespace Lab1AI.QnA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Env.Load();
            //string cogSvcEndpoint = Environment.GetEnvironmentVariable("https://langtranslatewesteu.cognitiveservices.azure.com/");
            //string cogSvcKey = Environment.GetEnvironmentVariable("0e02403e30fd4c7ba70a8b7eb5d0c6c5");
            //string qnaKey = Environment.GetEnvironmentVariable("ef4e6380edeb4cf08a9510d564871198");

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

                // Receive question and send it to QnA
                Response<AnswersResult> response = client.GetAnswers(question, proj);

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

        static async Task<string> Translate(string text, string detectedLanguageCode)
        {
            string translation = "";

            // Use the Translator translate function


            // Return the translation
            return translation;

        }
    }
}



// Get config settings from AppSettings
//IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
//IConfigurationRoot configuration = builder.Build();
//string cogSvcEndpoint = configuration["CognitiveServicesEndpoint"];
//string cogSvcKey = configuration["CognitiveServiceKey"];


//Translate the answer to the original language if needed
//string translatedAnswer = answer.Answer;
//if (detectedLanguageCode != "en")
//{
//    TranslationResult backTranslationResult = TextAnalyticsClient.Translate(new[] { answer.Answer }, detectedLanguageCode);
//    translatedAnswer = backTranslationResult.Translations[0].Text;
//}