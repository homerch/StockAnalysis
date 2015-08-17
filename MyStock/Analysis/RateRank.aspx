<%@ Page Title="集中度" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RateRank.aspx.cs" Inherits="WebApplication.Analysis.Rate" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1 itemprop="name">籌碼集中度</h1>
            </hgroup>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>
                <legend>近日籌碼集中度排行</legend>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="receiveDate,stockId" DataSourceID="EntityDataSource1" AllowPaging="True" Width="100%" OnRowDataBound="GridView1_RowDataBound" OnSelectedIndexChanged="GridView_SelectedIndexChanged">
                    <AlternatingRowStyle BackColor="#FFCC99" />
                    <Columns>
                        <asp:BoundField DataField="receiveDate" HeaderText="交易日期" ReadOnly="True" SortExpression="receiveDate" DataFormatString="{0:yyyy/MM/dd}" />
                        <asp:BoundField DataField="stockId" HeaderText="股號" ReadOnly="True" SortExpression="stockId" />
                        <asp:BoundField DataField="rate" HeaderText="集中度" SortExpression="rate" />
                        <asp:BoundField DataField="totalVolume" HeaderText="近日成交量" SortExpression="totalVolume" DataFormatString="{0:#,##0.;(#,##0);0}" />
                        <asp:BoundField DataField="avgVolume" HeaderText="平均日成交量" DataFormatString="{0:#,##0.;(#,##0);0}" />
                    </Columns>
                    <RowStyle HorizontalAlign="Right" />
                    <EmptyDataTemplate>
                        查無資料
                    </EmptyDataTemplate>
                </asp:GridView>
                <br />
                <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False" DataKeyNames="receiveDate,stockId" DataSourceID="EntityDataSource3" AllowPaging="True" Width="100%" OnRowDataBound="GridView2_RowDataBound" OnSelectedIndexChanged="GridView_SelectedIndexChanged">
                    <AlternatingRowStyle BackColor="#CCFF99" />
                    <Columns>
                        <asp:BoundField DataField="receiveDate" HeaderText="交易日期" ReadOnly="True" SortExpression="receiveDate" DataFormatString="{0:yyyy/MM/dd}" />
                        <asp:BoundField DataField="stockId" HeaderText="股號" ReadOnly="True" SortExpression="stockId" />
                        <asp:BoundField DataField="rate" HeaderText="集中度" SortExpression="rate" />
                        <asp:BoundField DataField="totalVolume" HeaderText="近日成交量" SortExpression="totalVolume" DataFormatString="{0:#,##0.;(#,##0);0}" />
                        <asp:BoundField DataField="avgVolume" HeaderText="平均日成交量" DataFormatString="{0:#,##0.;(#,##0);0}" />
                    </Columns>
                    <RowStyle HorizontalAlign="Right" />
                    <EmptyDataTemplate>
                        查無資料
                    </EmptyDataTemplate>
                </asp:GridView>
            </fieldset>
            <br />
            <fieldset>
                <legend>近週籌碼集中度排行</legend>
                <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" DataKeyNames="receiveDate,stockId" DataSourceID="EntityDataSource2" AllowPaging="True" Width="100%" OnRowDataBound="GridView1_RowDataBound" OnSelectedIndexChanged="GridView_SelectedIndexChanged">
                    <AlternatingRowStyle BackColor="#FFCC99" />
                    <Columns>
                        <asp:BoundField DataField="receiveDate" HeaderText="交易日期" ReadOnly="True" SortExpression="receiveDate" DataFormatString="{0:yyyy/MM/dd}" />
                        <asp:BoundField DataField="stockId" HeaderText="股號" ReadOnly="True" SortExpression="stockId" />
                        <asp:BoundField DataField="rate" HeaderText="集中度" SortExpression="rate" />
                        <asp:BoundField DataField="totalVolume" HeaderText="近週成交量" SortExpression="totalVolume" DataFormatString="{0:#,##0.;(#,##0);0}" />
                        <asp:BoundField DataField="avgVolume" HeaderText="平均週成交量" DataFormatString="{0:#,##0.;(#,##0);0}" />
                    </Columns>
                    <RowStyle HorizontalAlign="Right" />
                    <EmptyDataTemplate>
                        查無資料
                    </EmptyDataTemplate>
                </asp:GridView>
                <br />
                <asp:GridView ID="GridView4" runat="server" AutoGenerateColumns="False" DataKeyNames="receiveDate,stockId" DataSourceID="EntityDataSource4" AllowPaging="True" Width="100%" OnRowDataBound="GridView2_RowDataBound" OnSelectedIndexChanged="GridView_SelectedIndexChanged">
                    <AlternatingRowStyle BackColor="#CCFF99" />
                    <Columns>
                        <asp:BoundField DataField="receiveDate" HeaderText="交易日期" ReadOnly="True" SortExpression="receiveDate" DataFormatString="{0:yyyy/MM/dd}" />
                        <asp:BoundField DataField="stockId" HeaderText="股號" ReadOnly="True" SortExpression="stockId" />
                        <asp:BoundField DataField="rate" HeaderText="集中度" SortExpression="rate" />
                        <asp:BoundField DataField="totalVolume" HeaderText="近週成交量" SortExpression="totalVolume" DataFormatString="{0:#,##0.;(#,##0);0}" />
                        <asp:BoundField DataField="avgVolume" HeaderText="平均週成交量" DataFormatString="{0:#,##0.;(#,##0);0}" />
                    </Columns>
                    <RowStyle HorizontalAlign="Right" />
                    <EmptyDataTemplate>
                        查無資料
                    </EmptyDataTemplate>
                </asp:GridView>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>

        <asp:EntityDataSource ID="EntityDataSource1" runat="server" ConnectionString="name=stockdbaEntities" DefaultContainerName="stockdbaEntities" EnableFlattening="False" AutoPage="False"
        CommandText="select top(50) it.receiveDate, (it.stockId + ' ' + it.stockName) as stockId, it.rate, it.totalVolume, 
                     ANYELEMENT(
                     select VALUE sum(sub.totalVolume) from DailyRate as sub 
                     where sub.stockId = it.stockId
                     )/ANYELEMENT(
                     select VALUE count(distinct sub.receiveDate) from DailyRate as sub 
                     where sub.stockId = it.stockId
                     ) 
                     as avgVolume
                     from DailyRate as it
                     where it.receiveDate = ANYELEMENT(select VALUE TOP(1) sub.receiveDate from DailyRate as sub order by sub.receiveDate DESC) and it.rate > 0 order by it.rate desc"
        EntityTypeFilter="" OrderBy="" Select="" OnContextCreated="EntityDataSource_ContextCreated">
    </asp:EntityDataSource>
    <asp:EntityDataSource ID="EntityDataSource2" runat="server" ConnectionString="name=stockdbaEntities" DefaultContainerName="stockdbaEntities" EnableFlattening="False" AutoPage="False"
        CommandText="select top(50) it.receiveDate, (it.stockId + ' ' + it.stockName) as stockId, it.rate, it.totalVolume, 
                     ANYELEMENT(
                     select VALUE sum(sub.totalVolume) from WeeklyRate as sub 
                     where sub.stockId = it.stockId
                     )/ANYELEMENT(
                     select VALUE count(distinct sub.receiveDate) from WeeklyRate as sub 
                     where sub.stockId = it.stockId
                     ) 
                     as avgVolume
                     from WeeklyRate as it
                     where it.receiveDate = ANYELEMENT(select VALUE TOP(1) sub.receiveDate from WeeklyRate as sub order by sub.receiveDate DESC) and it.rate > 0 order by rate desc"
        EntityTypeFilter="" OrderBy="" Select="" OnContextCreated="EntityDataSource_ContextCreated">
    </asp:EntityDataSource>
    <asp:EntityDataSource ID="EntityDataSource3" runat="server" ConnectionString="name=stockdbaEntities" DefaultContainerName="stockdbaEntities" EnableFlattening="False" AutoPage="False"
        CommandText="select top(50) it.receiveDate, (it.stockId + ' ' + it.stockName) as stockId, it.rate, it.totalVolume, 
                     ANYELEMENT(
                     select VALUE sum(sub.totalVolume) from DailyRate as sub 
                     where sub.stockId = it.stockId
                     )/ANYELEMENT(
                     select VALUE count(distinct sub.receiveDate) from DailyRate as sub 
                     where sub.stockId = it.stockId
                     ) 
                     as avgVolume
                     from DailyRate as it
                     where it.receiveDate = ANYELEMENT(select VALUE TOP(1) sub.receiveDate from DailyRate as sub order by sub.receiveDate DESC) and it.rate < 0 order by rate"
        EntityTypeFilter="" OrderBy="" Select="" OnContextCreated="EntityDataSource_ContextCreated">
    </asp:EntityDataSource>
    <asp:EntityDataSource ID="EntityDataSource4" runat="server" ConnectionString="name=stockdbaEntities" DefaultContainerName="stockdbaEntities" EnableFlattening="False" AutoPage="False"
        CommandText="select top(50) it.receiveDate, (it.stockId + ' ' + it.stockName) as stockId, it.rate, it.totalVolume, 
                     ANYELEMENT(
                     select VALUE sum(sub.totalVolume) from WeeklyRate as sub 
                     where sub.stockId = it.stockId
                     )/ANYELEMENT(
                     select VALUE count(distinct sub.receiveDate) from WeeklyRate as sub 
                     where sub.stockId = it.stockId
                     ) 
                     as avgVolume
                     from WeeklyRate as it
                     where it.receiveDate = ANYELEMENT(select VALUE TOP(1) sub.receiveDate from WeeklyRate as sub order by sub.receiveDate DESC) and it.rate < 0 order by rate"
        EntityTypeFilter="" OrderBy="" Select="" OnContextCreated="EntityDataSource_ContextCreated">
    </asp:EntityDataSource>
</asp:Content>
