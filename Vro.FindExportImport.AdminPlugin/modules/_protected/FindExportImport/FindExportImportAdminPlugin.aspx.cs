using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using EPiServer.PlugIn;
using EPiServer.Shell.WebForms;

namespace Vro.FindExportImport.AdminPlugin
{
    [GuiPlugIn(DisplayName = "Find Export / Import",
        Description = "Find Export/Import allows to export and import Find optimizations",
        Area = PlugInArea.AdminMenu, UrlFromModuleFolder = "FindExportImportAdminPlugin.aspx")]
    public partial class FindExportImportAdminPlugin : WebFormsBase
    {
        private ExportManager _exportManager;

        protected override void OnInit(EventArgs e)
        {
            _exportManager = new ExportManager();
            var checkBoxes = _exportManager.GetExporters()
                .Select(exporter => new CheckBox
                {
                    ID = exporter.EntityKey,
                    Text = Helpers.GetEntityName(exporter.EntityKey),
                    Checked = true
                });

            foreach (var checkBox in checkBoxes)
            {
                var wrapper = new Panel {CssClass = "epi-padding-small"};
                wrapper.Controls.Add(checkBox);
                exporters.Controls.Add(wrapper);
            }


            base.OnInit(e);
        }

     

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            importResults.Text = "";
            importResultsPanel.Visible = false;
            SystemMessageContainer.Heading = "Find Export / Import";
            
        }

        protected void ExportClick(object sender, EventArgs e)
        {
            var exportersList = new List<string>();

            foreach (Panel wrapper in exporters.Controls)
            {
                var checkBox = wrapper.Controls[0] as CheckBox;
                if (checkBox != null && checkBox.Checked)
                {
                    exportersList.Add(checkBox.ID);
                }
            }

            var exportManager = new ExportManager();
            Response.Clear();
            Response.ContentType = "applicaiton/json";
            Response.AddHeader("content-disposition", "attachment; filename=FindOptimizations.json");
            exportManager.ExportToStream(Response.OutputStream, exportersList);
            Response.End();
        }

        protected void ImportClick(object sender, EventArgs e)
        {
            if (Request.Files.Count == 0 || Request.Files[0].ContentLength == 0)
            {
                SystemMessageContainer.Message = "No file selected for import";
                return;
            }

            var importManager = new ImportManager();
            var resultsMessage = importManager.ImportFromStream(Request.Files[0].InputStream);
            importResultsPanel.Visible = true;
            importResults.Text = !string.IsNullOrWhiteSpace(resultsMessage) ? resultsMessage.Replace(Environment.NewLine, "<br>") : "Import complete";
            
        }
    }
}