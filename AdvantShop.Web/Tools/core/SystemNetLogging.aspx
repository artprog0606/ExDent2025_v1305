<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SystemNetLogging.aspx.cs" Inherits="Tools.core.SystemNetLoggingPage" MasterPageFile="MasterPage.master" %>

<asp:Content runat="server" ID="cntHead" ContentPlaceHolderID="head">
    <title>AdvantShop.NET Core Tools</title>
</asp:Content>
<asp:Content runat="server" ID="cntMain" ContentPlaceHolderID="main">
    <table style="border-collapse: collapse; width: 100%; padding: 0; margin: 0;">
        <tr>
            <td style="border-bottom: 1px solid black; text-align: center;">
                <h1>System.Net.Logging</h1>
            </td>
        </tr>
        <tr>
            <td>
                <fieldset>
                    <asp:Button ID="btnActivateOneMinute" runat="server" OnClick="btnActivateOneMinute_Click" Text="Activate One Minute" />
                    <asp:Button ID="btnActivateThreeMinute" runat="server" OnClick="btnActivateThreeMinute_Click" Text="Activate Three Minute" />
                    <asp:Button ID="btnActivateFiveMinute" runat="server" OnClick="btnActivateFiveMinute_Click" Text="Activate Five Minute" />
                    <asp:Button ID="btnDeactivateLoggin" runat="server" OnClick="btnDeactivateLoggin_Click" Text="Deactivate Loggin" />
                </fieldset>
                <fieldset>
                    <asp:Repeater ID="LogRepeater" runat="server" OnItemCommand="LogRepeater_OnItemCommand">
                        <HeaderTemplate>
                            <table style="background-color: White; border: 1px none #CCCCCC; width: 99%; border-collapse: collapse;">
                                <tr style="color: White; background-color: #006699; font-weight: bold;">
                                    <th style="text-align: left;">Log file</th>
                                    <th style="text-align: left;"></th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr style="color: #000066;">
                                <td style="border-style: solid; border-width: 1px;">
                                    <asp:LinkButton runat="server" CommandName="Download" CommandArgument="<%# Container.DataItem %>" Text="<%# Container.DataItem %>"></asp:LinkButton>
                                </td>
                                <td style="border-style: solid; border-width: 1px;">
                                    <asp:LinkButton runat="server" CommandName="Delete" CommandArgument="<%# Container.DataItem %>" Text="Delete"></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </fieldset>
            </td>
        </tr>
    </table>
</asp:Content>
