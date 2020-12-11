using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace budoco
{
    /*

    
    Budoco doesn't use a standard ASPNET appsettings.json file.
    Instead, it uses a linux style config file.

    The following is a rant about why I'm not using the appsettings.json file.
 
    * The MS JSON parser doesn't allow file comments. Users need comments
    to understand the settings, and it can be useful to comment out a line
    and try a different line.

    * The code generated by "dotnet" new has the config file being loaded 
    in Startup.cs, but Serilog wants to start logging in Program.cs. I want
    to configure the log file location early, in Program.cs. Maybe there's
    a way to move things around, but it's not obvious.

    * The syntax here is so easy. With json file, much easier to miss
    a quote, brace, comma.

    In my lifetime Microsoft has had four different config styles.
        1) .ini, like in windows 3.1 Pretty much like this.
        2) The registry. That was scary to edit, unfriendly to backup,
        no comments, no way to version control it.
        3) XML files. They were okay. Verbose and ugly, but they worked.
        4) JSON. 
    In that same time period unix/linux has had pretty much one format 
    and it has worked fine.

    */


    public static class bd_config
    {

        public const string DbServer = "DbServer";
        public const string DbDatabase = "DbDatabase";
        public const string DbUser = "DbUser";
        public const string DbPassword = "DbPassword";
        public const string DebugSkipSendingEmails = "DebugSkipSendingEmails";
        public const string SmtpHost = "SmtpHost";
        public const string SmtpPort = "SmtpPort";
        public const string SmtpUser = "SmtpUser";
        public const string SmtpPassword = "SmtpPassword";
        public const string ImapHost = "ImapHost";
        public const string ImapPort = "ImapPort";
        public const string ImapUser = "ImapUser";
        public const string ImapPassword = "ImapPassword";
        public const string OutgoingEmailDisplayName = "OutgoingEmailDisplayName";
        public const string UseDeveloperExceptionPage = "UseDeveloperExceptionPage";
        public const string WebsiteUrlRootWithoutSlash = "WebsiteUrlRootWithoutSlash";
        public const string AppName = "AppName";
        public const string UseCustomCss = "UseCustomCss";
        public const string CustomCssFilename = "CustomCssFilename";
        public const string RowsPerPage = "RowsPerPage";
        public const string LogFileFolder = "LogFileFolder";
        public const string DateFormat = "DateFormat";
        public const string NewUserStartsInactive = "NewUserStartsInactive";
        public const string NewUserStartsReportOnly = "NewUserStartsReportOnly";
        public const string DebugAutoConfirmRegistration = "DebugAutoConfirmRegistration";
        public const string DebugEnableRunSql = "DebugEnableRunSql";
        public const string MaxNumberOfSendingRetries = "MaxNumberOfSendingRetries";
        public const string EnableIncomingEmail = "EnableIncomingEmail";
        public const string SecondsToSleepAfterCheckingIncomingEmail = "SecondsToSleepAfterCheckingIncomingEmail";
        public const string DebugSkipDeleteOfIncomingEmails = "DebugSkipDeleteOfIncomingEmails";
        public const string DebugPathToEmailFile = "DebugPathToEmailFile";
        public const string CheckForDangerousSqlKeywords = "CheckForDangerousSqlKeywords";
        public const string DangerousSqlKeywords = "DangerousSqlKeywords";
        public const string DebugFolderToSaveEmails = "DebugFolderToSaveEmails";

        public const string CustomFieldEnabled1 = "CustomFieldEnabled1";
        public const string CustomFieldLabelSingular1 = "CustomFieldLabelSingular1";
        public const string CustomFieldLabelPlural1 = "CustomFieldLabelPlural1";

        public const string CustomFieldEnabled2 = "CustomFieldEnabled2";
        public const string CustomFieldLabelSingular2 = "CustomFieldLabelSingular2";
        public const string CustomFieldLabelPlural2 = "CustomFieldLabelPlural2";

        public const string CustomFieldEnabled3 = "CustomFieldEnabled3";
        public const string CustomFieldLabelSingular3 = "CustomFieldLabelSingular3";
        public const string CustomFieldLabelPlural3 = "CustomFieldLabelPlural3";

        public const string CustomFieldEnabled4 = "CustomFieldEnabled4";
        public const string CustomFieldLabelSingular4 = "CustomFieldLabelSingular4";
        public const string CustomFieldLabelPlural4 = "CustomFieldLabelPlural4";

        public const string CustomFieldEnabled5 = "CustomFieldEnabled5";
        public const string CustomFieldLabelSingular5 = "CustomFieldLabelSingular5";
        public const string CustomFieldLabelPlural5 = "CustomFieldLabelPlural5";

        public const string CustomFieldEnabled6 = "CustomFieldEnabled6";
        public const string CustomFieldLabelSingular6 = "CustomFieldLabelSingular6";
        public const string CustomFieldLabelPlural6 = "CustomFieldLabelPlural6";

        public const string RegistrationRequestExpirationInHours = "RegistrationRequestExpirationInHours";
        public const string InviteUserExpirationInHours = "InviteUserExpirationInHours";
        public const string IssueEmailPreamble = "IssueEmailPreamble";
        public const string EnableIncomingEmailIssueCreation = "EnableIncomingEmailIssueCreation";
        public const string IncomingEmailFilterFile = "IncomingEmailFilterFile";


        static Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>();

        // This reads "budoco_config.txt" and loads it into a key/value pair
        // collection.
        // Valid lines are either:
        // key:value
        // or
        // # this is a comment
        // or
        // blank

        static object my_lock = new Object(); // for multiple threads

        public static void load_config()
        {
            bd_util.log("bd_config.load_config()");

            var lines = File.ReadLines("budoco_config_active.txt");

            int line_count = 0;

            foreach (var line in lines)
            {
                line_count++;
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("#"))
                    continue;

                string[] pair = line.Split(":");

                if (pair.Length < 2)
                    throw new Exception(
                        "Line " + line_count.ToString() + " is bad:\n" + line);

                // handle ":" in the value, like http://localhost
                if (pair.Length > 2)
                {
                    for (int i = 2; i < pair.Length; i++)
                    {
                        pair[1] += ":" + pair[i];
                    }
                }

                // We have a valid key=value line.
                string key = pair[0].Trim();

                if (key == "")
                    throw new Exception(
                        "Line " + line_count.ToString() + " is bad:\n" + line);

                string string_value = pair[1].Trim();
                int int_value;

                if (Int32.TryParse(string_value, out int_value))
                {
                    lock (my_lock)
                    {
                        dict[key] = int_value;
                    }
                }
                else
                {
                    lock (my_lock)
                    {
                        dict[key] = string_value;
                    }
                }
            }
        }

        public static void log_config()
        {
            bd_util.log("config:");
            foreach (var k in dict.Keys)
            {
                bd_util.log(k + "=" + dict[k].ToString());
            }
        }

        public static dynamic get(string key)
        {
            // we want to prevent somebody fetching
            // the val when it's in the middle of being changed (see above)
            lock (my_lock)
            {
                return dict[key];
            }
        }


    }
}
