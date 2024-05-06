# Blackbird.io Google Translate

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Google Translate is a multilingual neural machine translation service developed by Google to translate text, documents and websites from one language into another.

## Before setting up

Before you can connect you need to make sure that:

- You have a `Service account configuration string` for your Google Cloud project. You can create a service account in the Google Cloud Console and download the JSON key file.
- You have a `Project ID` for your Google Cloud project. You can find the Project ID in the Google Cloud Console.

## Connecting

1.  Navigate to Apps, and identify the Google Translate app. You can use search to find it.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My Google Translate connection'.
4. Fill in the 'Service account configuration string' and 'Project ID' fields.
5. Click _Connect_.

## Actions 

- **Translate to language** - Translates text to a specified language
- **Detect language** - Detects the language of the input text
- **Translate document** - Translates a document (file) to a specified language
- **Get supported languages** - Retrieves a list of supported languages

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
