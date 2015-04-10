<%@ Control Language="c#" AutoEventWireup="true" CodeBehind="SearchForm.ascx.cs"
    Inherits="Escc.RubbishAndRecycling.SiteFinder.Website.SearchForm" %>
<div class="form short-form">
    <h2>Find your nearest recycling site</h2>
    <div class="formPart">
        <asp:Label CssClass="formLabel" AssociatedControlID="wasteTypes" runat="server">I need to recycle</asp:Label>
        <asp:DropDownList ID="wasteTypes" CssClass="formControl" runat="server" />
    </div>
    <div class="formPart">
        <asp:Label CssClass="formLabel" AssociatedControlID="postcode" runat="server">Near postcode</asp:Label>
        <asp:TextBox ID="postcode" runat="server" CssClass="postcode"></asp:TextBox>
        <input type="text" name="ieBugFix" style="display:none;" />
    </div>
    <asp:Button ID="Go" Text="Find your nearest" CssClass="button" runat="server" />
</div>