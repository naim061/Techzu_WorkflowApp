# Techzu_WorkflowApp

╔══════════════════════════════════════════════════════════════════════════════╗
║           SPONSORSHIP REQUEST APPROVAL WORKFLOW - COMPLETE REFERENCE        ║
║                    Backend Architecture + API + Database + Workflow          ║
╚══════════════════════════════════════════════════════════════════════════════╝


═══════════════════════════════════════════════════════════════════════════════
SECTION 1: ARCHITECTURE
═══════════════════════════════════════════════════════════════════════════════

┌─────────────────────────────────────────────────────────────────────┐
│                        CLEAN ARCHITECTURE                          │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │                    API LAYER (Presentation)                  │   │
│  │                                                              │   │
│  │  Controllers/        Middleware/        Extensions/          │   │
│  │  ├── AuthController      ├── GlobalExceptionMiddleware      │   │
│  │  ├── SponsorshipReq...   │                                   │   │
│  │  ├── SponsorshipType...  ├── JwtExtensions                  │   │
│  │  └── BaseApiController   └── SwaggerExtensions              │   │
│  │                                                              │   │
│  │  Program.cs (Composition Root)                               │   │
│  │  • Register all services  • Configure JWT, CORS, Swagger    │   │
│  │  • Auto-migrate + Seed database                              │   │
│  └───────────────────────────┬──────────────────────────────────┘   │
│                              │                                      │
│                              ▼                                      │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │                 APPLICATION LAYER (Business Logic)           │   │
│  │                                                              │   │
│  │  Services/              DTOs/              Interfaces/       │   │
│  │  ├── AuthService        ├── Auth DTOs      ├── IAuthService  │   │
│  │  ├── SponsorshipReq...  ├── Sponsor DTOs   ├── ISponsor...   │   │
│  │  ├── SponsorshipType..  ├── Type DTOs      ├── ITokenService │   │
│  │  └── (No EF/DB refs)    └── User DTOs      ├── IPassword...  │   │
│  │                                            ├── ICurrent...   │   │
│  │  Common/                                          └── IUnit..│   │
│  │  ├── ApiResponse<T>                                             │   │
│  │  ├── CurrentUser                                                │   │
│  │  └── Exceptions/                                                │   │
│  │      ├── AppException, NotFoundException                        │   │
│  │      ├── UnauthorizedException, ForbiddenException              │   │
│  │      ├── ValidationException, ConflictException                 │   │
│  │      └── WorkflowException                                      │   │
│  └───────────────────────────┬──────────────────────────────────┘   │
│                              │                                      │
│                              ▼                                      │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │               INFRASTRUCTURE LAYER (External)                │   │
│  │                                                              │   │
│  │  Data/                  Repositories/       Services/        │   │
│  │  ├── AppDbContext        ├── GenericRepo      ├── TokenService│   │
│  │  └── Configurations/     ├── UserRepo         ├── PasswordSvc│   │
│  │      ├── UserConfig      ├── Sponsorship...   └── CurrentUser│   │
│  │      ├── RequestConfig   ├── RefreshToken...                │   │
│  │      ├── RoleConfig      └── (EF Core)      Extensions/     │   │
│  │      ├── SponsorshipType..                    └── InfraSvc   │   │
│  │      ├── SupportingDoc..                              Extensions│   │
│  │      └── WorkflowHistory..                                     │   │
│  │  UnitOfWork/                                                  │   │
│  │  └── UnitOfWork (transaction + save)                         │   │
│  └───────────────────────────┬──────────────────────────────────┘   │
│                              │                                      │
│                              ▼                                      │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │                     DOMAIN LAYER (Core)                      │   │
│  │                                                              │   │
│  │  Entities/          Enums/            Interfaces/            │   │
│  │  ├── User            ├── RequestStatus  ├── IGenericRepo     │   │
│  │  ├── Role            ├── UserRole       ├── IUserRepo        │   │
│  │  ├── RefreshToken    └── WorkflowAction ├── ISponsorship...  │   │
│  │  ├── SponsorshipReq                      ├── IRefreshToken.. │   │
│  │  ├── SponsorshipType Common/             ├── IUnitOfWork     │   │
│  │  ├── SupportingDoc └── BaseEntity                          │   │
│  │  └── WorkflowHist                                          │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │                    DATABASE (SQL Server)                      │   │
│  │                                                              │   │
│  │  Roles ← Users ← RefreshTokens                              │   │
│  │                    Users ← SponsorshipRequests ← Documents   │   │
│  │                                ↑                              │   │
│  │  SponsorshipTypes ← ───────────┘                             │   │
│  │  Users ← ── WorkflowHistories ──→ SponsorshipRequests        │   │
│  └──────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘


PROJECT STRUCTURE:
  D:\SponsorshipWorkflow\
  ├── SponsorshipWorkflow.slnx
  └── src\
      ├── SponsorshipWorkflow.Domain\
      │   ├── Entities\
      │   │   ├── User.cs
      │   │   ├── Role.cs
      │   │   ├── RefreshToken.cs
      │   │   ├── SponsorshipRequest.cs
      │   │   ├── SponsorshipType.cs
      │   │   ├── SupportingDocument.cs
      │   │   └── WorkflowHistory.cs
      │   ├── Enums\
      │   │   ├── RequestStatus.cs
      │   │   ├── UserRole.cs
      │   │   └── WorkflowAction.cs
      │   ├── Common\
      │   │   └── BaseEntity.cs
      │   └── Interfaces\
      │       ├── IUnitOfWork.cs
      │       └── Repositories\
      │           ├── IGenericRepository.cs
      │           ├── IUserRepository.cs
      │           ├── ISponsorshipRequestRepository.cs
      │           └── IRefreshTokenRepository.cs
      │
      ├── SponsorshipWorkflow.Application\
      │   ├── Common\
      │   │   ├── ApiResponse.cs
      │   │   ├── CurrentUser.cs
      │   │   └── Exceptions\
      │   │       └── AppException.cs (+ all exception types)
      │   ├── DTOs\
      │   │   ├── Auth\
      │   │   │   ├── LoginRequestDto.cs
      │   │   │   ├── LoginResponseDto.cs
      │   │   │   ├── RefreshTokenRequestDto.cs
      │   │   │   └── RevokeTokenRequestDto.cs
      │   │   ├── Sponsorship\
      │   │   │   ├── CreateSponsorshipRequestDto.cs
      │   │   │   ├── UpdateSponsorshipRequestDto.cs
      │   │   │   ├── SponsorshipRequestDto.cs
      │   │   │   └── WorkflowActionDto.cs
      │   │   ├── SponsorshipType\
      │   │   │   └── SponsorshipTypeDto.cs
      │   │   └── User\
      │   │       └── UserDto.cs
      │   ├── Interfaces\
      │   │   ├── IAuthService.cs
      │   │   ├── ITokenService.cs
      │   │   ├── IPasswordService.cs
      │   │   ├── ISponsorshipRequestService.cs
      │   │   ├── ISponsorshipTypeService.cs
      │   │   └── ICurrentUserService.cs
      │   └── Services\
      │       ├── AuthService.cs
      │       ├── SponsorshipRequestService.cs
      │       └── SponsorshipTypeService.cs
      │
      ├── SponsorshipWorkflow.Infrastructure\
      │   ├── Data\
      │   │   ├── AppDbContext.cs
      │   │   └── Configurations\
      │   │       ├── UserConfiguration.cs
      │   │       ├── RoleConfiguration.cs
      │   │       ├── RefreshTokenConfiguration.cs
      │   │       ├── SponsorshipRequestConfiguration.cs
      │   │       ├── SponsorshipTypeConfiguration.cs
      │   │       ├── SupportingDocumentConfiguration.cs
      │   │       └── WorkflowHistoryConfiguration.cs
      │   ├── Repositories\
      │   │   ├── GenericRepository.cs
      │   │   ├── UserRepository.cs
      │   │   ├── SponsorshipRequestRepository.cs
      │   │   └── RefreshTokenRepository.cs
      │   ├── Services\
      │   │   ├── TokenService.cs
      │   │   ├── PasswordService.cs
      │   │   └── CurrentUserService.cs
      │   ├── UnitOfWork\
      │   │   └── UnitOfWork.cs
      │   └── Extensions\
      │       └── InfrastructureServiceExtensions.cs
      │
      └── SponsorshipWorkflow.API\
          ├── Controllers\
          │   ├── BaseApiController.cs
          │   ├── AuthController.cs
          │   ├── SponsorshipRequestsController.cs
          │   └── SponsorshipTypesController.cs
          ├── Middleware\
          │   └── GlobalExceptionMiddleware.cs
          ├── Extensions\
          │   ├── JwtExtensions.cs
          │   └── SwaggerExtensions.cs
          ├── Program.cs
          └── appsettings.json


═══════════════════════════════════════════════════════════════════════════════
SECTION 2: COMPLETE ENDPOINT LIST
═══════════════════════════════════════════════════════════════════════════════

┌─────┬────────────────────────────────────────┬──────────┬────────────┬───────────────┐
│  #  │ Endpoint                               │ Method   │ Auth       │ Role          │
├─────┼────────────────────────────────────────┼──────────┼────────────┼───────────────┤
│     │         AUTHENTICATION                 │          │            │               │
├─────┼────────────────────────────────────────┼──────────┼────────────┼───────────────┤
│  1  │ /api/Auth/login                        │ POST     │ No         │ Anyone        │
│  2  │ /api/Auth/refresh-token                │ POST     │ No         │ Anyone        │
│  3  │ /api/Auth/logout                       │ POST     │ Yes        │ Any logged in │
│  4  │ /api/Auth/revoke-token                 │ POST     │ Yes        │ Any logged in │
│  5  │ /api/Auth/me                           │ GET      │ Yes        │ Any logged in │
├─────┼────────────────────────────────────────┼──────────┼────────────┼───────────────┤
│     │      SPONSORSHIP REQUESTS (READ)       │          │            │               │
├─────┼────────────────────────────────────────┼──────────┼────────────┼───────────────┤
│  6  │ /api/SponsorshipRequests               │ GET      │ Yes        │ Admin,Mgr,Fin │
│     │   ?page=1&pageSize=10&status=X         │          │            │               │
│  7  │ /api/SponsorshipRequests/{id}          │ GET      │ Yes        │ Any (own if R)│
│  8  │ /api/SponsorshipRequests/my-requests   │ GET      │ Yes        │ Requestor     │
│  9  │ /api/SponsorshipRequests/              │ GET      │ Yes        │ Manager,Admin │
│     │   pending-manager-approval             │          │            │               │
│ 10  │ /api/SponsorshipRequests/              │ GET      │ Yes        │ FinanceAdmin, │
│     │   pending-finance-review               │          │            │ Admin         │
│ 11  │ /api/SponsorshipRequests/{id}/         │ GET      │ Yes        │ Any           │
│     │   workflow-history                     │          │            │               │
├─────┼────────────────────────────────────────┼──────────┼────────────┼───────────────┤
│     │      SPONSORSHIP REQUESTS (WRITE)      │          │            │               │
├─────┼────────────────────────────────────────┼──────────┼────────────┼───────────────┤
│ 12  │ /api/SponsorshipRequests               │ POST     │ Yes        │ Requestor     │
│ 13  │ /api/SponsorshipRequests/{id}          │ PUT      │ Yes        │ Requestor     │
│ 14  │ /api/SponsorshipRequests/{id}/submit   │ POST     │ Yes        │ Requestor     │
│ 15  │ /api/SponsorshipRequests/{id}/         │ POST     │ Yes        │ Manager       │
│     │   manager-approve                      │          │            │               │
│ 16  │ /api/SponsorshipRequests/{id}/         │ POST     │ Yes        │ Manager       │
│     │   manager-reject                       │          │            │               │
│ 17  │ /api/SponsorshipRequests/{id}/         │ POST     │ Yes        │ FinanceAdmin  │
│     │   finance-approve                      │          │            │               │
│ 18  │ /api/SponsorshipRequests/{id}/         │ POST     │ Yes        │ FinanceAdmin  │
│     │   finance-reject                       │          │            │               │
│ 19  │ /api/SponsorshipRequests/{id}/cancel   │ POST     │ Yes        │ Requestor     │
├─────┼────────────────────────────────────────┼──────────┼────────────┼───────────────┤
│     │      SPONSORSHIP TYPES                 │          │            │               │
├─────┼────────────────────────────────────────┼──────────┼────────────┼───────────────┤
│ 20  │ /api/SponsorshipTypes                  │ GET      │ Yes        │ Any           │
│ 21  │ /api/SponsorshipTypes/{id}             │ GET      │ Yes        │ Any           │
│ 22  │ /api/SponsorshipTypes                  │ POST     │ Yes        │ SystemAdmin   │
│ 23  │ /api/SponsorshipTypes/{id}             │ PUT      │ Yes        │ SystemAdmin   │
│ 24  │ /api/SponsorshipTypes/{id}             │ DELETE   │ Yes        │ SystemAdmin   │
├─────┼────────────────────────────────────────┼──────────┼────────────┼───────────────┤
│     │      OTHER                             │          │            │               │
├─────┼────────────────────────────────────────┼──────────┼────────────┼───────────────┤
│ 25  │ /health                                │ GET      │ No         │ Anyone        │
│ 26  │ /swagger                               │ GET      │ No         │ Anyone        │
└─────┴────────────────────────────────────────┴──────────┴────────────┴───────────────┘


═══════════════════════════════════════════════════════════════════════════════
SECTION 3: WORKFLOW
═══════════════════════════════════════════════════════════════════════════════

3.1 STATUS FLOW DIAGRAM

                    ┌───────┐
                    │ Draft │
                    └───┬───┘
                        │
            ┌───────────┼───────────┐
            │           │           │
       [submit]    [cancel]    [edit/draft]
            │           │           │
            ▼           ▼           │
  ┌─────────────────┐  ┌──────────┐│
  │PendingManager   │  │Cancelled ││
  │   Approval      │  └──────────┘│
  └────────┬────────┘              │
           │                       │
    ┌──────┴──────┐                │
    │             │                │
[approve]    [reject]         [cancel]
    │             │                │
    ▼             ▼                ▼
┌──────────────┐ ┌──────────┐ ┌──────────┐
│PendingFinance│ │ Rejected │ │Cancelled │
│   Review     │ └──────────┘ └──────────┘
└──────┬───────┘
       │
  ┌────┴────┐
  │         │
[approve] [reject]
  │         │
  ▼         ▼
┌─────────┐ ┌──────────┐
│Approved │ │ Rejected │
│   ✅    │ └──────────┘
└─────────┘

VALID STATUS TRANSITIONS:
  Draft -> PendingManagerApproval      (Requestor submits)
  Draft -> Cancelled                   (Requestor cancels)
  PendingManagerApproval -> PendingFinanceReview   (Manager approves)
  PendingManagerApproval -> Rejected               (Manager rejects)
  PendingManagerApproval -> Cancelled              (Requestor cancels)
  PendingFinanceReview -> Approved                  (Finance approves)
  PendingFinanceReview -> Rejected                  (Finance rejects)


3.2 ROLE-BASED ACCESS MATRIX

┌────────────────────────┬───────────┬─────────┬───────────┬────────┐
│ Action                 │ Requestor │ Manager │ FinanceAd │ System │
├────────────────────────┼───────────┼─────────┼───────────┼────────┤
│ Create Request         │    YES    │   NO    │    NO     │   NO   │
│ Edit Draft Request     │    YES    │   NO    │    NO     │   NO   │
│ Submit Request         │    YES    │   NO    │    NO     │   NO   │
│ Cancel Request         │    YES    │   NO    │    NO     │   NO   │
│ View Own Requests      │    YES    │   NO    │    NO     │   NO   │
├────────────────────────┼───────────┼─────────┼───────────┼────────┤
│ View Manager Queue     │    NO     │   YES   │    NO     │  YES   │
│ Manager Approve        │    NO     │   YES   │    NO     │   NO   │
│ Manager Reject         │    NO     │   YES   │    NO     │   NO   │
├────────────────────────┼───────────┼─────────┼───────────┼────────┤
│ View Finance Queue     │    NO     │   NO    │    YES    │  YES   │
│ Finance Approve        │    NO     │   NO    │    YES    │   NO   │
│ Finance Reject         │    NO     │   NO    │    YES    │   NO   │
├────────────────────────┼───────────┼─────────┼───────────┼────────┤
│ View All Requests      │    NO     │   YES   │    YES    │  YES   │
│ View Any Details       │    Own    │   All   │    All    │  All   │
│ View Workflow History  │    Own    │   All   │    All    │  All   │
├────────────────────────┼───────────┼─────────┼───────────┼────────┤
│ Manage Sponsor Types   │    NO     │   NO    │    NO     │  YES   │
│ Paginated Listing      │    NO     │   YES   │    YES    │  YES   │
└────────────────────────┴───────────┴─────────┴───────────┴────────┘


3.3 COMPLETE HAPPY PATH (End-to-End Workflow)

STEP  ACTION                              API CALL
──────────────────────────────────────────────────────────────
 1    John Requestor logs in              POST   /api/Auth/login
      -> Gets JWT + Refresh tokens

 2    John fetches sponsorship types      GET    /api/SponsorshipTypes
      -> Populates form dropdown

 3    John creates a request              POST   /api/SponsorshipRequests
      { title: "Tech Conference",
        department: "Marketing",
        sponsorshipTypeId: 7,
        eventOrganisationName: "Malaysia Tech Summit",
        eventDate: "2026-09-15",
        requestedAmount: 15000,
        purpose: "Brand visibility",
        submitImmediately: false }
      -> Status: Draft
      -> Returns: REQ-2026-0001

 4    John submits the request            POST   /api/SponsorshipRequests/{id}/submit
      -> Status: PendingManagerApproval

 5    James Manager logs in               POST   /api/Auth/login
      -> Gets JWT tokens

 6    James views pending queue           GET    /api/SponsorshipRequests/pending-manager-approval
      -> Sees REQ-2026-0001

 7    James views details                 GET    /api/SponsorshipRequests/{id}
      -> Reviews request details

 8    James approves                      POST   /api/SponsorshipRequests/{id}/manager-approve
      { remarks: "Approved. Within budget." }
      -> Status: PendingFinanceReview

 9    Sarah Finance logs in               POST   /api/Auth/login
      -> Gets JWT tokens

10    Sarah views finance queue           GET    /api/SponsorshipRequests/pending-finance-review
      -> Sees REQ-2026-0001

11    Sarah views details                 GET    /api/SponsorshipRequests/{id}
      -> Reviews request details

12    Sarah approves                      POST   /api/SponsorshipRequests/{id}/finance-approve
      { remarks: "Budget allocated." }
      -> Status: Approved (FINAL)

13    John checks status                  GET    /api/SponsorshipRequests/{id}
      -> Status: Approved

14    John views audit trail              GET    /api/SponsorshipRequests/{id}/workflow-history
      -> 4 history entries:
         1. Created (John)
         2. Submitted (John)
         3. Approved by Manager (James)
         4. Approved by Finance (Sarah)


3.4 REJECTION PATH

STEP  ACTION                              API CALL
──────────────────────────────────────────────────────────────
 1-3   Same as happy path (create + submit)

 4     James rejects                      POST   /api/SponsorshipRequests/{id}/manager-reject
      { remarks: "Exceeds budget limit." }
      -> Status: Rejected (FINAL)
      -> Requestor sees rejection + remarks in history


3.5 CANCELLATION PATH

STEP  ACTION                              API CALL
──────────────────────────────────────────────────────────────
 1-2   Same as happy path (create draft)

 3     John cancels before submitting     POST   /api/SponsorshipRequests/{id}/cancel
      { reason: "No longer needed." }
      -> Status: Cancelled (FINAL)

 OR

 1-4   Create + Submit

 5     John cancels while pending mgr     POST   /api/SponsorshipRequests/{id}/cancel
      { reason: "Changed mind." }
      -> Status: Cancelled (FINAL)


3.6 TOKEN LIFECYCLE

┌──────────────┐
│  1. LOGIN    │
│  POST /login │
└──────┬───────┘
       │
       ▼
  ┌─────────────────────────────────────────┐
  │  ACCESS TOKEN  (expires: 60 minutes)    │
  │  REFRESH TOKEN (expires: 7 days)        │
  └──────┬──────────────────────┬───────────┘
         │                      │
         │ Every API call:      │ When access token
         │ Header:              │ expires or 401:
         │ Authorization:       │
         │ Bearer {access}      │
         │                      ▼
         │              ┌───────────────────────────┐
         │              │ POST /api/Auth/refresh-token│
         │              │ { "refreshToken": "old" }  │
         │              │                            │
         │              │ Returns NEW access+refresh │
         │              └───────────────────────────┘
         │
         │ On logout:
         ▼
  ┌──────────────────────────────────┐
  │  POST /api/Auth/logout           │
  │  (Revokes ALL refresh tokens)    │
  │  Client clears stored tokens     │
  └──────────────────────────────────┘


3.7 API CALL SEQUENCE PER PAGE

LOGIN PAGE:
  1. POST /api/Auth/login          -> Save tokens
  2. GET  /api/Auth/me             -> Get user info + role
  3. Redirect based on role

REQUESTOR - CREATE REQUEST:
  1. GET  /api/SponsorshipTypes            -> Populate dropdown
  2. POST /api/SponsorshipRequests         -> Create request
  3. Redirect to My Requests

REQUESTOR - MY REQUESTS DASHBOARD:
  1. GET  /api/SponsorshipRequests/my-requests  -> List all mine
  2. GET  /api/SponsorshipRequests/{id}          -> View detail
  3. POST /api/SponsorshipRequests/{id}/submit   -> Submit
  4. POST /api/SponsorshipRequests/{id}/cancel   -> Cancel

MANAGER - PENDING APPROVALS DASHBOARD:
  1. GET  /api/SponsorshipRequests/pending-manager-approval  -> Queue
  2. GET  /api/SponsorshipRequests/{id}                       -> Detail
  3. POST /api/SponsorshipRequests/{id}/manager-approve       -> Approve
     OR
  3. POST /api/SponsorshipRequests/{id}/manager-reject        -> Reject

FINANCE ADMIN - PENDING REVIEWS DASHBOARD:
  1. GET  /api/SponsorshipRequests/pending-finance-review     -> Queue
  2. GET  /api/SponsorshipRequests/{id}                       -> Detail
  3. POST /api/SponsorshipRequests/{id}/finance-approve       -> Approve
     OR
  3. POST /api/SponsorshipRequests/{id}/finance-reject        -> Reject

SYSTEM ADMIN - ALL REQUESTS DASHBOARD:
  1. GET  /api/SponsorshipRequests?page=1&pageSize=10          -> List
  2. GET  /api/SponsorshipRequests/{id}                         -> Detail
  3. GET  /api/SponsorshipRequests/{id}/workflow-history        -> History
  4. GET  /api/SponsorshipTypes                                 -> Types list
  5. POST /api/SponsorshipTypes                                 -> Add type
  6. PUT  /api/SponsorshipTypes/{id}                            -> Edit type
  7. DELETE /api/SponsorshipTypes/{id}                          -> Delete type


═══════════════════════════════════════════════════════════════════════════════
SECTION 4: TEST ACCOUNTS
═══════════════════════════════════════════════════════════════════════════════

┌────────────┬──────────────────────────┬─────────────┬────────────────────┐
│ Role       │ Email                    │ Password    │ Dashboard Shows    │
├────────────┼──────────────────────────┼─────────────┼────────────────────┤
│ Requestor  │ requestor@sponsorship.com│ Admin@1234  │ My Requests        │
│ Requestor  │ naim@sponsorship.com     │ Admin@1234  │ My Requests        │
│ Manager    │ manager@sponsorship.com  │ Admin@1234  │ Pending Approvals  │
│ FinanceAdm │ finance@sponsorship.com  │ Admin@1234  │ Pending Reviews    │
│ SystemAdm  │ admin@sponsorship.com    │ Admin@2026  │ All Requests       │
└────────────┴──────────────────────────┴─────────────┴────────────────────┘

All accounts use password: Admin@1234


═══════════════════════════════════════════════════════════════════════════════
SECTION 5: FULL DATABASE SCRIPT (SQL Server)
═══════════════════════════════════════════════════════════════════════════════

Note: The database is auto-created and seeded by the application via EF Core
Migrations. The script below is for REFERENCE, manual setup, or re-seeding.

The application handles database creation automatically:
  1. dotnet ef migrations add InitialCreate ...
  2. dotnet run (auto-migrates + seeds on startup)

Use this SQL script only if you need to re-seed data manually.

───────────────────────────────────────────────────────────────────────────────
COMPLETE SQL SCRIPT:
───────────────────────────────────────────────────────────────────────────────

USE master;
GO

-- Create database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SponsorshipWorkflowDB')
BEGIN
    CREATE DATABASE SponsorshipWorkflowDB;
END
GO

USE SponsorshipWorkflowDB;
GO

-- ================================================
-- TABLE: Roles
-- ================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Roles' AND xtype='U')
BEGIN
    CREATE TABLE Roles (
        Id          INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Name        NVARCHAR(50)    NOT NULL,
        Description NVARCHAR(200)   NULL,
        IsActive    BIT             NOT NULL DEFAULT 1,
        CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE UNIQUE INDEX IX_Roles_Name ON Roles(Name);
END
GO

-- ================================================
-- TABLE: Users
-- ================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id              UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY,
        FullName        NVARCHAR(150)       NOT NULL,
        Email           NVARCHAR(200)       NOT NULL,
        Department      NVARCHAR(100)       NULL,
        PasswordHash    NVARCHAR(500)       NOT NULL,
        PasswordSalt    NVARCHAR(500)       NOT NULL,
        RoleId          INT                 NOT NULL,
        IsActive        BIT                 NOT NULL DEFAULT 1,
        LastLoginAt     DATETIME2           NULL,
        CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt       DATETIME2           NULL,
        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
    );

    CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
    CREATE INDEX IX_Users_RoleId ON Users(RoleId);
END
GO

-- ================================================
-- TABLE: RefreshTokens
-- ================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RefreshTokens' AND xtype='U')
BEGIN
    CREATE TABLE RefreshTokens (
        Id          UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY,
        UserId      UNIQUEIDENTIFIER    NOT NULL,
        Token       NVARCHAR(500)       NOT NULL,
        ExpiresAt   DATETIME2           NOT NULL,
        CreatedAt   DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
        RevokedAt   DATETIME2           NULL,
        ReplacedBy  NVARCHAR(500)       NULL,
        CreatedByIp NVARCHAR(50)        NULL,
        RevokedByIp NVARCHAR(50)        NULL,
        IsRevoked   BIT                 NOT NULL DEFAULT 0,
        CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
            ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
    CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
END
GO

-- ================================================
-- TABLE: SponsorshipTypes
-- ================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SponsorshipTypes' AND xtype='U')
BEGIN
    CREATE TABLE SponsorshipTypes (
        Id          INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Name        NVARCHAR(100)   NOT NULL,
        Description NVARCHAR(300)   NULL,
        IsActive    BIT             NOT NULL DEFAULT 1,
        CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt   DATETIME2       NULL,
        CreatedBy   UNIQUEIDENTIFIER NULL
    );

    CREATE UNIQUE INDEX IX_SponsorshipTypes_Name ON SponsorshipTypes(Name);
END
GO

-- ================================================
-- TABLE: SponsorshipRequests
-- ================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SponsorshipRequests' AND xtype='U')
BEGIN
    CREATE TABLE SponsorshipRequests (
        Id                      UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY,
        RequestNumber           NVARCHAR(20)        NOT NULL,
        Title                   NVARCHAR(300)       NOT NULL,
        RequestorId             UNIQUEIDENTIFIER    NOT NULL,
        RequestorName           NVARCHAR(150)       NOT NULL,
        Department              NVARCHAR(100)       NOT NULL,
        SponsorshipTypeId       INT                 NOT NULL,
        EventOrganisationName   NVARCHAR(300)       NOT NULL,
        EventDate               DATE                NOT NULL,
        RequestedAmount         DECIMAL(18,2)       NOT NULL,
        Purpose                 NVARCHAR(2000)      NOT NULL,
        ExpectedBusinessBenefit NVARCHAR(2000)      NULL,
        Remarks                 NVARCHAR(1000)      NULL,
        Status                  NVARCHAR(50)        NOT NULL DEFAULT 'Draft',
        SubmittedAt             DATETIME2           NULL,
        CreatedAt               DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt               DATETIME2           NULL,
        CancelledAt             DATETIME2           NULL,
        CancelledReason         NVARCHAR(500)       NULL,
        CONSTRAINT FK_SR_Users     FOREIGN KEY (RequestorId) REFERENCES Users(Id),
        CONSTRAINT FK_SR_Types     FOREIGN KEY (SponsorshipTypeId) REFERENCES SponsorshipTypes(Id),
        CONSTRAINT CK_SR_Status    CHECK (Status IN (
            'Draft','PendingManagerApproval','PendingFinanceReview',
            'Approved','Rejected','Cancelled'
        )),
        CONSTRAINT CK_SR_Amount    CHECK (RequestedAmount > 0)
    );

    CREATE UNIQUE INDEX IX_SR_RequestNumber ON SponsorshipRequests(RequestNumber);
    CREATE INDEX IX_SR_RequestorId ON SponsorshipRequests(RequestorId);
    CREATE INDEX IX_SR_Status ON SponsorshipRequests(Status);
    CREATE INDEX IX_SR_CreatedAt ON SponsorshipRequests(CreatedAt);
END
GO

-- ================================================
-- TABLE: SupportingDocuments
-- ================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SupportingDocuments' AND xtype='U')
BEGIN
    CREATE TABLE SupportingDocuments (
        Id                      UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY,
        SponsorshipRequestId    UNIQUEIDENTIFIER    NOT NULL,
        FileName                NVARCHAR(300)       NOT NULL,
        StoredFileName          NVARCHAR(500)       NOT NULL,
        FileSize                BIGINT              NOT NULL,
        ContentType             NVARCHAR(100)       NOT NULL,
        FilePath                NVARCHAR(1000)      NOT NULL,
        UploadedAt              DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
        UploadedBy              UNIQUEIDENTIFIER    NOT NULL,
        CONSTRAINT FK_SD_Requests FOREIGN KEY (SponsorshipRequestId)
            REFERENCES SponsorshipRequests(Id) ON DELETE CASCADE,
        CONSTRAINT FK_SD_Users    FOREIGN KEY (UploadedBy) REFERENCES Users(Id)
    );

    CREATE INDEX IX_SD_RequestId ON SupportingDocuments(SponsorshipRequestId);
END
GO

-- ================================================
-- TABLE: WorkflowHistories
-- ================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WorkflowHistories' AND xtype='U')
BEGIN
    CREATE TABLE WorkflowHistories (
        Id                      UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY,
        SponsorshipRequestId    UNIQUEIDENTIFIER    NOT NULL,
        Action                  NVARCHAR(100)       NOT NULL,
        FromStatus              NVARCHAR(50)        NULL,
        ToStatus                NVARCHAR(50)        NOT NULL,
        ActorId                 UNIQUEIDENTIFIER    NOT NULL,
        ActorName               NVARCHAR(150)       NOT NULL,
        ActorRole               NVARCHAR(50)        NOT NULL,
        Remarks                 NVARCHAR(1000)      NULL,
        CreatedAt               DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
        IpAddress               NVARCHAR(50)        NULL,
        CONSTRAINT FK_WH_Requests FOREIGN KEY (SponsorshipRequestId)
            REFERENCES SponsorshipRequests(Id) ON DELETE CASCADE,
        CONSTRAINT FK_WH_Users    FOREIGN KEY (ActorId) REFERENCES Users(Id)
    );

    CREATE INDEX IX_WH_RequestId ON WorkflowHistories(SponsorshipRequestId);
    CREATE INDEX IX_WH_ActorId ON WorkflowHistories(ActorId);
    CREATE INDEX IX_WH_CreatedAt ON WorkflowHistories(CreatedAt);
END
GO

-- ================================================
-- SEED DATA: Roles
-- ================================================
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Requestor')
BEGIN
    SET IDENTITY_INSERT Roles ON;

    INSERT INTO Roles (Id, Name, Description, IsActive, CreatedAt) VALUES
    (1, 'Requestor',    'Can submit and manage own sponsorship requests',  1, GETUTCDATE()),
    (2, 'Manager',      'Can approve or reject requests at manager level', 1, GETUTCDATE()),
    (3, 'FinanceAdmin', 'Can perform final approval or rejection',         1, GETUTCDATE()),
    (4, 'SystemAdmin',  'Has full system access and management',           1, GETUTCDATE());

    SET IDENTITY_INSERT Roles OFF;
END
GO

-- ================================================
-- SEED DATA: SponsorshipTypes
-- ================================================
IF NOT EXISTS (SELECT 1 FROM SponsorshipTypes WHERE Name = 'Corporate Event')
BEGIN
    SET IDENTITY_INSERT SponsorshipTypes ON;

    INSERT INTO SponsorshipTypes (Id, Name, Description, IsActive, CreatedAt) VALUES
    (1, 'Corporate Event',       'Corporate events and conferences',         1, GETUTCDATE()),
    (2, 'Sports Event',          'Sports tournaments and activities',         1, GETUTCDATE()),
    (3, 'Community Outreach',    'Community programs and CSR activities',     1, GETUTCDATE()),
    (4, 'Education & Training',  'Educational programs and training',         1, GETUTCDATE()),
    (5, 'Cultural Event',        'Cultural and arts events',                  1, GETUTCDATE()),
    (6, 'Charity & Fundraising', 'Charitable causes and fundraisers',         1, GETUTCDATE()),
    (7, 'Industry Conference',   'Industry conferences and expos',            1, GETUTCDATE()),
    (8, 'Internal Team Event',   'Internal company team building events',     1, GETUTCDATE());

    SET IDENTITY_INSERT SponsorshipTypes OFF;
END
GO

-- ================================================
-- NOTE: Users are NOT seeded via SQL.
-- They are seeded by Program.cs on app startup
-- because passwords use PBKDF2 hashing (C# code).
--
-- After running the API once, users will be created.
-- Then you can view them:
--
-- SELECT Email, FullName, RoleId FROM Users;
--
-- To re-seed users, run in SSMS:
--   DELETE FROM WorkflowHistories;
--   DELETE FROM SupportingDocuments;
--   DELETE FROM SponsorshipRequests;
--   DELETE FROM RefreshTokens;
--   DELETE FROM Users;
--   Then restart the API.
-- ================================================

-- ================================================
-- SEED DATA: Sample Sponsorship Requests (run AFTER app has seeded users)
-- ================================================
-- Uncomment and run AFTER users exist in the database.

/*
DECLARE @RequestorId UNIQUEIDENTIFIER = 'A4000000-0000-0000-0000-000000000004';
DECLARE @Requestor2Id UNIQUEIDENTIFIER = 'A5000000-0000-0000-0000-000000000005';

INSERT INTO SponsorshipRequests (
    Id, RequestNumber, Title, RequestorId, RequestorName,
    Department, SponsorshipTypeId, EventOrganisationName,
    EventDate, RequestedAmount, Purpose, ExpectedBusinessBenefit,
    Status, SubmittedAt, CreatedAt
) VALUES
(
    NEWID(), 'REQ-2026-0001', 'Tech Conference KL 2026',
    @RequestorId, 'John Requestor', 'Marketing',
    7, 'Malaysia Tech Summit',
    DATEADD(MONTH, 3, GETUTCDATE()), 15000.00,
    'Sponsoring Malaysia Tech Summit for brand visibility.',
    '500+ industry attendees, brand logo on materials.',
    'PendingManagerApproval', GETUTCDATE(), GETUTCDATE()
),
(
    NEWID(), 'REQ-2026-0002', 'Community Football Tournament',
    @RequestorId, 'John Requestor', 'Marketing',
    2, 'KL Community Sports Club',
    DATEADD(MONTH, 2, GETUTCDATE()), 5000.00,
    'Annual football tournament for community relations.',
    'Community engagement, brand awareness.',
    'PendingFinanceReview', GETUTCDATE(), GETUTCDATE()
),
(
    NEWID(), 'REQ-2026-0003', 'Annual Charity Gala',
    @Requestor2Id, 'Emily Chen', 'Business Development',
    6, 'Hope Foundation Malaysia',
    DATEADD(MONTH, 4, GETUTCDATE()), 25000.00,
    'Sponsoring charity gala for CSR.',
    'CSR points, media coverage.',
    'Approved', GETUTCDATE(), GETUTCDATE()
),
(
    NEWID(), 'REQ-2026-0004', 'Team Building Retreat',
    @Requestor2Id, 'Emily Chen', 'Business Development',
    8, 'Internal HR Events',
    DATEADD(MONTH, 1, GETUTCDATE()), 8000.00,
    'Annual team building retreat.',
    'Improved team cohesion and morale.',
    'Draft', NULL, GETUTCDATE()
),
(
    NEWID(), 'REQ-2026-0005', 'Education Scholarship Program',
    @RequestorId, 'John Requestor', 'Marketing',
    4, 'National University of Malaysia',
    DATEADD(MONTH, 5, GETUTCDATE()), 50000.00,
    'Annual scholarship program.',
    'Graduate pipeline, brand presence.',
    'Rejected', GETUTCDATE(), GETUTCDATE()
);
GO
*/

-- ================================================
-- VERIFY SEED DATA
-- ================================================
SELECT Email, FullName, Department, r.Name AS Role, u.IsActive
FROM Users u
INNER JOIN Roles r ON u.RoleId = r.Id;
GO

SELECT Name, Description, IsActive FROM SponsorshipTypes;
GO

-- ================================================
-- USEFUL QUERIES
-- ================================================

-- View all sponsorship requests with details
SELECT
    sr.RequestNumber,
    sr.Title,
    sr.RequestorName,
    sr.Department,
    st.Name AS SponsorshipType,
    sr.RequestedAmount,
    sr.Status,
    sr.EventDate,
    sr.CreatedAt
FROM SponsorshipRequests sr
INNER JOIN SponsorshipTypes st ON sr.SponsorshipTypeId = st.Id
ORDER BY sr.CreatedAt DESC;
GO

-- View workflow history for a specific request
SELECT
    wh.Action,
    wh.FromStatus,
    wh.ToStatus,
    wh.ActorName,
    wh.ActorRole,
    wh.Remarks,
    wh.CreatedAt
FROM WorkflowHistories wh
INNER JOIN SponsorshipRequests sr ON wh.SponsorshipRequestId = sr.Id
WHERE sr.RequestNumber = 'REQ-2026-0001'
ORDER BY wh.CreatedAt ASC;
GO

-- View pending manager approvals
SELECT RequestNumber, Title, RequestorName, RequestedAmount
FROM SponsorshipRequests
WHERE Status = 'PendingManagerApproval'
ORDER BY CreatedAt DESC;
GO

-- View pending finance reviews
SELECT RequestNumber, Title, RequestorName, RequestedAmount
FROM SponsorshipRequests
WHERE Status = 'PendingFinanceReview'
ORDER BY CreatedAt DESC;
GO

-- Count requests by status
SELECT Status, COUNT(*) AS Count
FROM SponsorshipRequests
GROUP BY Status;
GO


═══════════════════════════════════════════════════════════════════════════════
SECTION 6: SETUP INSTRUCTIONS
═══════════════════════════════════════════════════════════════════════════════

PREREQUISITES:
  - .NET 10 SDK installed
  - SQL Server (LocalDB, Express, or full)
  - Visual Studio / VS Code

DATABASE CONNECTION:
  File: src/SponsorshipWorkflow.API/appsettings.json
  DefaultConnection: "Server=localhost;Database=SponsorshipWorkflowDB;
    Trusted_Connection=True;TrustServerCertificate=True;
    MultipleActiveResultSets=true"

  Change "localhost" to your SQL Server instance if needed.

FIRST TIME SETUP:
  1. Open CMD in D:\SponsorshipWorkflow
  2. dotnet restore
  3. dotnet build
  4. dotnet ef migrations add InitialCreate --project src\SponsorshipWorkflow.Infrastructure --startup-project src\SponsorshipWorkflow.API --output-dir Data\Migrations
  5. dotnet run --project src\SponsorshipWorkflow.API
  6. App auto-creates DB, runs migrations, seeds data
  7. Open browser: https://localhost:5064/swagger

RUNNING:
  dotnet run --project src\SponsorshipWorkflow.API

ACCESS:
  Swagger UI:  https://localhost:5064/swagger
  Health:      https://localhost:5064/health
  Login:       POST https://localhost:5064/api/Auth/login


═══════════════════════════════════════════════════════════════════════════════
SECTION 7: TECH STACK SUMMARY
═══════════════════════════════════════════════════════════════════════════════

Backend:
  Framework:     .NET 10
  ORM:           Entity Framework Core 9.0.5
  Database:      SQL Server
  Auth:          JWT (Access + Refresh Tokens)
  Password:      PBKDF2 SHA-512 (100,000 iterations)
  API Docs:      Swagger / Swashbuckle 6.9.0
  Architecture:  Clean Architecture (4 layers)

Key Packages:
  Microsoft.EntityFrameworkCore.SqlServer          9.0.5
  Microsoft.EntityFrameworkCore.Tools              9.0.5
  Microsoft.AspNetCore.Authentication.JwtBearer    9.0.5
  Swashbuckle.AspNetCore                           6.9.0
  Swashbuckle.AspNetCore.Annotations               6.9.0
  Microsoft.IdentityModel.Tokens                   8.9.0
  System.IdentityModel.Tokens.Jwt                  8.9.0

Frontend (To Build):
  Framework:     Blazor Server (.NET 10)
  Styling:       Bootstrap 5
  Auth:          JWT AuthenticationStateProvider
  UI Components: MudBlazor or Radzen (optional)
  Pages:         ~20-25 pages
  Components:    ~10-15 shared components


═══════════════════════════════════════════════════════════════════════════════
END OF DOCUMENT
═══════════════════════════════════════════════════════════════════════════════
