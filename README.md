# Radiology Report Explainer

PowerScribe plugin that translates radiology impressions into patient-friendly language using Claude AI.

## Features

- ✅ Translates medical radiology impressions into clear, patient-friendly language
- ✅ Uses Claude Sonnet 4.5 AI (Anthropic)
- ✅ Enforces 2000 character limit
- ✅ Includes legal disclaimer for patient safety
- ✅ Built in C# for PowerScribe integration

## Project Status

- **Core translation engine:** Complete and tested ✓
- **PowerScribe integration:** Pending (waiting for Nuance sandbox access)

## Technology Stack

- **Language:** C# (.NET 9)
- **AI API:** Anthropic Claude Sonnet 4.5
- **Target Platform:** PowerScribe 360 (Nuance/Microsoft)

## Files

- `RadiologyTranslationEngine.cs` - Core translation engine with Claude API integration
- `Program.cs` - Test console application
- `RadReportExplainer.csproj` - Project configuration

## Setup

1. Install .NET SDK 9.0+
2. Get Anthropic API key from https://console.anthropic.com
3. Run: `dotnet restore`
4. Run: `dotnet run`
5. Enter API key when prompted

## Next Steps

- PowerScribe COM API integration
- Plugin UI (button/hotkey trigger)
- Deployment packaging
