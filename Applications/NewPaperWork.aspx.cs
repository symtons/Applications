using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Applications
{
    public partial class NewPaperWork : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
                ShowTab("personal");
            }
        }

        private void InitializePage()
        {
            // Set default application date
            txtApplicationDate.Text = DateTime.Now.ToString("MM/dd/yyyy");

            // Initialize any default values
            SetDefaultValues();

            // Check if editing existing application
            string appId = Request.QueryString["id"];
            if (!string.IsNullOrEmpty(appId) && int.TryParse(appId, out int applicationId))
            {
                LoadApplicationData(applicationId);
                hfApplicationId.Value = applicationId.ToString();
            }
        }

        private void SetDefaultValues()
        {
            // Set default location availability
            chkNashville.Checked = true;
            chkFirstShift.Checked = true;

            // Set default days available
            chkMonday.Checked = true;
            chkTuesday.Checked = true;
            chkWednesday.Checked = true;
            chkThursday.Checked = true;
            chkFriday.Checked = true;
        }

        #region Tab Navigation

        protected void btnTabPersonal_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            ShowTab("personal");
        }

        protected void btnTabPosition_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            ShowTab("position");
        }

        protected void btnTabBackground_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            ShowTab("background");
        }

        protected void btnTabEducation_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            ShowTab("education");
        }

        protected void btnTabEmployment_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            ShowTab("employment");
        }

        protected void btnTabReferences_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            ShowTab("references");
        }

        protected void btnTabAuthorization_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            ShowTab("authorization");
        }

      

        #endregion

        #region Navigation Buttons

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            NavigateToPreviousTab();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            NavigateToNextTab();
        }

        private void NavigateToPreviousTab()
        {
            string currentTab = hfCurrentTab.Value.ToLower();
            switch (currentTab)
            {
                case "position":
                    ShowTab("personal");
                    break;
                case "background":
                    ShowTab("position");
                    break;
                case "education":
                    ShowTab("background");
                    break;
                case "employment":
                    ShowTab("education");
                    break;
                case "references":
                    ShowTab("employment");
                    break;
                case "authorization":
                    ShowTab("references");
                    break;
            }
        }

        private void NavigateToNextTab()
        {
            string currentTab = hfCurrentTab.Value.ToLower();
            switch (currentTab)
            {
                case "personal":
                    ShowTab("position");
                    break;
                case "position":
                    ShowTab("background");
                    break;
                case "background":
                    ShowTab("education");
                    break;
                case "education":
                    ShowTab("employment");
                    break;
                case "employment":
                    ShowTab("references");
                    break;
                case "references":
                    ShowTab("authorization");
                    break;
            }
        }

        #endregion

        #region Save Operations

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            //try
            //{
                SaveApplicationData(false);
                ShowMessage("Draft saved successfully!", "success");
            //}
            //catch (Exception ex)
            //{
            //    ShowMessage("Error saving draft: " + ex.Message, "error");
            //}
        }

        //protected void btnSubmitApplication_Click(object sender, EventArgs e)
        //{
        //    //try
        //    //{
        //        if (ValidateApplication())
        //        {
        //            SaveApplicationData(true);
        //            ShowMessage("Application submitted successfully!", "success");

        //            // Redirect or disable form after submission
        //            DisableFormAfterSubmission();
        //        }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    ShowMessage("Error submitting application: " + ex.Message, "error");
        //    //}
        //}


        protected void btnSubmitApplication_Click(object sender, EventArgs e)
        {
            //try
            //{
                if (ValidateApplication())
                {
                    int applicationId = SaveApplicationData(true);

                    ShowMessage("Application submitted successfully!", "success");

                    // Redirect to success page after successful save
                    Response.Redirect($"ApplicationSuccess.aspx?id={applicationId}", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            //}
            //catch (Exception ex)
            //{
            //    ShowMessage("Error submitting application: " + ex.Message, "error");
            //}
        }

        private void SaveCurrentTabData()
        {
            //try
            //{
                SaveApplicationData(false);
            //}
            //catch (Exception)
            //{
            //    // Silent save - don't show errors for auto-save
            //}
        }

        private int SaveApplicationData(bool isSubmitted)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int applicationId;

                        if (!string.IsNullOrEmpty(hfApplicationId.Value))
                        {
                            applicationId = Convert.ToInt32(hfApplicationId.Value);
                            UpdateApplication(connection, transaction, applicationId, isSubmitted);
                        }
                        else
                        {
                            applicationId = InsertApplication(connection, transaction, isSubmitted);
                            hfApplicationId.Value = applicationId.ToString();
                        }

                        transaction.Commit();
                        return applicationId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private int InsertApplication(SqlConnection connection, SqlTransaction transaction, bool isSubmitted)
        {
            string applicationNumber = GenerateApplicationNumber();
            string status = isSubmitted ? "Submitted" : "Draft";

            string sql = @"
                INSERT INTO [JobApplications] (
                    [ApplicationNumber], [IsSubmitted], [SubmissionDate], [CreatedDate], [LastModified], 
                    [Status], [ApplicationDate], [FirstName], [MiddleName], [LastName], 
                    [HomeAddress], [AptNumber], [City], [State], [Zip], 
                    [HomePhone], [CellPhone], [SSN], [DriversLicense], [DLState],
                    [EmergencyContactName], [EmergencyContactRelationship], [EmergencyContactAddress],
                    [Position1], [Position2], [SalaryDesired], [AvailableStartDate], [SalaryType], [EmploymentType],
                    [NashvilleLocation], [FranklinLocation], [ShelbyvilleLocation], [WaynesboroLocation], [OtherLocation],
                    [FirstShift], [SecondShift], [ThirdShift], [WeekendsOnly],
                    [MondayAvailable], [TuesdayAvailable], [WednesdayAvailable], [ThursdayAvailable],
                    [FridayAvailable], [SaturdayAvailable], [SundayAvailable],
                    [AppliedBefore], [AppliedBeforeWhen], [WorkedBefore], [WorkedBeforeWhen],
                    [FamilyEmployed], [FamilyEmployedWho], [USCitizen], [AlienNumber], [LegallyEntitled], [Over18],
                    [ArmedForces], [ConvictedOfCrime], [AbuseRegistry],
                    [ElementarySchool], [HighSchool], [UndergraduateSchool], [GraduateSchool],
                    [Elem1], [Elem2], [Elem3], [Elem4], [Elem5],
                    [HS9], [HS10], [HS11], [HS12],
                    [UG1], [UG2], [UG3], [UG4], [UG5],
                    [Grad1], [Grad2], [Grad3], [Grad4], [Grad5],
                    [ElemDiploma], [HSDiploma], [UGDegree], [GradDegree],
                    [UGSkills], [GradSkills], [SpecialKnowledge],
                    [LicenseType1], [LicenseState1], [LicenseNumber1], [LicenseExpiration1],
                    [LicenseType2], [LicenseState2], [LicenseNumber2], [LicenseExpiration2],
                    [Employer1], [EmploymentFrom1], [EmploymentTo1], [JobTitle1], [Supervisor1],
                    [EmployerAddress1], [EmployerCityStateZip1], [EmployerPhone1], [StartingPay1], [FinalPay1],
                    [WorkPerformed1], [ReasonLeaving1],
                    [Employer2], [EmploymentFrom2], [EmploymentTo2], [JobTitle2], [Supervisor2],
                    [EmployerAddress2], [EmployerCityStateZip2], [EmployerPhone2], [StartingPay2], [FinalPay2],
                    [WorkPerformed2], [ReasonLeaving2],
                    [Employer3], [EmploymentFrom3], [EmploymentTo3], [JobTitle3], [Supervisor3],
                    [EmployerAddress3], [EmployerCityStateZip3], [EmployerPhone3], [StartingPay3], [FinalPay3],
                    [WorkPerformed3], [ReasonLeaving3],
                    [Reference1Name], [Reference1Phone], [Reference1Email], [Reference1Years],
                    [Reference2Name], [Reference2Phone], [Reference2Email], [Reference2Years],
                    [Reference3Name], [Reference3Phone], [Reference3Email], [Reference3Years],
                    [DIDDFullName], [DIDDSSN], [DIDDDateOfBirth], [DIDDDriversLicense], [DIDDWitness],
                    [BGLastName], [BGFirstName], [BGMiddleName], [BGStreet], [BGCity], [BGState], [BGZipCode],
                    [BGSSN], [BGPhone], [BGOtherName], [BGNameChangeYear], [BGDriversLicense], [BGDLState],
                    [BGDateOfBirth], [BGNameOnLicense], [ProtectionNoAbuse], [ProtectionHadAbuse], [ProtectionWitness],
                    [ReferenceAuthName], [SSNLast4], [ApplicantSignature], [SignatureDate], [FinalAcknowledgment]
                )
                VALUES (
                    @ApplicationNumber, @IsSubmitted, @SubmissionDate, @CreatedDate, @LastModified,
                    @Status, @ApplicationDate, @FirstName, @MiddleName, @LastName,
                    @HomeAddress, @AptNumber, @City, @State, @Zip,
                    @HomePhone, @CellPhone, @SSN, @DriversLicense, @DLState,
                    @EmergencyContactName, @EmergencyContactRelationship, @EmergencyContactAddress,
                    @Position1, @Position2, @SalaryDesired, @AvailableStartDate, @SalaryType, @EmploymentType,
                    @NashvilleLocation, @FranklinLocation, @ShelbyvilleLocation, @WaynesboroLocation, @OtherLocation,
                    @FirstShift, @SecondShift, @ThirdShift, @WeekendsOnly,
                    @MondayAvailable, @TuesdayAvailable, @WednesdayAvailable, @ThursdayAvailable,
                    @FridayAvailable, @SaturdayAvailable, @SundayAvailable,
                    @AppliedBefore, @AppliedBeforeWhen, @WorkedBefore, @WorkedBeforeWhen,
                    @FamilyEmployed, @FamilyEmployedWho, @USCitizen, @AlienNumber, @LegallyEntitled, @Over18,
                    @ArmedForces, @ConvictedOfCrime, @AbuseRegistry,
                    @ElementarySchool, @HighSchool, @UndergraduateSchool, @GraduateSchool,
                    @Elem1, @Elem2, @Elem3, @Elem4, @Elem5,
                    @HS9, @HS10, @HS11, @HS12,
                    @UG1, @UG2, @UG3, @UG4, @UG5,
                    @Grad1, @Grad2, @Grad3, @Grad4, @Grad5,
                    @ElemDiploma, @HSDiploma, @UGDegree, @GradDegree,
                    @UGSkills, @GradSkills, @SpecialKnowledge,
                    @LicenseType1, @LicenseState1, @LicenseNumber1, @LicenseExpiration1,
                    @LicenseType2, @LicenseState2, @LicenseNumber2, @LicenseExpiration2,
                    @Employer1, @EmploymentFrom1, @EmploymentTo1, @JobTitle1, @Supervisor1,
                    @EmployerAddress1, @EmployerCityStateZip1, @EmployerPhone1, @StartingPay1, @FinalPay1,
                    @WorkPerformed1, @ReasonLeaving1,
                    @Employer2, @EmploymentFrom2, @EmploymentTo2, @JobTitle2, @Supervisor2,
                    @EmployerAddress2, @EmployerCityStateZip2, @EmployerPhone2, @StartingPay2, @FinalPay2,
                    @WorkPerformed2, @ReasonLeaving2,
                    @Employer3, @EmploymentFrom3, @EmploymentTo3, @JobTitle3, @Supervisor3,
                    @EmployerAddress3, @EmployerCityStateZip3, @EmployerPhone3, @StartingPay3, @FinalPay3,
                    @WorkPerformed3, @ReasonLeaving3,
                    @Reference1Name, @Reference1Phone, @Reference1Email, @Reference1Years,
                    @Reference2Name, @Reference2Phone, @Reference2Email, @Reference2Years,
                    @Reference3Name, @Reference3Phone, @Reference3Email, @Reference3Years,
                    @DIDDFullName, @DIDDSSN, @DIDDDateOfBirth, @DIDDDriversLicense, @DIDDWitness,
                    @BGLastName, @BGFirstName, @BGMiddleName, @BGStreet, @BGCity, @BGState, @BGZipCode,
                    @BGSSN, @BGPhone, @BGOtherName, @BGNameChangeYear, @BGDriversLicense, @BGDLState,
                    @BGDateOfBirth, @BGNameOnLicense, @ProtectionNoAbuse, @ProtectionHadAbuse, @ProtectionWitness,
                    @ReferenceAuthName, @SSNLast4, @ApplicantSignature, @SignatureDate, @FinalAcknowledgment
                );
                SELECT SCOPE_IDENTITY();";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                AddApplicationParameters(command, applicationNumber, status, isSubmitted);
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        private void UpdateApplication(SqlConnection connection, SqlTransaction transaction, int applicationId, bool isSubmitted)
        {
            string status = isSubmitted ? "Submitted" : "Draft";

            string sql = @"
                UPDATE [JobApplications] SET
                    [IsSubmitted] = @IsSubmitted, [SubmissionDate] = @SubmissionDate, [LastModified] = @LastModified,
                    [Status] = @Status, [ApplicationDate] = @ApplicationDate, [FirstName] = @FirstName, 
                    [MiddleName] = @MiddleName, [LastName] = @LastName, [HomeAddress] = @HomeAddress, 
                    [AptNumber] = @AptNumber, [City] = @City, [State] = @State, [Zip] = @Zip,
                    [HomePhone] = @HomePhone, [CellPhone] = @CellPhone, [SSN] = @SSN, 
                    [DriversLicense] = @DriversLicense, [DLState] = @DLState,
                    [EmergencyContactName] = @EmergencyContactName, [EmergencyContactRelationship] = @EmergencyContactRelationship, 
                    [EmergencyContactAddress] = @EmergencyContactAddress,
                    [Position1] = @Position1, [Position2] = @Position2, [SalaryDesired] = @SalaryDesired, 
                    [AvailableStartDate] = @AvailableStartDate, [SalaryType] = @SalaryType, [EmploymentType] = @EmploymentType,
                    [NashvilleLocation] = @NashvilleLocation, [FranklinLocation] = @FranklinLocation, 
                    [ShelbyvilleLocation] = @ShelbyvilleLocation, [WaynesboroLocation] = @WaynesboroLocation, [OtherLocation] = @OtherLocation,
                    [FirstShift] = @FirstShift, [SecondShift] = @SecondShift, [ThirdShift] = @ThirdShift, [WeekendsOnly] = @WeekendsOnly,
                    [MondayAvailable] = @MondayAvailable, [TuesdayAvailable] = @TuesdayAvailable, [WednesdayAvailable] = @WednesdayAvailable,
                    [ThursdayAvailable] = @ThursdayAvailable, [FridayAvailable] = @FridayAvailable, [SaturdayAvailable] = @SaturdayAvailable, 
                    [SundayAvailable] = @SundayAvailable,
                    [AppliedBefore] = @AppliedBefore, [AppliedBeforeWhen] = @AppliedBeforeWhen, [WorkedBefore] = @WorkedBefore, 
                    [WorkedBeforeWhen] = @WorkedBeforeWhen, [FamilyEmployed] = @FamilyEmployed, [FamilyEmployedWho] = @FamilyEmployedWho,
                    [USCitizen] = @USCitizen, [AlienNumber] = @AlienNumber, [LegallyEntitled] = @LegallyEntitled, [Over18] = @Over18,
                    [ArmedForces] = @ArmedForces, [ConvictedOfCrime] = @ConvictedOfCrime, [AbuseRegistry] = @AbuseRegistry,
                    [ElementarySchool] = @ElementarySchool, [HighSchool] = @HighSchool, [UndergraduateSchool] = @UndergraduateSchool, 
                    [GraduateSchool] = @GraduateSchool,
                    [Elem1] = @Elem1, [Elem2] = @Elem2, [Elem3] = @Elem3, [Elem4] = @Elem4, [Elem5] = @Elem5,
                    [HS9] = @HS9, [HS10] = @HS10, [HS11] = @HS11, [HS12] = @HS12,
                    [UG1] = @UG1, [UG2] = @UG2, [UG3] = @UG3, [UG4] = @UG4, [UG5] = @UG5,
                    [Grad1] = @Grad1, [Grad2] = @Grad2, [Grad3] = @Grad3, [Grad4] = @Grad4, [Grad5] = @Grad5,
                    [ElemDiploma] = @ElemDiploma, [HSDiploma] = @HSDiploma, [UGDegree] = @UGDegree, [GradDegree] = @GradDegree,
                    [UGSkills] = @UGSkills, [GradSkills] = @GradSkills, [SpecialKnowledge] = @SpecialKnowledge,
                    [LicenseType1] = @LicenseType1, [LicenseState1] = @LicenseState1, [LicenseNumber1] = @LicenseNumber1, 
                    [LicenseExpiration1] = @LicenseExpiration1,
                    [LicenseType2] = @LicenseType2, [LicenseState2] = @LicenseState2, [LicenseNumber2] = @LicenseNumber2, 
                    [LicenseExpiration2] = @LicenseExpiration2,
                    [Employer1] = @Employer1, [EmploymentFrom1] = @EmploymentFrom1, [EmploymentTo1] = @EmploymentTo1, 
                    [JobTitle1] = @JobTitle1, [Supervisor1] = @Supervisor1,
                    [EmployerAddress1] = @EmployerAddress1, [EmployerCityStateZip1] = @EmployerCityStateZip1, 
                    [EmployerPhone1] = @EmployerPhone1, [StartingPay1] = @StartingPay1, [FinalPay1] = @FinalPay1,
                    [WorkPerformed1] = @WorkPerformed1, [ReasonLeaving1] = @ReasonLeaving1,
                    [Employer2] = @Employer2, [EmploymentFrom2] = @EmploymentFrom2, [EmploymentTo2] = @EmploymentTo2, 
                    [JobTitle2] = @JobTitle2, [Supervisor2] = @Supervisor2,
                    [EmployerAddress2] = @EmployerAddress2, [EmployerCityStateZip2] = @EmployerCityStateZip2, 
                    [EmployerPhone2] = @EmployerPhone2, [StartingPay2] = @StartingPay2, [FinalPay2] = @FinalPay2,
                    [WorkPerformed2] = @WorkPerformed2, [ReasonLeaving2] = @ReasonLeaving2,
                    [Employer3] = @Employer3, [EmploymentFrom3] = @EmploymentFrom3, [EmploymentTo3] = @EmploymentTo3, 
                    [JobTitle3] = @JobTitle3, [Supervisor3] = @Supervisor3,
                    [EmployerAddress3] = @EmployerAddress3, [EmployerCityStateZip3] = @EmployerCityStateZip3, 
                    [EmployerPhone3] = @EmployerPhone3, [StartingPay3] = @StartingPay3, [FinalPay3] = @FinalPay3,
                    [WorkPerformed3] = @WorkPerformed3, [ReasonLeaving3] = @ReasonLeaving3,
                    [Reference1Name] = @Reference1Name, [Reference1Phone] = @Reference1Phone, [Reference1Email] = @Reference1Email, 
                    [Reference1Years] = @Reference1Years,
                    [Reference2Name] = @Reference2Name, [Reference2Phone] = @Reference2Phone, [Reference2Email] = @Reference2Email, 
                    [Reference2Years] = @Reference2Years,
                    [Reference3Name] = @Reference3Name, [Reference3Phone] = @Reference3Phone, [Reference3Email] = @Reference3Email, 
                    [Reference3Years] = @Reference3Years,
                    [DIDDFullName] = @DIDDFullName, [DIDDSSN] = @DIDDSSN, [DIDDDateOfBirth] = @DIDDDateOfBirth, 
                    [DIDDDriversLicense] = @DIDDDriversLicense, [DIDDWitness] = @DIDDWitness,
                    [BGLastName] = @BGLastName, [BGFirstName] = @BGFirstName, [BGMiddleName] = @BGMiddleName, 
                    [BGStreet] = @BGStreet, [BGCity] = @BGCity, [BGState] = @BGState, [BGZipCode] = @BGZipCode,
                    [BGSSN] = @BGSSN, [BGPhone] = @BGPhone, [BGOtherName] = @BGOtherName, [BGNameChangeYear] = @BGNameChangeYear, 
                    [BGDriversLicense] = @BGDriversLicense, [BGDLState] = @BGDLState,
                    [BGDateOfBirth] = @BGDateOfBirth, [BGNameOnLicense] = @BGNameOnLicense, [ProtectionNoAbuse] = @ProtectionNoAbuse, 
                    [ProtectionHadAbuse] = @ProtectionHadAbuse, [ProtectionWitness] = @ProtectionWitness,
                    [ReferenceAuthName] = @ReferenceAuthName, [SSNLast4] = @SSNLast4, [ApplicantSignature] = @ApplicantSignature, 
                    [SignatureDate] = @SignatureDate, [FinalAcknowledgment] = @FinalAcknowledgment
                WHERE [ApplicationId] = @ApplicationId";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@ApplicationId", applicationId);
                AddApplicationParameters(command, null, status, isSubmitted);
                command.ExecuteNonQuery();
            }
        }

        private void AddApplicationParameters(SqlCommand command, string applicationNumber, string status, bool isSubmitted)
        {
            DateTime now = DateTime.Now;

            // Application metadata
            if (!string.IsNullOrEmpty(applicationNumber))
                command.Parameters.AddWithValue("@ApplicationNumber", applicationNumber);
           

            command.Parameters.AddWithValue("@IsSubmitted", isSubmitted);
            command.Parameters.AddWithValue("@SubmissionDate", isSubmitted ? (object)now : DBNull.Value);
            command.Parameters.AddWithValue("@CreatedDate", GetDateTimeOrNow(txtApplicationDate.Text));
            command.Parameters.AddWithValue("@LastModified", GetDateTimeOrNow(txtApplicationDate.Text));
            command.Parameters.AddWithValue("@Status", status);

            // Personal Information
            command.Parameters.AddWithValue("@ApplicationDate", GetDateTimeOrNow(txtApplicationDate.Text));

            command.Parameters.AddWithValue("@FirstName", GetTextBoxValue(txtFirstName));
            command.Parameters.AddWithValue("@MiddleName", GetTextBoxValue(txtMiddleName));

            command.Parameters.AddWithValue("@LastName", GetTextBoxValue(txtLastName));
            command.Parameters.AddWithValue("@HomeAddress", GetTextBoxValue(txtHomeAddress));
            command.Parameters.AddWithValue("@AptNumber", GetTextBoxValue(txtAptNumber));
            command.Parameters.AddWithValue("@City", GetTextBoxValue(txtCity));
            command.Parameters.AddWithValue("@State", GetTextBoxValue(txtState));
            command.Parameters.AddWithValue("@Zip", GetTextBoxValue(txtZipCode));
            command.Parameters.AddWithValue("@HomePhone", GetTextBoxValue(txtPhoneNumber));
            command.Parameters.AddWithValue("@CellPhone", GetTextBoxValue(txtCellNumber));
            command.Parameters.AddWithValue("@SSN", GetTextBoxValue(txtSSN));
            command.Parameters.AddWithValue("@DriversLicense", GetTextBoxValue(txtDriversLicense));
            command.Parameters.AddWithValue("@DLState", GetTextBoxValue(txtDLState));

            // Emergency Contact
            command.Parameters.AddWithValue("@EmergencyContactName", GetTextBoxValue(txtEmergencyContactName));
            command.Parameters.AddWithValue("@EmergencyContactRelationship", GetTextBoxValue(txtEmergencyContactRelationship));
            command.Parameters.AddWithValue("@EmergencyContactAddress", GetTextBoxValue(txtEmergencyContactAddress));

            // Position Information
            command.Parameters.AddWithValue("@Position1", GetTextBoxValue(txtPosition1));
            command.Parameters.AddWithValue("@Position2", GetTextBoxValue(txtPosition2));
            command.Parameters.AddWithValue("@SalaryDesired", GetTextBoxValue(txtSalaryDesired));
            command.Parameters.AddWithValue("@AvailableStartDate", GetTextBoxValue(txtAvailableStartDate));
            command.Parameters.AddWithValue("@SalaryType", GetSalaryType());
            command.Parameters.AddWithValue("@EmploymentType", GetEmploymentType());

            // Location Preferences
            command.Parameters.AddWithValue("@NashvilleLocation", GetCheckBoxValue(chkNashville));
            command.Parameters.AddWithValue("@FranklinLocation", GetCheckBoxValue(chkFranklin));
            command.Parameters.AddWithValue("@ShelbyvilleLocation", GetCheckBoxValue(chkShelbyville));
            command.Parameters.AddWithValue("@WaynesboroLocation", GetCheckBoxValue(chkWaynesboro));
            command.Parameters.AddWithValue("@OtherLocation", GetCheckBoxValue(chkOtherLocation));

            // Shift Preferences
            command.Parameters.AddWithValue("@FirstShift", GetCheckBoxValue(chkFirstShift));
            command.Parameters.AddWithValue("@SecondShift", GetCheckBoxValue(chkSecondShift));
            command.Parameters.AddWithValue("@ThirdShift", GetCheckBoxValue(chkThirdShift));
            command.Parameters.AddWithValue("@WeekendsOnly", GetCheckBoxValue(chkWeekendsOnly));

            // Day Availability
            command.Parameters.AddWithValue("@MondayAvailable", GetCheckBoxValue(chkMonday));
            command.Parameters.AddWithValue("@TuesdayAvailable", GetCheckBoxValue(chkTuesday));
            command.Parameters.AddWithValue("@WednesdayAvailable", GetCheckBoxValue(chkWednesday));
            command.Parameters.AddWithValue("@ThursdayAvailable", GetCheckBoxValue(chkThursday));
            command.Parameters.AddWithValue("@FridayAvailable", GetCheckBoxValue(chkFriday));
            command.Parameters.AddWithValue("@SaturdayAvailable", GetCheckBoxValue(chkSaturday));
            command.Parameters.AddWithValue("@SundayAvailable", GetCheckBoxValue(chkSunday));

            // Background Questions
            command.Parameters.AddWithValue("@AppliedBefore", GetRadioButtonValue(rbAppliedBeforeYes));
            command.Parameters.AddWithValue("@AppliedBeforeWhen", GetTextBoxValue(txtAppliedBeforeWhen));
            command.Parameters.AddWithValue("@WorkedBefore", GetRadioButtonValue(rbWorkedBeforeYes));
            command.Parameters.AddWithValue("@WorkedBeforeWhen", GetTextBoxValue(txtWorkedBeforeWhen));
            command.Parameters.AddWithValue("@FamilyEmployed", GetRadioButtonValue(rbFamilyEmployedYes));
            command.Parameters.AddWithValue("@FamilyEmployedWho", GetTextBoxValue(txtFamilyEmployedWho));
            command.Parameters.AddWithValue("@USCitizen", GetRadioButtonValue(rbUSCitizenYes));
            command.Parameters.AddWithValue("@AlienNumber", GetTextBoxValue(txtAlienNumber));
            command.Parameters.AddWithValue("@LegallyEntitled", GetRadioButtonValue(rbLegallyEntitledYes));
            command.Parameters.AddWithValue("@Over18", GetRadioButtonValue(rbOver18Yes));
            command.Parameters.AddWithValue("@ArmedForces", GetRadioButtonValue(rbArmedForcesYes));
            command.Parameters.AddWithValue("@ConvictedOfCrime", GetRadioButtonValue(rbConvictedYes));
            command.Parameters.AddWithValue("@AbuseRegistry", GetRadioButtonValue(rbAbuseRegistryYes));

            // Education
            command.Parameters.AddWithValue("@ElementarySchool", GetTextBoxValue(txtElementarySchool));
            command.Parameters.AddWithValue("@HighSchool", GetTextBoxValue(txtHighSchool));
            command.Parameters.AddWithValue("@UndergraduateSchool", GetTextBoxValue(txtUndergraduateSchool));
            command.Parameters.AddWithValue("@GraduateSchool", GetTextBoxValue(txtGraduateSchool));

            // Education Years
            command.Parameters.AddWithValue("@Elem1", GetCheckBoxValue(chkElem1));
            command.Parameters.AddWithValue("@Elem2", GetCheckBoxValue(chkElem2));
            command.Parameters.AddWithValue("@Elem3", GetCheckBoxValue(chkElem3));
            command.Parameters.AddWithValue("@Elem4", GetCheckBoxValue(chkElem4));
            command.Parameters.AddWithValue("@Elem5", GetCheckBoxValue(chkElem5));
            command.Parameters.AddWithValue("@HS9", GetCheckBoxValue(chkHS9));
            command.Parameters.AddWithValue("@HS10", GetCheckBoxValue(chkHS10));
            command.Parameters.AddWithValue("@HS11", GetCheckBoxValue(chkHS11));
            command.Parameters.AddWithValue("@HS12", GetCheckBoxValue(chkHS12));
            command.Parameters.AddWithValue("@UG1", GetCheckBoxValue(chkUG1));
            command.Parameters.AddWithValue("@UG2", GetCheckBoxValue(chkUG2));
            command.Parameters.AddWithValue("@UG3", GetCheckBoxValue(chkUG3));
            command.Parameters.AddWithValue("@UG4", GetCheckBoxValue(chkUG4));
            command.Parameters.AddWithValue("@UG5", GetCheckBoxValue(chkUG5));
            command.Parameters.AddWithValue("@Grad1", GetCheckBoxValue(chkGrad1));
            command.Parameters.AddWithValue("@Grad2", GetCheckBoxValue(chkGrad2));
            command.Parameters.AddWithValue("@Grad3", GetCheckBoxValue(chkGrad3));
            command.Parameters.AddWithValue("@Grad4", GetCheckBoxValue(chkGrad4));
            command.Parameters.AddWithValue("@Grad5", GetCheckBoxValue(chkGrad5));

            // Education Completion
            command.Parameters.AddWithValue("@ElemDiploma", GetRadioButtonValue(rbElemDiplomaYes));
            command.Parameters.AddWithValue("@HSDiploma", GetRadioButtonValue(rbHSDiplomaYes));
            command.Parameters.AddWithValue("@UGDegree", GetRadioButtonValue(rbUGDegreeYes));
            command.Parameters.AddWithValue("@GradDegree", GetRadioButtonValue(rbGradDegreeYes));

            // Skills and Knowledge
            command.Parameters.AddWithValue("@UGSkills", GetTextBoxValue(txtUGSkills));
            command.Parameters.AddWithValue("@GradSkills", GetTextBoxValue(txtGradSkills));
            command.Parameters.AddWithValue("@SpecialKnowledge", GetTextBoxValue(txtSpecialKnowledge));

            // Licenses
            command.Parameters.AddWithValue("@LicenseType1", GetTextBoxValue(txtLicenseType1));
            command.Parameters.AddWithValue("@LicenseState1", GetTextBoxValue(txtLicenseState1));
            command.Parameters.AddWithValue("@LicenseNumber1", GetTextBoxValue(txtLicenseNumber1));
            command.Parameters.AddWithValue("@LicenseExpiration1", GetTextBoxValue(txtLicenseExpiration1));
            command.Parameters.AddWithValue("@LicenseType2", GetTextBoxValue(txtLicenseType2));
            command.Parameters.AddWithValue("@LicenseState2", GetTextBoxValue(txtLicenseState2));
            command.Parameters.AddWithValue("@LicenseNumber2", GetTextBoxValue(txtLicenseNumber2));
            command.Parameters.AddWithValue("@LicenseExpiration2", GetTextBoxValue(txtLicenseExpiration2));

            // Employment History 1
            command.Parameters.AddWithValue("@Employer1", GetTextBoxValue(txtEmployer1));
            command.Parameters.AddWithValue("@EmploymentFrom1", GetTextBoxValue(txtEmploymentFrom1));
            command.Parameters.AddWithValue("@EmploymentTo1", GetTextBoxValue(txtEmploymentTo1));
            command.Parameters.AddWithValue("@JobTitle1", GetTextBoxValue(txtJobTitle1));
            command.Parameters.AddWithValue("@Supervisor1", GetTextBoxValue(txtSupervisor1));
            command.Parameters.AddWithValue("@EmployerAddress1", GetTextBoxValue(txtEmployerAddress1));
            command.Parameters.AddWithValue("@EmployerCityStateZip1", GetTextBoxValue(txtEmployerCityStateZip1));
            command.Parameters.AddWithValue("@EmployerPhone1", GetTextBoxValue(txtEmployerPhone1));
            command.Parameters.AddWithValue("@StartingPay1", GetTextBoxValue(txtStartingPay1));
            command.Parameters.AddWithValue("@FinalPay1", GetTextBoxValue(txtFinalPay1));
            command.Parameters.AddWithValue("@WorkPerformed1", GetTextBoxValue(txtWorkPerformed1));
            command.Parameters.AddWithValue("@ReasonLeaving1", GetTextBoxValue(txtReasonLeaving1));

            // Employment History 2
            command.Parameters.AddWithValue("@Employer2", GetTextBoxValue(txtEmployer2));
            command.Parameters.AddWithValue("@EmploymentFrom2", GetTextBoxValue(txtEmploymentFrom2));
            command.Parameters.AddWithValue("@EmploymentTo2", GetTextBoxValue(txtEmploymentTo2));
            command.Parameters.AddWithValue("@JobTitle2", GetTextBoxValue(txtJobTitle2));
            command.Parameters.AddWithValue("@Supervisor2", GetTextBoxValue(txtSupervisor2));
            command.Parameters.AddWithValue("@EmployerAddress2", GetTextBoxValue(txtEmployerAddress2));
            command.Parameters.AddWithValue("@EmployerCityStateZip2", GetTextBoxValue(txtEmployerCityStateZip2));
            command.Parameters.AddWithValue("@EmployerPhone2", GetTextBoxValue(txtEmployerPhone2));
            command.Parameters.AddWithValue("@StartingPay2", GetTextBoxValue(txtStartingPay2));
            command.Parameters.AddWithValue("@FinalPay2", GetTextBoxValue(txtFinalPay2));
            command.Parameters.AddWithValue("@WorkPerformed2", GetTextBoxValue(txtWorkPerformed2));
            command.Parameters.AddWithValue("@ReasonLeaving2", GetTextBoxValue(txtReasonLeaving2));

            // Employment History 3
            command.Parameters.AddWithValue("@Employer3", GetTextBoxValue(txtEmployer3));
            command.Parameters.AddWithValue("@EmploymentFrom3", GetTextBoxValue(txtEmploymentFrom3));
            command.Parameters.AddWithValue("@EmploymentTo3", GetTextBoxValue(txtEmploymentTo3));
            command.Parameters.AddWithValue("@JobTitle3", GetTextBoxValue(txtJobTitle3));
            command.Parameters.AddWithValue("@Supervisor3", GetTextBoxValue(txtSupervisor3));
            command.Parameters.AddWithValue("@EmployerAddress3", GetTextBoxValue(txtEmployerAddress3));
            command.Parameters.AddWithValue("@EmployerCityStateZip3", GetTextBoxValue(txtEmployerCityStateZip3));
            command.Parameters.AddWithValue("@EmployerPhone3", GetTextBoxValue(txtEmployerPhone3));
            command.Parameters.AddWithValue("@StartingPay3", GetTextBoxValue(txtStartingPay3));
            command.Parameters.AddWithValue("@FinalPay3", GetTextBoxValue(txtFinalPay3));
            command.Parameters.AddWithValue("@WorkPerformed3", GetTextBoxValue(txtWorkPerformed3));
            command.Parameters.AddWithValue("@ReasonLeaving3", GetTextBoxValue(txtReasonLeaving3));

            // References
            command.Parameters.AddWithValue("@Reference1Name", GetTextBoxValue(txtReference1Name));
            command.Parameters.AddWithValue("@Reference1Phone", GetTextBoxValue(txtReference1Phone));
            command.Parameters.AddWithValue("@Reference1Email", GetTextBoxValue(txtReference1Email));
            command.Parameters.AddWithValue("@Reference1Years", GetTextBoxValue(txtReference1Years));
            command.Parameters.AddWithValue("@Reference2Name", GetTextBoxValue(txtReference2Name));
            command.Parameters.AddWithValue("@Reference2Phone", GetTextBoxValue(txtReference2Phone));
            command.Parameters.AddWithValue("@Reference2Email", GetTextBoxValue(txtReference2Email));
            command.Parameters.AddWithValue("@Reference2Years", GetTextBoxValue(txtReference2Years));
            command.Parameters.AddWithValue("@Reference3Name", GetTextBoxValue(txtReference3Name));
            command.Parameters.AddWithValue("@Reference3Phone", GetTextBoxValue(txtReference3Phone));
            command.Parameters.AddWithValue("@Reference3Email", GetTextBoxValue(txtReference3Email));
            command.Parameters.AddWithValue("@Reference3Years", GetTextBoxValue(txtReference3Years));

            // DIDD Information
            command.Parameters.AddWithValue("@DIDDFullName", GetTextBoxValue(txtDIDDFullName));
            command.Parameters.AddWithValue("@DIDDSSN", GetTextBoxValue(txtDIDDSSN));
            command.Parameters.AddWithValue("@DIDDDateOfBirth", GetTextBoxValue(txtDIDDDateOfBirth));
            command.Parameters.AddWithValue("@DIDDDriversLicense", GetTextBoxValue(txtDIDDDriversLicense));
            command.Parameters.AddWithValue("@DIDDWitness", GetTextBoxValue(txtDIDDWitness));

            // Background Check Information
            command.Parameters.AddWithValue("@BGLastName", GetTextBoxValue(txtBGLastName));
            command.Parameters.AddWithValue("@BGFirstName", GetTextBoxValue(txtBGFirstName));
            command.Parameters.AddWithValue("@BGMiddleName", GetTextBoxValue(txtBGMiddleName));
            command.Parameters.AddWithValue("@BGStreet", GetTextBoxValue(txtBGStreet));
            command.Parameters.AddWithValue("@BGCity", GetTextBoxValue(txtBGCity));
            command.Parameters.AddWithValue("@BGState", GetTextBoxValue(txtBGState));
            command.Parameters.AddWithValue("@BGZipCode", GetTextBoxValue(txtBGZipCode));
            command.Parameters.AddWithValue("@BGSSN", GetTextBoxValue(txtBGSSN));
            command.Parameters.AddWithValue("@BGPhone", GetTextBoxValue(txtBGPhone));
            command.Parameters.AddWithValue("@BGOtherName", GetTextBoxValue(txtBGOtherName));
            command.Parameters.AddWithValue("@BGNameChangeYear", GetTextBoxValue(txtBGNameChangeYear));
            command.Parameters.AddWithValue("@BGDriversLicense", GetTextBoxValue(txtBGDriversLicense));
            command.Parameters.AddWithValue("@BGDLState", GetTextBoxValue(txtBGDLState));
            command.Parameters.AddWithValue("@BGDateOfBirth", GetTextBoxValue(txtBGDateOfBirth));
            command.Parameters.AddWithValue("@BGNameOnLicense", GetTextBoxValue(txtBGNameOnLicense));

            // Protection from Abuse
            command.Parameters.AddWithValue("@ProtectionNoAbuse", GetCheckBoxValue(chkProtectionNoAbuse));
            command.Parameters.AddWithValue("@ProtectionHadAbuse", GetCheckBoxValue(chkProtectionHadAbuse));
            command.Parameters.AddWithValue("@ProtectionWitness", GetTextBoxValue(txtProtectionWitness));

            // Authorization
            command.Parameters.AddWithValue("@ReferenceAuthName", GetTextBoxValue(txtReferenceAuthName));
            command.Parameters.AddWithValue("@SSNLast4", GetTextBoxValue(txtSSNLast4));
            command.Parameters.AddWithValue("@ApplicantSignature", GetTextBoxValue(txtApplicantSignature));
            
            command.Parameters.AddWithValue("@SignatureDate", GetDateTimeOrNow(txtSignatureDate.Text));
            command.Parameters.AddWithValue("@FinalAcknowledgment", GetCheckBoxValue(chkFinalAcknowledgment));
        }

        private void LoadApplicationData(int applicationId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM [JobApplications] WHERE [ApplicationId] = @ApplicationId";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationId", applicationId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Personal Information
                            SetTextBoxValue(txtApplicationDate, reader["ApplicationDate"]);
                            SetTextBoxValue(txtFirstName, reader["FirstName"]);
                            SetTextBoxValue(txtMiddleName, reader["MiddleName"]);
                            SetTextBoxValue(txtLastName, reader["LastName"]);
                            SetTextBoxValue(txtHomeAddress, reader["HomeAddress"]);
                            SetTextBoxValue(txtAptNumber, reader["AptNumber"]);
                            SetTextBoxValue(txtCity, reader["City"]);
                            SetTextBoxValue(txtState, reader["State"]);
                            SetTextBoxValue(txtZipCode, reader["Zip"]);
                            SetTextBoxValue(txtPhoneNumber, reader["HomePhone"]);
                            SetTextBoxValue(txtCellNumber, reader["CellPhone"]);
                            SetTextBoxValue(txtSSN, reader["SSN"]);
                            SetTextBoxValue(txtDriversLicense, reader["DriversLicense"]);
                            SetTextBoxValue(txtDLState, reader["DLState"]);

                            // Emergency Contact
                            SetTextBoxValue(txtEmergencyContactName, reader["EmergencyContactName"]);
                            SetTextBoxValue(txtEmergencyContactRelationship, reader["EmergencyContactRelationship"]);
                            SetTextBoxValue(txtEmergencyContactAddress, reader["EmergencyContactAddress"]);

                            // Position Information
                            SetTextBoxValue(txtPosition1, reader["Position1"]);
                            SetTextBoxValue(txtPosition2, reader["Position2"]);
                            SetTextBoxValue(txtSalaryDesired, reader["SalaryDesired"]);
                            SetTextBoxValue(txtAvailableStartDate, reader["AvailableStartDate"]);

                            // Location Preferences
                            SetCheckBoxValue(chkNashville, reader["NashvilleLocation"]);
                            SetCheckBoxValue(chkFranklin, reader["FranklinLocation"]);
                            SetCheckBoxValue(chkShelbyville, reader["ShelbyvilleLocation"]);
                            SetCheckBoxValue(chkWaynesboro, reader["WaynesboroLocation"]);
                            SetCheckBoxValue(chkOtherLocation, reader["OtherLocation"]);

                            // Shift Preferences
                            SetCheckBoxValue(chkFirstShift, reader["FirstShift"]);
                            SetCheckBoxValue(chkSecondShift, reader["SecondShift"]);
                            SetCheckBoxValue(chkThirdShift, reader["ThirdShift"]);
                            SetCheckBoxValue(chkWeekendsOnly, reader["WeekendsOnly"]);

                            // Day Availability
                            SetCheckBoxValue(chkMonday, reader["MondayAvailable"]);
                            SetCheckBoxValue(chkTuesday, reader["TuesdayAvailable"]);
                            SetCheckBoxValue(chkWednesday, reader["WednesdayAvailable"]);
                            SetCheckBoxValue(chkThursday, reader["ThursdayAvailable"]);
                            SetCheckBoxValue(chkFriday, reader["FridayAvailable"]);
                            SetCheckBoxValue(chkSaturday, reader["SaturdayAvailable"]);
                            SetCheckBoxValue(chkSunday, reader["SundayAvailable"]);

                            // Background Questions
                            SetRadioButtonValue(rbAppliedBeforeYes, rbAppliedBeforeNo, reader["AppliedBefore"]);
                            SetTextBoxValue(txtAppliedBeforeWhen, reader["AppliedBeforeWhen"]);
                            SetRadioButtonValue(rbWorkedBeforeYes, rbWorkedBeforeNo, reader["WorkedBefore"]);
                            SetTextBoxValue(txtWorkedBeforeWhen, reader["WorkedBeforeWhen"]);
                            SetRadioButtonValue(rbFamilyEmployedYes, rbFamilyEmployedNo, reader["FamilyEmployed"]);
                            SetTextBoxValue(txtFamilyEmployedWho, reader["FamilyEmployedWho"]);
                            SetRadioButtonValue(rbUSCitizenYes, rbUSCitizenNo, reader["USCitizen"]);
                            SetTextBoxValue(txtAlienNumber, reader["AlienNumber"]);
                            SetRadioButtonValue(rbLegallyEntitledYes, rbLegallyEntitledNo, reader["LegallyEntitled"]);
                            SetRadioButtonValue(rbOver18Yes, rbOver18No, reader["Over18"]);
                            SetRadioButtonValue(rbArmedForcesYes, rbArmedForcesNo, reader["ArmedForces"]);
                            SetRadioButtonValue(rbConvictedYes, rbConvictedNo, reader["ConvictedOfCrime"]);
                            SetRadioButtonValue(rbAbuseRegistryYes, rbAbuseRegistryNo, reader["AbuseRegistry"]);

                            // Education
                            SetTextBoxValue(txtElementarySchool, reader["ElementarySchool"]);
                            SetTextBoxValue(txtHighSchool, reader["HighSchool"]);
                            SetTextBoxValue(txtUndergraduateSchool, reader["UndergraduateSchool"]);
                            SetTextBoxValue(txtGraduateSchool, reader["GraduateSchool"]);

                            // Education Years
                            SetCheckBoxValue(chkElem1, reader["Elem1"]);
                            SetCheckBoxValue(chkElem2, reader["Elem2"]);
                            SetCheckBoxValue(chkElem3, reader["Elem3"]);
                            SetCheckBoxValue(chkElem4, reader["Elem4"]);
                            SetCheckBoxValue(chkElem5, reader["Elem5"]);
                            SetCheckBoxValue(chkHS9, reader["HS9"]);
                            SetCheckBoxValue(chkHS10, reader["HS10"]);
                            SetCheckBoxValue(chkHS11, reader["HS11"]);
                            SetCheckBoxValue(chkHS12, reader["HS12"]);
                            SetCheckBoxValue(chkUG1, reader["UG1"]);
                            SetCheckBoxValue(chkUG2, reader["UG2"]);
                            SetCheckBoxValue(chkUG3, reader["UG3"]);
                            SetCheckBoxValue(chkUG4, reader["UG4"]);
                            SetCheckBoxValue(chkUG5, reader["UG5"]);
                            SetCheckBoxValue(chkGrad1, reader["Grad1"]);
                            SetCheckBoxValue(chkGrad2, reader["Grad2"]);
                            SetCheckBoxValue(chkGrad3, reader["Grad3"]);
                            SetCheckBoxValue(chkGrad4, reader["Grad4"]);
                            SetCheckBoxValue(chkGrad5, reader["Grad5"]);

                            // Education Completion
                            SetRadioButtonValue(rbElemDiplomaYes, rbElemDiplomaNo, reader["ElemDiploma"]);
                            SetRadioButtonValue(rbHSDiplomaYes, rbHSDiplomaNo, reader["HSDiploma"]);
                            SetRadioButtonValue(rbUGDegreeYes, rbUGDegreeNo, reader["UGDegree"]);
                            SetRadioButtonValue(rbGradDegreeYes, rbGradDegreeNo, reader["GradDegree"]);

                            // Skills and Knowledge
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

                            // Employment History
                            SetTextBoxValue(txtEmployer1, reader["Employer1"]);
                            SetTextBoxValue(txtEmploymentFrom1, reader["EmploymentFrom1"]);
                            SetTextBoxValue(txtEmploymentTo1, reader["EmploymentTo1"]);
                            SetTextBoxValue(txtJobTitle1, reader["JobTitle1"]);
                            SetTextBoxValue(txtSupervisor1, reader["Supervisor1"]);
                            SetTextBoxValue(txtEmployerAddress1, reader["EmployerAddress1"]);
                            SetTextBoxValue(txtEmployerCityStateZip1, reader["EmployerCityStateZip1"]);
                            SetTextBoxValue(txtEmployerPhone1, reader["EmployerPhone1"]);
                            SetTextBoxValue(txtStartingPay1, reader["StartingPay1"]);
                            SetTextBoxValue(txtFinalPay1, reader["FinalPay1"]);
                            SetTextBoxValue(txtWorkPerformed1, reader["WorkPerformed1"]);
                            SetTextBoxValue(txtReasonLeaving1, reader["ReasonLeaving1"]);

                            SetTextBoxValue(txtEmployer2, reader["Employer2"]);
                            SetTextBoxValue(txtEmploymentFrom2, reader["EmploymentFrom2"]);
                            SetTextBoxValue(txtEmploymentTo2, reader["EmploymentTo2"]);
                            SetTextBoxValue(txtJobTitle2, reader["JobTitle2"]);
                            SetTextBoxValue(txtSupervisor2, reader["Supervisor2"]);
                            SetTextBoxValue(txtEmployerAddress2, reader["EmployerAddress2"]);
                            SetTextBoxValue(txtEmployerCityStateZip2, reader["EmployerCityStateZip2"]);
                            SetTextBoxValue(txtEmployerPhone2, reader["EmployerPhone2"]);
                            SetTextBoxValue(txtStartingPay2, reader["StartingPay2"]);
                            SetTextBoxValue(txtFinalPay2, reader["FinalPay2"]);
                            SetTextBoxValue(txtWorkPerformed2, reader["WorkPerformed2"]);
                            SetTextBoxValue(txtReasonLeaving2, reader["ReasonLeaving2"]);

                            SetTextBoxValue(txtEmployer3, reader["Employer3"]);
                            SetTextBoxValue(txtEmploymentFrom3, reader["EmploymentFrom3"]);
                            SetTextBoxValue(txtEmploymentTo3, reader["EmploymentTo3"]);
                            SetTextBoxValue(txtJobTitle3, reader["JobTitle3"]);
                            SetTextBoxValue(txtSupervisor3, reader["Supervisor3"]);
                            SetTextBoxValue(txtEmployerAddress3, reader["EmployerAddress3"]);
                            SetTextBoxValue(txtEmployerCityStateZip3, reader["EmployerCityStateZip3"]);
                            SetTextBoxValue(txtEmployerPhone3, reader["EmployerPhone3"]);
                            SetTextBoxValue(txtStartingPay3, reader["StartingPay3"]);
                            SetTextBoxValue(txtFinalPay3, reader["FinalPay3"]);
                            SetTextBoxValue(txtWorkPerformed3, reader["WorkPerformed3"]);
                            SetTextBoxValue(txtReasonLeaving3, reader["ReasonLeaving3"]);

                            // References
                            SetTextBoxValue(txtReference1Name, reader["Reference1Name"]);
                            SetTextBoxValue(txtReference1Phone, reader["Reference1Phone"]);
                            SetTextBoxValue(txtReference1Email, reader["Reference1Email"]);
                            SetTextBoxValue(txtReference1Years, reader["Reference1Years"]);
                            SetTextBoxValue(txtReference2Name, reader["Reference2Name"]);
                            SetTextBoxValue(txtReference2Phone, reader["Reference2Phone"]);
                            SetTextBoxValue(txtReference2Email, reader["Reference2Email"]);
                            SetTextBoxValue(txtReference2Years, reader["Reference2Years"]);
                            SetTextBoxValue(txtReference3Name, reader["Reference3Name"]);
                            SetTextBoxValue(txtReference3Phone, reader["Reference3Phone"]);
                            SetTextBoxValue(txtReference3Email, reader["Reference3Email"]);
                            SetTextBoxValue(txtReference3Years, reader["Reference3Years"]);

                            // DIDD Information
                            SetTextBoxValue(txtDIDDFullName, reader["DIDDFullName"]);
                            SetTextBoxValue(txtDIDDSSN, reader["DIDDSSN"]);
                            SetTextBoxValue(txtDIDDDateOfBirth, reader["DIDDDateOfBirth"]);
                            SetTextBoxValue(txtDIDDDriversLicense, reader["DIDDDriversLicense"]);
                            SetTextBoxValue(txtDIDDWitness, reader["DIDDWitness"]);

                            // Background Check Information
                            SetTextBoxValue(txtBGLastName, reader["BGLastName"]);
                            SetTextBoxValue(txtBGFirstName, reader["BGFirstName"]);
                            SetTextBoxValue(txtBGMiddleName, reader["BGMiddleName"]);
                            SetTextBoxValue(txtBGStreet, reader["BGStreet"]);
                            SetTextBoxValue(txtBGCity, reader["BGCity"]);
                            SetTextBoxValue(txtBGState, reader["BGState"]);
                            SetTextBoxValue(txtBGZipCode, reader["BGZipCode"]);
                            SetTextBoxValue(txtBGSSN, reader["BGSSN"]);
                            SetTextBoxValue(txtBGPhone, reader["BGPhone"]);
                            SetTextBoxValue(txtBGOtherName, reader["BGOtherName"]);
                            SetTextBoxValue(txtBGNameChangeYear, reader["BGNameChangeYear"]);
                            SetTextBoxValue(txtBGDriversLicense, reader["BGDriversLicense"]);
                            SetTextBoxValue(txtBGDLState, reader["BGDLState"]);
                            SetTextBoxValue(txtBGDateOfBirth, reader["BGDateOfBirth"]);
                            SetTextBoxValue(txtBGNameOnLicense, reader["BGNameOnLicense"]);

                            // Protection from Abuse
                            SetRadioButtonValue(rbConvictedYes, rbConvictedNo, reader["ProtectionNoAbuse"]);
                            SetRadioButtonValue(rbConvictedYes, rbConvictedNo, reader["ProtectionHadAbuse"]);
                            SetTextBoxValue(txtProtectionWitness, reader["ProtectionWitness"]);

                            // Authorization
                            SetTextBoxValue(txtReferenceAuthName, reader["ReferenceAuthName"]);
                            SetTextBoxValue(txtSSNLast4, reader["SSNLast4"]);
                            SetTextBoxValue(txtApplicantSignature, reader["ApplicantSignature"]);
                            SetTextBoxValue(txtSignatureDate, reader["SignatureDate"]);
                            SetCheckBoxValue(chkFinalAcknowledgment, reader["FinalAcknowledgment"]);
                        }
                    }
                }
            }
        }

        #endregion

        #region Validation

        private bool ValidateApplication()
        {
            List<string> errors = new List<string>();

            // Required fields validation
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
                errors.Add("First Name is required");
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
                errors.Add("Last Name is required");
            if (string.IsNullOrWhiteSpace(txtHomeAddress.Text))
                errors.Add("Home Address is required");
            if (string.IsNullOrWhiteSpace(txtCity.Text))
                errors.Add("City is required");
            if (string.IsNullOrWhiteSpace(txtState.Text))
                errors.Add("State is required");
            if (string.IsNullOrWhiteSpace(txtZipCode.Text))
                errors.Add("ZIP Code is required");
            if (string.IsNullOrWhiteSpace(txtPosition1.Text))
                errors.Add("At least one position preference is required");

            // Final acknowledgment required for submission
            if (!chkFinalAcknowledgment.Checked)
                errors.Add("Final acknowledgment must be checked to submit application");

            if (errors.Count > 0)
            {
                ShowMessage("Please correct the following errors:<br/>" + string.Join("<br/>", errors), "error");
                return false;
            }

            return true;
        }

        #endregion

        #region Helper Methods

        private string GenerateApplicationNumber()
        {
            string year = DateTime.Now.Year.ToString();
            string prefix = "APP" + year + "-";

            // Get next sequence number from database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = @"
                    SELECT ISNULL(MAX(CAST(RIGHT(ApplicationNumber, 4) AS INT)), 0) + 1 
                    FROM [JobApplications] 
                    WHERE ApplicationNumber LIKE @Prefix + '%'";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Prefix", prefix);
                    int nextNumber = (int)command.ExecuteScalar();
                    return prefix + nextNumber.ToString("0000");
                }
            }
        }

        private void ShowMessage(string message, string messageType)
        {
            lblMessage.Text = message;
            pnlMessages.CssClass = "message-panel " + messageType;
            pnlMessages.Visible = true;
        }

        private void DisableFormAfterSubmission()
        {
            // Disable all form controls
            foreach (Control control in Page.Controls)
            {
                DisableControlsRecursively(control);
            }

            // Hide save and submit buttons
            btnSaveDraft.Visible = false;
            btnSubmitApplication.Visible = false;
        }

        private void DisableControlsRecursively(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.ReadOnly = true;
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.Enabled = false;
                }
                else if (control is RadioButton radioButton)
                {
                    radioButton.Enabled = false;
                }
                else if (control is DropDownList dropDown)
                {
                    dropDown.Enabled = false;
                }

                if (control.HasControls())
                {
                    DisableControlsRecursively(control);
                }
            }
        }

        // Control value helper methods
  
      

        private bool GetRadioButtonValue(RadioButton radioButton)
        {
            return radioButton?.Checked ?? false;
        }

      

        private object GetRadioButtonBooleanValue(RadioButton yesButton, RadioButton noButton)
        {
            if (yesButton == null && noButton == null)
                return DBNull.Value;

            if (yesButton?.Checked == true)
                return true;

            if (noButton?.Checked == true)
                return false;

            // Neither button is checked - return null
            return DBNull.Value;
        }

        private void SetTextBoxValue(TextBox textBox, object value)
        {
            if (textBox != null && value != null && value != DBNull.Value)
            {
                textBox.Text = value.ToString();
            }
        }
        protected void cvFinalAcknowledgment_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = chkFinalAcknowledgment.Checked;
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
            if (value != null && value != DBNull.Value)
            {
                bool boolValue = Convert.ToBoolean(value);
                if (yesButton != null) yesButton.Checked = boolValue;
                if (noButton != null) noButton.Checked = !boolValue;
            }
        }
        private void ShowTab(string tabName)
        {
            // Reset all tab buttons to inactive state
            btnTabPersonal.CssClass = "tab-button";
            btnTabPosition.CssClass = "tab-button";
            btnTabBackground.CssClass = "tab-button";
            btnTabEducation.CssClass = "tab-button";
            btnTabEmployment.CssClass = "tab-button";
            btnTabReferences.CssClass = "tab-button";
            btnTabAuthorization.CssClass = "tab-button";

            // Hide all tab content using CSS classes (not Visible property)
            pnlPersonalTab.CssClass = "tab-content";
            pnlPositionTab.CssClass = "tab-content";
            pnlBackgroundTab.CssClass = "tab-content";
            pnlEducationTab.CssClass = "tab-content";
            pnlEmploymentTab.CssClass = "tab-content";
            pnlReferencesTab.CssClass = "tab-content";
            pnlAuthorizationTab.CssClass = "tab-content";

            // Show selected tab and set button active based on tab name
            switch (tabName.ToLower())
            {
                case "personal":
                    btnTabPersonal.CssClass = "tab-button active";
                    pnlPersonalTab.CssClass = "tab-content active";
                    break;
                case "position":
                    btnTabPosition.CssClass = "tab-button active";
                    pnlPositionTab.CssClass = "tab-content active";
                    break;
                case "background":
                    btnTabBackground.CssClass = "tab-button active";
                    pnlBackgroundTab.CssClass = "tab-content active";
                    break;
                case "education":
                    btnTabEducation.CssClass = "tab-button active";
                    pnlEducationTab.CssClass = "tab-content active";
                    break;
                case "employment":
                    btnTabEmployment.CssClass = "tab-button active";
                    pnlEmploymentTab.CssClass = "tab-content active";
                    break;
                case "references":
                    btnTabReferences.CssClass = "tab-button active";
                    pnlReferencesTab.CssClass = "tab-content active";
                    break;
                case "authorization":
                    btnTabAuthorization.CssClass = "tab-button active";
                    pnlAuthorizationTab.CssClass = "tab-content active";
                    break;
            }

            // Store current tab in hidden field for navigation purposes
            hfCurrentTab.Value = tabName;

            // Update the UpdatePanel to refresh the UI
            upMain.Update();
        }

        private DateTime GetDateTimeOrNow(string dateText)
        {
            if (DateTime.TryParse(dateText, out DateTime result))
            {
                return result;
            }
            return DateTime.Now;
        }
        private object GetTextBoxValue(TextBox textBox)
        {
            if (textBox == null || string.IsNullOrWhiteSpace(textBox.Text))
            {
                return DBNull.Value;  // Return DBNull.Value instead of empty string
            }
            return textBox.Text.Trim();
        }

        // 2. UPDATE GetCheckBoxValue method (this one is probably fine already)
        private bool GetCheckBoxValue(CheckBox checkBox)
        {
            return checkBox?.Checked ?? false;
        }

        private object GetSalaryType()
        {
            if (rbHourly?.Checked == true) return "Hourly";
            if (rbYearly?.Checked == true) return "Yearly";
            return DBNull.Value; // Return null if neither is selected
        }

        private object GetEmploymentType()
        {
            if (rbFullTime?.Checked == true) return "Full Time";
            if (rbPartTime?.Checked == true) return "Part Time";
            if (rbTemporary?.Checked == true) return "Temporary";
            return DBNull.Value; // Return null if neither is selected
        }
        #endregion
    }
}