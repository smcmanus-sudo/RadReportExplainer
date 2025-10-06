using System;
using System.Threading.Tasks;

namespace RadReportExplainer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("Radiology Translation Engine - Test Program");
            Console.WriteLine("==============================================\n");

            // Get API key from environment variable or user input
            string apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
            
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.Write("Enter your Anthropic API key: ");
                apiKey = Console.ReadLine();
            }

            var engine = new RadiologyTranslationEngine(apiKey);

            // Test with sample radiology impressions
            await RunTests(engine);

            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task RunTests(RadiologyTranslationEngine engine)
        {
            // Sample impressions for testing
            var testCases = new[]
            {
                new
                {
                    Name = "Chest X-ray: Pneumonia",
                    Impression = "Right lower lobe pneumonia. Small right pleural effusion. Heart size normal. No pneumothorax."
                },
                new
                {
                    Name = "Brain MRI: Normal",
                    Impression = "No acute intracranial abnormality. No mass effect, midline shift, or abnormal enhancement. Ventricles and sulci are normal in size and configuration."
                },
                new
                {
                    Name = "Knee X-ray: Arthritis",
                    Impression = "Mild degenerative changes of the medial compartment with joint space narrowing and small osteophytes. No acute fracture or dislocation."
                }
            };

            foreach (var testCase in testCases)
            {
                Console.WriteLine($"\n{'─',60}");
                Console.WriteLine($"TEST: {testCase.Name}");
                Console.WriteLine($"{'─',60}");
                Console.WriteLine($"\nORIGINAL IMPRESSION:\n{testCase.Impression}\n");
                Console.WriteLine("Translating...\n");

                var result = await engine.TranslateAsync(testCase.Impression);

                if (result.Success)
                {
                    Console.WriteLine("✓ TRANSLATION SUCCESSFUL\n");
                    Console.WriteLine($"Response Time: {result.ResponseTimeMs:F0}ms");
                    Console.WriteLine($"Character Count: {result.CharacterCount}/{2000}\n");
                    Console.WriteLine(result.GetFormattedOutput());
                }
                else
                {
                    Console.WriteLine($"✗ TRANSLATION FAILED\n");
                    Console.WriteLine($"Error: {result.ErrorMessage}");
                }

                Console.WriteLine($"\n{'─',60}\n");
            }
        }
    }
}