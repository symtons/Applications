using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Applications
{
    public partial class NewPaperWork : System.Web.UI.Page
    {
        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
                SetActiveTab("Personal");
            }

            // Always ensure checkboxes are functional on every page load
            EnsureAllControlsFunctional();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // Final check to ensure all controls are functional before rendering
            EnsureAllControlsFunctional();
            RegisterClientScripts();
        }

        #endregion

        #region Initialization

        private void InitializePage()
        {
            try
            {
                // Initialize default values
                hfCurrentTab.Value = "personal";

                // Check if editing existing application
                if (Request.QueryString["ApplicationId"] != null &&
                    int.TryParse(Request.QueryString["ApplicationId"], out int applicationId))
                {
                    hfApplicationId.Value = applicationId.ToString();
                    LoadApplicationData(applicationId);
                }
                else
                {
                    // New application - generate application number
                    GenerateApplicationNumber();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error initializing page: " + ex.Message);
            }
        }

        private void GenerateApplicationNumber()
        {
            string applicationNumber = "APP" + DateTime.Now.ToString("yyyyMMddHHmmss");
            ViewState["ApplicationNumber"] = applicationNumber;
        }

        private void EnsureAllControlsFunctional()
        {
            // This method ensures ALL checkboxes and radio buttons are functional
            // regardless of which tab is currently active

            try
            {
                // Position tab controls (using actual control names from ASPX)
                EnsureControlFunctional(chkNashville);
                EnsureControlFunctional(chkFranklin);
                EnsureControlFunctional(chkShelbyville);
                EnsureControlFunctional(chkWaynesboro);
                EnsureControlFunctional(chkOtherLocation);

                EnsureControlFunctional(chkFirstShift);
                EnsureControlFunctional(chkSecondShift);
                EnsureControlFunctional(chkThirdShift);
                EnsureControlFunctional(chkWeekendsOnly);

                EnsureControlFunctional(chkMonday);
                EnsureControlFunctional(chkTuesday);
                EnsureControlFunctional(chkWednesday);
                EnsureControlFunctional(chkThursday);
                EnsureControlFunctional(chkFriday);
                EnsureControlFunctional(chkSaturday);
                EnsureControlFunctional(chkSunday);

                // Background tab controls
                EnsureControlFunctional(rbAppliedBeforeYes);
                EnsureControlFunctional(rbAppliedBeforeNo);
                EnsureControlFunctional(rbWorkedBeforeYes);
                EnsureControlFunctional(rbWorkedBeforeNo);
                EnsureControlFunctional(rbFamilyEmployedYes);
                EnsureControlFunctional(rbFamilyEmployedNo);

                // Employment type radio buttons
                EnsureControlFunctional(rbFullTime);
                EnsureControlFunctional(rbPartTime);
                EnsureControlFunctional(rbTemporary);

                // Salary type radio buttons
                EnsureControlFunctional(rbHourly);
                EnsureControlFunctional(rbYearly);

                // Education checkboxes (using actual control names)
                EnsureControlFunctional(rbElemDiplomaYes);
                EnsureControlFunctional(rbElemDiplomaNo);
                EnsureControlFunctional(rbHSDiplomaYes);
                EnsureControlFunctional(rbHSDiplomaNo);
                EnsureControlFunctional(rbUGDegreeYes);
                EnsureControlFunctional(rbUGDegreeNo);
                EnsureControlFunctional(rbGradDegreeYes);
                EnsureControlFunctional(rbGradDegreeNo);

                // Year checkboxes for education
                EnsureControlFunctional(chkElem1);
                EnsureControlFunctional(chkElem2);
                EnsureControlFunctional(chkElem3);
                EnsureControlFunctional(chkElem4);
                EnsureControlFunctional(chkElem5);
                EnsureControlFunctional(chkHS9);
                EnsureControlFunctional(chkHS10);
                EnsureControlFunctional(chkHS11);
                EnsureControlFunctional(chkHS12);
                EnsureControlFunctional(chkUG1);
                EnsureControlFunctional(chkUG2);
                EnsureControlFunctional(chkUG3);
                EnsureControlFunctional(chkUG4);
                EnsureControlFunctional(chkGrad1);
                EnsureControlFunctional(chkGrad2);
                EnsureControlFunctional(chkGrad3);
                EnsureControlFunctional(chkGrad4);
                EnsureControlFunctional(chkGrad5);

                // Criminal history and background checkboxes
                EnsureControlFunctional(rbConvicted7YearsYes);
                EnsureControlFunctional(rbConvicted7YearsNo);
                EnsureControlFunctional(rbChargedInvestigationYes);
                EnsureControlFunctional(rbChargedInvestigationNo);
                EnsureControlFunctional(chkDIDDNoAbuse);
                EnsureControlFunctional(chkDIDDHadAbuse);

                // Authorization controls
                EnsureControlFunctional(chkFinalAcknowledgment);
            }
            catch (Exception ex)
            {
                // Log error but don't break the page
                System.Diagnostics.Debug.WriteLine("Error ensuring controls functional: " + ex.Message);
            }
        }

        private void EnsureControlFunctional(WebControl control)
        {
            if (control != null)
            {
                control.Enabled = true;
                control.Visible = true;

                // Add CSS class to force visibility
                if (!control.CssClass.Contains("force-visible"))
                {
                    control.CssClass += " force-visible";
                }
            }
        }

        private void RegisterClientScripts()
        {
            // Register script to ensure checkboxes work after postbacks
            string script = @"
                if (window.NewHireFormControls) {
                    window.NewHireFormControls.initializeAll();
                    window.NewHireFormControls.restoreStates();
                }
            ";

            ScriptManager.RegisterStartupScript(this, GetType(), "EnsureCheckboxes", script, true);
        }

        #endregion

        #region Tab Navigation

        protected void btnTabPersonal_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("Personal");
        }

        protected void btnTabPosition_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("Position");
        }

        protected void btnTabBackground_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("Background");
        }

        protected void btnTabEducation_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("Education");
        }

        protected void btnTabEmployment_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("Employment");
        }

        protected void btnTabReferences_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("References");
        }

        protected void btnTabAuthorization_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("Authorization");
        }

        private void SetActiveTab(string tabName)
        {
            // Save all form data before switching tabs
            SaveAllTabData();

            // Reset all tab buttons
            btnTabPersonal.CssClass = "tab-button";
            btnTabPosition.CssClass = "tab-button";
            btnTabBackground.CssClass = "tab-button";
            btnTabEducation.CssClass = "tab-button";
            btnTabEmployment.CssClass = "tab-button";
            btnTabReferences.CssClass = "tab-button";
            btnTabAuthorization.CssClass = "tab-button";

            // Hide all tab content
            pnlPersonalTab.CssClass = "tab-content";
            pnlPositionTab.CssClass = "tab-content";
            pnlBackgroundTab.CssClass = "tab-content";
            pnlEducationTab.CssClass = "tab-content";
            pnlEmploymentTab.CssClass = "tab-content";
            pnlReferencesTab.CssClass = "tab-content";
            pnlAuthorizationTab.CssClass = "tab-content";

            // Show selected tab and update navigation
            switch (tabName)
            {
                case "Personal":
                    btnTabPersonal.CssClass = "tab-button active";
                    pnlPersonalTab.CssClass = "tab-content active";
                    btnPrevious.Visible = false;
                    btnNext.Visible = true;
                    btnSubmitApplication.Visible = false;
                    break;
                case "Position":
                    btnTabPosition.CssClass = "tab-button active";
                    pnlPositionTab.CssClass = "tab-content active";
                    btnPrevious.Visible = true;
                    btnNext.Visible = true;
                    btnSubmitApplication.Visible = false;
                    break;
                case "Background":
                    btnTabBackground.CssClass = "tab-button active";
                    pnlBackgroundTab.CssClass = "tab-content active";
                    btnPrevious.Visible = true;
                    btnNext.Visible = true;
                    btnSubmitApplication.Visible = false;
                    break;
                case "Education":
                    btnTabEducation.CssClass = "tab-button active";
                    pnlEducationTab.CssClass = "tab-content active";
                    btnPrevious.Visible = true;
                    btnNext.Visible = true;
                    btnSubmitApplication.Visible = false;
                    break;
                case "Employment":
                    btnTabEmployment.CssClass = "tab-button active";
                    pnlEmploymentTab.CssClass = "tab-content active";
                    btnPrevious.Visible = true;
                    btnNext.Visible = true;
                    btnSubmitApplication.Visible = false;
                    break;
                case "References":
                    btnTabReferences.CssClass = "tab-button active";
                    pnlReferencesTab.CssClass = "tab-content active";
                    btnPrevious.Visible = true;
                    btnNext.Visible = true;
                    btnSubmitApplication.Visible = false;
                    break;
                case "Authorization":
                    btnTabAuthorization.CssClass = "tab-button active";
                    pnlAuthorizationTab.CssClass = "tab-content active";
                    btnPrevious.Visible = true;
                    btnNext.Visible = false;
                    btnSubmitApplication.Visible = true;
                    break;
            }

            hfCurrentTab.Value = tabName.ToLower();

            // Ensure all controls remain functional after tab change
            EnsureAllControlsFunctional();

            upMain.Update();
        }

        #endregion

        #region Navigation Buttons

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();

            string currentTab = hfCurrentTab.Value;
            switch (currentTab)
            {
                case "position":
                    SetActiveTab("Personal");
                    break;
                case "background":
                    SetActiveTab("Position");
                    break;
                case "education":
                    SetActiveTab("Background");
                    break;
                case "employment":
                    SetActiveTab("Education");
                    break;
                case "references":
                    SetActiveTab("Employment");
                    break;
                case "authorization":
                    SetActiveTab("References");
                    break;
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();

            string currentTab = hfCurrentTab.Value;
            switch (currentTab)
            {
                case "personal":
                    SetActiveTab("Position");
                    break;
                case "position":
                    SetActiveTab("Background");
                    break;
                case "background":
                    SetActiveTab("Education");
                    break;
                case "education":
                    SetActiveTab("Employment");
                    break;
                case "employment":
                    SetActiveTab("References");
                    break;
                case "references":
                    SetActiveTab("Authorization");
                    break;
            }
        }

        #endregion

        #region Save and Submit

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            try
            {
                SaveApplicationData(false);
                ShowSuccessMessage("Application draft saved successfully!");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error saving draft: " + ex.Message);
            }
        }

        protected void btnSubmitApplication_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateRequiredFields())
                {
                    ShowErrorMessage("Please complete all required fields and acknowledge the final statement before submitting.");
                    return;
                }

                SaveApplicationData(true);
                ShowSuccessMessage("Application submitted successfully! You will be redirected to your dashboard.");

                // Redirect after a brief delay
                ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                    "setTimeout(function(){ window.location = 'EmployeeDashboard.aspx'; }, 2000);", true);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error submitting application: " + ex.Message);
            }
        }

        #endregion

        #region Data Operations

        private void SaveCurrentTabData()
        {
            try
            {
                string currentTab = hfCurrentTab.Value?.ToLower() ?? "personal";

                switch (currentTab)
                {
                    case "personal":
                        SavePersonalTabData();
                        break;
                    case "position":
                        SavePositionTabData();
                        break;
                    case "background":
                        SaveBackgroundTabData();
                        break;
                    case "education":
                        SaveEducationTabData();
                        break;
                    case "employment":
                        SaveEmploymentTabData();
                        break;
                    case "references":
                        SaveReferencesTabData();
                        break;
                    case "authorization":
                        SaveAuthorizationTabData();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error saving tab data: " + ex.Message);
            }
        }

        private void SaveAllTabData()
        {
            // Save data from all tabs to preserve checkbox states
            try
            {
                SavePersonalTabData();
                SavePositionTabData();
                SaveBackgroundTabData();
                SaveEducationTabData();
                SaveEmploymentTabData();
                SaveReferencesTabData();
                SaveAuthorizationTabData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error saving all tab data: " + ex.Message);
            }
        }

        private void SavePersonalTabData()
        {
            ViewState["PersonalData"] = new
            {
                FirstName = GetTextBoxValue(txtFirstName),
                MiddleName = GetTextBoxValue(txtMiddleName),
                LastName = GetTextBoxValue(txtLastName),
                HomeAddress = GetTextBoxValue(txtHomeAddress),
                AptNumber = GetTextBoxValue(txtAptNumber),
                City = GetTextBoxValue(txtCity),
                State = GetTextBoxValue(txtState),
                ZipCode = GetTextBoxValue(txtZipCode),
                SSN = GetTextBoxValue(txtSSN),
                DriversLicense = GetTextBoxValue(txtDriversLicense),
                DLState = GetTextBoxValue(txtDLState),
                PhoneNumber = GetTextBoxValue(txtPhoneNumber),
                CellNumber = GetTextBoxValue(txtCellNumber),
                EmergencyContactName = GetTextBoxValue(txtEmergencyContactName),
                EmergencyContactRelationship = GetTextBoxValue(txtEmergencyContactRelationship),
                EmergencyContactAddress = GetTextBoxValue(txtEmergencyContactAddress)
            };
        }

        private void SavePositionTabData()
        {
            ViewState["PositionData"] = new
            {
                Position1 = GetTextBoxValue(txtPosition1),
                Position2 = GetTextBoxValue(txtPosition2),
                SalaryDesired = GetTextBoxValue(txtSalaryDesired),
                SalaryType = GetRadioButtonValue(rbHourly) ? "Hourly" : (GetRadioButtonValue(rbYearly) ? "Yearly" : ""),
                AvailableStartDate = GetTextBoxValue(txtAvailableStartDate),
                EmploymentSought = GetRadioButtonValue(rbFullTime) ? "Full Time" : (GetRadioButtonValue(rbPartTime) ? "Part Time" : (GetRadioButtonValue(rbTemporary) ? "Temporary" : "")),
                // Always preserve checkbox values
                NashvilleLocation = GetCheckBoxValue(chkNashville),
                FranklinLocation = GetCheckBoxValue(chkFranklin),
                ShelbyvilleLocation = GetCheckBoxValue(chkShelbyville),
                WaynesboroLocation = GetCheckBoxValue(chkWaynesboro),
                OtherLocation = GetCheckBoxValue(chkOtherLocation),
                FirstShift = GetCheckBoxValue(chkFirstShift),
                SecondShift = GetCheckBoxValue(chkSecondShift),
                ThirdShift = GetCheckBoxValue(chkThirdShift),
                WeekendsOnly = GetCheckBoxValue(chkWeekendsOnly),
                MondayAvailable = GetCheckBoxValue(chkMonday),
                TuesdayAvailable = GetCheckBoxValue(chkTuesday),
                WednesdayAvailable = GetCheckBoxValue(chkWednesday),
                ThursdayAvailable = GetCheckBoxValue(chkThursday),
                FridayAvailable = GetCheckBoxValue(chkFriday),
                SaturdayAvailable = GetCheckBoxValue(chkSaturday),
                SundayAvailable = GetCheckBoxValue(chkSunday)
            };
        }

        private void SaveBackgroundTabData()
        {
            ViewState["BackgroundData"] = new
            {
                PreviouslyAppliedToTPA = GetRadioButtonValue(rbAppliedBeforeYes),
                PreviousApplicationDate = GetTextBoxValue(txtAppliedBeforeWhen),
                PreviouslyWorkedForTPA = GetRadioButtonValue(rbWorkedBeforeYes),
                PreviousWorkDate = GetTextBoxValue(txtWorkedBeforeWhen),
                FamilyMembersEmployedByTPA = GetRadioButtonValue(rbFamilyEmployedYes),
                FamilyMemberDetails = GetTextBoxValue(txtFamilyEmployedWho),
                ConvictedCriminal7Years = GetRadioButtonValue(rbConvicted7YearsYes),
                ChargedInvestigation = GetRadioButtonValue(rbChargedInvestigationYes),
                DIDDNoAbuse = GetCheckBoxValue(chkDIDDNoAbuse),
                DIDDHadAbuse = GetCheckBoxValue(chkDIDDHadAbuse)
            };
        }

        private void SaveEducationTabData()
        {
            ViewState["EducationData"] = new
            {
                ElementarySchool = GetTextBoxValue(txtElementarySchool),
                HighSchool = GetTextBoxValue(txtHighSchool),
                UndergraduateSchool = GetTextBoxValue(txtUndergraduateSchool),
                GraduateSchool = GetTextBoxValue(txtGraduateSchool),
                ElementaryDiploma = GetRadioButtonValue(rbElemDiplomaYes),
                HighSchoolDiploma = GetRadioButtonValue(rbHSDiplomaYes),
                UndergraduateDegree = GetRadioButtonValue(rbUGDegreeYes),
                GraduateDegree = GetRadioButtonValue(rbGradDegreeYes),
                // Year checkboxes
                Elem1 = GetCheckBoxValue(chkElem1),
                Elem2 = GetCheckBoxValue(chkElem2),
                Elem3 = GetCheckBoxValue(chkElem3),
                Elem4 = GetCheckBoxValue(chkElem4),
                Elem5 = GetCheckBoxValue(chkElem5),
                HS9 = GetCheckBoxValue(chkHS9),
                HS10 = GetCheckBoxValue(chkHS10),
                HS11 = GetCheckBoxValue(chkHS11),
                HS12 = GetCheckBoxValue(chkHS12),
                UG1 = GetCheckBoxValue(chkUG1),
                UG2 = GetCheckBoxValue(chkUG2),
                UG3 = GetCheckBoxValue(chkUG3),
                UG4 = GetCheckBoxValue(chkUG4),
                Grad1 = GetCheckBoxValue(chkGrad1),
                Grad2 = GetCheckBoxValue(chkGrad2),
                Grad3 = GetCheckBoxValue(chkGrad3),
                Grad4 = GetCheckBoxValue(chkGrad4),
                Grad5 = GetCheckBoxValue(chkGrad5)
            };
        }

        private void SaveEmploymentTabData()
        {
            ViewState["EmploymentData"] = new
            {
                // Employment history will be handled by the existing controls in the ASPX
                // This method can be expanded when those controls are identified
            };
        }

        private void SaveReferencesTabData()
        {
            ViewState["ReferencesData"] = new
            {
                // References will be handled by the existing controls in the ASPX
                // This method can be expanded when those controls are identified
            };
        }

        private void SaveAuthorizationTabData()
        {
            ViewState["AuthorizationData"] = new
            {
                FinalAcknowledgment = GetCheckBoxValue(chkFinalAcknowledgment)
            };
        }

        // Helper methods for safe value extraction
        private string GetTextBoxValue(TextBox textBox)
        {
            return textBox?.Text ?? "";
        }

        private bool GetCheckBoxValue(CheckBox checkBox)
        {
            return checkBox?.Checked ?? false;
        }

        private bool GetRadioButtonValue(RadioButton radioButton)
        {
            return radioButton?.Checked ?? false;
        }

        private void SaveApplicationData(bool isSubmitted)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TPAHRConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Save all tab data before database operations
                        SaveAllTabData();

                        // Save main application record
                        int applicationId = SaveMainApplicationRecord(connection, transaction, isSubmitted);

                        // Save related data (when controls are identified)
                        // SaveEducationData(connection, transaction, applicationId);
                        // SaveEmploymentHistoryData(connection, transaction, applicationId);
                        // SaveReferencesData(connection, transaction, applicationId);

                        transaction.Commit();

                        if (string.IsNullOrEmpty(hfApplicationId.Value))
                        {
                            hfApplicationId.Value = applicationId.ToString();
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private int SaveMainApplicationRecord(SqlConnection connection, SqlTransaction transaction, bool isSubmitted)
        {
            string status = isSubmitted ? "Submitted" : "Draft";
            bool isUpdate = !string.IsNullOrEmpty(hfApplicationId.Value);

            string sql;
            if (isUpdate)
            {
                sql = @"
                    UPDATE [EmploymentApplications] SET
                        [Status] = @Status,
                        [FirstName] = @FirstName, [MiddleName] = @MiddleName, [LastName] = @LastName,
                        [HomeAddress] = @HomeAddress, [AptNumber] = @AptNumber, [City] = @City, [State] = @State, [ZipCode] = @ZipCode,
                        [SSN] = @SSN, [DriversLicense] = @DriversLicense, [DLState] = @DLState,
                        [PhoneNumber] = @PhoneNumber, [CellNumber] = @CellNumber,
                        [EmergencyContactName] = @EmergencyContactName, [EmergencyContactRelationship] = @EmergencyContactRelationship,
                        [EmergencyContactAddress] = @EmergencyContactAddress,
                        [Position1] = @Position1, [Position2] = @Position2, [SalaryDesired] = @SalaryDesired,
                        [SalaryType] = @SalaryType, [EmploymentSought] = @EmploymentSought, [AvailableStartDate] = @AvailableStartDate,
                        [NashvilleLocation] = @NashvilleLocation, [FranklinLocation] = @FranklinLocation,
                        [ShelbyvilleLocation] = @ShelbyvilleLocation, [WaynesboroLocation] = @WaynesboroLocation, [OtherLocation] = @OtherLocation,
                        [FirstShift] = @FirstShift, [SecondShift] = @SecondShift, [ThirdShift] = @ThirdShift, [WeekendsOnly] = @WeekendsOnly,
                        [MondayAvailable] = @MondayAvailable, [TuesdayAvailable] = @TuesdayAvailable, [WednesdayAvailable] = @WednesdayAvailable,
                        [ThursdayAvailable] = @ThursdayAvailable, [FridayAvailable] = @FridayAvailable, [SaturdayAvailable] = @SaturdayAvailable, [SundayAvailable] = @SundayAvailable,
                        [PreviouslyAppliedToTPA] = @PreviouslyAppliedToTPA, [PreviousApplicationDate] = @PreviousApplicationDate,
                        [PreviouslyWorkedForTPA] = @PreviouslyWorkedForTPA, [PreviousWorkDate] = @PreviousWorkDate,
                        [FamilyMembersEmployedByTPA] = @FamilyMembersEmployedByTPA, [FamilyMemberDetails] = @FamilyMemberDetails,
                        [ConvictedCriminal7Years] = @ConvictedCriminal7Years, [ChargedInvestigation] = @ChargedInvestigation,
                        [DIDDNoAbuse] = @DIDDNoAbuse, [DIDDHadAbuse] = @DIDDHadAbuse,
                        [FinalAcknowledgment] = @FinalAcknowledgment,
                        [UpdatedAt] = @UpdatedAt" + (isSubmitted ? ", [SubmittedAt] = @SubmittedAt" : "") + @"
                    WHERE [Id] = @ApplicationId";
            }
            else
            {
                sql = @"
                    INSERT INTO [EmploymentApplications] (
                        [ApplicationNumber], [ApplicationDate], [Status],
                        [FirstName], [MiddleName], [LastName], [HomeAddress], [AptNumber], [City], [State], [ZipCode],
                        [SSN], [DriversLicense], [DLState], [PhoneNumber], [CellNumber],
                        [EmergencyContactName], [EmergencyContactRelationship], [EmergencyContactAddress],
                        [Position1], [Position2], [SalaryDesired], [SalaryType], [EmploymentSought], [AvailableStartDate],
                        [NashvilleLocation], [FranklinLocation], [ShelbyvilleLocation], [WaynesboroLocation], [OtherLocation],
                        [FirstShift], [SecondShift], [ThirdShift], [WeekendsOnly],
                        [MondayAvailable], [TuesdayAvailable], [WednesdayAvailable], [ThursdayAvailable],
                        [FridayAvailable], [SaturdayAvailable], [SundayAvailable],
                        [PreviouslyAppliedToTPA], [PreviousApplicationDate], [PreviouslyWorkedForTPA], [PreviousWorkDate],
                        [FamilyMembersEmployedByTPA], [FamilyMemberDetails],
                        [ConvictedCriminal7Years], [ChargedInvestigation], [DIDDNoAbuse], [DIDDHadAbuse],
                        [FinalAcknowledgment],
                        [CreatedAt], [UpdatedAt]" + (isSubmitted ? ", [SubmittedAt]" : "") + @"
                    )
                    VALUES (
                        @ApplicationNumber, @ApplicationDate, @Status,
                        @FirstName, @MiddleName, @LastName, @HomeAddress, @AptNumber, @City, @State, @ZipCode,
                        @SSN, @DriversLicense, @DLState, @PhoneNumber, @CellNumber,
                        @EmergencyContactName, @EmergencyContactRelationship, @EmergencyContactAddress,
                        @Position1, @Position2, @SalaryDesired, @SalaryType, @EmploymentSought, @AvailableStartDate,
                        @NashvilleLocation, @FranklinLocation, @ShelbyvilleLocation, @WaynesboroLocation, @OtherLocation,
                        @FirstShift, @SecondShift, @ThirdShift, @WeekendsOnly,
                        @MondayAvailable, @TuesdayAvailable, @WednesdayAvailable, @ThursdayAvailable,
                        @FridayAvailable, @SaturdayAvailable, @SundayAvailable,
                        @PreviouslyAppliedToTPA, @PreviousApplicationDate, @PreviouslyWorkedForTPA, @PreviousWorkDate,
                        @FamilyMembersEmployedByTPA, @FamilyMemberDetails,
                        @ConvictedCriminal7Years, @ChargedInvestigation, @DIDDNoAbuse, @DIDDHadAbuse,
                        @FinalAcknowledgment,
                        @CreatedAt, @UpdatedAt" + (isSubmitted ? ", @SubmittedAt" : "") + @"
                    ); SELECT SCOPE_IDENTITY();";
            }

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                // Add parameters
                if (isUpdate)
                {
                    command.Parameters.AddWithValue("@ApplicationId", int.Parse(hfApplicationId.Value));
                }
                else
                {
                    command.Parameters.AddWithValue("@ApplicationNumber", ViewState["ApplicationNumber"]?.ToString() ?? "");
                    command.Parameters.AddWithValue("@ApplicationDate", DateTime.Now);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                }

                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                if (isSubmitted)
                {
                    command.Parameters.AddWithValue("@SubmittedAt", DateTime.Now);
                }

                // Personal Information
                command.Parameters.AddWithValue("@FirstName", GetTextBoxValue(txtFirstName));
                command.Parameters.AddWithValue("@MiddleName", GetTextBoxValue(txtMiddleName));
                command.Parameters.AddWithValue("@LastName", GetTextBoxValue(txtLastName));
                command.Parameters.AddWithValue("@HomeAddress", GetTextBoxValue(txtHomeAddress));
                command.Parameters.AddWithValue("@AptNumber", GetTextBoxValue(txtAptNumber));
                command.Parameters.AddWithValue("@City", GetTextBoxValue(txtCity));
                command.Parameters.AddWithValue("@State", GetTextBoxValue(txtState));
                command.Parameters.AddWithValue("@ZipCode", GetTextBoxValue(txtZipCode));
                command.Parameters.AddWithValue("@SSN", GetTextBoxValue(txtSSN));
                command.Parameters.AddWithValue("@DriversLicense", GetTextBoxValue(txtDriversLicense));
                command.Parameters.AddWithValue("@DLState", GetTextBoxValue(txtDLState));
                command.Parameters.AddWithValue("@PhoneNumber", GetTextBoxValue(txtPhoneNumber));
                command.Parameters.AddWithValue("@CellNumber", GetTextBoxValue(txtCellNumber));
                command.Parameters.AddWithValue("@EmergencyContactName", GetTextBoxValue(txtEmergencyContactName));
                command.Parameters.AddWithValue("@EmergencyContactRelationship", GetTextBoxValue(txtEmergencyContactRelationship));
                command.Parameters.AddWithValue("@EmergencyContactAddress", GetTextBoxValue(txtEmergencyContactAddress));

                // Position Information
                command.Parameters.AddWithValue("@Position1", GetTextBoxValue(txtPosition1));
                command.Parameters.AddWithValue("@Position2", GetTextBoxValue(txtPosition2));

                decimal salaryDesired = 0;
                decimal.TryParse(GetTextBoxValue(txtSalaryDesired), out salaryDesired);
                command.Parameters.AddWithValue("@SalaryDesired", salaryDesired);

                command.Parameters.AddWithValue("@SalaryType", GetRadioButtonValue(rbHourly) ? "Hourly" : (GetRadioButtonValue(rbYearly) ? "Yearly" : ""));
                command.Parameters.AddWithValue("@EmploymentSought", GetRadioButtonValue(rbFullTime) ? "Full Time" : (GetRadioButtonValue(rbPartTime) ? "Part Time" : (GetRadioButtonValue(rbTemporary) ? "Temporary" : "")));

                DateTime availableStartDate;
                if (DateTime.TryParse(GetTextBoxValue(txtAvailableStartDate), out availableStartDate))
                {
                    command.Parameters.AddWithValue("@AvailableStartDate", availableStartDate);
                }
                else
                {
                    command.Parameters.AddWithValue("@AvailableStartDate", DBNull.Value);
                }

                // Location Checkboxes - Always save current checkbox states
                command.Parameters.AddWithValue("@NashvilleLocation", GetCheckBoxValue(chkNashville));
                command.Parameters.AddWithValue("@FranklinLocation", GetCheckBoxValue(chkFranklin));
                command.Parameters.AddWithValue("@ShelbyvilleLocation", GetCheckBoxValue(chkShelbyville));
                command.Parameters.AddWithValue("@WaynesboroLocation", GetCheckBoxValue(chkWaynesboro));
                command.Parameters.AddWithValue("@OtherLocation", GetCheckBoxValue(chkOtherLocation));

                // Shift Checkboxes
                command.Parameters.AddWithValue("@FirstShift", GetCheckBoxValue(chkFirstShift));
                command.Parameters.AddWithValue("@SecondShift", GetCheckBoxValue(chkSecondShift));
                command.Parameters.AddWithValue("@ThirdShift", GetCheckBoxValue(chkThirdShift));
                command.Parameters.AddWithValue("@WeekendsOnly", GetCheckBoxValue(chkWeekendsOnly));

                // Days Available Checkboxes
                command.Parameters.AddWithValue("@MondayAvailable", GetCheckBoxValue(chkMonday));
                command.Parameters.AddWithValue("@TuesdayAvailable", GetCheckBoxValue(chkTuesday));
                command.Parameters.AddWithValue("@WednesdayAvailable", GetCheckBoxValue(chkWednesday));
                command.Parameters.AddWithValue("@ThursdayAvailable", GetCheckBoxValue(chkThursday));
                command.Parameters.AddWithValue("@FridayAvailable", GetCheckBoxValue(chkFriday));
                command.Parameters.AddWithValue("@SaturdayAvailable", GetCheckBoxValue(chkSaturday));
                command.Parameters.AddWithValue("@SundayAvailable", GetCheckBoxValue(chkSunday));

                // Background Information
                command.Parameters.AddWithValue("@PreviouslyAppliedToTPA", GetRadioButtonValue(rbAppliedBeforeYes));
                command.Parameters.AddWithValue("@PreviousApplicationDate", GetTextBoxValue(txtAppliedBeforeWhen));
                command.Parameters.AddWithValue("@PreviouslyWorkedForTPA", GetRadioButtonValue(rbWorkedBeforeYes));
                command.Parameters.AddWithValue("@PreviousWorkDate", GetTextBoxValue(txtWorkedBeforeWhen));
                command.Parameters.AddWithValue("@FamilyMembersEmployedByTPA", GetRadioButtonValue(rbFamilyEmployedYes));
                command.Parameters.AddWithValue("@FamilyMemberDetails", GetTextBoxValue(txtFamilyEmployedWho));

                // Criminal/Background checks
                command.Parameters.AddWithValue("@ConvictedCriminal7Years", GetRadioButtonValue(rbConvicted7YearsYes));
                command.Parameters.AddWithValue("@ChargedInvestigation", GetRadioButtonValue(rbChargedInvestigationYes));
                command.Parameters.AddWithValue("@DIDDNoAbuse", GetCheckBoxValue(chkDIDDNoAbuse));
                command.Parameters.AddWithValue("@DIDDHadAbuse", GetCheckBoxValue(chkDIDDHadAbuse));

                // Authorization
                command.Parameters.AddWithValue("@FinalAcknowledgment", GetCheckBoxValue(chkFinalAcknowledgment));

                if (isUpdate)
                {
                    command.ExecuteNonQuery();
                    return int.Parse(hfApplicationId.Value);
                }
                else
                {
                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        private void LoadApplicationData(int applicationId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TPAHRConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Load main application data
                string sql = "SELECT * FROM [EmploymentApplications] WHERE [Id] = @ApplicationId";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationId", applicationId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Load personal information
                            SetTextBoxValue(txtFirstName, reader["FirstName"]);
                            SetTextBoxValue(txtMiddleName, reader["MiddleName"]);
                            SetTextBoxValue(txtLastName, reader["LastName"]);
                            SetTextBoxValue(txtHomeAddress, reader["HomeAddress"]);
                            SetTextBoxValue(txtAptNumber, reader["AptNumber"]);
                            SetTextBoxValue(txtCity, reader["City"]);
                            SetTextBoxValue(txtState, reader["State"]);
                            SetTextBoxValue(txtZipCode, reader["ZipCode"]);

                            // Load position information with all checkbox states
                            SetTextBoxValue(txtPosition1, reader["Position1"]);
                            SetTextBoxValue(txtPosition2, reader["Position2"]);
                            SetTextBoxValue(txtSalaryDesired, reader["SalaryDesired"]);

                            // Load all checkbox values - these will work on all tabs
                            SetCheckBoxValue(chkNashville, reader["NashvilleLocation"]);
                            SetCheckBoxValue(chkFranklin, reader["FranklinLocation"]);
                            SetCheckBoxValue(chkShelbyville, reader["ShelbyvilleLocation"]);
                            SetCheckBoxValue(chkWaynesboro, reader["WaynesboroLocation"]);
                            SetCheckBoxValue(chkOtherLocation, reader["OtherLocation"]);

                            SetCheckBoxValue(chkFirstShift, reader["FirstShift"]);
                            SetCheckBoxValue(chkSecondShift, reader["SecondShift"]);
                            SetCheckBoxValue(chkThirdShift, reader["ThirdShift"]);
                            SetCheckBoxValue(chkWeekendsOnly, reader["WeekendsOnly"]);

                            SetCheckBoxValue(chkMonday, reader["MondayAvailable"]);
                            SetCheckBoxValue(chkTuesday, reader["TuesdayAvailable"]);
                            SetCheckBoxValue(chkWednesday, reader["WednesdayAvailable"]);
                            SetCheckBoxValue(chkThursday, reader["ThursdayAvailable"]);
                            SetCheckBoxValue(chkFriday, reader["FridayAvailable"]);
                            SetCheckBoxValue(chkSaturday, reader["SaturdayAvailable"]);
                            SetCheckBoxValue(chkSunday, reader["SundayAvailable"]);

                            // Load background information
                            SetRadioButtonValue(rbAppliedBeforeYes, rbAppliedBeforeNo, reader["PreviouslyAppliedToTPA"]);
                            SetTextBoxValue(txtAppliedBeforeWhen, reader["PreviousApplicationDate"]);
                            SetRadioButtonValue(rbWorkedBeforeYes, rbWorkedBeforeNo, reader["PreviouslyWorkedForTPA"]);
                            SetTextBoxValue(txtWorkedBeforeWhen, reader["PreviousWorkDate"]);
                            SetRadioButtonValue(rbFamilyEmployedYes, rbFamilyEmployedNo, reader["FamilyMembersEmployedByTPA"]);
                            SetTextBoxValue(txtFamilyEmployedWho, reader["FamilyMemberDetails"]);

                            // Load criminal/background data
                            SetRadioButtonValue(rbConvicted7YearsYes, rbConvicted7YearsNo, reader["ConvictedCriminal7Years"]);
                            SetRadioButtonValue(rbChargedInvestigationYes, rbChargedInvestigationNo, reader["ChargedInvestigation"]);
                            SetCheckBoxValue(chkDIDDNoAbuse, reader["DIDDNoAbuse"]);
                            SetCheckBoxValue(chkDIDDHadAbuse, reader["DIDDHadAbuse"]);

                            // Load authorization
                            SetCheckBoxValue(chkFinalAcknowledgment, reader["FinalAcknowledgment"]);
                        }
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        private void SetTextBoxValue(TextBox textBox, object value)
        {
            if (textBox != null && value != null && value != DBNull.Value)
            {
                textBox.Text = value.ToString();
            }
        }

        private void SetCheckBoxValue(CheckBox checkBox, object value)
        {
            if (checkBox != null && value != null && value != DBNull.Value)
            {
                checkBox.Checked = Convert.ToBoolean(value);
            }
        }

        private void SetRadioButtonValue(RadioButton yesButton, RadioButton noButton, object value)
        {
            if (yesButton != null && noButton != null && value != null && value != DBNull.Value)
            {
                bool boolValue = Convert.ToBoolean(value);
                yesButton.Checked = boolValue;
                noButton.Checked = !boolValue;
            }
        }

        private bool ValidateRequiredFields()
        {
            bool isValid = true;

            // Validate required personal information
            if (string.IsNullOrWhiteSpace(GetTextBoxValue(txtFirstName)))
                isValid = false;
            if (string.IsNullOrWhiteSpace(GetTextBoxValue(txtLastName)))
                isValid = false;

            // Validate final acknowledgment
            if (!GetCheckBoxValue(chkFinalAcknowledgment))
                isValid = false;

            return isValid;
        }

        private void ShowSuccessMessage(string message)
        {
            pnlMessages.Visible = true;
            pnlMessages.CssClass = "message-panel success";
            lblMessage.Text = message;
            lblMessage.CssClass = "message-text";
        }

        private void ShowErrorMessage(string message)
        {
            pnlMessages.Visible = true;
            pnlMessages.CssClass = "message-panel error";
            lblMessage.Text = message;
            lblMessage.CssClass = "message-text";
        }

        // =====================================================================
        // ONLY THE SECTIONS TO CHANGE IN NewPaperWork.aspx.cs
        // Replace these specific methods with the corrected versions
        // =====================================================================

        // 1. REPLACE THE LoadEducationInformation METHOD (around line 496-520)
        private void LoadEducationInformation(SqlDataReader reader)
        {
            // School names
            SetTextBoxValue(txtElementarySchool, reader["ElementarySchool"]);
            SetTextBoxValue(txtHighSchool, reader["HighSchool"]);
            SetTextBoxValue(txtUndergraduateSchool, reader["UndergraduateSchool"]);
            SetTextBoxValue(txtGraduateSchool, reader["GraduateSchool"]);

            // Elementary year checkboxes
            SetCheckBoxValue(chkElem1, reader["Elem1"]);
            SetCheckBoxValue(chkElem2, reader["Elem2"]);
            SetCheckBoxValue(chkElem3, reader["Elem3"]);
            SetCheckBoxValue(chkElem4, reader["Elem4"]);
            SetCheckBoxValue(chkElem5, reader["Elem5"]);

            // High school year checkboxes
            SetCheckBoxValue(chkHS9, reader["HS9"]);
            SetCheckBoxValue(chkHS10, reader["HS10"]);
            SetCheckBoxValue(chkHS11, reader["HS11"]);
            SetCheckBoxValue(chkHS12, reader["HS12"]);

            // Undergraduate year checkboxes
            SetCheckBoxValue(chkUG1, reader["UG1"]);
            SetCheckBoxValue(chkUG2, reader["UG2"]);
            SetCheckBoxValue(chkUG3, reader["UG3"]);
            SetCheckBoxValue(chkUG4, reader["UG4"]);
            SetCheckBoxValue(chkUG5, reader["UG5"]);

            // Graduate year checkboxes
            SetCheckBoxValue(chkGrad1, reader["Grad1"]);
            SetCheckBoxValue(chkGrad2, reader["Grad2"]);
            SetCheckBoxValue(chkGrad3, reader["Grad3"]);
            SetCheckBoxValue(chkGrad4, reader["Grad4"]);
            SetCheckBoxValue(chkGrad5, reader["Grad5"]);

            // Diploma/Degree radio buttons
            SetRadioButtonValue(rbElemDiplomaYes, rbElemDiplomaNo, reader["ElemDiploma"]);
            SetRadioButtonValue(rbHSDiplomaYes, rbHSDiplomaNo, reader["HSDiploma"]);
            SetRadioButtonValue(rbUGDegreeYes, rbUGDegreeNo, reader["UGDegree"]);
            SetRadioButtonValue(rbGradDegreeYes, rbGradDegreeNo, reader["GradDegree"]);

            // Skills and special knowledge
            SetTextBoxValue(txtUGSkills, reader["UGSkills"]);
            SetTextBoxValue(txtGradSkills, reader["GradSkills"]);
            SetTextBoxValue(txtSpecialKnowledge, reader["SpecialKnowledge"]);

            // Licenses
            SetTextBoxValue(txtLicenseType1, reader["LicenseType1"]);
            SetTextBoxValue(txtLicenseState1, reader["LicenseState1"]);
            SetTextBoxValue(txtLicenseNumber1, reader["LicenseNumber1"]);
            SetTextBoxValue(txtLicenseExpiration1, reader["LicenseExpiration1"]);

            SetTextBoxValue(txtLicenseType2, reader["LicenseType2"]);
            SetTextBoxValue(txtLicenseState2, reader["LicenseState2"]);
            SetTextBoxValue(txtLicenseNumber2, reader["LicenseNumber2"]);
            SetTextBoxValue(txtLicenseExpiration2, reader["LicenseExpiration2"]);
        }

        // 2. REPLACE THE SaveEducationInformation METHOD (around line 808-840)
        private void SaveEducationInformation(SqlConnection connection, SqlTransaction transaction, int applicationId)
        {
            string query = @"
        UPDATE JobApplications 
        SET ElementarySchool = @ElementarySchool,
            HighSchool = @HighSchool,
            UndergraduateSchool = @UndergraduateSchool,
            GraduateSchool = @GraduateSchool,
            Elem1 = @Elem1,
            Elem2 = @Elem2,
            Elem3 = @Elem3,
            Elem4 = @Elem4,
            Elem5 = @Elem5,
            HS9 = @HS9,
            HS10 = @HS10,
            HS11 = @HS11,
            HS12 = @HS12,
            UG1 = @UG1,
            UG2 = @UG2,
            UG3 = @UG3,
            UG4 = @UG4,
            UG5 = @UG5,
            Grad1 = @Grad1,
            Grad2 = @Grad2,
            Grad3 = @Grad3,
            Grad4 = @Grad4,
            Grad5 = @Grad5,
            ElemDiploma = @ElemDiploma,
            HSDiploma = @HSDiploma,
            UGDegree = @UGDegree,
            GradDegree = @GradDegree,
            UGSkills = @UGSkills,
            GradSkills = @GradSkills,
            SpecialKnowledge = @SpecialKnowledge,
            LicenseType1 = @LicenseType1,
            LicenseState1 = @LicenseState1,
            LicenseNumber1 = @LicenseNumber1,
            LicenseExpiration1 = @LicenseExpiration1,
            LicenseType2 = @LicenseType2,
            LicenseState2 = @LicenseState2,
            LicenseNumber2 = @LicenseNumber2,
            LicenseExpiration2 = @LicenseExpiration2
        WHERE ApplicationId = @ApplicationId";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@ApplicationId", applicationId);
                command.Parameters.AddWithValue("@ElementarySchool", GetTextBoxValue(txtElementarySchool));
                command.Parameters.AddWithValue("@HighSchool", GetTextBoxValue(txtHighSchool));
                command.Parameters.AddWithValue("@UndergraduateSchool", GetTextBoxValue(txtUndergraduateSchool));
                command.Parameters.AddWithValue("@GraduateSchool", GetTextBoxValue(txtGraduateSchool));

                // Elementary year checkboxes
                command.Parameters.AddWithValue("@Elem1", GetCheckBoxValue(chkElem1));
                command.Parameters.AddWithValue("@Elem2", GetCheckBoxValue(chkElem2));
                command.Parameters.AddWithValue("@Elem3", GetCheckBoxValue(chkElem3));
                command.Parameters.AddWithValue("@Elem4", GetCheckBoxValue(chkElem4));
                command.Parameters.AddWithValue("@Elem5", GetCheckBoxValue(chkElem5));

                // High school year checkboxes
                command.Parameters.AddWithValue("@HS9", GetCheckBoxValue(chkHS9));
                command.Parameters.AddWithValue("@HS10", GetCheckBoxValue(chkHS10));
                command.Parameters.AddWithValue("@HS11", GetCheckBoxValue(chkHS11));
                command.Parameters.AddWithValue("@HS12", GetCheckBoxValue(chkHS12));

                // Undergraduate year checkboxes
                command.Parameters.AddWithValue("@UG1", GetCheckBoxValue(chkUG1));
                command.Parameters.AddWithValue("@UG2", GetCheckBoxValue(chkUG2));
                command.Parameters.AddWithValue("@UG3", GetCheckBoxValue(chkUG3));
                command.Parameters.AddWithValue("@UG4", GetCheckBoxValue(chkUG4));
                command.Parameters.AddWithValue("@UG5", GetCheckBoxValue(chkUG5));

                // Graduate year checkboxes
                command.Parameters.AddWithValue("@Grad1", GetCheckBoxValue(chkGrad1));
                command.Parameters.AddWithValue("@Grad2", GetCheckBoxValue(chkGrad2));
                command.Parameters.AddWithValue("@Grad3", GetCheckBoxValue(chkGrad3));
                command.Parameters.AddWithValue("@Grad4", GetCheckBoxValue(chkGrad4));
                command.Parameters.AddWithValue("@Grad5", GetCheckBoxValue(chkGrad5));

                // Diploma/Degree radio buttons
                command.Parameters.AddWithValue("@ElemDiploma", GetRadioButtonValue(rbElemDiplomaYes));
                command.Parameters.AddWithValue("@HSDiploma", GetRadioButtonValue(rbHSDiplomaYes));
                command.Parameters.AddWithValue("@UGDegree", GetRadioButtonValue(rbUGDegreeYes));
                command.Parameters.AddWithValue("@GradDegree", GetRadioButtonValue(rbGradDegreeYes));

                command.Parameters.AddWithValue("@UGSkills", GetTextBoxValue(txtUGSkills));
                command.Parameters.AddWithValue("@GradSkills", GetTextBoxValue(txtGradSkills));
                command.Parameters.AddWithValue("@SpecialKnowledge", GetTextBoxValue(txtSpecialKnowledge));
                command.Parameters.AddWithValue("@LicenseType1", GetTextBoxValue(txtLicenseType1));
                command.Parameters.AddWithValue("@LicenseState1", GetTextBoxValue(txtLicenseState1));
                command.Parameters.AddWithValue("@LicenseNumber1", GetTextBoxValue(txtLicenseNumber1));
                command.Parameters.AddWithValue("@LicenseExpiration1", GetTextBoxValue(txtLicenseExpiration1));
                command.Parameters.AddWithValue("@LicenseType2", GetTextBoxValue(txtLicenseType2));
                command.Parameters.AddWithValue("@LicenseState2", GetTextBoxValue(txtLicenseState2));
                command.Parameters.AddWithValue("@LicenseNumber2", GetTextBoxValue(txtLicenseNumber2));
                command.Parameters.AddWithValue("@LicenseExpiration2", GetTextBoxValue(txtLicenseExpiration2));

                command.ExecuteNonQuery();
            }
        }

        // 3. ADD THIS MISSING METHOD (add it in the Validation Events region around line 248)
        protected void cvFinalAcknowledgment_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Validate that the final acknowledgment checkbox is checked
            args.IsValid = chkFinalAcknowledgment != null && chkFinalAcknowledgment.Checked;
        }

        #endregion
    }
}