# Blackbird.io Google Translate

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Google Translate is a multilingual neural machine translation service developed by Google to translate text, documents and websites from one language into another.

## Before setting up

Before you can connect you need to make sure that:

- You have a `Service account configuration string` for your Google Cloud project. You can create a service account or use an existing one in the Google Cloud Console (IAM & Admin -> Service accounts) and generate a key for it. The key will be a JSON file that contains the service account configuration string. You should copy the content of the JSON file and paste it into the `Service account configuration string` field.

## Connecting

1.  Navigate to Apps, and identify the Google Translate app. You can use search to find it.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My Google Translate connection'.
4. Fill in the 'Service account configuration string' field.
5. Click _Connect_.

![connection.png](/image/README/Connection.png)

## Actions 

- **Translate** - Translates text to a specified language. Action also supports glossary and adaptive datasets.
- **Detect language** - Detects the language of the input text
- **Translate document** - Translates a document to a specified language
  - Supported input document formats:
    - DOC application/msword
    - DOCX application/vnd.openxmlformats-officedocument.wordprocessingml.document
    - PDF application/pdf
    - PPT application/vnd.ms-powerpoint
    - PPTX application/vnd.openxmlformats-officedocument.presentationml.presentation
    - XLS application/vnd.ms-excel
    - XLSX application/vnd.openxmlformats-officedocument.spreadsheetml.sheet

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
