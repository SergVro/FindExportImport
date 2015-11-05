<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FindExportImportAdminPlugin.aspx.cs" Inherits="Vro.FindExportImport.AdminPlugin.FindExportImportAdminPlugin" %>

<%@ Register TagPrefix="EPiServerUI" Namespace="EPiServer.UI.WebControls" Assembly="EPiServer.UI, Version=7.9.1.0, Culture=neutral, PublicKeyToken=8fe83dea738b45b7" %>
<asp:Content ContentPlaceHolderID="MainRegion" runat="server">
    <EPiServerUI:TabStrip runat="server" ID="actionTab" GeneratesPostBack="True" TargetID="TabView">
		<EPiServerUI:Tab Text="Export" runat="server" ID="tab1" sticky="True" />
		<EPiServerUI:Tab Text="Import" runat="server" ID="tab2" sticky="True" />
		<EPiServerUI:Tab Text="Delete" runat="server" ID="tab3" sticky="True" />
    </EPiServerUI:TabStrip>
    <asp:Panel ID="tabView" runat="server">
        <asp:Panel ID="exportPanel" runat="server">
            <div class="epi-formArea epi-padding">
                <div class="epi-size25">
                    <h2>Here you can export Find optimizations to a JSON file</h2> 
                    <p>
                        <asp:Label runat="server" Text="Please select a site for optimizations to export:" AssociatedControlID="exportSite"></asp:Label>
                        <asp:DropDownList runat="server" ID="exportSite" AutoPostBack="False"/>
                    </p>
                    <p>
                        <asp:Label runat="server" Text="Please select a language for optimizations to export:" AssociatedControlID="exportLanguage"></asp:Label>
                        <asp:DropDownList runat="server" ID="exportLanguage" AutoPostBack="False"/>
                    </p>
                    <p>
                        Please select optimization types to export form the list below:
                    </p>
                    <asp:Panel runat="server" ID="exportersPanel"></asp:Panel>        
                    <asp:Panel runat="server" CssClass="EP-systemMessage" ID="exportResultPanel" Visible="False">
	                    <asp:Literal runat="server" Text="" ID="exportResults" />
	                </asp:Panel>
                </div>
            </div>
            <div class="epi-buttonContainer">
                <EPiServerUI:ToolButton ID="exportButton" Text="Export"
                        ToolTip="Re-index selected sites" SkinID="Export" OnClick="ExportClick"
                        runat="server" CausesValidation="false" /> 

            </div>
        </asp:Panel>
        <asp:Panel ID="importPanel" runat="server">
            <div class="epi-formArea epi-padding">
                <div class="epi-size25">
                    <h2>Here you can import Find optimizations from a JSON file.</h2>
                     <p>
                        <asp:Label runat="server" Text="Please select a site to import optimizations:" AssociatedControlID="importSite"></asp:Label>
                        <asp:DropDownList runat="server" ID="importSite" AutoPostBack="False"/>
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
                <asp:Panel ID="deletePanel" runat="server" >
            <div class="epi-formArea epi-padding">
                <div class="epi-size25">
                    <h2>Here you can delete Find optimizations. </h2> 
                    <b style="color:red">There is no way to restore deleted optimizations.</b>
                    <p>
                        <asp:Label runat="server" Text="Please select a site for optimizations to delete:" AssociatedControlID="deleteSite"></asp:Label>
                        <asp:DropDownList runat="server" ID="deleteSite" AutoPostBack="False"/>
                    </p>
                    <p>
                        <asp:Label runat="server" Text="Please select a language for optimizations to delete:" AssociatedControlID="deleteLanguage"></asp:Label>
                        <asp:DropDownList runat="server" ID="deleteLanguage" AutoPostBack="False"/>
                    </p>
                    <p>
                        Please select optimization types to delete form the list below:
                    </p>
                    <asp:Panel runat="server" ID="deletersPanel"></asp:Panel>              
                    <asp:Panel runat="server" CssClass="EP-systemMessage" ID="deleteResultsPanel" Visible="False">
	                    <asp:Literal runat="server" Text="" ID="deleteResults" />
    	            </asp:Panel>
                </div>
            </div>
            <div class="epi-buttonContainer epi-formArea">
                <b style="color: red">
                    <asp:CheckBox runat="server" ID="confirmUnderstand" Checked="False" Text="I do understand that this is irreversible" />
                </b>
                <EPiServerUI:ToolButton ID="deleteButton" Text="Delete"
                        ToolTip="Re-index selected sites" SkinID="Delete" OnClick="DeleteClick" OnClientClick="return confirm('Are you sure want to delete selected optimizations?')"
                        runat="server" CausesValidation="false" /> 

            </div>
        </asp:Panel>
    </asp:Panel>
    <script src="Scripts/AdminPlugin.js"></script>
    <script>
        $(function () {
            window.epi.findExportImportPlugin.init({
                exportSite: "<%=exportSite.ClientID%>",
                exportLanguage: "<%=exportLanguage.ClientID%>",
                deleteSite: "<%=deleteSite.ClientID%>",
                deleteLanguage: "<%=deleteLanguage.ClientID%>",
                confirmUnderstand: "<%=confirmUnderstand.ClientID%>",
                exportResultPanel: "<%=exportResultPanel.ClientID%>",
                exportButton: "<%=exportButton.ClientID%>",
                deleteButton: "<%=deleteButton.ClientID%>"
            });
        });
    </script>
</asp:Content>