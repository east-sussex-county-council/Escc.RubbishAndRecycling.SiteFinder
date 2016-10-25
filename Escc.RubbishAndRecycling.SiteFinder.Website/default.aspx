<%@ Register TagPrefix="uc1" TagName="FindNearestSiteSrch" Src="RecyclingSiteFinder.ascx" %>
<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="True" Inherits="Escc.RubbishAndRecycling.SiteFinder.Website.WasteSearch" %>

<asp:Content runat="server" ContentPlaceHolderID="metadata">
	<Metadata:MetadataControl id="headcontent" runat="server" 
		Title="Find your nearest recycling site"
		IpsvPreferredTerms="Recycling (waste)"
		DateCreated="2004-11-16"
		LgslNumbers="534"
		/>
    <ClientDependency:Css runat="server" Files="FormsSmall" />
    <EastSussexGovUK:ContextContainer runat="server" Desktop="true">
        <ClientDependency:Css runat="server" Files="FormsMedium" MediaConfiguration="Medium" />
        <ClientDependency:Css runat="server" Files="FormsLarge" MediaConfiguration="Large" />
    </EastSussexGovUK:ContextContainer>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="content">
<div class="article" id="container" runat="server">
<article>
    <div class="content text-content">
        <h1>Find your nearest recycling site</h1>

        <p ID="litError" Runat="server" visible="false"></p>
        <uc1:FindNearestSiteSrch id="Srch" runat="server" DisplayTitle="false" />
      
        <NavigationControls:PagingController id="paging" runat="server" ResultsTextSingular="result" ResultsTextPlural="results" />
        <NavigationControls:PagingBarControl id="pagingTop" runat="server" PagingControllerId="paging" />

        <asp:Repeater id="rptResults" runat="server">
            <ItemTemplate>
		        <dl class="itemDetail">
			        <dt>Name:</dt>
			        <dd><%# "<a href=\"" + DataBinder.Eval(Container.DataItem, "URL").ToString() + "\">" + DataBinder.Eval(Container.DataItem, "Title") + "</a>" %></dd>
			        <dt>Distance:</dt>
			        <dd><%# DataBinder.Eval(Container.DataItem, "Miles") + " miles" %></dd>
		        </dl>
	        </ItemTemplate>
        </asp:Repeater>

        <NavigationControls:PagingBarControl id="pagingBottom" runat="server" PagingControllerId="paging" />

        <EastSussexGovUK:Share runat="server" />
    </div>
</article>
</div>
</asp:Content>