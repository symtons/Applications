<%@ Page Title="Application Submitted Successfully" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ApplicationSuccess.aspx.cs" Inherits="Applications.ApplicationSuccess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- Success Page Header -->
    <div class="onboarding-page-header">
        <div class="header-content">
            <div class="header-left">
                <h1>
                    <span class="material-icons" style="color: #10b981;">check_circle</span>
                    Application Successfully Submitted!
                </h1>
                <p class="page-description">Your application has been received and saved in our system.</p>
            </div>
        </div>
    </div>

    <div class="application-container">
        
        <!-- Success Information Section -->
        <div class="form-section">
            <div class="section-header">
                <h2><span class="material-icons">info</span>Application Details</h2>
            </div>
            
            <div class="message-panel success" style="margin-bottom: 2rem;">
                <div class="message-content">
                    <div class="success-details">
                        <div class="detail-row">
                            <strong>Application Number:</strong> 
                            <asp:Label ID="lblApplicationNumber" runat="server" CssClass="application-number"></asp:Label>
                        </div>
                        <div class="detail-row">
                            <strong>Submitted Date:</strong> 
                            <asp:Label ID="lblSubmissionDate" runat="server"></asp:Label>
                        </div>
                        <div class="detail-row">
                            <strong>Applicant Name:</strong> 
                            <asp:Label ID="lblApplicantName" runat="server"></asp:Label>
                        </div>
                        <div class="detail-row">
                            <strong>Status:</strong> 
                            <span class="status-badge">Submitted</span>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Instructions -->
            <div class="form-section">
                <h3>What happens next?</h3>
                <div class="next-steps">
                    <div class="step-item">
                        <span class="step-number">1</span>
                        <div class="step-content">
                            <h4>Application Review</h4>
                            <p>Our HR team will review your application within 3-5 business days.</p>
                        </div>
                    </div>
                    <div class="step-item">
                        <span class="step-number">2</span>
                        <div class="step-content">
                            <h4>Initial Contact</h4>
                            <p>If your qualifications match our requirements, we will contact you to schedule an interview.</p>
                        </div>
                    </div>
                    <div class="step-item">
                        <span class="step-number">3</span>
                        <div class="step-content">
                            <h4>Follow-up</h4>
                            <p>Keep an eye on your email and phone for updates on your application status.</p>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Action Buttons -->
            <div class="action-section">
                <div class="action-buttons">
                    <asp:Button ID="btnDownloadPDF" runat="server" 
                               Text="Download Application PDF" 
                               CssClass="btn-tpa btn-primary"
                               OnClick="btnDownloadPDF_Click" />
                    
                    <asp:Button ID="btnPrintApplication" runat="server" 
                               Text="Print Application" 
                               CssClass="btn-tpa btn-secondary"
                               OnClick="btnPrintApplication_Click" />
                               
                    <asp:HyperLink ID="lnkBackToDashboard" runat="server" 
                                  NavigateUrl="~/Default.aspx" 
                                  CssClass="btn-tpa btn-secondary">
                        Back to Dashboard
                    </asp:HyperLink>
                </div>
            </div>

            <!-- Contact Information -->
            <div class="contact-info-section">
                <h3>Need Help?</h3>
                <div class="contact-details">
                    <div class="contact-item">
                        <span class="material-icons">email</span>
                        <div>
                            <strong>Email:</strong> hr@tennesseepersonalassistance.com
                        </div>
                    </div>
                    <div class="contact-item">
                        <span class="material-icons">phone</span>
                        <div>
                            <strong>Phone:</strong> (615) 555-0123
                        </div>
                    </div>
                    <div class="contact-item">
                        <span class="material-icons">schedule</span>
                        <div>
                            <strong>Business Hours:</strong> Monday - Friday, 8:00 AM - 5:00 PM CST
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>

    <!-- Add CSS styles to tpa-common.css -->
    <style>
        .success-details {
            display: flex;
            flex-direction: column;
            gap: 0.75rem;
        }

        .detail-row {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            font-size: 1rem;
        }

        .application-number {
            font-family: 'Courier New', monospace;
            background: #f3f4f6;
            padding: 0.25rem 0.5rem;
            border-radius: 4px;
            font-weight: bold;
        }

        .status-badge {
            background: #10b981;
            color: white;
            padding: 0.25rem 0.75rem;
            border-radius: 20px;
            font-size: 0.875rem;
            font-weight: 600;
        }

        .next-steps {
            display: flex;
            flex-direction: column;
            gap: 1.5rem;
            margin-top: 1rem;
        }

        .step-item {
            display: flex;
            align-items: flex-start;
            gap: 1rem;
        }

        .step-number {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 2rem;
            height: 2rem;
            background: #1976d2;
            color: white;
            border-radius: 50%;
            font-weight: bold;
            flex-shrink: 0;
        }

        .step-content h4 {
            margin: 0 0 0.5rem 0;
            color: #1976d2;
            font-weight: 600;
        }

        .step-content p {
            margin: 0;
            color: #6b7280;
            line-height: 1.5;
        }

        .action-section {
            margin: 2rem 0;
            padding: 2rem;
            background: #f8f9fa;
            border-radius: 8px;
            text-align: center;
        }

        .action-buttons {
            display: flex;
            gap: 1rem;
            justify-content: center;
            flex-wrap: wrap;
        }

        .contact-info-section {
            margin-top: 2rem;
            padding: 1.5rem;
            background: #f1f5f9;
            border-radius: 8px;
        }

        .contact-details {
            display: flex;
            flex-direction: column;
            gap: 1rem;
            margin-top: 1rem;
        }

        .contact-item {
            display: flex;
            align-items: center;
            gap: 0.75rem;
        }

        .contact-item .material-icons {
            color: #1976d2;
        }

        @media (max-width: 768px) {
            .action-buttons {
                flex-direction: column;
                align-items: center;
            }

            .action-buttons .btn-tpa {
                width: 100%;
                max-width: 280px;
            }
        }
    </style>

</asp:Content>