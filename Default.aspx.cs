#define imagesTable11
#define addTableRow

using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace AzureConn
{
    public partial class Default : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string AllowedExts = ".jpg, .png, .gif, .jpeg";
        public string imageFolderPath = string.Empty;

        public string fullAddress = "";
        public string cNameAddress = "";
        public string databaseName = "";
        public string userId = "";
        public string password = "";
        
        public string query = "SELECT so.name FROM sysobjects so WHERE so.xtype = 'U' ";
        protected void Page_Load(object sender, EventArgs e)
        {
            imageFolderPath = Server.MapPath("./App_Images/");

            //Write log to ApplicationInsights using log4net
            log.Debug("--targetFramework='4.8': This is test log ---");

            if (!IsPostBack)
            {
                txtFullAddress.Text = fullAddress;
                txtCNameAddress.Text = cNameAddress;
                txtDatabaseName.Text = databaseName;
                txtUserId.Text = userId;
                txtPassword.Text = password;
                txtQuery.Text = query;
            }
        }

        protected void btnFullAddress_Click(object sender, EventArgs e)
        {
            Label1.Text = txtFullAddress.Text;
            txtResults.Text = Get_SQL_Data(Label1.Text, txtQuery.Text);
        }

        protected void btnCNameAddress_Click(object sender, EventArgs e)
        {
            Label1.Text = txtCNameAddress.Text;
            txtResults.Text = Get_SQL_Data(Label1.Text, txtQuery.Text);
        }

        private string Get_SQL_Data(string serverName, string queryString)
        {
            pnResults.Visible = true;
            pnImageFiles.Visible = false;

            string outString = string.Empty;
            string sqlString = string.Empty;
            string query = queryString;

            if (txtPassword.Text.Length < 1) txtPassword.Text = "web";
            string password = ConfigurationManager.AppSettings[txtPassword.Text];
            bool contains = ("web,prod").IndexOf(txtPassword.Text, StringComparison.OrdinalIgnoreCase) >= 0;

            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
            sb.DataSource = serverName;
            sb.UserID = txtUserId.Text;
            sb.Password = contains ? password : txtPassword.Text;
            sb.InitialCatalog = txtDatabaseName.Text;
            SqlConnection cn = new SqlConnection(sb.ConnectionString);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            cmd.Connection = cn;

            try
            {
                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                DataSet  ds = new DataSet();
                sqlda.Fill(ds, "author");
                DataTable dt = ds.Tables["author"];
                int r1 = dt.Rows.Count;

                bool isGood = (dt != null && dt.Rows.Count > 0) ? true : false;
                if (isGood)
                {
                    if (rdoTrue.Checked)
                        outString = Newtonsoft.Json.JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
                    else
                        outString = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                }

                dt.Dispose();
            }
            catch (Exception ex)
            {
                outString = ex.Message;
            }
            return outString;
        }


        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.Current.GetOwinContext().Authentication.Challenge(
                  new AuthenticationProperties { RedirectUri = "/" },
                  OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            //Not Working: Context.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            //Not Working: Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Request.GetOwinContext().Authentication.SignOut();
            Session.Abandon();
        }

        // Maximum request length exceeded.
        // https://stackoverflow.com/questions/3853767/maximum-request-length-exceeded

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            pnResults.Visible = false;
            pnImageFiles.Visible = false;
            pnImageTable.Visible = true;

            string uploadMessage = "<br />";

            int uploadedFileCount = 0;
            string strFileName;
            string strFilePath;

            HttpPostedFile uploadFile = Request.Files[0];   //return 1 even if no file is uploaded
            if (uploadFile.ContentLength != 0)
            {
                uploadedFileCount = Request.Files.Count;
            }

            if (uploadFile.FileName != "" && uploadedFileCount > 0) 
            {
                // Create the directory if it does not exist.
                //if (!Directory.Exists(imageFolderPath))
                //{
                //    Directory.CreateDirectory(imageFolderPath);
                //}

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFile oFile = Request.Files[i];
                    strFileName = oFile.FileName;
                    strFileName = Path.GetFileName(strFileName);    //no path supplied

                    string ext = Path.GetExtension(strFileName).ToLower();
                    Boolean isValidExt = (".jpg, .png, .gif, .jpeg").Contains(ext);
                    if (isValidExt)
                    {
                        // Save the uploaded file to the server.
                        strFilePath = imageFolderPath + strFileName;
                        if (File.Exists(strFilePath))
                        {
                            oFile.SaveAs(strFilePath);
                            uploadMessage += strFileName + " already exists on the server! <br />";
                        }
                        else
                        {
                            oFile.SaveAs(strFilePath);
                            uploadMessage += strFileName + " has been successfully uploaded. <br />";
                        }
                        uploadedFileCount++;
                    }
                    else
                    {
                        uploadMessage += ext + " is not allowed file type.";
                    }
                }
                uploadMessage += "Total " + uploadedFileCount.ToString() + "files are uploaded.<br />";
            }
            else
            {
                uploadMessage += "Click 'Choose file' to select the file to upload.<br />";
            }
            uploadMessage += "<br />";

            //display image files
            btnViewImages_Click(sender, e);
            lblTableMessage.Text = uploadMessage;

        }

        protected void btnViewImages_Click(object sender, EventArgs e)
        {
            var dirInfo = new DirectoryInfo(imageFolderPath);
            FileInfo[] files = dirInfo.GetFiles("*.*").OrderBy(f => f.CreationTime).ToArray();
            StringBuilder sb = new StringBuilder();

#if imagesTable
            pnResults.Visible = false;
            pnImageFiles.Visible = true;

            sb.Append("<table class='styled-table'><thead><tr><th>file name</th><th>Date</th><th>size</th></tr></thead>");

            foreach (var imageFile in files)
            {
                Boolean isValidExt = AllowedExts.Contains(imageFile.Extension);
                if (isValidExt)
                    sb.AppendLine(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", imageFile.Name, imageFile.LastAccessTime.ToString("MM/dd/yyyy HH:mm:ss"), imageFile.Length / 1024 + " KB"));
            }
            sb.Append("</table>");
            lblUploadResult.Text = sb.ToString();

#elif addTableRow
            pnResults.Visible = false;
            pnImageFiles.Visible = false;
            pnImageTable.Visible = true;

            lblTableMessage.Text = string.Empty;

            Table tbl = (Table)this.FindControl("UploadTable");
            int imagefiles = 0;

            foreach (var imageFile in files)
            {
                Boolean isValidExt = AllowedExts.Contains(imageFile.Extension);
                if (isValidExt)
                {
                    TableRow tRow = new TableRow();

                    TableCell tCell1 = new TableCell();
                    tCell1.Controls.Add(new Label());
                    tRow.Cells.Add(tCell1);
                    tCell1.Text = imageFile.Name;

                    TableCell tCell2 = new TableCell();
                    tCell2.Controls.Add(new Label());
                    tRow.Cells.Add(tCell2);
                    tCell2.HorizontalAlign = HorizontalAlign.Center;
                    tCell2.Text = imageFile.CreationTime.ToString("MM/dd/yyyy HH:mm:ss");

                    TableCell tCell3 = new TableCell();
                    tCell3.Controls.Add(new Label());
                    tCell3.HorizontalAlign = HorizontalAlign.Left;
                    tRow.Cells.Add(tCell3);
                    //tCell3.Text = imageFile.Length / 1024 + " KB";

                    CheckBox chkDeleteImage = new CheckBox();
                    chkDeleteImage.Checked = false;
                    chkDeleteImage.Text = "Delete";
                    chkDeleteImage.ID = "Delete_" + imageFile.Name;
                    tCell3.Controls.Add(chkDeleteImage);

                    //tbl.Rows.Add(tRow);
                    tbl.Rows.AddAt(1, tRow);  //right below header row
                    imagefiles++;
                }
            }
            lblImageFileCount.Text = "Total images are " + imagefiles.ToString();

#else 
            pnResults.Visible = true;
            pnImageFiles.Visible = false;

            foreach (var imageFile in files)
            {
                Boolean isValidExt = AllowedExts.Contains(imageFile.Extension);
                if (isValidExt)
                    sb.AppendLine(imageFile.Name + " - " + imageFile.LastAccessTime.ToString("MM/dd/yyyy HH:mm:ss") + " - " + imageFile.Length / 1024 + " KB");
            }
            txtResults.Text = sb.ToString();
#endif
        }

        protected void btnDeleteImage_Click(object sender, EventArgs e)
        {
            string[] deletes = Request.Form.AllKeys.Where(x => x.StartsWith("Delete_")).ToArray();

            string imageFileNameToDelete = string.Empty;
            string deleteMessage = string.Empty;

            List<string> listKeys = new List<string>();

            //get image file name to be deleted
            foreach (string key in Request.Form.AllKeys)
            {
                //All checkboxes that are checked are sent through the postback
                if (key.StartsWith("Delete_"))
                {
                    listKeys.Add(key);
                }
            }

            if (listKeys.Count> 0) 
            {
                //delete image file
                string imageFileName = string.Empty;
                deleteMessage = "Below files are deleted. <ul>";

                for (int i = 0; i < listKeys.Count; i++)
                {
                    imageFileName = listKeys[i].Remove(0, 7);           //remove "Delete_" from filename
                    imageFileNameToDelete = Path.Combine(imageFolderPath, imageFileName);
                    if (File.Exists(imageFileNameToDelete))
                    {
                        File.Delete(imageFileNameToDelete);
                        deleteMessage += "<li>" + imageFileName + "</li>";
                    }
                }
                deleteMessage += "</ul>";
            }

            //re-display remaining image files
            btnViewImages_Click(sender, e);
            lblTableMessage.Text = deleteMessage;

        }


    }

}