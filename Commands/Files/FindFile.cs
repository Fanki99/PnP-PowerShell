﻿using System.Management.Automation;
using Microsoft.SharePoint.Client;
using SharePointPnP.PowerShell.CmdletHelpAttributes;
using SharePointPnP.PowerShell.Commands.Base.PipeBinds;

namespace SharePointPnP.PowerShell.Commands.Files
{
    [Cmdlet(VerbsCommon.Find, "SPOFile", DefaultParameterSetName = "Web")]
    [CmdletHelp("Finds a file in the virtual file system of the web.",
         Category = CmdletHelpCategory.Files,
         OutputType = typeof(File),
         OutputTypeLink = "https://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.file.aspx")]
    [CmdletExample(
         Code = @"PS:> Find-SPOFile -Match *.master",
         Remarks = "Will return all masterpages located in the current web.",
         SortOrder = 1)]
      [CmdletExample(
         Code = @"PS:> Find-SPOFile -List ""Documents"" -Match *.pdf",
         Remarks = "Will return all pdf files located in given list.",
         SortOrder = 2)]
     [CmdletExample(
         Code = @"PS:> Find-SPOFile -Folder ""Shared Documents/Sub Folder"" -Match *.docx",
         Remarks = "Will return all docx files located in given folder.",
         SortOrder = 3)]
    public class FindFile : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Wildcard query", ValueFromPipeline = true, ParameterSetName = "Web")]
        [Parameter(Mandatory = true, HelpMessage = "Wildcard query", ValueFromPipeline = true, ParameterSetName = "List")]
        [Parameter(Mandatory = true, HelpMessage = "Wildcard query", ValueFromPipeline = true, ParameterSetName = "Folder")]
        public string Match = string.Empty;

        [Parameter(Mandatory = true, HelpMessage = "List title, url or an actual List object to query", ParameterSetName = "List")]
        public ListPipeBind List;

        [Parameter(Mandatory = true, HelpMessage = "Folder object or relative url of a folder to query", ParameterSetName = "Folder")]
        public FolderPipeBind Folder;

        protected override void ExecuteCmdlet()
        {
            switch (ParameterSetName)
            {
                case "List":
                {
                    var list = List.GetList(SelectedWeb);
                    WriteObject(list.FindFiles(Match));
                    break;
                }
                case "Folder":
                {
                    var folder = Folder.GetFolder(SelectedWeb);
                    WriteObject(folder.FindFiles(Match));
                    break;
                }
                default:
                {
                    WriteObject(SelectedWeb.FindFiles(Match));
                    break;
                }
            }
        }
    }
}
