using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data;
using System.Net;

namespace budoco.Pages.Report
{
    public sealed class EditModel : PageModel
    {
        [FromQuery]
        public int id { get; set; }

        [BindProperty]
        public string name { get; set; }

        [BindProperty]
        public string chartType { get; set; }

        [BindProperty]
        public string sqlText { get; set; }

        public void OnGet()
        {
            if (!bd_util.check_user_permissions(HttpContext, bd_util.MUST_BE_ADMIN) /*|| security.user.can_edit_reports*/)
            {
                return;
            }

            // add or edit?
            if (id == 0)
            {
                //sql_text.Value = Request.Form["sql_text"]; // if coming from search.aspx
                chartType = "table";
            }
            else
            {
                // Get this entry's data from the db and fill in the form
                var sql = @"select
                    rp_desc, rp_sql, rp_chart_type
                    from reports where rp_id=@id";

                var dict = new Dictionary<string, object>
                {
                    ["@id"] = id
                };

                DataRow dr = bd_db.get_datarow(sql, dict);

                // Fill in this form
                name = (string)dr["rp_desc"];
                chartType = (string)dr["rp_chart_type"];

                //if (Util.get_setting("HtmlEncodeSql","0") == "1")
                //{
                //  sql_text.Value = Server.HtmlEncode((string) dr["rp_sql"]);
                //}
                //else
                //{
                sqlText = (string)dr["rp_sql"];
                //}
            }
        }

        public void OnPost()
        {
            if (!bd_util.check_user_permissions(HttpContext, bd_util.MUST_BE_ADMIN) /*|| security.user.can_edit_reports*/)
            {
                return;
            }

            if (!IsValid())
            {
                return;
            }

            if (id == 0)
            {
                Create();

                bd_util.set_flash_msg(HttpContext, bd_util.CREATE_WAS_SUCCESSFUL);
            }
            else
            {
                Update();

                bd_util.set_flash_msg(HttpContext, bd_util.UPDATE_WAS_SUCCESSFUL);
            }

            Response.Redirect("/Report/Reports");
        }

        private bool IsValid()
        {
            var errs = new List<string>();

            if (string.IsNullOrEmpty(name))
            {
                errs.Add("Description is required.");
            }

            if (string.IsNullOrEmpty(sqlText))
            {
                errs.Add("The SQL statement is required.");
            }

            if (errs.Count == 0)
            {
                return true;
            }

            if (id == 0)
            {
                errs.Insert(0, "Report was not created.");
            }
            else
            {
                errs.Insert(0, "Report was not updated.");
            }

            bd_util.set_flash_errs(HttpContext, errs);

            return false;
        }

        private void Create()
        {
            var sql = @"insert into reports
                (rp_desc, rp_sql, rp_chart_type)
                values (@name, @sql, @chart_type)";

            var dict = new Dictionary<string, object>
            {
                ["@name"] = name,
                ["@chart_type"] = chartType,
                ["@sql"] = WebUtility.HtmlDecode(sqlText)
            };

            bd_db.exec(sql, dict);
        }

        private void Update()
        {
            var sql = @"
                update reports set
                    rp_desc = @name,
                    rp_sql = @sql,
                    rp_chart_type = @chart_type
                where
                    rp_id = @id";

            var dict = new Dictionary<string, object>
            {
                ["@id"] = id,
                ["@name"] = name,
                ["@chart_type"] = chartType,
                ["@sql"] = WebUtility.HtmlDecode(sqlText)
            };

            bd_db.exec(sql, dict);
        }
    }
}
