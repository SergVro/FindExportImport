<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FindExportImportAdminPlugin.aspx.cs" Inherits="Vro.FindExportImport.AdminPlugin.FindExportImportAdminPlugin" %>

<%@ Register TagPrefix="EPiServerUI" Namespace="EPiServer.UI.WebControls" Assembly="EPiServer.UI, Version=7.9.1.0, Culture=neutral, PublicKeyToken=8fe83dea738b45b7" %>
<asp:Content ContentPlaceHolderID="MainRegion" runat="server">
    <EPiServerUI:TabStrip runat="server" ID="actionTab" GeneratesPostBack="True" TargetID="TabView">
		<EPiServerUI:Tab Text="Export" runat="server" ID="Tab1" sticky="True" />
		<EPiServerUI:Tab Text="Import" runat="server" ID="Tab2" sticky="True" />
    </EPiServerUI:TabStrip>
    <asp:Panel ID="TabView" runat="server">
        <asp:Panel ID="Export" runat="server">
            <div class="epi-formArea epi-padding">
                <div class="epi-size15">
                    <h2>Here you can export Find optimizations to a JSON file</h2> 
                    <p>
                        Please select optimizations to export form the list below:
                    </p>
                    <asp:Panel runat="server" ID="exporters"></asp:Panel>              
                </div>
            </div>
            <div class="epi-buttonContainer">
                <EPiServerUI:ToolButton ID="exportButton" Text="Export"
                        ToolTip="Re-index selected sites" SkinID="Export" OnClick="ExportClick"
                        runat="server" CausesValidation="false" /> 

            </div>
        </asp:Panel>
        <asp:Panel ID="Import" runat="server">
            <div class="epi-formArea epi-padding">
                <div class="epi-size15">
                    <h2>Here you can import Find optimizations from a JSON file.</h2>
                    <div>
                        <asp:Label runat="server" AssociatedControlID="fileToImport" Text="Select a file to import" />
                        <input id="fileToImport" type="file" runat="server" accept=".json" />
                    </div>
                	<asp:Panel runat="server" CssClass="EP-systemMessage" ID="importResultsPanel">
	                    <asp:Literal runat="server" Text="" ID="importResults" />
	                </asp:Panel>
                </div>
            </div>
            <div class="epi-buttonContainer">
                  <EPiServerUI:ToolButton ID="importButton" Text="Import"
                        ToolTip="Re-index selected sites" SkinID="Import" OnClick="ImportClick"
                        runat="server" CausesValidation="false" /> 
            </div>
        </asp:Panel>
    </asp:Panel>
        
</asp:Content>