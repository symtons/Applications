using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.Applications
{
    public partial class ApplicationDocuments : Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string UploadsPath => Server.MapPath("~/uploads/");
        private const int MaxFileSize = 10 * 1024 * 1024; // 10MB
        private readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadApplicationInfo();
                LoadApplicationDocuments();
            }
        }

        private void LoadApplicationInfo()
        {
            string applicationIdParam = Request.QueryString["id"];

            if (string.IsNullOrEmpty(applicationIdParam) || !int.TryParse(applicationIdParam, out int applicationId))
            {
                Response.Redirect("~/Applications/NewPaperWork.aspx");
                return;
            }

            hfApplicationId.Value = applicationId.ToString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        SELECT ApplicationNumber, FirstName, LastName, Status, DateApplied
                        FROM Applications 
                        WHERE Id = @ApplicationId";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ApplicationId", applicationId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string applicationNumber = reader["ApplicationNumber"].ToString();
                                string fullName = $"{reader["FirstName"]} {reader["LastName"]}";
                                string status = reader["Status"].ToString();
                                DateTime dateApplied = Convert.ToDateTime(reader["DateApplied"]);

                                litApplicationInfo.Text = $"{applicationNumber} - {fullName} (Applied: {dateApplied:MMM dd, yyyy})";
                            }
                            else
                            {
                                ShowMessage("Job application not found.", "error");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading job application: " + ex.Message, "error");
            }
        }

        protected void btnUploadDocument_Click(object sender, EventArgs e)
        {
            //try
            //{
                if (string.IsNullOrEmpty(hfApplicationId.Value))
                {
                    ShowMessage("Invalid job application ID.", "error");
                    return;
                }

                int applicationId = Convert.ToInt32(hfApplicationId.Value);

                // Check if file upload control exists and has a file
                if (fuDocument == null)
                {
                    ShowMessage("File upload control not found.", "error");
                    return;
                }

                if (!fuDocument.HasFile || fuDocument.PostedFile == null)
                {
                    ShowMessage("Please select a file to upload.", "warning");
                    return;
                }

                var file = fuDocument.PostedFile;

                // Additional null check
                if (file == null || file.ContentLength == 0)
                {
                    ShowMessage("Invalid file or file is empty.", "warning");
                    return;
                }

                if (file.ContentLength > MaxFileSize)
                {
                    ShowMessage("File size must be less than 10MB.", "error");
                    return;
                }

                string extension = Path.GetExtension(file.FileName).ToLower();
                if (!Array.Exists(AllowedExtensions, ext => ext == extension))
                {
                    ShowMessage("Invalid file type. Only PDF, Word documents, and images are allowed.", "error");
                    return;
                }

                if (!Directory.Exists(UploadsPath))
                {
                    Directory.CreateDirectory(UploadsPath);
                }

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
                string storedFileName = $"app_{applicationId}_{timestamp}_{uniqueId}{extension}";
                string filePath = Path.Combine(UploadsPath, storedFileName);

                file.SaveAs(filePath);
                SaveDocumentToDatabase(applicationId, file, storedFileName);

                // Clear form
                txtDocumentDescription.Text = string.Empty;
                ddlDocumentCategory.SelectedIndex = 0;

                LoadApplicationDocuments();
                ShowMessage("Document uploaded successfully!", "success");
            //}
            //catch (Exception ex)
            //{
            //    ShowMessage("Error uploading document: " + ex.Message, "error");
            //}
        }

        protected void rptUploadedDocuments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                int documentId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName)
                {
                    case "ViewDocument":
                        ViewDocument(documentId);
                        break;
                    case "DownloadDocument":
                        DownloadDocument(documentId);
                        break;
                    case "DeleteDocument":
                        DeleteDocument(documentId);
                        LoadApplicationDocuments();
                        ShowMessage("Document deleted successfully.", "success");
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error processing document: " + ex.Message, "error");
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hfApplicationId.Value))
            {
                Response.Redirect($"NewPaperWork.aspx?id={hfApplicationId.Value}");
            }
            else
            {
                Response.Redirect("~NewPaperWork.aspx");
            }
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hfApplicationId.Value))
            {
                Response.Redirect($"ApplicationSuccess.aspx?id={hfApplicationId.Value}");
            }
            else
            {
                Response.Redirect("ApplicationSuccess.aspx");
            }
        }

        private void SaveDocumentToDatabase(int applicationId, HttpPostedFile file, string storedFileName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    INSERT INTO Documents (
                        ApplicationId, DocumentName, OriginalFileName, StoredFileName, FilePath, FileUrl,
                        ContentType, FileSize, FileExtension, Category, Section, DocumentType,
                        Description, AccessLevel, IsPublic, IsConfidential, UploadedById, CreatedDate
                    ) VALUES (
                        @ApplicationId, @DocumentName, @OriginalFileName, @StoredFileName, @FilePath, @FileUrl,
                        @ContentType, @FileSize, @FileExtension, @Category, @Section, @DocumentType,
                        @Description, @AccessLevel, @IsPublic, @IsConfidential, @UploadedById, @CreatedDate
                    )";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    string documentName = string.IsNullOrEmpty(txtDocumentDescription.Text)
                        ? Path.GetFileNameWithoutExtension(file.FileName)
                        : txtDocumentDescription.Text;

                    cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                    cmd.Parameters.AddWithValue("@DocumentName", documentName);
                    cmd.Parameters.AddWithValue("@OriginalFileName", file.FileName);
                    cmd.Parameters.AddWithValue("@StoredFileName", storedFileName);
                    cmd.Parameters.AddWithValue("@FilePath", "~/uploads/" + storedFileName);
                    cmd.Parameters.AddWithValue("@FileUrl", "~/uploads/" + storedFileName);
                    cmd.Parameters.AddWithValue("@ContentType", file.ContentType);
                    cmd.Parameters.AddWithValue("@FileSize", file.ContentLength);
                    cmd.Parameters.AddWithValue("@FileExtension", Path.GetExtension(file.FileName).ToUpper().TrimStart('.'));
                    cmd.Parameters.AddWithValue("@Category", ddlDocumentCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@Section", "JOB_APPLICATION_DOCUMENTS");
                    cmd.Parameters.AddWithValue("@DocumentType", "JOB_APPLICATION_SUPPORT");
                    cmd.Parameters.AddWithValue("@Description", txtDocumentDescription.Text ?? "");
                    cmd.Parameters.AddWithValue("@AccessLevel", "EMPLOYEE");
                    cmd.Parameters.AddWithValue("@IsPublic", false);
                    cmd.Parameters.AddWithValue("@IsConfidential", false);
                    cmd.Parameters.AddWithValue("@UploadedById", GetCurrentUserId());
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.UtcNow);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadApplicationDocuments()
        {
            if (string.IsNullOrEmpty(hfApplicationId.Value))
            {
                pnlNoDocuments.Visible = true;
                rptUploadedDocuments.Visible = false;
                return;
            }

            int applicationId = Convert.ToInt32(hfApplicationId.Value);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT Id, DocumentName, OriginalFileName, FileExtension, FileSize, 
                           Category, Description, CreatedDate, FilePath
                    FROM Documents 
                    WHERE ApplicationId = @ApplicationId
                    ORDER BY CreatedDate DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ApplicationId", applicationId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            rptUploadedDocuments.DataSource = dt;
                            rptUploadedDocuments.DataBind();
                            rptUploadedDocuments.Visible = true;
                            pnlNoDocuments.Visible = false;
                        }
                        else
                        {
                            rptUploadedDocuments.Visible = false;
                            pnlNoDocuments.Visible = true;
                        }
                    }
                }
            }
        }

        private void DeleteDocument(int documentId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string getFilePathSql = "SELECT FilePath FROM Documents WHERE Id = @Id";
                string filePath = "";

                using (SqlCommand cmd = new SqlCommand(getFilePathSql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", documentId);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        filePath = Server.MapPath(result.ToString());
                    }
                }

                string deleteSql = "DELETE FROM Documents WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(deleteSql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", documentId);
                    cmd.ExecuteNonQuery();
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        private void ViewDocument(int documentId)
        {
            string script = $@"
                window.open('ViewDocument.aspx?id={documentId}', '_blank', 
                'width=800,height=600,scrollbars=yes,resizable=yes');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ViewDocument", script, true);
        }

        private void DownloadDocument(int documentId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = "SELECT FilePath, OriginalFileName, ContentType FROM Documents WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", documentId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string filePath = Server.MapPath(reader["FilePath"].ToString());
                            string originalFileName = reader["OriginalFileName"].ToString();
                            string contentType = reader["ContentType"].ToString();

                            if (File.Exists(filePath))
                            {
                                Response.Clear();
                                Response.ContentType = contentType;
                                Response.AddHeader("Content-Disposition", $"attachment; filename=\"{originalFileName}\"");
                                Response.TransmitFile(filePath);
                                Response.End();
                            }
                        }
                    }
                }
            }
        }

        protected string GetFileIcon(string extension)
        {
            extension = extension.ToLower();

            switch (extension)
            {
                case "pdf":
                    return "picture_as_pdf";
                case "doc":
                case "docx":
                    return "description";
                case "jpg":
                case "jpeg":
                case "png":
                    return "image";
                default:
                    return "insert_drive_file";
            }
        }

        protected string FormatCategory(string category)
        {
            switch (category.ToUpper())
            {
                case "REFERENCE_LETTER":
                    return "Letter of Recommendation";
                case "CERTIFICATE":
                    return "Certificate/Credential";
                case "TRANSCRIPT":
                    return "Transcript/Educational Record";
                case "LICENSE":
                    return "Professional License";
                case "RESUME":
                    return "Resume/CV";
                case "OTHER":
                    return "Other";
                default:
                    return category;
            }
        }

        protected string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024.0).ToString("F1") + " KB";
            return (bytes / (1024.0 * 1024.0)).ToString("F1") + " MB";
        }

        private int GetCurrentUserId()
        {
            // TODO: Replace with your actual method to get current user ID
            // This could be from Session, User.Identity, or your authentication system
            return 1; // Placeholder
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = $"alert-panel {type}";
            litMessage.Text = message;

            // Auto-hide success messages after 5 seconds
            if (type == "success")
            {
                string script = @"
                    setTimeout(function() {
                        var panel = document.getElementById('" + pnlMessage.ClientID + @"');
                        if (panel) panel.style.display = 'none';
                    }, 5000);";
                ScriptManager.RegisterStartupScript(this, GetType(), "HideMessage", script, true);
            }
        }
    }
}