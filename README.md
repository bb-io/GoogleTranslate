# Blackbird.io Google Translate

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Google Translate is a multilingual neural machine translation service developed by Google to translate text, documents and websites from one language into another.

## Before setting up

You can connect the Google Translate app to Google Cloud using either a Service Account key or Workload Identity Federation (WIF).

### Service account key

- Create or use an existing service account in Google Cloud Console (IAM & Admin → Service accounts)

- Generate a key for for the service account in JSON format.

- Copy key (JSON file full contents) and paste into the "Client configuration JSON string" field during connection.

### Workload Identity Federation (keyless WIF)

- Blackbird supports WIF to obtain short‑lived Google credentials without storing service account keys.

- Typical setup:

	1. In Google Cloud, create a Workload Identity Pool and an OIDC Provider.
	2. Configure the Provider to accept the Azure tokens described below.
	3. Grant the target Google service account the role Workload Identity User, binding it to the provider/attribute conditions.
	4. In Blackbird, create a WIF connection using the dedicated WIF client configuration (see "Connecting" below).

- Azure parameters used by Blackbird:

	- Issuer (URL): `https://sts.windows.net/832b2f36-ef08-4574-9237-b3458f0519d4`
	- Directory (tenant) ID: `832b2f36-ef08-4574-9237-b3458f0519d4`
	- Object ID: `5f0bdb0c-f5f3-4596-82ae-fd2ca1719cc1`	
	- Application (client) ID: `52501bbf-4c87-464f-a96a-7eb82dbb59bf`
	- Application ID URI (audience): `api://52501bbf-4c87-464f-a96a-7eb82dbb59bf`

- Configuration caveats for WIF:

	- Use the Application ID URI as the audience in OIDC provider (`aud` claim).
	- Use the Application (client) ID as the `google.subject`, i.e., `google.subject = assertion.appid`.
	- If you will impersonate a Google service account via WIF (e.g., for Translation API access), download the client configuration from the "Connected service accounts" tab in Blackbird and use it for the connection as the configuration must have `service_account_impersonation_url` parameter in it.

## Connecting

1. Navigate to Apps, and identify the Google Translate app. You can use search to find it.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'Production'.
4. Fill in the 'Service account configuration string' field with JSON content downloaded from Google Cloud Platform.
5. Click _Connect_.

![connection.png](/image/README/Connection.png)

## Actions 

- **Translate text** - Translates text to a specified language. Supports glossary, adaptive datasets and custom AutoML models.
- **Translate** - Translates a document to a specified language
  - Supports glossary, adaptive datasets and custom AutoML models.
  - Works with Blackbirds interoperable actions, so HTML, XLIFF and other Blackbird's interoperable files could be passed as is.
  - Natively supported document formats by Google itself: PDF, DOC, DOCX, PPT, PPTX, XLS, XLSX.
- **Create dataset** - Creates a Cloud Translation dataset for training a custom model. Optionally imports a `.tsv` or `.tmx` file. Files supplied directly to Blackbird are first uploaded to a user-provided Google Cloud Storage bucket.
- **Start custom model training** - Starts training a custom model from an existing populated dataset and returns an operation name.

Google Cloud Translation requires custom datasets, custom models, and their Cloud Storage input files to use `us-central1`. If the user's existing bucket is in another region, a bucket in `us-central1` must be used for this workflow. The connected service account must be allowed to upload objects to that bucket. Google recommends the Storage Admin role for the complete custom-model setup flow.

### Creating a custom model dataset with training data

The **Create dataset** action can create an empty dataset or create a dataset and import training data in the same flight. Training data can be supplied either through **Input file** or through **GCS input source URL**.

Use only one of the following input modes:

| Input | Empty dataset | Input file | Existing GCS file |
| --- | --- | --- | --- |
| **Input file** | Empty | Select or map a `.tsv` or `.tmx` file from an earlier step | Empty |
| **GCS bucket name** | Empty | Bucket name only, for example `translation-training-data` | Empty |
| **GCS input source URL** | Empty | Empty | Full URI, for example `gs://translation-training-data/en-es.tsv` |

When **Input file** is used, the app downloads the file from Blackbird, uploads it to a unique path in the specified bucket, and imports that object into the new dataset. The bucket must be in `us-central1`. Do not include the `gs://` prefix or an object path in **GCS bucket name**. Uploaded objects are not deleted automatically.

Both file input options support `.tsv` and `.tmx` files. See Google's [data preparation requirements](https://cloud.google.com/translate/docs/advanced/custom-nmt-prepare) for file contents and training requirements.

## Events

- **On custom model training completed** - Polls the operation returned by **Start custom model training** and triggers once when training succeeds, fails, or is cancelled.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
