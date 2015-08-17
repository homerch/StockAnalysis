<%@ Page Title="-籌碼分析" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataQuery.aspx.cs" Inherits="WebApplication.Analysis.DataQuery" %>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1 itemprop="name">籌碼集中度</h1>
            </hgroup>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <fieldset>
        <legend>查詢條件</legend>
        <ol>
            <li>
                <br />
                <asp:Label ID="Label2" runat="server" Text="證券商"></asp:Label>
                <asp:TextBox ID="tbBrokerName" runat="server" Width="95%">美林</asp:TextBox>
                <asp:TextBox ID="tbBrokerBranch" runat="server" Width="95%">總公司</asp:TextBox>
                <%--<asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="請輸入證券商名稱" ControlToValidate="tbStockId" CssClass="field-validation-error" Operator="DataTypeCheck" Type="Integer"></asp:CompareValidator>--%>
            </li>
            <li>
                <br />
                <asp:Label ID="Label1" runat="server" Text="股票代碼"></asp:Label>
                <asp:TextBox ID="tbStockId" runat="server" Width="95%">2317</asp:TextBox>
                <asp:Button ID="btnQuery" runat="server" Text="執行" OnClick="btnQuery_Click" />
            </li>
        </ol>
    </fieldset>
      <asp:Panel ID="Panel1" runat="server" HorizontalAlign="center">
                <br />
                <div id="up_container">
                    <br />
                    <fieldset>
                        <legend>累積買超排行</legend>
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" Width="100%" OnRowDataBound="GridView1_RowDataBound" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                            <AlternatingRowStyle BackColor="#FFCC99" />
                            <Columns>
                                <asp:BoundField DataField="stockId" HeaderText="股票代碼" />
                                <asp:BoundField DataField="stockName" HeaderText="股名" />
                                <asp:BoundField DataField="tradeAmt" HeaderText="成交金額" SortExpression="tradeAmt" DataFormatString="{0:#,##0.;(#,##0);0}" />
                                <asp:BoundField DataField="netVolume" DataFormatString="{0:#,##0.;(#,##0);0}" HeaderText="股數" />
                                <asp:BoundField DataField="avgValue" DataFormatString="{0:F2}" HeaderText="均價" />
                                <asp:CommandField SelectText="明細" ShowSelectButton="True" />
                                <asp:HyperLinkField DataNavigateUrlFields="stockId" DataNavigateUrlFormatString="~/Analysis/Lab_Branch_Lite.aspx?stockId={0}" Text="個股分析" />
                            </Columns>
                            <RowStyle HorizontalAlign="Right" />
                            <EmptyDataTemplate>
                                查無資料
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <br />
                        <asp:Chart ID="ChartOB1" runat="server" Width="900px" Height="800px" IsSoftShadows="False" Visible="False" EnableViewState="True">
                            <BorderSkin BackColor="Transparent" PageColor="Transparent" SkinStyle="Emboss" />
                            <Legends>
                                <asp:Legend LegendStyle="Row" IsTextAutoFit="False" Docking="Top" IsDockedInsideChartArea="False" Name="Default" BackColor="Transparent" Font="Trebuchet MS, 8.25pt, style=Bold" Alignment="Far">
                                </asp:Legend>
                            </Legends>
                            <Series>
                                <asp:Series YValuesPerPoint="4" ChartArea="Price" XValueType="DateTime" IsVisibleInLegend="False" Name="Price" ChartType="Stock" BorderColor="180, 26, 59, 105" CustomProperties="OpenCloseStyle=Candlestick" IsXValueIndexed="True"></asp:Series>
                                <asp:Series ChartArea="Price" XValueType="DateTime" Name="均價" BorderWidth="3" ChartType="Point" BorderColor="180, 26, 59, 105" IsXValueIndexed="True" MarkerColor="Yellow"></asp:Series>
                                <asp:Series ChartArea="Volume" XValueType="DateTime" Name="買入" Color="Red" IsXValueIndexed="True"></asp:Series>
                                <asp:Series ChartArea="Volume" XValueType="DateTime" Name="賣出" Color="0, 192, 0" IsXValueIndexed="True"></asp:Series>
                                <asp:Series ChartArea="Volume" XValueType="DateTime" Name="合計" IsXValueIndexed="True" BorderWidth="3" ChartType="Line" Color="Blue"></asp:Series>
                            </Series>
                            <ChartAreas>
                                <asp:ChartArea Name="Price" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent" BackGradientStyle="TopBottom">
                                    <Area3DStyle Rotation="10" Perspective="10" Inclination="15" IsRightAngleAxes="False" WallWidth="0" IsClustered="False"></Area3DStyle>
                                    <Position Y="10" Height="42" Width="88" X="3"></Position>
                                    <AxisY LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False">
                                        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                        <MajorGrid LineColor="64, 64, 64, 64" />
                                    </AxisY>
                                    <AxisX LineColor="64, 64, 64, 64" IsLabelAutoFit="False">
                                        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                        <MajorGrid LineColor="64, 64, 64, 64" />
                                    </AxisX>
                                </asp:ChartArea>
                                <asp:ChartArea Name="Volume" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent" AlignWithChartArea="Price" BackGradientStyle="TopBottom">
                                    <Area3DStyle Rotation="10" Perspective="10" Inclination="15" IsRightAngleAxes="False" WallWidth="0" IsClustered="False"></Area3DStyle>
                                    <Position Y="51.84195" Height="42" Width="88" X="3"></Position>
                                    <AxisY LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False">
                                        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                        <MajorGrid LineColor="64, 64, 64, 64" />
                                    </AxisY>
                                    <AxisX LineColor="64, 64, 64, 64" IsLabelAutoFit="False">
                                        <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                        <MajorGrid LineColor="64, 64, 64, 64" />
                                    </AxisX>
                                </asp:ChartArea>
                            </ChartAreas>
                            <Titles>
                                <asp:Title Alignment="TopLeft" DockedToChartArea="Price" Name="Price" IsDockedInsideChartArea="False">
                                </asp:Title>
                                <asp:Title Alignment="BottomRight" DockedToChartArea="Volume" Name="TimeStamp" Text="Time" Docking="Bottom" IsDockedInsideChartArea="False">
                                </asp:Title>
                            </Titles>
                        </asp:Chart>
                    </fieldset>
                </div>
            </asp:Panel>
</asp:Content>
