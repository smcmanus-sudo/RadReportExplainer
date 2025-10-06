using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RadReportExplainer
{
    /// <summary>
    /// Translates radiology impressions into patient-friendly language using Claude AI
    /// </summary>
    public class RadiologyTranslationEngine
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private const int MAX_CHARACTERS = 2000;
        private const string CLAUDE_API_URL = "https://api.anthropic.com/v1/messages";
        private const string CLAUDE_MODEL = "claude-sonnet-4-5-20250929";

        public RadiologyTranslationEngine(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        }

        /// <summary>
        /// Translates a radiology impression into patient-friendly language
        /// </summary>
        /// <param name="impression">The radiology impression text</param>
        /// <returns>Translation result with patient summary and disclaimer</returns>
        public async Task<TranslationResult> TranslateAsync(string impression)
        {
            if (string.IsNullOrWhiteSpace(impression))
            {
                throw new ArgumentException("Impression text cannot be empty", nameof(impression));
            }

            try
            {
                var startTime = DateTime.UtcNow;

                // Call Claude API
                string patientSummary = await CallClaudeApiAsync(impression);

                // Enforce character limit
                if (patientSummary.Length > MAX_CHARACTERS)
                {
                    patientSummary = patientSummary.Substring(0, MAX_CHARACTERS - 3) + "...";
                }

                var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                return new TranslationResult
                {
                    Success = true,
                    PatientSummary = patientSummary,
                    Disclaimer = GetLegalDisclaimer(),
                    CharacterCount = patientSummary.Length,
                    ResponseTimeMs = responseTime
                };
            }
            catch (Exception ex)
            {
                return new TranslationResult
                {
                    Success = false,
                    ErrorMessage = $"Translation failed: {ex.Message}"
                };
            }
        }

        private async Task<string> CallClaudeApiAsync(string impression)
        {
            var prompt = BuildPrompt(impression);

            var requestBody = new
            {
                model = CLAUDE_MODEL,
                max_tokens = 1024,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(CLAUDE_API_URL, httpContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ClaudeApiResponse>(responseContent);

            if (apiResponse?.content == null || apiResponse.content.Length == 0)
            {
                throw new Exception("Invalid response from Claude API");
            }

            return apiResponse.content[0].text;
        }

        private string BuildPrompt(string impression)
        {
            return $@"You are translating a radiology report impression into patient-friendly language.

CRITICAL REQUIREMENTS:
- Use simple, clear language that a patient without medical training can understand
- Avoid medical jargon, or explain technical terms in plain English
- Be accurate - do not add information that isn't in the original impression
- Be reassuring and warm in tone, but honest about findings
- Keep your response under {MAX_CHARACTERS} characters
- Do not include any introductory phrases like 'Here is the translation' - start directly with the patient-friendly explanation

RADIOLOGY IMPRESSION:
{impression}

Provide the patient-friendly translation now:";
        }

        private string GetLegalDisclaimer()
        {
            return @"
────────────────────────────────────────────────────────────
IMPORTANT NOTICE TO PATIENTS:

This patient-friendly summary is provided for educational purposes only and is not a substitute for the official radiology report above. It is intended to help you understand medical terminology, but should not be used for self-diagnosis or treatment decisions.

Please discuss any questions, concerns, or findings with your healthcare provider. Your doctor is the best resource for interpreting your results and determining appropriate next steps for your care.

This summary was generated using artificial intelligence technology to assist in translating medical language. While every effort is made to ensure accuracy, only the official radiology report above should be considered the authoritative medical record.
────────────────────────────────────────────────────────────";
        }

        // API Response models
        private class ClaudeApiResponse
        {
            public ContentItem[] content { get; set; }
        }

        private class ContentItem
        {
            public string text { get; set; }
        }
    }

    /// <summary>
    /// Result of a translation operation
    /// </summary>
    public class TranslationResult
    {
        public bool Success { get; set; }
        public string PatientSummary { get; set; }
        public string Disclaimer { get; set; }
        public int CharacterCount { get; set; }
        public double ResponseTimeMs { get; set; }
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the complete formatted output to insert into the report
        /// </summary>
        public string GetFormattedOutput()
        {
            if (!Success)
            {
                return $"Error: {ErrorMessage}";
            }

            return $@"

PATIENT-FRIENDLY SUMMARY:
{PatientSummary}
{Disclaimer}";
        }
    }
}