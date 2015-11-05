using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using EPiServer.Find.Helpers.Text;
using EPiServer.Logging.Compatibility;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.Shell;
using EPiServer.Shell.WebForms;
using Newtonsoft.Json;
using PlugInArea = EPiServer.PlugIn.PlugInArea;

// ReSharper disable once CheckNamespace
namespace Vro.FindExportImport.AdminPlugin
{
    [GuiPlugIn(DisplayName = "Find Export / Import",
        Description = "Find Export / Import allows to export and import Find optimizations",
        Area = PlugInArea.AdminMenu, UrlFromModuleFolder = "FindExportImportAdminPlugin.aspx")]
    public partial class FindExportImportAdminPlugin : WebFormsBase
    {
        private ExportManager _exportManager;
        private ImportManager _importManager;
        private ILog _log = LogManager.GetLogger(typeof (FindExportImportAdminPlugin));
        private List<CheckboxId> _exportersCheckBoxes;
        private List<CheckboxId> _deletersCheckBoxes;

        protected override void OnInit(EventArgs e)
        {
            _importManager = new ImportManager();
            _exportManager = new ExportManager();

            _exportersCheckBoxes = CreateCheckBoxes(exportersPanel, _exportManager.GetExporters().Select(exporter => new cbLinks
            {
                Id = exporter.EntityKey, Text = Helpers.GetEntityName(exporter.EntityKey), Link = exporter.UiUrl
            }), true);

            _deletersCheckBoxes = CreateCheckBoxes(deletersPanel, _exportManager.GetExporters().Select(exporter => new cbLinks
            {
                Id = exporter.EntityKey, Text = Helpers.GetEntityName(exporter.EntityKey), Link = exporter.UiUrl
            }), false);

            _exportManager.GetSites().ForEach(s => exportSite.Items.Add(new ListItem(s.Name, s.Id)));
            _exportManager.GetSites().ForEach(s => importSite.Items.Add(new ListItem(s.Name, s.Id)));
            _exportManager.GetSites().ForEach(s => deleteSite.Items.Add(new ListItem(s.Name, s.Id)));

            _exportManager.GetLanguages().ForEach(l => exportLanguage.Items.Add(new ListItem(l.Name, l.Id)));
            _exportManager.GetLanguages().ForEach(l => deleteLanguage.Items.Add(new ListItem(l.Name, l.Id)));

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            exportResults.Text = "";
            exportResultPanel.Visible = false;

            importResults.Text = "";
            importResultsPanel.Visible = false;

            deleteResults.Text = "";
            deleteResultsPanel.Visible = false;

            SystemMessageContainer.Heading = "Find Export / Import";
            confirmUnderstand.Checked = false;
        }

        private List<CheckboxId> CreateCheckBoxes(Panel container, IEnumerable<cbLinks> cbLinks, bool defaultChecked)
        {
            var checkBoxList = new List<CheckboxId>();
            cbLinks.ToList().ForEach(c =>
            {
                var checkBox = new CheckBox
                {
                    ID = container.ID + c.Id,
                    Text = c.Text,
                    Checked = defaultChecked
                };

                checkBoxList.Add(new CheckboxId
                {
                    CheckBox = checkBox,
                    Id = c.Id
                });

                var wrapper = new Panel { CssClass = "epi-padding-small" };
                var hyperLink = new HyperLink
                {
                    Text = "(0)",
                    ID = "link"+ container.ID + c.Id,
                    Target = "_blank",
                    NavigateUrl = Paths.ToResource("Find", c.Link)
                };
                wrapper.Controls.Add(checkBox);
                wrapper.Controls.Add(hyperLink);
                container.Controls.Add(wrapper);
            });

            return checkBoxList;
        }

        protected void ExportClick(object sender, EventArgs e)
        {
            var exportersList = _exportersCheckBoxes.Where(c => c.CheckBox.Checked).Select(c => c.Id).ToList();
            _log.DebugFormat("Export EPiServer Find optimizations. User {0}. Optimizations: {1}, site: {2}, language: {3}", 
                PrincipalInfo.Current.Name, string.Join(",", exportersList), exportSite.SelectedItem.Text, exportLanguage.SelectedItem.Text);
            if (exportersList.Any())
            {
                Response.Clear();
                Response.ContentType = "applicaiton/json";
                Response.AddHeader("content-disposition", "attachment; filename=FindOptimizations.json");
                _exportManager.ExportToStream(exportersList, exportSite.SelectedValue, exportLanguage.SelectedValue,
                    Response.OutputStream);
                Response.End();
            }
            else
            {
                exportResultPanel.Visible = true;
                exportResults.Text = "No optimizations selected to export";
            }
        }

        protected void ImportClick(object sender, EventArgs e)
        {
            if (Request.Files.Count == 0 || Request.Files[0].ContentLength == 0)
            {
                SystemMessageContainer.Message = "No file selected for import";
                return;
            }
            var resultsMessage = _importManager.ImportFromStream(importSite.SelectedValue, Request.Files[0].InputStream);
            resultsMessage = !string.IsNullOrWhiteSpace(resultsMessage) ? resultsMessage.Replace(Environment.NewLine, "<br>") : "Import complete";
            importResultsPanel.Visible = true;
            importResults.Text = resultsMessage;
            _log.DebugFormat("Import EPiServer Find optimizations complete. User: {0}. Site: {1}. Results: {2}.", 
                PrincipalInfo.Current.Name, importSite.SelectedItem.Text, resultsMessage);
        }

        protected void DeleteClick(object sender, EventArgs e)
        {
            var deletersList = _deletersCheckBoxes.Where(c => c.CheckBox.Checked).Select(c => c.Id).ToList();
            _log.WarnFormat("Deleting EPiServer Find optimizations. User: {0}. Optimizations: {1}, site: {2}, language: {3}", 
                PrincipalInfo.Current.Name, string.Join(",", deletersList), deleteSite.SelectedItem.Text, deleteLanguage.SelectedItem.Text);

            _exportManager.Delete(deletersList, deleteSite.SelectedValue, deleteLanguage.SelectedValue);
            deleteResultsPanel.Visible = true;
            deleteResults.Text = "Deletion complete";
        }

        public string GetAllIds()
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var ids = new Dictionary<string, string>();
            AddControlsId(ids, Controls);
            var serializer = new JsonSerializer();
            serializer.Serialize(stringWriter, ids);
            return stringWriter.ToString();
        }

        private void AddControlsId(Dictionary<string, string> idDictionary, ControlCollection controlCollection)
        {
            foreach (Control control in controlCollection)
            {
                if (!string.IsNullOrEmpty(control.ID) && !idDictionary.ContainsKey(control.ID))
                {
                    idDictionary.Add(control.ID, control.ClientID);
                }
                if (control.Controls.Count > 0)
                {
                    AddControlsId(idDictionary, control.Controls);
                }
            }
        }
    }

    public class cbLinks
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
    }

    public class CheckboxId
    {
        public CheckBox CheckBox { get; set; }
        public string Id { get; set; }
    }
}