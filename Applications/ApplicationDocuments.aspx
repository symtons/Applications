<%@ Page Title="Upload Supporting Documents" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ApplicationDocuments.aspx.cs" Inherits="TPASystem2.Applications.ApplicationDocuments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <style>

        /* ===============================================
   APPLICATION DOCUMENTS PAGE STYLES
   Add these styles to tpa-common.css
   =============================================== */

/* Application Info in Header */
.application-info {
    background: rgba(255, 255, 255, 0.2);
    padding: 1rem 1.5rem;
    border-radius: 8px;
    margin-top: 1rem;
    font-size: 1rem;
    border: 1px solid rgba(255, 255, 255, 0.3);
}

/* Upload Section Styling */
.upload-section {
    background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
    padding: 2rem;
    border-radius: 12px;
    border: 2px dashed #cbd5e1;
    margin-bottom: 2rem;
    transition: all 0.3s ease;
}

.upload-section:hover {
    border-color: var(--tpa-primary);
    background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%);
}

/* File Upload Input Enhancement */
.file-upload {
    position: relative;
    background: #ffffff;
    border: 2px solid #e2e8f0;
    border-radius: 8px;
    padding: 1.2rem;
    cursor: pointer;
    transition: all 0.3s ease;
    font-size: 0.95rem;
}

.file-upload:hover {
    border-color: var(--tpa-primary);
    background: #f8fafc;
    box-shadow: 0 4px 12px rgba(25, 118, 210, 0.1);
}

.file-upload:focus {
    outline: none;
    border-color: var(--tpa-primary);
    box-shadow: 0 0 0 3px rgba(25, 118, 210, 0.1);
}

/* Empty Documents State */
.empty-documents {
    text-align: center;
    padding: 3rem 2rem;
    background: linear-gradient(135deg, #f8fafc 0%, #fff 100%);
    border: 2px dashed #e2e8f0;
    border-radius: 12px;
    color: #64748b;
}

.empty-documents .material-icons {
    font-size: 4rem;
    color: #cbd5e1;
    margin-bottom: 1rem;
    display: block;
}

.empty-documents p {
    margin: 0.5rem 0;
    font-size: 1rem;
    font-weight: 500;
}

.empty-documents small {
    font-size: 0.85rem;
    color: #94a3b8;
}

/* Documents Grid */
.documents-grid {
    display: grid;
    grid-template-columns: 1fr;
    gap: 1.5rem;
}

/* Document Item Enhanced */
.document-item {
    display: flex;
    align-items: center;
    background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
    padding: 1.5rem;
    border-radius: 16px;
    border: 2px solid #e2e8f0;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
    transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
    gap: 1.5rem;
    position: relative;
    overflow: hidden;
}

.document-item::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    width: 4px;
    height: 100%;
    background: linear-gradient(180deg, var(--tpa-primary), var(--tpa-secondary));
    opacity: 0;
    transition: opacity 0.3s ease;
}

.document-item:hover {
    transform: translateY(-4px);
    box-shadow: 0 8px 32px rgba(25, 118, 210, 0.15);
    border-color: var(--tpa-primary);
}

.document-item:hover::before {
    opacity: 1;
}

/* Document Icon Enhanced */
.document-icon {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 70px;
    height: 70px;
    background: linear-gradient(135deg, var(--tpa-primary), var(--tpa-secondary));
    border-radius: 16px;
    flex-shrink: 0;
    box-shadow: 0 4px 16px rgba(25, 118, 210, 0.3);
    transition: all 0.3s ease;
}

.document-item:hover .document-icon {
    transform: scale(1.05);
    box-shadow: 0 6px 20px rgba(25, 118, 210, 0.4);
}

.document-icon .material-icons {
    color: white;
    font-size: 2rem;
}

/* Document Info Enhanced */
.document-info {
    flex: 1;
    min-width: 0;
}

.document-info h5 {
    margin: 0 0 0.5rem 0;
    font-size: 1.1rem;
    font-weight: 600;
    color: #1e293b;
    word-break: break-word;
    line-height: 1.3;
}

.document-category {
    margin: 0 0 0.5rem 0;
    font-size: 0.85rem;
    font-weight: 600;
    color: white;
    background: linear-gradient(135deg, var(--tpa-primary), var(--tpa-secondary));
    padding: 0.3rem 0.75rem;
    border-radius: 20px;
    display: inline-block;
    box-shadow: 0 2px 8px rgba(25, 118, 210, 0.2);
}

.document-details {
    margin: 0 0 0.25rem 0;
    font-size: 0.8rem;
    color: #64748b;
    text-transform: uppercase;
    letter-spacing: 0.05em;
    font-weight: 500;
}

.document-date {
    margin: 0 0 0.25rem 0;
    font-size: 0.8rem;
    color: #94a3b8;
    font-style: italic;
}

.document-description {
    margin: 0.5rem 0 0 0;
    font-size: 0.85rem;
    color: #64748b;
    font-style: italic;
    background: rgba(100, 116, 139, 0.1);
    padding: 0.5rem;
    border-radius: 6px;
    border-left: 3px solid var(--tpa-primary);
}

/* Document Actions Enhanced */
.document-actions {
    display: flex;
    gap: 0.75rem;
    flex-shrink: 0;
}

.btn-action {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 44px;
    height: 44px;
    border-radius: 12px;
    text-decoration: none;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    border: none;
    cursor: pointer;
    position: relative;
    overflow: hidden;
}

.btn-action::before {
    content: '';
    position: absolute;
    top: 50%;
    left: 50%;
    width: 0;
    height: 0;
    border-radius: 50%;
    transition: all 0.3s ease;
    transform: translate(-50%, -50%);
    z-index: 0;
}

.btn-action:hover::before {
    width: 100px;
    height: 100px;
}

.btn-action .material-icons {
    font-size: 1.3rem;
    position: relative;
    z-index: 1;
}

.btn-action.view {
    background: rgba(34, 197, 94, 0.1);
    color: #22c55e;
    border: 2px solid rgba(34, 197, 94, 0.2);
}

.btn-action.view::before {
    background: rgba(34, 197, 94, 0.1);
}

.btn-action.view:hover {
    color: #16a34a;
    border-color: #22c55e;
    transform: scale(1.1);
    box-shadow: 0 4px 16px rgba(34, 197, 94, 0.3);
}

.btn-action.download {
    background: rgba(59, 130, 246, 0.1);
    color: #3b82f6;
    border: 2px solid rgba(59, 130, 246, 0.2);
}

.btn-action.download::before {
    background: rgba(59, 130, 246, 0.1);
}

.btn-action.download:hover {
    color: #2563eb;
    border-color: #3b82f6;
    transform: scale(1.1);
    box-shadow: 0 4px 16px rgba(59, 130, 246, 0.3);
}

.btn-action.delete {
    background: rgba(239, 68, 68, 0.1);
    color: #ef4444;
    border: 2px solid rgba(239, 68, 68, 0.2);
}

.btn-action.delete::before {
    background: rgba(239, 68, 68, 0.1);
}

.btn-action.delete:hover {
    color: #dc2626;
    border-color: #ef4444;
    transform: scale(1.1);
    box-shadow: 0 4px 16px rgba(239, 68, 68, 0.3);
}

/* Form Footer Enhanced */
.form-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 2rem 0;
    border-top: 2px solid #e2e8f0;
    margin-top: 2rem;
}

.form-footer-left,
.form-footer-right {
    display: flex;
    gap: 1rem;
    align-items: center;
}

/* Button Styles */
.btn-secondary {
    background: #6b7280;
    color: white;
    border: none;
    padding: 1rem 2rem;
    border-radius: 12px;
    font-size: 0.95rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    gap: 0.5rem;
    box-shadow: 0 2px 8px rgba(107, 114, 128, 0.3);
}

.btn-secondary:hover {
    background: #4b5563;
    transform: translateY(-2px);
    box-shadow: 0 4px 16px rgba(107, 114, 128, 0.4);
}

.btn-outline {
    background: transparent;
    color: var(--tpa-primary);
    border: 2px solid var(--tpa-primary);
    padding: 1rem 2rem;
    border-radius: 12px;
    font-size: 0.95rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.btn-outline:hover {
    background: var(--tpa-primary);
    color: white;
    transform: translateY(-2px);
    box-shadow: 0 4px 16px rgba(25, 118, 210, 0.3);
}

.btn-tpa {
    background: linear-gradient(135deg, var(--tpa-primary), var(--tpa-secondary));
    color: white;
    border: none;
    padding: 1rem 2rem;
    border-radius: 12px;
    font-size: 0.95rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    gap: 0.5rem;
    box-shadow: 0 4px 16px rgba(25, 118, 210, 0.3);
}

.btn-tpa:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 20px rgba(25, 118, 210, 0.4);
}

/* Alert Panel */
.alert-panel {
    display: flex;
    align-items: center;
    padding: 1.25rem 1.5rem;
    margin: 1.5rem 0;
    border-radius: 12px;
    border: 2px solid;
    font-size: 0.95rem;
    font-weight: 500;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    gap: 1rem;
}

.alert-icon {
    font-size: 1.5rem;
}

.alert-panel.success {
    background: linear-gradient(135deg, #dcfce7 0%, #bbf7d0 100%);
    color: #166534;
    border-color: #22c55e;
}

.alert-panel.error {
    background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%);
    color: #991b1b;
    border-color: #ef4444;
}

.alert-panel.warning {
    background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
    color: #92400e;
    border-color: #fbbf24;
}

.alert-panel.info {
    background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
    color: #1e40af;
    border-color: #3b82f6;
}

/* Responsive Design */
@media (max-width: 768px) {
    .upload-section {
        padding: 1.5rem;
    }
    
    .document-item {
        flex-direction: column;
        align-items: flex-start;
        gap: 1rem;
        padding: 1.25rem;
    }
    
    .document-actions {
        width: 100%;
        justify-content: center;
        margin-top: 1rem;
    }
    
    .document-icon {
        width: 60px;
        height: 60px;
    }
    
    .document-icon .material-icons {
        font-size: 1.5rem;
    }
    
    .btn-action {
        width: 40px;
        height: 40px;
    }
    
    .form-footer {
        flex-direction: column;
        gap: 1.5rem;
        text-align: center;
    }
    
    .form-footer-left,
    .form-footer-right {
        width: 100%;
        justify-content: center;
    }
    
    .btn-secondary,
    .btn-outline,
    .btn-tpa {
        width: 100%;
        justify-content: center;
    }
}

@media (max-width: 480px) {
    .documents-grid {
        gap: 1rem;
    }
    
    .document-item {
        padding: 1rem;
    }
    
    .document-info h5 {
        font-size: 1rem;
    }
    
    .application-info {
        padding: 0.75rem 1rem;
        font-size: 0.9rem;
    }
    
    .empty-documents {
        padding: 2rem 1rem;
    }
    
    .empty-documents .material-icons {
        font-size: 3rem;
    }
}
    </style>

 

    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <!-- Welcome Header - Matching EmployeeLeavePortal Style -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1><span class="material-icons">attach_file</span>Upload Supporting Documents</h1>
                <p>Upload any supporting documents for your job application (letters of recommendation, certificates, transcripts, etc.)</p>
                <div class="application-info">
                    <strong>Job Application:</strong> 
                    <asp:Literal ID="litApplicationInfo" runat="server"></asp:Literal>
                </div>
            </div>
        </div>
    </div>

    <!-- Hidden Fields -->
    <asp:HiddenField ID="hfApplicationId" runat="server" />

    <!-- Main Content -->
    <div class="form-container">
        <!-- Upload Section - OUTSIDE UpdatePanel -->
        <div class="form-section">
            <div class="section-header">
                <h4><span class="material-icons">cloud_upload</span>Upload Documents</h4>
            </div>

            <div class="upload-section">
                <div class="form-grid">
                    <div class="form-group">
                        <label class="form-label">Select Document:</label>
                        <asp:FileUpload ID="fuDocument" runat="server" 
                                      CssClass="form-input file-upload" 
                                      accept=".pdf,.doc,.docx,.jpg,.jpeg,.png"
                                      ClientIDMode="Static" />
                        <small class="form-help">Accepted formats: PDF, Word documents, Images (Max 10MB)</small>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Document Description:</label>
                        <asp:TextBox ID="txtDocumentDescription" runat="server" 
                                   CssClass="form-input" 
                                   placeholder="Brief description of the document"></asp:TextBox>
                    </div>
                </div>
                
                <div class="form-grid">
                    <div class="form-group">
                        <label class="form-label">Document Category:</label>
                        <asp:DropDownList ID="ddlDocumentCategory" runat="server" CssClass="form-input">
                            <asp:ListItem Value="REFERENCE_LETTER">Letter of Recommendation</asp:ListItem>
                            <asp:ListItem Value="CERTIFICATE">Certificate/Credential</asp:ListItem>
                            <asp:ListItem Value="TRANSCRIPT">Transcript/Educational Record</asp:ListItem>
                            <asp:ListItem Value="LICENSE">Professional License</asp:ListItem>
                            <asp:ListItem Value="RESUME">Resume/CV</asp:ListItem>
                            <asp:ListItem Value="OTHER" Selected="True">Other</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label class="form-label">&nbsp;</label>
                        <asp:Button ID="btnUploadDocument" runat="server" 
                                  Text="Upload Document" 
                                  CssClass="btn-tpa" 
                                  OnClick="btnUploadDocument_Click" 
                                  UseSubmitBehavior="false" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Uploaded Documents List - INSIDE UpdatePanel -->
        <div class="form-section">
            <div class="section-header">
                <h4><span class="material-icons">folder</span>Uploaded Documents</h4>
            </div>

            <asp:UpdatePanel ID="upDocumentsList" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="pnlNoDocuments" runat="server" Visible="true" CssClass="empty-documents">
                        <span class="material-icons">description</span>
                        <p>No documents uploaded yet</p>
                        <small>You can skip this step if you don't have supporting documents to upload</small>
                    </asp:Panel>
                    
                    <asp:Repeater ID="rptUploadedDocuments" runat="server" OnItemCommand="rptUploadedDocuments_ItemCommand">
                        <HeaderTemplate>
                            <div class="documents-grid">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="document-item">
                                <div class="document-icon">
                                    <span class="material-icons">
                                        <%# GetFileIcon(Eval("FileExtension").ToString()) %>
                                    </span>
                                </div>
                                <div class="document-info">
                                    <h5><%# Eval("DocumentName") %></h5>
                                    <p class="document-category"><%# FormatCategory(Eval("Category").ToString()) %></p>
                                    <p class="document-details">
                                        <%# Eval("FileExtension") %> • <%# FormatFileSize(Convert.ToInt64(Eval("FileSize"))) %>
                                    </p>
                                    <p class="document-date">Uploaded: <%# Convert.ToDateTime(Eval("CreatedDate")).ToString("MMM dd, yyyy") %></p>
                                    <%# !string.IsNullOrEmpty(Eval("Description").ToString()) ? "<p class=\"document-description\">" + Eval("Description") + "</p>" : "" %>
                                </div>
                                <div class="document-actions">
                                    <asp:LinkButton ID="btnViewDocument" runat="server" 
                                                  CommandName="ViewDocument" 
                                                  CommandArgument='<%# Eval("Id") %>' 
                                                  CssClass="btn-action view"
                                                  ToolTip="View Document">
                                        <span class="material-icons">visibility</span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDownloadDocument" runat="server" 
                                                  CommandName="DownloadDocument" 
                                                  CommandArgument='<%# Eval("Id") %>' 
                                                  CssClass="btn-action download"
                                                  ToolTip="Download Document">
                                        <span class="material-icons">download</span>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDeleteDocument" runat="server" 
                                                  CommandName="DeleteDocument" 
                                                  CommandArgument='<%# Eval("Id") %>' 
                                                  CssClass="btn-action delete"
                                                  ToolTip="Delete Document"
                                                  OnClientClick="return confirm('Are you sure you want to delete this document?');">
                                        <span class="material-icons">delete</span>
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnUploadDocument" />
                </Triggers>
            </asp:UpdatePanel>
        </div>

        <!-- Navigation Buttons -->
        <div class="form-section">
            <div class="form-footer">
                <div class="form-footer-left">
                    <asp:Button ID="btnBack" runat="server" 
                              Text="← Back to Job Application" 
                              CssClass="btn-secondary" 
                              OnClick="btnBack_Click" />
                </div>
                <div class="form-footer-right">
                    <asp:Button ID="btnSkip" runat="server" 
                              Text="Skip & Continue" 
                              CssClass="btn-outline" 
                              OnClick="btnContinue_Click" />
                    <asp:Button ID="btnContinue" runat="server" 
                              Text="Continue to Success Page" 
                              CssClass="btn-tpa" 
                              OnClick="btnContinue_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Success/Error Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert-panel">
        <span class="material-icons alert-icon">info</span>
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

</asp:Content>