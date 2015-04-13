<%@ Register TagPrefix="uc1" TagName="FindNearestSiteSrch" Src="RecyclingSiteFinder.ascx" %>
<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="True" Inherits="Escc.RubbishAndRecycling.SiteFinder.Website.WasteSearch" %>

<asp:Content runat="server" ContentPlaceHolderID="metadata">
	<Egms:MetadataControl id="headcontent" runat="server" 
		Title="Find your nearest recycling site"
		IpsvPreferredTerms="Recycling (waste)"
		DateCreated="2004-11-16"
		LgslNumbers="534"
		/>
    <Egms:Css runat="server" Files="FormsSmall" />
    <EastSussexGovUK:ContextContainer runat="server" Desktop="true">
        <Egms:Css runat="server" Files="FormsMedium" MediaConfiguration="Medium" />
        <Egms:Css runat="server" Files="FormsLarge" MediaConfiguration="Large" />
    </EastSussexGovUK:ContextContainer>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="content">
<div class="article" id="container" runat="server">
<article>
        <div class="text">
        <h1>Find your nearest recycling site</h1>

            <p ID="litError" Runat="server" visible="false"></p>
            </div>
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

    <EastSussexGovUK:Related runat="server" id="related">
        <PagesTemplate>
	        <ul>
	        <li><a href="/environment/rubbishandrecycling/recyclingsites/wastesites.htm">Household waste recycling sites &#8211; map</a></li>
	        </ul>
        </PagesTemplate>
    </EastSussexGovUK:Related>

    <EastSussexGovUK:Share runat="server" />
</article>
</div>
</asp:Content>