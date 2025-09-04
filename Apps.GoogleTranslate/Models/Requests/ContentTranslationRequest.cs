﻿using Apps.GoogleTranslate.DataSourceHandlers;
using Apps.GoogleTranslate.DataSourceHandlers.Enums;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;
using Blackbird.Applications.SDK.Blueprints.Handlers;

namespace Apps.GoogleTranslate.Models.Requests;

public class ContentTranslationRequest : BaseGoogleTranslationRequest, ITranslateFileInput
{
    [Display("Content file")]
    public FileReference File { get; set; } = new();

    [Display("File translation strategy", Description = "Select whether to use Google Translate's own file processing capabilities or use Blackbird interoperability mode")]
    [StaticDataSource(typeof(FileTranslationStrategyHandler))]
    public string? FileTranslationStrategy { get; set; }

    [Display("Output file handling", Description = "Determine the format of the output file. The default Blackbird behavior is to convert to interoperable XLIFF for future steps unless native translation strategy is used.")]
    [StaticDataSource(typeof(ProcessFileFormatHandler))]
    public string? OutputFileHandling { get; set; }
}
