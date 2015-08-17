<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="KeepService.Default" EnableViewStateMac ="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnDailyInit" runat="server" Text="DailyInit" OnClick="btnDailyInit_Click" />
        <asp:Button ID="btnWorkInit" runat="server" Text="WorkInit" OnClick="btnWorkInit_Click" />
        <asp:Button ID="btnNode" runat="server" Text="NewNode" OnClick="btnNode_Click" />
        <br />
        <br />
        <asp:Calendar ID="Calendar1" runat="server" SelectedDate="12/16/2013 16:26:34"></asp:Calendar>
        <br />
        <asp:Button ID="btnDaily" runat="server" Text="DailyRate" OnClick="btnDaily_Click"  />
        <asp:Button ID="btnWeek" runat="server" Text="WeekRate" OnClick="btnWeek_Click"  />
        <br />
        <asp:Label ID="lbResult" runat="server" Text="------------"></asp:Label>
    </div>
    </form>
</body>
</html>
