﻿#if !ONPREMISES
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using SharePointPnP.PowerShell.CmdletHelpAttributes;
using SharePointPnP.PowerShell.Commands.Utilities;
using System;
using System.IO;
using System.Management.Automation;

namespace SharePointPnP.PowerShell.Commands.Provisioning
{

    [Cmdlet(VerbsCommunications.Read, "PnPProvisioningHierarchy")]
    [CmdletHelp("Loads/Reads a PnP provisioning hierarchy from the file system and returns an in-memory instance of this template.",
        Category = CmdletHelpCategory.Provisioning, SupportedPlatform = CmdletSupportedPlatform.Online)]
    [CmdletExample(
       Code = @"PS:> Read-PnPProvisioningHierarchy -Path hierarchy.pnp",
       Remarks = "Reads a PnP provisioning hierarchy file from the file system and returns an in-memory instance",
       SortOrder = 1)]
    public class ReadProvisioningHierarchy : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Filename to read from, optionally including full path.")]
        public string Path;

        [Parameter(Mandatory = false, HelpMessage = "Allows you to specify ITemplateProviderExtension to execute while loading the template.")]
        public ITemplateProviderExtension[] TemplateProviderExtensions;

        protected override void ProcessRecord()
        {
            if (!System.IO.Path.IsPathRooted(Path))
            {
                Path = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Path);
            }
            WriteObject(LoadProvisioningHierarchyFromFile(Path, TemplateProviderExtensions));
        }

        internal static ProvisioningHierarchy LoadProvisioningHierarchyFromFile(string templatePath, ITemplateProviderExtension[] templateProviderExtensions)
        {
            // Prepare the File Connector
            string templateFileName = System.IO.Path.GetFileName(templatePath);

            // Prepare the template path
            var fileInfo = new FileInfo(templatePath);
            FileConnectorBase fileConnector = new FileSystemConnector(fileInfo.DirectoryName, "");

            // Load the provisioning template file
            Stream stream = fileConnector.GetFileStream(templateFileName);
            var isOpenOfficeFile = FileUtilities.IsOpenOfficeFile(stream);

            XMLTemplateProvider provider;
            if (isOpenOfficeFile)
            {
                provider = new XMLOpenXMLTemplateProvider(new OpenXMLConnector(templateFileName, fileConnector));
                templateFileName = templateFileName.Substring(0, templateFileName.LastIndexOf(".", StringComparison.Ordinal)) + ".xml";
            }
            else
            {
                provider = new XMLFileSystemTemplateProvider(fileConnector.Parameters[FileConnectorBase.CONNECTIONSTRING] + "", "");
            }

            ProvisioningHierarchy provisioningHierarchy = provider.GetHierarchy(templateFileName);

            // Return the result
            return provisioningHierarchy;
        }
    }
}
#endif