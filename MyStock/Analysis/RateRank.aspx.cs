using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Threading.Tasks;
using ServiceLibrary;
using System.Data.Objects;
using System.Data.Entity;
using System.Collections.Concurrent;

namespace WebApplication.Analysis
{
    public partial class Rate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //EntityDataSource1.WhereParameters["rDate"].DefaultValue = DateTime.Now.ToString("yyyy/MM/dd");
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

                e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(((GridView)sender), "Select$" + e.Row.RowIndex);
                //
                if (float.Parse(e.Row.Cells[3].Text) >= float.Parse(e.Row.Cells[4].Text))
                {
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.Green;
                }
            }
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

                e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(((GridView)sender), "Select$" + e.Row.RowIndex);
                //
                if (float.Parse(e.Row.Cells[3].Text) >= float.Parse(e.Row.Cells[4].Text))
                {
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    e.Row.Cells[3].ForeColor = System.Drawing.Color.Green;
                }
            }
        }

        protected void GridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = ((GridView)sender).SelectedRow;

            String stockId = row.Cells[1].Text.Split(' ')[0];
            Response.Redirect(String.Format("~/Analysis/Lab_Branch_Lite.aspx?stockId={0}", stockId));
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

            for (int i = 0; i < this.GridView3.Rows.Count; i++)
            {
                Page.ClientScript.RegisterForEventValidation(this.GridView3.UniqueID, "Select$" + i.ToString());
            }

            for (int i = 0; i < this.GridView4.Rows.Count; i++)
            {
                Page.ClientScript.RegisterForEventValidation(this.GridView4.UniqueID, "Select$" + i.ToString());
            }
            base.Render(writer);
        }

        protected void EntityDataSource_ContextCreated(object sender, EntityDataSourceContextCreatedEventArgs e)
        {
            e.Context.Connection.Open();
            e.Context.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        }
    }
}