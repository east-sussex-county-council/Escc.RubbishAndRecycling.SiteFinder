<%@ Control Language="c#" AutoEventWireup="true" CodeBehind="TopicSection_RecyclingSiteFinder.ascx.cs"
    Inherits="Escc.RubbishAndRecycling.SiteFinder.Website.RecyclingSiteFinder" %>
<form runat="server">
    <div class="form short-form">
        <h2 id="h2" runat="server">Find your nearest recycling site</h2>
        <div class="formPart">
            <asp:Label CssClass="formLabel" AssociatedControlID="wasteTypes" runat="server">I need to recycle</asp:Label>
            <asp:DropDownList ID="wasteTypes" CssClass="formControl" runat="server" />
        </div>
        <div class="formPart">
            <asp:Label CssClass="formLabel" AssociatedControlID="postcode" runat="server">Near postcode</asp:Label>
            <asp:TextBox ID="postcode" runat="server" CssClass="postcode describedby-tip" aria-describedby="postcode-help" data-tip-positions="right"></asp:TextBox>
            <p id="postcode-help">We won't keep or share your postcode.</p>
        </div>
        <asp:Button ID="Go" Text="Find your nearest" CssClass="button" runat="server" />
    </div>
</form>