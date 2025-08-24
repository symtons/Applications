using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;

namespace Applications
{
    public partial class ApplicationSuccess : System.Web.UI.Page
    {
        private ApplicationData GetApplicationData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    SELECT [ApplicationNumber], [FirstName], [MiddleName], [LastName], [HomeAddress], [AptNumber], 
                           [City], [State], [Zip], [HomePhone], [CellPhone], [SSN], [DriversLicense], [DLState],
                           [EmergencyContactName], [EmergencyContactRelationship], [EmergencyContactAddress],
                           [Position1], [Position2], [SalaryDesired], [SalaryType], [EmploymentType], [AvailableStartDate],
                           [NashvilleLocation], [FranklinLocation], [ShelbyvilleLocation], [WaynesboroLocation],
                           [FirstShift], [SecondShift], [ThirdShift], [WeekendsOnly],
                           [MondayAvailable], [TuesdayAvailable], [WednesdayAvailable], [ThursdayAvailable],
                           [FridayAvailable], [SaturdayAvailable], [SundayAvailable],
                           [ElementarySchool], [HighSchool], [UndergraduateSchool], [GraduateSchool],
                           [UGSkills], [GradSkills], [SpecialKnowledge],
                           [LicenseType1], [LicenseState1], [LicenseNumber1], [LicenseExpiration1],
                           [LicenseType2], [LicenseState2], [LicenseNumber2], [LicenseExpiration2],
                           [Employer1], [EmploymentFrom1], [EmploymentTo1], [JobTitle1],
                           [Supervisor1], [EmployerAddress1], [EmployerCityStateZip1], [EmployerPhone1],
                           [StartingPay1], [FinalPay1], [ReasonLeaving1],
                           [Employer2], [EmploymentFrom2], [EmploymentTo2], [JobTitle2],
                           [Supervisor2], [EmployerAddress2], [EmployerCityStateZip2], [EmployerPhone2],
                           [StartingPay2], [FinalPay2], [ReasonLeaving2],
                           [Reference1Name], [Reference1Phone], [Reference1Email], [Reference1Years],
                           [Reference2Name], [Reference2Phone], [Reference2Email], [Reference2Years],
                           [Reference3Name], [Reference3Phone], [Reference3Email], [Reference3Years],
                           [FinalAcknowledgment], [SubmissionDate], [Status]
                    FROM [dbo].[JobApplications] 
                    WHERE [ApplicationId] = @ApplicationId";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationId", applicationId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ApplicationData
                            {
                                ApplicationNumber = reader["ApplicationNumber"]?.ToString() ?? "",
                                FullName = $"{reader["FirstName"]} {reader["LastName"]}",
                                MiddleName = reader["MiddleName"]?.ToString() ?? "",
                                HomeAddress = reader["HomeAddress"]?.ToString() ?? "",
                                AptNumber = reader["AptNumber"]?.ToString() ?? "",
                                City = reader["City"]?.ToString() ?? "",
                                State = reader["State"]?.ToString() ?? "",
                                Zip = reader["Zip"]?.ToString() ?? "",
                                HomePhone = reader["HomePhone"]?.ToString() ?? "",
                                CellPhone = reader["CellPhone"]?.ToString() ?? "",
                                SSN = reader["SSN"]?.ToString() ?? "",
                                DriversLicense = reader["DriversLicense"]?.ToString() ?? "",
                                DLState = reader["DLState"]?.ToString() ?? "",
                                EmergencyContactName = reader["EmergencyContactName"]?.ToString() ?? "",
                                EmergencyContactRelationship = reader["EmergencyContactRelationship"]?.ToString() ?? "",
                                EmergencyContactAddress = reader["EmergencyContactAddress"]?.ToString() ?? "",
                                Position1 = reader["Position1"]?.ToString() ?? "",
                                Position2 = reader["Position2"]?.ToString() ?? "",
                                SalaryDesired = reader["SalaryDesired"]?.ToString() ?? "",
                                SalaryType = reader["SalaryType"]?.ToString() ?? "",
                                EmploymentType = reader["EmploymentType"]?.ToString() ?? "",
                                AvailableStartDate = reader["AvailableStartDate"]?.ToString() ?? "",
                                NashvilleLocation = Convert.ToBoolean(reader["NashvilleLocation"] ?? false),
                                FranklinLocation = Convert.ToBoolean(reader["FranklinLocation"] ?? false),
                                ShelbyvilleLocation = Convert.ToBoolean(reader["ShelbyvilleLocation"] ?? false),
                                WaynesboroLocation = Convert.ToBoolean(reader["WaynesboroLocation"] ?? false),
                                FirstShift = Convert.ToBoolean(reader["FirstShift"] ?? false),
                                SecondShift = Convert.ToBoolean(reader["SecondShift"] ?? false),
                                ThirdShift = Convert.ToBoolean(reader["ThirdShift"] ?? false),
                                WeekendsOnly = Convert.ToBoolean(reader["WeekendsOnly"] ?? false),
                                MondayAvailable = Convert.ToBoolean(reader["MondayAvailable"] ?? false),
                                TuesdayAvailable = Convert.ToBoolean(reader["TuesdayAvailable"] ?? false),
                                WednesdayAvailable = Convert.ToBoolean(reader["WednesdayAvailable"] ?? false),
                                ThursdayAvailable = Convert.ToBoolean(reader["ThursdayAvailable"] ?? false),
                                FridayAvailable = Convert.ToBoolean(reader["FridayAvailable"] ?? false),
                                SaturdayAvailable = Convert.ToBoolean(reader["SaturdayAvailable"] ?? false),
                                SundayAvailable = Convert.ToBoolean(reader["SundayAvailable"] ?? false),
                                ElementarySchool = reader["ElementarySchool"]?.ToString() ?? "",
                                HighSchool = reader["HighSchool"]?.ToString() ?? "",
                                UndergraduateSchool = reader["UndergraduateSchool"]?.ToString() ?? "",
                                GraduateSchool = reader["GraduateSchool"]?.ToString() ?? "",
                                HSSkills = "", // Column doesn't exist, set to empty
                                UGSkills = reader["UGSkills"]?.ToString() ?? "",
                                GradSkills = reader["GradSkills"]?.ToString() ?? "",
                                SpecialKnowledge = reader["SpecialKnowledge"]?.ToString() ?? "",
                                LicenseType1 = reader["LicenseType1"]?.ToString() ?? "",
                                LicenseState1 = reader["LicenseState1"]?.ToString() ?? "",
                                LicenseNumber1 = reader["LicenseNumber1"]?.ToString() ?? "",
                                LicenseExpiration1 = reader["LicenseExpiration1"]?.ToString() ?? "",
                                LicenseType2 = reader["LicenseType2"]?.ToString() ?? "",
                                LicenseState2 = reader["LicenseState2"]?.ToString() ?? "",
                                LicenseNumber2 = reader["LicenseNumber2"]?.ToString() ?? "",
                                LicenseExpiration2 = reader["LicenseExpiration2"]?.ToString() ?? "",
                                LicenseType3 = "", // Column doesn't exist, set to empty
                                LicenseState3 = "", // Column doesn't exist, set to empty
                                LicenseNumber3 = "", // Column doesn't exist, set to empty
                                LicenseExpiration3 = "", // Column doesn't exist, set to empty
                                DIDDTraining = "", // Column doesn't exist, set to empty
                                Employer1 = reader["Employer1"]?.ToString() ?? "",
                                EmploymentFrom1 = reader["EmploymentFrom1"]?.ToString() ?? "",
                                EmploymentTo1 = reader["EmploymentTo1"]?.ToString() ?? "",
                                JobTitle1 = reader["JobTitle1"]?.ToString() ?? "",
                                Supervisor1 = reader["Supervisor1"]?.ToString() ?? "",
                                EmployerAddress1 = reader["EmployerAddress1"]?.ToString() ?? "",
                                EmployerCityStateZip1 = reader["EmployerCityStateZip1"]?.ToString() ?? "",
                                EmployerPhone1 = reader["EmployerPhone1"]?.ToString() ?? "",
                                HourlyRate1 = reader["StartingPay1"]?.ToString() ?? "", // Using StartingPay1 for HourlyRate1
                                ReasonForLeaving1 = reader["ReasonLeaving1"]?.ToString() ?? "",
                                MayContact1 = "", // Column doesn't exist, set to empty
                                Employer2 = reader["Employer2"]?.ToString() ?? "",
                                EmploymentFrom2 = reader["EmploymentFrom2"]?.ToString() ?? "",
                                EmploymentTo2 = reader["EmploymentTo2"]?.ToString() ?? "",
                                JobTitle2 = reader["JobTitle2"]?.ToString() ?? "",
                                Supervisor2 = reader["Supervisor2"]?.ToString() ?? "",
                                EmployerAddress2 = reader["EmployerAddress2"]?.ToString() ?? "",
                                EmployerCityStateZip2 = reader["EmployerCityStateZip2"]?.ToString() ?? "",
                                EmployerPhone2 = reader["EmployerPhone2"]?.ToString() ?? "",
                                HourlyRate2 = reader["StartingPay2"]?.ToString() ?? "", // Using StartingPay2 for HourlyRate2
                                ReasonForLeaving2 = reader["ReasonLeaving2"]?.ToString() ?? "",
                                MayContact2 = "", // Column doesn't exist, set to empty
                                Reference1Name = reader["Reference1Name"]?.ToString() ?? "",
                                Reference1Phone = reader["Reference1Phone"]?.ToString() ?? "",
                                Reference1Email = reader["Reference1Email"]?.ToString() ?? "",
                                Reference1Years = reader["Reference1Years"]?.ToString() ?? "",
                                Reference2Name = reader["Reference2Name"]?.ToString() ?? "",
                                Reference2Phone = reader["Reference2Phone"]?.ToString() ?? "",
                                Reference2Email = reader["Reference2Email"]?.ToString() ?? "",
                                Reference2Years = reader["Reference2Years"]?.ToString() ?? "",
                                Reference3Name = reader["Reference3Name"]?.ToString() ?? "",
                                Reference3Phone = reader["Reference3Phone"]?.ToString() ?? "",
                                Reference3Email = reader["Reference3Email"]?.ToString() ?? "",
                                Reference3Years = reader["Reference3Years"]?.ToString() ?? "",
                                FinalAcknowledgment = Convert.ToBoolean(reader["FinalAcknowledgment"] ?? false),
                                SubmissionDate = Convert.ToDateTime(reader["SubmissionDate"]).ToString("MMMM dd, yyyy"),
                                Status = reader["Status"]?.ToString() ?? "Submitted"
                            };
                        }
                    }
                }
            }

            return new ApplicationData(); // Return empty data if not found
        }
        readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int applicationId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Get application ID from query string
                if (!int.TryParse(Request.QueryString["id"], out applicationId))
                {
                    // Redirect to main page if no valid ID
                    Response.Redirect("~/NewPaperWork.aspx");
                    return;
                }

                LoadApplicationDetails();
            }
        }

        private void LoadApplicationDetails()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    SELECT [ApplicationNumber], [SubmissionDate], [FirstName], [LastName], [Status]
                    FROM [dbo].[JobApplications] 
                    WHERE [ApplicationId] = @ApplicationId";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationId", applicationId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblApplicationNumber.Text = reader["ApplicationNumber"]?.ToString() ?? "N/A";
                            lblSubmissionDate.Text = Convert.ToDateTime(reader["SubmissionDate"]).ToString("MMMM dd, yyyy 'at' h:mm tt");
                            lblApplicantName.Text = $"{reader["FirstName"]} {reader["LastName"]}";

                            // Store application ID for button events
                            ViewState["ApplicationId"] = applicationId;
                        }
                        else
                        {
                            // Application not found, redirect back
                            Response.Redirect("~/NewPaperWork.aspx");
                        }
                    }
                }
            }
        }

        protected void btnDownloadPDF_Click(object sender, EventArgs e)
        {
            try
            {
                applicationId = (int)ViewState["ApplicationId"];
                byte[] pdfData = GenerateApplicationPDF();

                // Set response headers for PDF download
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", $"attachment; filename=Application_{lblApplicationNumber.Text}.pdf");
                Response.AddHeader("Content-Length", pdfData.Length.ToString());

                // Write PDF data to response
                Response.BinaryWrite(pdfData);
                Response.End();
            }
            catch (Exception ex)
            {
                // Handle error - could show message or log
                Response.Write($"<script>alert('Error generating PDF: {ex.Message}');</script>");
            }
        }

        protected void btnPrintApplication_Click(object sender, EventArgs e)
        {
            try
            {
                applicationId = (int)ViewState["ApplicationId"];
                byte[] pdfData = GenerateApplicationPDF();

                // Set response headers for PDF printing
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", $"inline; filename=Application_{lblApplicationNumber.Text}.pdf");

                // Write PDF data to response for printing
                Response.BinaryWrite(pdfData);
                Response.End();
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('Error generating PDF: {ex.Message}');</script>");
            }
        }

        private byte[] GenerateApplicationPDF()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Create PDF document
                Document document = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Get application data from database
                var applicationData = GetApplicationData();

                // Add company header
                AddCompanyHeader(document);

                // Add application content
                AddApplicationContent(document, applicationData);

                document.Close();

                return ms.ToArray();
            }
        }

        private void AddCompanyHeader(Document document)
        {
            // Company header
            iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLUE);
            iTextSharp.text.Font subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.DARK_GRAY);

            Paragraph title = new Paragraph("Tennessee Personal Assistance, Inc.", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);

            Paragraph subtitle = new Paragraph("Employment Application", subtitleFont);
            subtitle.Alignment = Element.ALIGN_CENTER;
            subtitle.SpacingAfter = 20f;
            document.Add(subtitle);

            // Add application number and date
            Paragraph appInfo = new Paragraph($"Application Number: {lblApplicationNumber.Text} | Date Submitted: {lblSubmissionDate.Text}",
                FontFactory.GetFont(FontFactory.HELVETICA, 10));
            appInfo.Alignment = Element.ALIGN_CENTER;
            appInfo.SpacingAfter = 20f;
            document.Add(appInfo);

            // Add separator line
            Chunk lineBreak = new Chunk(new LineSeparator(1f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -1));
            document.Add(lineBreak);
            document.Add(new Paragraph(" ")); // Add some space
        }

        private void AddApplicationContent(Document document, ApplicationData data)
        {
            iTextSharp.text.Font sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLUE);
            iTextSharp.text.Font labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            iTextSharp.text.Font valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            // Personal Information Section
            AddSection(document, "PERSONAL INFORMATION", sectionFont);
            PdfPTable personalTable = CreateTwoColumnTable();
            AddTableRow(personalTable, "Full Name:", data.FullName, labelFont, valueFont);
            AddTableRow(personalTable, "Middle Name:", data.MiddleName, labelFont, valueFont);
            AddTableRow(personalTable, "Home Address:", data.HomeAddress, labelFont, valueFont);
            AddTableRow(personalTable, "Apt Number:", data.AptNumber, labelFont, valueFont);
            AddTableRow(personalTable, "City, State, Zip:", $"{data.City}, {data.State} {data.Zip}", labelFont, valueFont);
            AddTableRow(personalTable, "Home Phone:", data.HomePhone, labelFont, valueFont);
            AddTableRow(personalTable, "Cell Phone:", data.CellPhone, labelFont, valueFont);
            AddTableRow(personalTable, "SSN:", data.SSN, labelFont, valueFont);
            AddTableRow(personalTable, "Driver's License:", data.DriversLicense, labelFont, valueFont);
            AddTableRow(personalTable, "DL State:", data.DLState, labelFont, valueFont);
            AddTableRow(personalTable, "Emergency Contact:", data.EmergencyContactName, labelFont, valueFont);
            AddTableRow(personalTable, "Emergency Relationship:", data.EmergencyContactRelationship, labelFont, valueFont);
            AddTableRow(personalTable, "Emergency Address:", data.EmergencyContactAddress, labelFont, valueFont);
            document.Add(personalTable);

            // Position Information Section
            AddSection(document, "POSITION INFORMATION", sectionFont);
            PdfPTable positionTable = CreateTwoColumnTable();
            AddTableRow(positionTable, "Position 1:", data.Position1, labelFont, valueFont);
            AddTableRow(positionTable, "Position 2:", data.Position2, labelFont, valueFont);
            AddTableRow(positionTable, "Salary Desired:", data.SalaryDesired, labelFont, valueFont);
            AddTableRow(positionTable, "Salary Type:", data.SalaryType, labelFont, valueFont);
            AddTableRow(positionTable, "Employment Type:", data.EmploymentType, labelFont, valueFont);
            AddTableRow(positionTable, "Available Start Date:", data.AvailableStartDate, labelFont, valueFont);
            document.Add(positionTable);

            // Location Availability
            AddSection(document, "LOCATION AVAILABILITY", sectionFont);
            PdfPTable locationTable = CreateTwoColumnTable();
            AddTableRow(locationTable, "Nashville:", data.NashvilleLocation ? "Yes" : "No", labelFont, valueFont);
            AddTableRow(locationTable, "Franklin:", data.FranklinLocation ? "Yes" : "No", labelFont, valueFont);
            AddTableRow(locationTable, "Shelbyville:", data.ShelbyvilleLocation ? "Yes" : "No", labelFont, valueFont);
            AddTableRow(locationTable, "Waynesboro:", data.WaynesboroLocation ? "Yes" : "No", labelFont, valueFont);
            document.Add(locationTable);

            // Shift Availability
            AddSection(document, "SHIFT AVAILABILITY", sectionFont);
            PdfPTable shiftTable = CreateTwoColumnTable();
            AddTableRow(shiftTable, "First Shift:", data.FirstShift ? "Yes" : "No", labelFont, valueFont);
            AddTableRow(shiftTable, "Second Shift:", data.SecondShift ? "Yes" : "No", labelFont, valueFont);
            AddTableRow(shiftTable, "Third Shift:", data.ThirdShift ? "Yes" : "No", labelFont, valueFont);
            AddTableRow(shiftTable, "Weekends Only:", data.WeekendsOnly ? "Yes" : "No", labelFont, valueFont);
            document.Add(shiftTable);

            // Days Available
            AddSection(document, "DAYS AVAILABLE", sectionFont);
            string daysAvailable = "";
            if (data.MondayAvailable) daysAvailable += "Monday, ";
            if (data.TuesdayAvailable) daysAvailable += "Tuesday, ";
            if (data.WednesdayAvailable) daysAvailable += "Wednesday, ";
            if (data.ThursdayAvailable) daysAvailable += "Thursday, ";
            if (data.FridayAvailable) daysAvailable += "Friday, ";
            if (data.SaturdayAvailable) daysAvailable += "Saturday, ";
            if (data.SundayAvailable) daysAvailable += "Sunday, ";
            daysAvailable = daysAvailable.TrimEnd(' ', ',');

            Paragraph daysText = new Paragraph(daysAvailable, valueFont);
            daysText.SpacingAfter = 15f;
            document.Add(daysText);

            // Education Section
            AddSection(document, "EDUCATION", sectionFont);
            PdfPTable educationTable = CreateTwoColumnTable();
            AddTableRow(educationTable, "Elementary School:", data.ElementarySchool, labelFont, valueFont);
            AddTableRow(educationTable, "High School:", data.HighSchool, labelFont, valueFont);
            AddTableRow(educationTable, "Undergraduate School:", data.UndergraduateSchool, labelFont, valueFont);
            AddTableRow(educationTable, "Graduate School:", data.GraduateSchool, labelFont, valueFont);
            AddTableRow(educationTable, "High School Skills:", data.HSSkills, labelFont, valueFont);
            AddTableRow(educationTable, "Undergraduate Skills:", data.UGSkills, labelFont, valueFont);
            AddTableRow(educationTable, "Graduate Skills:", data.GradSkills, labelFont, valueFont);
            AddTableRow(educationTable, "Special Knowledge:", data.SpecialKnowledge, labelFont, valueFont);
            document.Add(educationTable);

            // Licenses Section
            AddSection(document, "LICENSES/CERTIFICATIONS", sectionFont);
            PdfPTable licensesTable = new PdfPTable(4);
            licensesTable.WidthPercentage = 100f;
            licensesTable.SetWidths(new float[] { 2f, 1f, 2f, 1.5f });

            // Header row
            AddHeaderCell(licensesTable, "License Type", labelFont);
            AddHeaderCell(licensesTable, "State", labelFont);
            AddHeaderCell(licensesTable, "Number", labelFont);
            AddHeaderCell(licensesTable, "Expiration", labelFont);

            // License data
            AddTableCell(licensesTable, data.LicenseType1, valueFont);
            AddTableCell(licensesTable, data.LicenseState1, valueFont);
            AddTableCell(licensesTable, data.LicenseNumber1, valueFont);
            AddTableCell(licensesTable, data.LicenseExpiration1, valueFont);

            AddTableCell(licensesTable, data.LicenseType2, valueFont);
            AddTableCell(licensesTable, data.LicenseState2, valueFont);
            AddTableCell(licensesTable, data.LicenseNumber2, valueFont);
            AddTableCell(licensesTable, data.LicenseExpiration2, valueFont);

            AddTableCell(licensesTable, data.LicenseType3, valueFont);
            AddTableCell(licensesTable, data.LicenseState3, valueFont);
            AddTableCell(licensesTable, data.LicenseNumber3, valueFont);
            AddTableCell(licensesTable, data.LicenseExpiration3, valueFont);

            document.Add(licensesTable);

            // DIDD Training
            if (!string.IsNullOrEmpty(data.DIDDTraining))
            {
                AddSection(document, "DIDD TRAINING", sectionFont);
                Paragraph diddText = new Paragraph(data.DIDDTraining, valueFont);
                diddText.SpacingAfter = 15f;
                document.Add(diddText);
            }

            // Employment History Section
            AddSection(document, "EMPLOYMENT HISTORY", sectionFont);
            AddEmploymentHistory(document, "Current/Most Recent Employment", data.Employer1, data.EmploymentFrom1, data.EmploymentTo1,
                                data.JobTitle1, data.Supervisor1, data.EmployerAddress1, data.EmployerCityStateZip1,
                                data.EmployerPhone1, data.HourlyRate1, data.ReasonForLeaving1, data.MayContact1, labelFont, valueFont);

            if (!string.IsNullOrEmpty(data.Employer2))
            {
                AddEmploymentHistory(document, "Previous Employment", data.Employer2, data.EmploymentFrom2, data.EmploymentTo2,
                                   data.JobTitle2, data.Supervisor2, data.EmployerAddress2, data.EmployerCityStateZip2,
                                   data.EmployerPhone2, data.HourlyRate2, data.ReasonForLeaving2, data.MayContact2, labelFont, valueFont);
            }

            // References Section
            AddSection(document, "PROFESSIONAL REFERENCES", sectionFont);
            AddReference(document, "Reference 1", data.Reference1Name, data.Reference1Phone, data.Reference1Email, data.Reference1Years, labelFont, valueFont);
            AddReference(document, "Reference 2", data.Reference2Name, data.Reference2Phone, data.Reference2Email, data.Reference2Years, labelFont, valueFont);
            AddReference(document, "Reference 3", data.Reference3Name, data.Reference3Phone, data.Reference3Email, data.Reference3Years, labelFont, valueFont);

            // Final acknowledgment
            if (data.FinalAcknowledgment)
            {
                AddSection(document, "ACKNOWLEDGMENT", sectionFont);
                Paragraph ackText = new Paragraph("I acknowledge that I have read and understand all the terms and conditions of this application.", valueFont);
                ackText.SpacingAfter = 15f;
                document.Add(ackText);
            }

            // Add signature and date
            AddNoticeSection(document);
        }

        private void AddTableRow(PdfPTable table, string label, string value, iTextSharp.text.Font labelFont, iTextSharp.text.Font valueFont)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, labelFont));
            labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            labelCell.PaddingBottom = 5f;
            table.AddCell(labelCell);

            PdfPCell valueCell = new PdfPCell(new Phrase(value ?? "", valueFont));
            valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            valueCell.PaddingBottom = 5f;
            table.AddCell(valueCell);
        }

        private PdfPTable CreateTwoColumnTable()
        {
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100f;
            table.SetWidths(new float[] { 1f, 2f });
            table.SpacingAfter = 15f;
            return table;
        }

        private void AddSection(Document document, string title, iTextSharp.text.Font font)
        {
            Paragraph section = new Paragraph(title, font);
            section.SpacingBefore = 15f;
            section.SpacingAfter = 10f;
            document.Add(section);
        }

        private void AddHeaderCell(PdfPTable table, string text, iTextSharp.text.Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.Padding = 5f;
            table.AddCell(cell);
        }

        private void AddTableCell(PdfPTable table, string text, iTextSharp.text.Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text ?? "", font));
            cell.Padding = 5f;
            table.AddCell(cell);
        }

        private void AddEmploymentHistory(Document document, string title, string employer, string fromDate, string toDate,
                                        string jobTitle, string supervisor, string address, string cityStateZip,
                                        string phone, string hourlyRate, string reasonForLeaving, string mayContact,
                                        iTextSharp.text.Font labelFont, iTextSharp.text.Font valueFont)
        {
            if (string.IsNullOrEmpty(employer)) return;

            Paragraph empTitle = new Paragraph(title, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
            empTitle.SpacingBefore = 10f;
            empTitle.SpacingAfter = 5f;
            document.Add(empTitle);

            PdfPTable empTable = CreateTwoColumnTable();
            AddTableRow(empTable, "Employer:", employer, labelFont, valueFont);
            AddTableRow(empTable, "Employment Dates:", $"{fromDate} to {toDate}", labelFont, valueFont);
            AddTableRow(empTable, "Job Title:", jobTitle, labelFont, valueFont);
            AddTableRow(empTable, "Supervisor:", supervisor, labelFont, valueFont);
            AddTableRow(empTable, "Address:", address, labelFont, valueFont);
            AddTableRow(empTable, "City, State, Zip:", cityStateZip, labelFont, valueFont);
            AddTableRow(empTable, "Phone:", phone, labelFont, valueFont);
            AddTableRow(empTable, "Hourly Rate:", hourlyRate, labelFont, valueFont);
            AddTableRow(empTable, "Reason for Leaving:", reasonForLeaving, labelFont, valueFont);
            AddTableRow(empTable, "May We Contact:", mayContact, labelFont, valueFont);
            document.Add(empTable);
        }

        private void AddReference(Document document, string title, string name, string phone, string email, string years,
                                 iTextSharp.text.Font labelFont, iTextSharp.text.Font valueFont)
        {
            if (string.IsNullOrEmpty(name)) return;

            Paragraph refTitle = new Paragraph(title, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11));
            refTitle.SpacingBefore = 8f;
            refTitle.SpacingAfter = 5f;
            document.Add(refTitle);

            PdfPTable refTable = CreateTwoColumnTable();
            AddTableRow(refTable, "Name:", name, labelFont, valueFont);
            AddTableRow(refTable, "Phone:", phone, labelFont, valueFont);
            AddTableRow(refTable, "Email:", email, labelFont, valueFont);
            AddTableRow(refTable, "Years Known:", years, labelFont, valueFont);
            document.Add(refTable);
        }

        private void AddNoticeSection(Document document)
        {
            iTextSharp.text.Font noticeFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.DARK_GRAY);

            Paragraph notice = new Paragraph("\n\nNOTICE: This application will be kept on file for 90 days. " +
                "All information must be complete and accurate. Tennessee Personal Assistance, Inc. is an equal opportunity employer.",
                noticeFont);
            notice.Alignment = Element.ALIGN_JUSTIFIED;
            notice.SpacingBefore = 20f;
            notice.SetLeading(12f, 0f); // Fixed: added second parameter for multipliedLeading
            document.Add(notice);

            // Add signature line
            Paragraph signature = new Paragraph("\n\nApplicant Signature: ________________________________     Date: ________________",
                FontFactory.GetFont(FontFactory.HELVETICA, 10));
            signature.SpacingBefore = 30f;
            document.Add(signature);
        }

        // Helper class to hold application data
        private class ApplicationData
        {
            public string ApplicationNumber { get; set; } = "";
            public string FullName { get; set; } = "";
            public string MiddleName { get; set; } = "";
            public string HomeAddress { get; set; } = "";
            public string AptNumber { get; set; } = "";
            public string City { get; set; } = "";
            public string State { get; set; } = "";
            public string Zip { get; set; } = "";
            public string HomePhone { get; set; } = "";
            public string CellPhone { get; set; } = "";
            public string SSN { get; set; } = "";
            public string DriversLicense { get; set; } = "";
            public string DLState { get; set; } = "";
            public string EmergencyContactName { get; set; } = "";
            public string EmergencyContactRelationship { get; set; } = "";
            public string EmergencyContactAddress { get; set; } = "";

            // Position Information
            public string Position1 { get; set; } = "";
            public string Position2 { get; set; } = "";
            public string SalaryDesired { get; set; } = "";
            public string SalaryType { get; set; } = "";
            public string EmploymentType { get; set; } = "";
            public string AvailableStartDate { get; set; } = "";

            // Location Availability
            public bool NashvilleLocation { get; set; }
            public bool FranklinLocation { get; set; }
            public bool ShelbyvilleLocation { get; set; }
            public bool WaynesboroLocation { get; set; }

            // Shift Availability
            public bool FirstShift { get; set; }
            public bool SecondShift { get; set; }
            public bool ThirdShift { get; set; }
            public bool WeekendsOnly { get; set; }

            // Days Available
            public bool MondayAvailable { get; set; }
            public bool TuesdayAvailable { get; set; }
            public bool WednesdayAvailable { get; set; }
            public bool ThursdayAvailable { get; set; }
            public bool FridayAvailable { get; set; }
            public bool SaturdayAvailable { get; set; }
            public bool SundayAvailable { get; set; }

            // Education
            public string ElementarySchool { get; set; } = "";
            public string HighSchool { get; set; } = "";
            public string UndergraduateSchool { get; set; } = "";
            public string GraduateSchool { get; set; } = "";
            public string HSSkills { get; set; } = "";
            public string UGSkills { get; set; } = "";
            public string GradSkills { get; set; } = "";
            public string SpecialKnowledge { get; set; } = "";

            // Licenses
            public string LicenseType1 { get; set; } = "";
            public string LicenseState1 { get; set; } = "";
            public string LicenseNumber1 { get; set; } = "";
            public string LicenseExpiration1 { get; set; } = "";
            public string LicenseType2 { get; set; } = "";
            public string LicenseState2 { get; set; } = "";
            public string LicenseNumber2 { get; set; } = "";
            public string LicenseExpiration2 { get; set; } = "";
            public string LicenseType3 { get; set; } = "";
            public string LicenseState3 { get; set; } = "";
            public string LicenseNumber3 { get; set; } = "";
            public string LicenseExpiration3 { get; set; } = "";
            public string DIDDTraining { get; set; } = "";

            // Employment History
            public string Employer1 { get; set; } = "";
            public string EmploymentFrom1 { get; set; } = "";
            public string EmploymentTo1 { get; set; } = "";
            public string JobTitle1 { get; set; } = "";
            public string Supervisor1 { get; set; } = "";
            public string EmployerAddress1 { get; set; } = "";
            public string EmployerCityStateZip1 { get; set; } = "";
            public string EmployerPhone1 { get; set; } = "";
            public string HourlyRate1 { get; set; } = "";
            public string ReasonForLeaving1 { get; set; } = "";
            public string MayContact1 { get; set; } = "";

            public string Employer2 { get; set; } = "";
            public string EmploymentFrom2 { get; set; } = "";
            public string EmploymentTo2 { get; set; } = "";
            public string JobTitle2 { get; set; } = "";
            public string Supervisor2 { get; set; } = "";
            public string EmployerAddress2 { get; set; } = "";
            public string EmployerCityStateZip2 { get; set; } = "";
            public string EmployerPhone2 { get; set; } = "";
            public string HourlyRate2 { get; set; } = "";
            public string ReasonForLeaving2 { get; set; } = "";
            public string MayContact2 { get; set; } = "";

            // References
            public string Reference1Name { get; set; } = "";
            public string Reference1Phone { get; set; } = "";
            public string Reference1Email { get; set; } = "";
            public string Reference1Years { get; set; } = "";
            public string Reference2Name { get; set; } = "";
            public string Reference2Phone { get; set; } = "";
            public string Reference2Email { get; set; } = "";
            public string Reference2Years { get; set; } = "";
            public string Reference3Name { get; set; } = "";
            public string Reference3Phone { get; set; } = "";
            public string Reference3Email { get; set; } = "";
            public string Reference3Years { get; set; } = "";

            // Final
            public bool FinalAcknowledgment { get; set; }
            public string SubmissionDate { get; set; } = "";
            public string Status { get; set; } = "";

            // Legacy properties for compatibility
            public string PhoneNumber => HomePhone;
        }
    }
}