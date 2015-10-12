<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FindExportImportAdminPlugin.aspx.cs" Inherits="Vro.FindExportImport.AdminPlugin.FindExportImportAdminPlugin" %>

<%@ Register TagPrefix="EPiServerUI" Namespace="EPiServer.UI.WebControls" Assembly="EPiServer.UI, Version=7.9.1.0, Culture=neutral, PublicKeyToken=8fe83dea738b45b7" %>
<asp:Content ContentPlaceHolderID="MainRegion" runat="server">
    <EPiServerUI:TabStrip runat="server" ID="actionTab" GeneratesPostBack="True" TargetID="TabView">
		<EPiServerUI:Tab Text="Export" runat="server" ID="Tab1" sticky="True" />
		<EPiServerUI:Tab Text="Import" runat="server" ID="Tab2" sticky="True" />
		<EPiServerUI:Tab Text="Delete" runat="server" ID="Tab3" sticky="True" />
    </EPiServerUI:TabStrip>
    <asp:Panel ID="TabView" runat="server">
        <asp:Panel ID="Export" runat="server">
            <div class="epi-formArea epi-padding">
                <div class="epi-size15">
                    <h2>Here you can export Find optimizations to a JSON file</h2> 
                    <p>
                        Please select a site to export: <asp:DropDownList runat="server" ID="exportSite" AutoPostBack="False"/>
                    </p>
                    <p>
                        Please select a language to export: <asp:DropDownList runat="server" ID="exportLanguage" AutoPostBack="False"/>
                    </p>
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
                     <p>
                        Please select a site to import optimizations: <asp:DropDownList runat="server" ID="importSite" AutoPostBack="False"/>
                    </p>
                    <div>
                        <asp:Label runat="server" AssociatedControlID="fileToImport" Text="Select a file to import" />
                        <input id="fileToImport" type="file" runat="server" accept=".json" />
                    </div>
                	<asp:Panel runat="server" CssClass="EP-systemMessage" ID="importResultsPanel" Visible="False">
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
                <asp:Panel ID="Delete" runat="server">
            <div class="epi-formArea epi-padding">
                <div class="epi-size15">
                    <h2>Here you can delete Find optimizations. </h2> 
                    <b style="color:red">There is no way to restore deleted optimizations.</b>
                    <p>
                        Please select a site for optimizations to delete: <asp:DropDownList runat="server" ID="deleteSite" AutoPostBack="False"/>
                    </p>
                    <p>
                        Please select a language for optimizations to delete: <asp:DropDownList runat="server" ID="deleteLanguage" AutoPostBack="False"/>
                    </p>
                    <p>
                        Please select optimization types to delete form the list below:
                    </p>
                    <asp:Panel runat="server" ID="deleters"></asp:Panel>              
                    <asp:Panel runat="server" CssClass="EP-systemMessage" ID="deleteResultsPanel" Visible="False">
	                    <asp:Literal runat="server" Text="" ID="deleteResults" />
    	            </asp:Panel>
                </div>
            </div>
            <div class="epi-buttonContainer">
                <EPiServerUI:ToolButton ID="ToolButton1" Text="Delete"
                        ToolTip="Re-index selected sites" SkinID="Delete" OnClick="DeleteClick" OnClientClick="return confirm('Are you sure want to delete selected optimizations?')"
                        runat="server" CausesValidation="false" /> 

            </div>
        </asp:Panel>
    </asp:Panel>
        
</asp:Content>