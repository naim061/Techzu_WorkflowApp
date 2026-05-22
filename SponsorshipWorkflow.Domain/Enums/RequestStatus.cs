using System;
using System.Collections.Generic;
using System.Text;

namespace SponsorshipWorkflow.Domain.Enums
{


    public enum RequestStatus
    {
        Draft,
        PendingManagerApproval,
        PendingFinanceReview,
        Approved,
        Rejected,
        Cancelled
    }
    public enum UserRole
    {
        Requestor = 1,
        Manager = 2,
        FinanceAdmin = 3,
        SystemAdmin = 4
    }

    public enum WorkflowAction
    {
        Create,
        Submit,
        ManagerApprove,
        ManagerReject,
        FinanceApprove,
        FinanceReject,
        Cancel,
        SaveDraft
    }



}
