<%@ Page Title="個股-籌碼分析" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lab_Branch_Lite.aspx.cs" Inherits="WebApplication.Analysis.Lab_Branch_Lite" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1 itemprop="name">個股-籌碼分析</h1>
            </hgroup>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <fieldset>
        <legend>查詢條件</legend>
        <ol>
            <li>
                <br />
                <asp:Label ID="Label1" runat="server" Text="股票代碼"></asp:Label>
                <asp:TextBox ID="tbStockId" runat="server" Width="95%">2317</asp:TextBox>
                <asp:Button ID="btnQuery" runat="server" Text="執行" OnClick="btnQuery_Click" />
                <%--<asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="請輸入股票代碼" ControlToValidate="tbStockId" CssClass="field-validation-error" Operator="DataTypeCheck" Type="Integer"></asp:CompareValidator>--%>
            </li>
        </ol>
    </fieldset>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" HorizontalAlign="center">
                <br />
                <fieldset>
                    <legend>近日走勢圖</legend>
                    <asp:Chart ID="MainChart" runat="server" Width="900px" Height="800px" IsSoftShadows="False" EnableViewState="True" OnClick="MainChart_Click">
                        <BorderSkin BackColor="Transparent" PageColor="Transparent" SkinStyle="Emboss" />
                        <Legends>
                            <asp:Legend LegendStyle="Row" IsTextAutoFit="False" DockedToChartArea="Rate" Docking="Top" IsDockedInsideChartArea="False" Name="Default" BackColor="Transparent" Font="Trebuchet MS, 8.25pt, style=Bold" Alignment="Far">
                            </asp:Legend>
                        </Legends>
                        <Series>
                            <asp:Series YValuesPerPoint="4" ChartArea="Price" XValueType="DateTime" IsVisibleInLegend="False" Name="Price" ChartType="Stock" BorderColor="180, 26, 59, 105" CustomProperties="OpenCloseStyle=Candlestick" IsXValueIndexed="True"></asp:Series>
                            <asp:Series ChartArea="Volume" XValueType="DateTime" IsVisibleInLegend="False" Name="Volume" Color="224, 64, 10" BorderColor="180, 26, 59, 105" IsXValueIndexed="True"></asp:Series>
                            <asp:Series ChartArea="Rate" Name="日集中度" IsXValueIndexed="True" IsVisibleInLegend="true" XValueType="DateTime" Color="Fuchsia" ChartType="Line"></asp:Series>
                            <asp:Series ChartArea="Rate" Name="週集中度" IsXValueIndexed="True" IsVisibleInLegend="true" XValueType="DateTime" Color="Blue" ChartType="Line"></asp:Series>
                            <asp:Series ChartArea="Rate" Name="總集中度" IsXValueIndexed="True" IsVisibleInLegend="true" XValueType="DateTime" Color="Gold" BorderWidth="3" ChartType="Line" YAxisType="Secondary"></asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="Price" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent" BackGradientStyle="TopBottom">
                                <Area3DStyle Rotation="10" Perspective="10" Inclination="15" IsRightAngleAxes="False" WallWidth="0" IsClustered="False"></Area3DStyle>
                                <AxisY LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisY>
                                <AxisX LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsMarksNextToAxis="False">
                                    <MinorGrid Enabled="True" />
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisX>
                            </asp:ChartArea>
                            <asp:ChartArea Name="Rate" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent" AlignWithChartArea="Price" BackGradientStyle="TopBottom">
                                <AxisY2 LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisY2>
                                <Area3DStyle Rotation="10" Perspective="10" Inclination="15" IsRightAngleAxes="False" WallWidth="0" IsClustered="False"></Area3DStyle>
                                <AxisY LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisY>
                                <AxisX LineColor="64, 64, 64, 64" IsLabelAutoFit="False">
                                    <MinorGrid Enabled="True" />
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisX>
                            </asp:ChartArea>
                            <asp:ChartArea Name="Volume" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent" AlignWithChartArea="Price" BackGradientStyle="TopBottom">
                                <Area3DStyle Rotation="10" Perspective="10" Inclination="15" IsRightAngleAxes="False" WallWidth="0" IsClustered="False"></Area3DStyle>
                                <AxisY LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisY>
                                <AxisX LineColor="64, 64, 64, 64" IsLabelAutoFit="False">
                                    <MinorGrid Enabled="True" />
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisX>
                            </asp:ChartArea>
                        </ChartAreas>
                        <Titles>
                            <asp:Title Alignment="TopLeft" DockedToChartArea="Price" Name="Price" IsDockedInsideChartArea="False">
                            </asp:Title>
                            <asp:Title Alignment="BottomRight" DockedToChartArea="Rate" Name="TimeStamp" Text="Time" Docking="Bottom" IsDockedInsideChartArea="False">
                            </asp:Title>
                        </Titles>
                    </asp:Chart>
                    <asp:Chart ID="ChartBubbleDay" runat="server" Width="900px" Height="300px" IsSoftShadows="False" Visible="False" EnableViewState="True" OnClick="ChartBubble_Click">
                        <BorderSkin BackColor="Transparent" PageColor="Transparent" SkinStyle="Emboss" />
                        <Legends>
                            <asp:Legend LegendStyle="Row" IsTextAutoFit="False" Docking="Top" IsDockedInsideChartArea="False" Name="Default" BackColor="Transparent" Font="Trebuchet MS, 8.25pt, style=Bold" Alignment="Far">
                            </asp:Legend>
                        </Legends>
                        <Series>
                            <asp:Series ChartArea="Price" Name="日買超" ChartType="Bubble" YValuesPerPoint="2" MarkerBorderColor="Black" MarkerColor="Red" MarkerStyle="Circle"></asp:Series>
                            <asp:Series ChartArea="Price" Name="日賣超" ChartType="Bubble" YValuesPerPoint="2" MarkerBorderColor="Black" MarkerColor="Green" MarkerImageTransparentColor="White" MarkerStyle="Circle"></asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="Price" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent" BackGradientStyle="TopBottom">
                                <Area3DStyle Rotation="10" Perspective="10" Inclination="15" IsRightAngleAxes="False" WallWidth="0" IsClustered="False"></Area3DStyle>
                                <AxisY LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False" Title="均價">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisY>
                                <AxisX LineColor="64, 64, 64, 64" IsLabelAutoFit="False" Title="股數">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisX>
                            </asp:ChartArea>
                        </ChartAreas>
                        <Titles>
                            <asp:Title Alignment="BottomRight" DockedToChartArea="Price" Name="日TimeStamp" Text="Time" Docking="Bottom" IsDockedInsideChartArea="False">
                            </asp:Title>
                        </Titles>
                    </asp:Chart>
                    <br />
                </fieldset>
                <br />
                <fieldset>
                    <legend>買賣超</legend>
                    <asp:Chart ID="ChartFocus" runat="server" Width="900px" Height="800px" IsSoftShadows="False" Visible="False" EnableViewState="True">
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
                                    <MinorGrid Enabled="True" />
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
                                    <MinorGrid Enabled="True" />
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisX>
                            </asp:ChartArea>
                        </ChartAreas>
                        <Titles>
                            <asp:Title Alignment="TopLeft" DockedToChartArea="Price" Name="Price" IsDockedInsideChartArea="False" Text="Title">
                            </asp:Title>
                        </Titles>
                    </asp:Chart>
                    <asp:Chart ID="ChartBubble" runat="server" Width="900px" Height="1200px" IsSoftShadows="False" Visible="False" EnableViewState="True" OnClick="ChartBubble_Click">
                        <BorderSkin BackColor="Transparent" PageColor="Transparent" SkinStyle="Emboss" />
                        <Legends>
                            <asp:Legend LegendStyle="Row" IsTextAutoFit="False" Docking="Top" IsDockedInsideChartArea="False" Name="Default" BackColor="Transparent" Font="Trebuchet MS, 8.25pt, style=Bold" Alignment="Far">
                            </asp:Legend>
                        </Legends>
                        <Series>
                            <asp:Series ChartArea="Day" Name="日買超" ChartType="Bubble" YValuesPerPoint="2" MarkerBorderColor="Black" MarkerColor="Red" MarkerStyle="Circle" MarkerImageTransparentColor="Transparent"></asp:Series>
                            <asp:Series ChartArea="Day" Name="日賣超" ChartType="Bubble" YValuesPerPoint="2" MarkerBorderColor="Black" MarkerColor="Green" MarkerImageTransparentColor="Transparent" MarkerStyle="Circle"></asp:Series>
                            <asp:Series ChartArea="Week" Name="週買超" ChartType="Bubble" YValuesPerPoint="2" MarkerBorderColor="Black" MarkerColor="OrangeRed" MarkerStyle="Circle" MarkerImageTransparentColor="Transparent"></asp:Series>
                            <asp:Series ChartArea="Week" Name="週賣超" ChartType="Bubble" YValuesPerPoint="2" MarkerBorderColor="Black" MarkerColor="YellowGreen" MarkerImageTransparentColor="Transparent" MarkerStyle="Circle"></asp:Series>
                            <asp:Series ChartArea="Price" Name="總買超" ChartType="Bubble" YValuesPerPoint="2" MarkerBorderColor="Black" MarkerColor="LightPink" MarkerStyle="Circle" MarkerImageTransparentColor="Transparent"></asp:Series>
                            <asp:Series ChartArea="Price" Name="總賣超" ChartType="Bubble" YValuesPerPoint="2" MarkerBorderColor="Black" MarkerColor="LightGreen" MarkerImageTransparentColor="Transparent" MarkerStyle="Circle"></asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="Day" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent" BackGradientStyle="TopBottom">
                                <Area3DStyle Rotation="10" Perspective="10" Inclination="15" IsRightAngleAxes="False" WallWidth="0" IsClustered="False"></Area3DStyle>
                                <AxisY LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False" Title="均價">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisY>
                                <AxisX LineColor="64, 64, 64, 64" IsLabelAutoFit="False" Title="股數">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisX>
                            </asp:ChartArea>
                        </ChartAreas>
                        <ChartAreas>
                            <asp:ChartArea Name="Week" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent" BackGradientStyle="TopBottom">
                                <Area3DStyle Rotation="10" Perspective="10" Inclination="15" IsRightAngleAxes="False" WallWidth="0" IsClustered="False"></Area3DStyle>
                                <AxisY LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False" Title="均價">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisY>
                                <AxisX LineColor="64, 64, 64, 64" IsLabelAutoFit="False" Title="股數">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisX>
                            </asp:ChartArea>
                        </ChartAreas>
                        <ChartAreas>
                            <asp:ChartArea Name="Price" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent" BackGradientStyle="TopBottom">
                                <Area3DStyle Rotation="10" Perspective="10" Inclination="15" IsRightAngleAxes="False" WallWidth="0" IsClustered="False"></Area3DStyle>
                                <AxisY LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False" Title="均價">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisY>
                                <AxisX LineColor="64, 64, 64, 64" IsLabelAutoFit="False" Title="股數">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisX>
                            </asp:ChartArea>
                        </ChartAreas>
                        <Titles>
                            <asp:Title Alignment="TopLeft" DockedToChartArea="Day" Name="Price" IsDockedInsideChartArea="False" Text="Title">
                            </asp:Title>
                            <asp:Title Alignment="BottomRight" DockedToChartArea="Day" Name="日TimeStamp" Text="Time" Docking="Bottom" IsDockedInsideChartArea="False">
                            </asp:Title>
                            <asp:Title Alignment="BottomRight" DockedToChartArea="Week" Name="週TimeStamp" Text="Time" Docking="Bottom" IsDockedInsideChartArea="False">
                            </asp:Title>
                            <asp:Title Alignment="BottomRight" DockedToChartArea="Price" Name="TimeStamp" Text="Time" Docking="Bottom" IsDockedInsideChartArea="False">
                            </asp:Title>
                        </Titles>
                    </asp:Chart>
                </fieldset>
                <%--                <br />
                <fieldset>
                    <legend>分價圖</legend>
                    <asp:Chart ID="ChartPV" runat="server" Width="900px" Height="800px" IsSoftShadows="False" EnableViewState="True">
                        <BorderSkin BackColor="Transparent" PageColor="Transparent" SkinStyle="Emboss" />
                        <Legends>
                            <asp:Legend LegendStyle="Row" IsTextAutoFit="False" Docking="Top" IsDockedInsideChartArea="False" Name="Default" BackColor="Transparent" Font="Trebuchet MS, 8.25pt, style=Bold" Alignment="Far">
                            </asp:Legend>
                        </Legends>
                        <Series>
                            <asp:Series ChartArea="Price" ChartType="Bar" Legend="Default" Name="價量" IsVisibleInLegend ="false">
                            </asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="Price" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent" BackGradientStyle="TopBottom">
                                <Area3DStyle Rotation="10" Perspective="10" Inclination="15" IsRightAngleAxes="False" WallWidth="0" IsClustered="False"></Area3DStyle>
                                <AxisY LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisY>
                                <AxisX LineColor="64, 64, 64, 64" IsLabelAutoFit="False">
                                    <MinorGrid Enabled="True" />
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisX>
                            </asp:ChartArea>
                        </ChartAreas>
                        <Titles>
                           <asp:Title Alignment="TopLeft" DockedToChartArea="Price" Name="Price" IsDockedInsideChartArea="False">
                            </asp:Title>
                            <asp:Title Alignment="BottomRight" DockedToChartArea="Price" Name="TimeStamp" Text="Time" Docking="Bottom" IsDockedInsideChartArea="False">
                            </asp:Title>
                        </Titles>
                    </asp:Chart>
                </fieldset>--%>
                <br />
                <fieldset>
                    <legend>近期買超累積排行</legend>
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" Width="100%" OnRowDataBound="GridView1_RowDataBound" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                        <AlternatingRowStyle BackColor="#FFCC99" />
                        <Columns>
                            <asp:BoundField DataField="brokerName" HeaderText="證券商" />
                            <asp:BoundField DataField="brokerBranch" HeaderText="分公司" />
                            <asp:BoundField DataField="netVolume" DataFormatString="{0:#,##0.;(#,##0);0}" HeaderText="股數" />
                            <asp:BoundField DataField="avgValue" DataFormatString="{0:F2}" HeaderText="均價" />
                            <asp:BoundField DataField="latestVolume" DataFormatString="{0:#,##0.;-#,##0;0}" HeaderText="近日買賣超" />
                            <asp:CommandField SelectText="明細" ShowSelectButton="True" />
                            <asp:HyperLinkField DataNavigateUrlFields="brokerName,brokerBranch" DataNavigateUrlFormatString="~/Analysis/Broker_Branch_Lite.aspx?brokerName={0}&brokerBranch={1}" Text="證券商分析" />
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
                                    <MinorGrid Enabled="True" />
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
                                    <MinorGrid Enabled="True" />
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisX>
                            </asp:ChartArea>
                        </ChartAreas>
                        <Titles>
                            <asp:Title Alignment="TopLeft" DockedToChartArea="Price" Name="Price" IsDockedInsideChartArea="False" Text="Title">
                            </asp:Title>
                        </Titles>
                    </asp:Chart>
                </fieldset>
                <br />
                <fieldset>
                    <legend>近期賣超累積排行</legend>
                    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" Width="100%" OnRowDataBound="GridView2_RowDataBound" OnSelectedIndexChanged="GridView2_SelectedIndexChanged">
                        <AlternatingRowStyle BackColor="#CCFF99" />
                        <Columns>
                            <asp:BoundField DataField="brokerName" HeaderText="證券商" />
                            <asp:BoundField DataField="brokerBranch" HeaderText="分公司" />
                            <asp:BoundField DataField="netVolume" DataFormatString="{0:#,##0.;(#,##0);0}" HeaderText="股數" />
                            <asp:BoundField DataField="avgValue" DataFormatString="{0:F2}" HeaderText="均價" />
                            <asp:BoundField DataField="latestVolume" DataFormatString="{0:#,##0.;-#,##0;0}" HeaderText="近日買賣超" />
                            <asp:CommandField SelectText="明細" ShowSelectButton="True" />
                            <asp:HyperLinkField DataNavigateUrlFields="brokerName,brokerBranch" DataNavigateUrlFormatString="~/Analysis/Broker_Branch_Lite.aspx?brokerName={0}&brokerBranch={1}" Text="證券商分析" />
                        </Columns>
                        <RowStyle HorizontalAlign="Right" />
                        <EmptyDataTemplate>
                            查無資料
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <br />
                    <asp:Chart ID="ChartOS1" runat="server" Width="900px" Height="800px" IsSoftShadows="False" Visible="False" EnableViewState="True">
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
                                <AxisX2>
                                    <MinorGrid Enabled="True" />
                                </AxisX2>
                                <Position Y="10" Height="42" Width="88" X="3"></Position>
                                <AxisY LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisY>
                                <AxisX LineColor="64, 64, 64, 64" IsLabelAutoFit="False">
                                    <MinorGrid Enabled="True" />
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisX>
                            </asp:ChartArea>
                            <asp:ChartArea Name="Volume" BorderColor="64, 64, 64, 64" BorderDashStyle="Solid" BackSecondaryColor="White" BackColor="64, 165, 191, 228" ShadowColor="Transparent" AlignWithChartArea="Price" BackGradientStyle="TopBottom">
                                <Area3DStyle Rotation="10" Perspective="10" Inclination="15" IsRightAngleAxes="False" WallWidth="0" IsClustered="False"></Area3DStyle>
                                <AxisX2>
                                    <MinorGrid Enabled="True" />
                                </AxisX2>
                                <Position Y="51.84195" Height="42" Width="88" X="3"></Position>
                                <AxisY LineColor="64, 64, 64, 64" IsLabelAutoFit="False" IsStartedFromZero="False">
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisY>
                                <AxisX LineColor="64, 64, 64, 64" IsLabelAutoFit="False">
                                    <MinorGrid Enabled="True" />
                                    <LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" IsEndLabelVisible="False" />
                                    <MajorGrid LineColor="64, 64, 64, 64" />
                                </AxisX>
                            </asp:ChartArea>
                        </ChartAreas>
                        <Titles>
                            <asp:Title Alignment="TopLeft" DockedToChartArea="Price" Name="Price" IsDockedInsideChartArea="False">
                            </asp:Title>
                        </Titles>
                    </asp:Chart>
                </fieldset>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnQuery" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <ajaxToolkit:UpdatePanelAnimationExtender ID="UpdatePanel1_UpdatePanelAnimationExtender" BehaviorID="animation" runat="server" Enabled="True" TargetControlID="UpdatePanel1"
        AlwaysFinishOnUpdatingAnimation="true">
        <Animations>
                <OnUpdating>
                    <Sequence>                                               
                        <%-- Disable all the controls --%>
                        <Parallel duration="0">
                            <EnableAction AnimationTarget="btnQuery" Enabled="false" />
                        </Parallel>
                        
                        <%-- Do each of the selected effects --%>
                        <Parallel duration=".25" Fps="30">
                             <FadeOut AnimationTarget="UpdatePanel1" minimumOpacity=".2" />
                        </Parallel>
                    </Sequence>
                </OnUpdating>
                <OnUpdated>
                    <Sequence>
                        <%-- Do each of the selected effects --%>
                        <Parallel duration=".25" Fps="30">
                            <FadeIn AnimationTarget="UpdatePanel1" minimumOpacity=".2" />
                        </Parallel>
                        
                        <%-- Enable all the controls --%>
                        <Parallel duration="0">
                            <EnableAction AnimationTarget="btnQuery" Enabled="true" />
                        </Parallel>                            
                    </Sequence>
                </OnUpdated>
        </Animations>
    </ajaxToolkit:UpdatePanelAnimationExtender>

</asp:Content>
