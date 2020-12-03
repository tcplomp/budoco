﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;
using System;

namespace budoco.Pages
{
    public class FileModel : PageModel
    {
        [FromQuery]
        public int pa_id { get; set; }

        public ActionResult OnGetFile()
        {

            if (!bd_util.check_user_permissions(HttpContext))
            {
                HttpContext.Response.StatusCode = 403;
                return null;
            }

            var sql = @"/*File*/select pa_file_content_type, pa_content
                from post_attachments where pa_id = " + pa_id.ToString();

            DataRow dr = bd_db.get_datarow(sql);

            if (dr == null)
            {
                HttpContext.Response.StatusCode = 410; //"Gone"
                return null;
            }

            // can i do this in one step?
            sql = "/*File*/select pa_content from post_attachments where pa_id = " + pa_id.ToString();
            byte[] bytea = bd_db.get_bytea(sql);

            return File(bytea, (string)dr["pa_file_content_type"]);
        }
    }
}
