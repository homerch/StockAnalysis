using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HtmlAgilityPack;
using ServiceLibrary;

namespace WebApplication
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowIndex & 1) == 1)
                {
                    e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.backgroundColor='#FF9999';";
                    e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='#FFCC99';";
                }
                else
                {
                    e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.backgroundColor='#FF9999';";
                    e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='transparent';";
                }
                e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.GridView1, "Select$" + e.Row.RowIndex);

                // -------------------------------------------------------------
                int changeIndex = 0;
                foreach (DataControlField item in GridView1.Columns)
                {
                    if (item.HeaderText.Contains("漲跌幅"))
                    {
                        changeIndex = GridView1.Columns.IndexOf(item);
                        break;
                    }
                }
                if (float.Parse(e.Row.Cells[changeIndex].Text) >= 0)
                {
                    e.Row.Cells[changeIndex].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[changeIndex].Text = "▲" + e.Row.Cells[changeIndex].Text;
                }
                else
                {
                    e.Row.Cells[changeIndex].ForeColor = System.Drawing.Color.Green;
                    e.Row.Cells[changeIndex].Text = "▼" + e.Row.Cells[changeIndex].Text.Substring(1);
                }
            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = GridView1.SelectedRow;
            String stockId = row.Cells[1].Text.Split(' ')[0];
            Response.Redirect(String.Format("~/Analysis/Lab_Branch_Lite.aspx?stockId={0}", stockId));
        }

        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowIndex & 1) == 1)
                {
                    e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.backgroundColor='#99CCFF';";
                    e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='#CCFF99';";
                }
                else
                {
                    e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer';this.style.backgroundColor='#99CCFF';";
                    e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='transparent';";
                }
                e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.GridView2, "Select$" + e.Row.RowIndex);

                // -------------------------------------------------------------
                int netVolumeIndex = 0;
                foreach (DataControlField item in GridView2.Columns)
                {
                    if (item.HeaderText.Contains("買賣超"))
                    {
                        netVolumeIndex = GridView2.Columns.IndexOf(item);
                        break;
                    }
                }
                if (float.Parse(e.Row.Cells[netVolumeIndex].Text) >= 0)
                {
                    e.Row.Cells[netVolumeIndex].ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    e.Row.Cells[netVolumeIndex].ForeColor = System.Drawing.Color.Green;
                }
            }
        }

        protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = GridView2.SelectedRow;
            Response.Redirect(String.Format("~/Analysis/Broker_Branch_Lite.aspx?brokerName={0}&brokerBranch={1}", row.Cells[1].Text, row.Cells[2].Text));
        }

        protected override void Render(HtmlTextWriter writer)
        {
            for (int i = 0; i < this.GridView1.Rows.Count; i++)
            {
                Page.ClientScript.RegisterForEventValidation(this.GridView1.UniqueID, "Select$" + i.ToString());
            }
            for (int i = 0; i < this.GridView2.Rows.Count; i++)
            {
                Page.ClientScript.RegisterForEventValidation(this.GridView2.UniqueID, "Select$" + i.ToString());
            }
            base.Render(writer);
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            foreach (DataControlField item in GridView1.Columns)
            {
                item.HeaderText = item.HeaderText.Replace("▲", "").Replace("▼", "");

                if (item.SortExpression == e.SortExpression)
                {
                    if (e.SortDirection == SortDirection.Ascending)
                    {
                        item.HeaderText = item.HeaderText + "▲";
                    }
                    else
                    {
                        item.HeaderText = item.HeaderText + "▼";
                    }
                }
            }
        }

        protected void GridView2_Sorting(object sender, GridViewSortEventArgs e)
        {
            foreach (DataControlField item in GridView2.Columns)
            {
                item.HeaderText = item.HeaderText.Replace("▲", "").Replace("▼", "");

                if (item.SortExpression == e.SortExpression)
                {
                    if (e.SortDirection == SortDirection.Ascending)
                    {
                        item.HeaderText = item.HeaderText + "▲";
                    }
                    else
                    {
                        item.HeaderText = item.HeaderText + "▼";
                    }
                }
            }
        }

        protected void EntityDataSource_ContextCreated(object sender, EntityDataSourceContextCreatedEventArgs e)
        {
            e.Context.Connection.Open();
            e.Context.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        }
    }
}