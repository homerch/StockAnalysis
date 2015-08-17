<%@ Page Title="台股籌碼分析" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication._Default" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1 itemprop="name">台股籌碼分析</h1>
            </hgroup>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <br />
            <fieldset>
                <legend>近日交易狀況</legend>
                <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="receiveDate,stockId" DataSourceID="EntityDataSource1" OnRowDataBound="GridView1_RowDataBound" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" Width="100%" AllowSorting="True" OnSorting="GridView1_Sorting">
                    <AlternatingRowStyle BackColor="#FFCC99" />
                    <Columns>
                        <asp:BoundField DataField="receiveDate" HeaderText="交易日期" ReadOnly="True" SortExpression="receiveDate" DataFormatString="{0:yyyy/MM/dd}" />
                        <asp:BoundField DataField="stockId" HeaderText="股號" ReadOnly="True" SortExpression="stockId" />
                        <asp:BoundField DataField="name" HeaderText="股名" ReadOnly="True" SortExpression="name" />
                        <asp:BoundField DataField="tradeAmt" HeaderText="成交金額" SortExpression="tradeAmt" DataFormatString="{0:#,##0.;(#,##0);0}" />
                        <asp:BoundField DataField="change" HeaderText="漲跌幅" SortExpression="change" DataFormatString="{0:F2}" />
                        <asp:BoundField DataField="openPrice" HeaderText="開盤價" SortExpression="openPrice" DataFormatString="{0:F2}" />
                        <asp:BoundField DataField="highPrice" HeaderText="最高價" SortExpression="highPrice" DataFormatString="{0:F2}" />
                        <asp:BoundField DataField="lowPrice" HeaderText="最低價" SortExpression="lowPrice" DataFormatString="{0:F2}" />
                        <asp:BoundField DataField="lastPrice" HeaderText="收盤價" SortExpression="lastPrice" DataFormatString="{0:F2}" />
                        <asp:BoundField DataField="oldPrice" HeaderText="昨收" SortExpression="oldPrice" DataFormatString="{0:F2}" />
                        <%--<asp:BoundField DataField="tradeRec" HeaderText="成交筆數" SortExpression="tradeRec" DataFormatString="{0:#,##0.;(#,##0);0}" />--%>
                        <%--<asp:BoundField DataField="tradeQty" HeaderText="成交股數" SortExpression="tradeQty" DataFormatString="{0:#,##0.;(#,##0);0}" />--%>
                    </Columns>
                    <HeaderStyle HorizontalAlign="Right" />
                    <RowStyle HorizontalAlign="Right" />
                    <EmptyDataTemplate>
                        查無資料
                    </EmptyDataTemplate>
                </asp:GridView>
            </fieldset>
            <br />
            <fieldset>
                <legend>近日證券商交易狀況</legend>
                <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" DataSourceID="EntityDataSource2" OnRowDataBound="GridView2_RowDataBound" OnSelectedIndexChanged="GridView2_SelectedIndexChanged" Width="100%" AllowPaging="True" AllowSorting="True" OnSorting="GridView2_Sorting">
                    <AlternatingRowStyle BackColor="#CCFF99" />
                    <Columns>
                        <asp:BoundField DataField="receiveDate" HeaderText="交易日期" SortExpression="receiveDate" ReadOnly="True" DataFormatString="{0:yyyy/MM/dd}" />
                        <asp:BoundField DataField="brokerName" HeaderText="證券商" SortExpression="brokerName" ReadOnly="True" />
                        <asp:BoundField DataField="brokerBranch" HeaderText="分公司" SortExpression="brokerBranch" ReadOnly="True" />
                        <asp:BoundField DataField="tradeAmt" HeaderText="成交金額" SortExpression="tradeAmt" DataFormatString="{0:#,##0.;(#,##0);0}" />
                        <asp:BoundField DataField="netVolume" HeaderText="買賣超" SortExpression="netVolume" DataFormatString="{0:#,##0.;-#,##0;0}" />
                        <asp:BoundField DataField="buyVolume" HeaderText="買進股數" SortExpression="buyVolume" DataFormatString="{0:#,##0.;(#,##0);0}" />
                        <asp:BoundField DataField="sellVolume" HeaderText="賣出股數" SortExpression="sellVolume" DataFormatString="{0:#,##0.;(#,##0);0}" />
                    </Columns>
                    <HeaderStyle HorizontalAlign="Right" />
                    <RowStyle HorizontalAlign="Right" />
                    <EmptyDataTemplate>
                        查無資料
                    </EmptyDataTemplate>
                </asp:GridView>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:EntityDataSource ID="EntityDataSource1" runat="server" ConnectionString="name=stockdbaEntities" DefaultContainerName="stockdbaEntities" EnableFlattening="False" AutoPage="False"
        CommandText="select main.receiveDate, main.stockId, main.name, main.tradeAmt, main.openPrice, main.highPrice, main.lowPrice, main.lastPrice, main.oldPrice,
                     (case when main.oldPrice != 0 then (main.lastPrice - main.oldPrice)*100/main.oldPrice else 0 end ) as change
                     from 
                     (
                     select it.receiveDate, it.stockId, it.Company.name, it.tradeAmt, it.openPrice, it.highPrice, it.lowPrice, it.lastPrice,
                     ANYELEMENT(select VALUE TOP(1) sub.lastPrice from DailySummary as sub where sub.stockId = it.stockId and sub.receiveDate != it.receiveDate order by sub.receiveDate DESC) as oldPrice 
                     from DailySummary as it
                     where it.receiveDate = ANYELEMENT(select VALUE TOP(1) sub.receiveDate from DailySummary as sub order by sub.receiveDate DESC)
                     ) as main
                     order by main.tradeAmt DESC"
        EntityTypeFilter="" OrderBy="" Select="" OnContextCreated="EntityDataSource_ContextCreated">
    </asp:EntityDataSource>

    <asp:EntityDataSource ID="EntityDataSource2" runat="server" ConnectionString="name=stockdbaEntities" DefaultContainerName="stockdbaEntities" AutoPage="False"
        CommandText="select it.receiveDate, it.brokerName, it.brokerBranch, sum(it.buyVolume - it.sellVolume) as netVolume, sum(it.buyVolume) as buyVolume, sum(it.sellVolume) as sellVolume, 
                     sum((it.buyVolume + it.sellVolume)*it.avgValue) as tradeAmt
                     from DailySettlement as it
                     where it.receiveDate = ANYELEMENT(select VALUE TOP(1) sub.receiveDate from DailySettlement as sub order by sub.receiveDate DESC)
                     group by it.receiveDate, it.brokerName, it.brokerBranch 
                     order by tradeAmt DESC"
        EnableFlattening="False" OnContextCreated="EntityDataSource_ContextCreated">
    </asp:EntityDataSource>

</asp:Content>
